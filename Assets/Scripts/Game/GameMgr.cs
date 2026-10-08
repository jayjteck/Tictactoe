using Enum;
using Event;
using SingletonBase;

namespace Game
{
    public class GameMgr : SingletonAutoMono<GameMgr>
    {
        #region 公共变量
    
        /// <summary>
        /// 当前游戏模式
        /// </summary>
        public E_GameMode currentGameMode = E_GameMode.Local;
    
        /// <summary>
        /// 在线模式时，是否是本玩家的回合
        /// </summary>
        public bool isMyTurn = true;
    
        /// <summary>
        /// 在线模式下，本玩家被分配的角色（X或O）
        /// </summary>
        public E_PlayerType localPlayerType = E_PlayerType.X;

        /// <summary>
        /// 本地模式下，本玩家被分配的角色（X或O）
        /// </summary>
        public E_PlayerType currentPlayer = E_PlayerType.X;
    
        /// <summary>
        /// 已落子数
        /// </summary>
        public int moveCount = 0;

        /// <summary>
        /// 玩家点击了的格子
        /// 0：玩家未点击，1：玩家X点击，2：玩家O点击
        /// </summary>
        public int[] playerClickCells = new int[9];
    
        public bool isGameOver = false;
    
        #endregion

        #region 私有变量
    
        private E_WinnerType _winner = E_WinnerType.None;
    
        /// <summary>
        /// 所有赢的方式
        /// </summary>
        private static readonly int[][] _winLines = new int[][]
        {
            new int[] { 0, 1, 2 }, // 第一行
            new int[] { 3, 4, 5 }, // 第二行
            new int[] { 6, 7, 8 }, // 第三行
            new int[] { 0, 3, 6 }, // 第一列
            new int[] { 1, 4, 7 }, // 第二列
            new int[] { 2, 5, 8 }, // 第三列
            new int[] { 0, 4, 8 }, // 主对角线
            new int[] { 2, 4, 6 }, // 副对角线
        };

        #endregion

        #region 公共属性

        /// <summary>
        /// 当前对局结果（X/O/平局/未结束），供 UI 面板读取
        /// </summary>
        public E_WinnerType Winner => _winner;

        #endregion
    
        #region Unity生命周期函数

        private void OnEnable()
        {
            EventCenter.Instance.AddEventListener<int>(E_EventType.PlayerMove, OnPlayerMove);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<int>(E_EventType.PlayerMove, OnPlayerMove);
        }

        #endregion
    
    
        #region 私有方法

        private void CheckWinner()
        {
            //判断胜利
            foreach (int[] line in _winLines)
            {
                int a = playerClickCells[line[0]];
                int b = playerClickCells[line[1]];
                int c = playerClickCells[line[2]];

                //如果三个位置都是同一个玩家点击的，那么这个玩家就胜利
                //a != 0可以排除三个位置都是0的情况
                if (a != 0 && a == b && b == c)
                {
                    switch (a)
                    {
                        case 1:
                            print("玩家X胜利");
                            _winner = E_WinnerType.X;
                            break;
                        case 2:
                            print("玩家O胜利");
                            _winner = E_WinnerType.O;
                            break;
                    }
                }
            }

            //当所有格子都被点击且无人胜利，则平局
            if (moveCount >= 9 && _winner == E_WinnerType.None)
            {
                _winner = E_WinnerType.Dogfall;
            }
        
            // 胜负判定完后的统一处理
            switch (_winner)
            {
                case E_WinnerType.X:
                case E_WinnerType.O:
                    isGameOver = true;
                    EventCenter.Instance.EventTrigger<E_WinnerType>(E_EventType.GameOver, _winner);
                    break;
                case E_WinnerType.Dogfall:
                    isGameOver = true;
                    EventCenter.Instance.EventTrigger<E_WinnerType>(E_EventType.GameOver, E_WinnerType.Dogfall);
                    break;
                case E_WinnerType.None:
                    // 未结束，不做事
                    break;
            }
        }
    
