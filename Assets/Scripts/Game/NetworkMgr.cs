using System.Collections.Generic;
using Data;
using Enum;
using Event;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using WebSocketSharp;

namespace Game
{
    public class NetworkMgr : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        #region 私有字段

        /// <summary>
        /// 房间中的玩家头像
        /// </summary>
        private Dictionary<Player, GameObject> _playerImages = new Dictionary<Player, GameObject>();

        /// <summary> 落子同步事件码 </summary>
        private const byte MoveEventCode = 1;
    
        /// <summary> 重新开局事件码 </summary>
        private const byte RestartEventCode = 2;

        #endregion
    
        #region 单例模式

        private static NetworkMgr instance;

        public static NetworkMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    //在场景上创建空物体
                    GameObject obj = new GameObject();

                    //得到T脚本的类名，为对象改名，这样在编辑器中可以明确的看到该单例模式脚本对象依附的GameObject
                    obj.name = typeof(NetworkMgr).ToString();

                    //动态挂载对应的单例模式脚本
                    instance = obj.AddComponent<NetworkMgr>();
                
                    //过场景时不移除对象，保证它在整个游戏生命周期中都存在
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            UIManager.Instance.ShowPanel<WaitPanel>();
        
            //把本地玩家的昵称同步给 Photon（要在连接前设置）
            PhotonNetwork.NickName = PlayerDataManager.Instance.playerData.playerName;
        
            PhotonNetwork.GameVersion = "1.0";
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// 断开连接服务器
        /// </summary>
        public void DisConnect()
        {
            PhotonNetwork.Disconnect();
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roomName">房间名</param>
        public void CreateRoom(string roomName)
        {
            if (roomName.IsNullOrEmpty())
            {
                ShowHintPanel("房间名不能为空");
                return;
            }
        
            UIManager.Instance.ShowPanel<WaitPanel>();
        
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = 2
            };

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="roomName">房间名</param>
        public void JoinRoom(string roomName)
        {
            if (roomName.IsNullOrEmpty())
            {
                ShowHintPanel("房间名不能为空");
                return;
            }
        
            UIManager.Instance.ShowPanel<WaitPanel>();
        
            PhotonNetwork.JoinRoom(roomName);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        /// <summary>
        /// 开始游戏（只有房主能点）
        /// </summary>
        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                ShowHintPanel("只有房主才能开始游戏");
                return;
            }
        
            // 房间不足 2 人，不能开始游戏
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                ShowHintPanel("当前房间人数不够2人，无法开始游戏");
                return;
            }

            // 关闭房间，防止游戏开始后新玩家加入
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            // 设置房间自定义属性，通知房间内所有人“游戏开始”
            var props = new ExitGames.Client.Photon.Hashtable
            {
                { "GameStarted", true }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    
        /// <summary>
        /// 请求重新开局（点击"再来一局"按钮）
        /// </summary>
        public void RequestRestart()
        {
            PhotonNetwork.RaiseEvent(
                RestartEventCode,
                null,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },  // 发给所有人（包括自己）
                SendOptions.SendReliable
            );
        }
    
        #endregion
    
        #region 事件回调函数

        //连接到服务器时调用
        public override void OnConnectedToMaster()
        {
            print("连接服务器成功");
        
            UIManager.Instance.HidePanel<WaitPanel>();
        
            UIManager.Instance.ShowPanel<MultiplayerMenuPanel>();
            UIManager.Instance.HidePanel<MainMenuPanel>();
        }
    
        public override void OnDisconnected(DisconnectCause cause)
        {
            print($"断开原因：{cause}");
        
            // 关闭等待面板（连接超时/失败时，否则会一直卡在"连接中"）
            UIManager.Instance.HidePanel<WaitPanel>();
        
            UIManager.Instance.ShowPanel<MainMenuPanel>();
            UIManager.Instance.HidePanel<MultiplayerMenuPanel>();
        
            // 可选：超时给个明确提示
            if (cause == DisconnectCause.ClientTimeout)
            {
                ShowHintPanel("连接超时，请检查网络后重试");
            }
        }

        //房间创建完毕时调用
        public override void OnCreatedRoom()
        {
            print("房间创建完毕");
        
            UIManager.Instance.HidePanel<WaitPanel>();
        
            UIManager.Instance.ShowPanel<LobbyPanel>();
            UIManager.Instance.HidePanel<MultiplayerMenuPanel>();
        }
    
        //创建房间失败时调用
        //避免万一创建失败时，WaitPanel 一直卡着不消失
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            UIManager.Instance.HidePanel<WaitPanel>();
        