        /// <summary>
        /// 玩家点击格子的统一入口（本地 + 联网共用）
        /// </summary>
        /// <param name="cellIndex">点击的格子的索引</param>
        private void OnPlayerMove(int cellIndex)
        {
            //游戏结束了，落子无效
            if (isGameOver) 
                return;

            // 该格子已被点击过，落子无效
            if (playerClickCells[cellIndex] != 0)
                return;

            E_PlayerType player;

            if (currentGameMode == E_GameMode.Local)
            {
                // 本地模式：当前回合的玩家落子
                player = currentPlayer;
            }
            else
            {
                // 联网模式：只有自己的回合才能落子
                if (!isMyTurn) 
                    return;
            
                player = localPlayerType;
            }

            // 执行落子
            ApplyMove(cellIndex, player);

            // 本地模式：落子后立即切换回合
            if (currentGameMode == E_GameMode.Local)
            {
                // 本地模式：没结束时才切换回合
                if (!isGameOver)
                    ChangeCurrentPlayer();
            }
            // 联网模式：回合切换由收到对方 RPC 后触发，或由 NetworkManager 处理
            else if (currentGameMode == E_GameMode.Online)
            {
                // 联网模式：无论是否结束，都要把这次落子发给对方 （否则制胜的那一步发不出去，对方收不到、不会显示 GameoverPanel）
                // 通知 NetworkManager 发送落子数据
                EventCenter.Instance.EventTrigger<CellUpdateData>(E_EventType.OnlineMyMoveSent, new CellUpdateData(cellIndex, player));
            
                // 只有没结束时，才结束自己的回合、切换回合显示
                if (!isGameOver)
                {
                    // 自己的回合结束，等待对方
                    isMyTurn = false;
            
                    // 告诉 UI：现在轮到对方（否则自己这边不会刷新）
                    E_PlayerType opponent = (localPlayerType == E_PlayerType.X) ? E_PlayerType.O : E_PlayerType.X;
                    EventCenter.Instance.EventTrigger<E_PlayerType>(E_EventType.TurnChanged, opponent);
                }
            }
        }
    
        /// <summary>
        /// 执行一次落子操作（不判断回合，不切换玩家）
        /// </summary>
        /// <param name="cellIndex">格子索引 0-8</param>
        /// <param name="player">落子玩家</param>
        private void ApplyMove(int cellIndex, E_PlayerType player)
        {
            //记录数据
            playerClickCells[cellIndex] = (player == E_PlayerType.X) ? 1 : 2;
            moveCount++;

            //通知 UI 更新格子图片
            EventCenter.Instance.EventTrigger<CellUpdateData>(E_EventType.CellSpriteUpdate, new CellUpdateData(cellIndex, player));

            //检查胜负
            CheckWinner();
        }
    
        /// <summary>
        /// 改变当前玩家
        /// </summary>
        private void ChangeCurrentPlayer()
        {
            //改变当前玩家
            currentPlayer = (currentPlayer == E_PlayerType.X) ? E_PlayerType.O : E_PlayerType.X;

            // 发事件，让 UI 自己更新显示
            EventCenter.Instance.EventTrigger<E_PlayerType>(E_EventType.TurnChanged, currentPlayer);
        }
    
        #endregion
    
        #region 公共方法
    
        /// <summary>
        /// 联网模式：接收对手的落子（由 NetworkMgr 收到消息后调用）
        /// </summary>
        public void ReceiveOpponentMove(int cellIndex, E_PlayerType opponentPlayer)
        {
            ApplyMove(cellIndex, opponentPlayer);

            if (!isGameOver)
            {
                isMyTurn = true;
            
                EventCenter.Instance.EventTrigger<E_PlayerType>(E_EventType.TurnChanged, localPlayerType);
            }
        }
    
        /// <summary>
        /// 重置游戏
        /// </summary>
        public void Restart()
        {
            isGameOver =  false;
            moveCount = 0;
            currentPlayer = E_PlayerType.X;
            _winner =  E_WinnerType.None;
            playerClickCells = new int[9];
        
            // 本地模式重置回合UI
            if (currentGameMode == E_GameMode.Local)
            {
                EventCenter.Instance.EventTrigger<E_PlayerType>(E_EventType.TurnChanged, E_PlayerType.X);
            }
            else if (currentGameMode == E_GameMode.Online)
            {
                // 联网模式：重置回合，让 X（先手）先走
                isMyTurn = (localPlayerType == E_PlayerType.X);
            
                EventCenter.Instance.EventTrigger<E_PlayerType>(E_EventType.TurnChanged, E_PlayerType.X);
            }
        
            //通知 Cell 和 Panel 重置
            EventCenter.Instance.EventTrigger(E_EventType.GameRestart);
        }
    
        /// <summary>
        /// 设置在线模式下本地玩家的棋子类型
        /// </summary>
        public void SetLocalPlayerType(E_PlayerType type)
        {
            localPlayerType = type;
            isMyTurn = (type == E_PlayerType.X); // X先手
        }

        #endregion
    
    }
}