            ShowHintPanel($"创建房间失败：{message}");
        }

        //自己加入房间时调用
        public override void OnJoinedRoom()
        {
            print("房间加入成功");
        
            UIManager.Instance.HidePanel<WaitPanel>();
        
            UIManager.Instance.ShowPanel<LobbyPanel>();
            UIManager.Instance.HidePanel<MultiplayerMenuPanel>();
        
            //先把本玩家头像索引设为自定义属性，其他玩家才能读到
            SetLocalHeadProperty();

            // 显示房间里已有的所有玩家（包括自己）的头像
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                ShowPlayerHeadImage(p);
            }
        }

        //加入指定房间失败时调用
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            ShowHintPanel("加入房间失败");
        }
    
        //其他玩家加入了你当前所在房间时调用
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print("其他玩家加入");

            // 显示新加入玩家的头像
            ShowPlayerHeadImage(newPlayer);
        }

        //其他玩家离开了你当前所在房间时调用
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print("其他玩家离开");
        
            //隐藏新加入玩家的头像
            HidePlayerHeadImage(otherPlayer);
        
            // 判断游戏是否已经开始：只有对局中/对局后，对方离开时才跟着退出；
            // 大厅阶段（还没开始游戏）就留在房间里等新玩家
            bool gameStarted = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("GameStarted", out object started) && (bool)started;

            if (gameStarted)
            {
                LeaveRoom();
            }
        }

        //自己离开房间时调用
        public override void OnLeftRoom()
        {
            print("离开房间");
        
            // 隐藏房间里已有的所有玩家（包括自己）的头像
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                HidePlayerHeadImage(p);
            }
        
            // 关掉对局相关的面板
            UIManager.Instance.HidePanel<MultiplayerGamePanel>();
            UIManager.Instance.HidePanel<GameoverPanel>();
        
            UIManager.Instance.ShowPanel<MultiplayerMenuPanel>();
            UIManager.Instance.HidePanel<LobbyPanel>();
        }

        //房间自定义属性变化时调用（接收“开始游戏”的通知）
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey("GameStarted") && (bool)propertiesThatChanged["GameStarted"]) 
            {
                print("收到开始游戏通知，打开游戏面板");

                // 分配角色：房主 = X（先手），另一个 = O
                E_PlayerType myRole = PhotonNetwork.IsMasterClient ? E_PlayerType.X : E_PlayerType.O;

                GameMgr.Instance.currentGameMode = E_GameMode.Online;
                GameMgr.Instance.SetLocalPlayerType(myRole);   // 设置本玩家是 X 还是 O，并让 X 先手
                GameMgr.Instance.Restart();                     // 重置棋盘数据

                UIManager.Instance.HidePanel<LobbyPanel>();
                UIManager.Instance.ShowPanel<MultiplayerGamePanel>();
            }
        }
    
        //某个玩家的自定义属性变化时（比如头像索引稍后才同步到），刷新他的头像
        //OnPlayerPropertiesUpdate很重要：新玩家加入时，他的 headIndex属性可能还没同步到你这，你会先按默认头像显示他，等他属性同步过来，这个回调会帮你刷新成正确头像
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if (changedProps.ContainsKey("headIndex") && _playerImages.TryGetValue(targetPlayer, out GameObject img))
            {
                img.GetComponent<PlayerImageItem>().SetHead((int)changedProps["headIndex"]);
            }
        }

        #endregion

        #region 落子同步（RaiseEvent，不需要 PhotonView）

        public override void OnEnable()
        {
            base.OnEnable();
            // 订阅：本地落子后，把这次落子发给对手
            EventCenter.Instance.AddEventListener<CellUpdateData>(E_EventType.OnlineMyMoveSent, OnLocalMoveMade);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventCenter.Instance.RemoveEventListener<CellUpdateData>(E_EventType.OnlineMyMoveSent, OnLocalMoveMade);
        }

        /// <summary>
        /// 本地落子后回调：通过 RaiseEvent 发给对手
        /// </summary>
        private void OnLocalMoveMade(CellUpdateData data)
        {
            object[] content = new object[] { data.cellIndex, (int)data.player };

            PhotonNetwork.RaiseEvent(
                MoveEventCode,
                content,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },  // 只发给对方
                SendOptions.SendReliable                                       // 可靠传输，不丢包
            );

            Debug.Log($"发送落子：格子{data.cellIndex}");
        }

        /// <summary>
        /// IOnEventCallback 回调
        /// </summary>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == MoveEventCode)
            {
                object[] data = (object[])photonEvent.CustomData;
                int cellIndex = (int)data[0];
                E_PlayerType opponentPlayer = (E_PlayerType)(int)data[1];

                Debug.Log($"收到对手落子：格子{cellIndex}，玩家{opponentPlayer}");

                GameMgr.Instance.ReceiveOpponentMove(cellIndex, opponentPlayer);
            }
            else if (photonEvent.Code == RestartEventCode)
            {
                Debug.Log("收到重开通知");
                GameMgr.Instance.Restart();
            }
        }

        #endregion

        #region 私有方法

        //显示一个玩家的头像图片
        private void ShowPlayerHeadImage(Player player)
        {
            GameObject playerImage = Instantiate(
                Resources.Load<GameObject>("Player/PlayerImage"),
                GameObject.Find("PlayersImageBackground").transform,
                false
            );

            //将玩家头像添加进字典
            _playerImages[player] = playerImage;
        
            //设置昵称和头像
            PlayerImageItem item = playerImage.GetComponent<PlayerImageItem>();
            item.SetName(player.NickName);
            item.SetHead(GetHeadIndex(player));

            Debug.Log($"实例化成功：{playerImage.name}，玩家：{player.NickName}");
        }
    
        //隐藏一个玩家的头像
        private void HidePlayerHeadImage(Player player)
        {
            if (_playerImages.TryGetValue(player, out GameObject img))
            {
                Destroy(img);
                _playerImages.Remove(player);
            }
        }

        //显示提示面板
        private void ShowHintPanel(string hintText)
        {
            UIManager.Instance.ShowPanel<HintPanel>();
            UIManager.Instance.GetPanel<HintPanel>().hintText.text = hintText;
        }
    
        //把本玩家头像索引设为自定义属性
        private void SetLocalHeadProperty()
        {
            var props = new ExitGames.Client.Photon.Hashtable
            {
                { "headIndex", PlayerDataManager.Instance.nowHeadIndex }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    
        //读取某玩家的头像索引（没设置过就默认 0）
        private int GetHeadIndex(Player player)
        {
            if (player.CustomProperties != null && player.CustomProperties.ContainsKey("headIndex"))
            {
                return (int)player.CustomProperties["headIndex"];
            }
            return 0;
        }

        #endregion

    }
}
