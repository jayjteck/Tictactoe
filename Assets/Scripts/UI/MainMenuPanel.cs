using Data;
using Enum;
using Event;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuPanel : BasePanel
    {
        #region 公开字段
    
        public Image playerHeadImage;
        public TextMeshProUGUI playerName;
        public Button changePlayerInfoButton;
    
        public Button singleButton;
        public Button multiplayerButton;
        public Button exitButton;
    
        #endregion
    
        #region Unity生命周期函数

        private void OnEnable()
        {
            //每次显示面板时刷新
            RefreshPlayerInfo();
        
            //订阅：玩家信息变化时刷新显示
            EventCenter.Instance.AddEventListener(E_EventType.PlayerInfoChanged, RefreshPlayerInfo);
        
            changePlayerInfoButton.onClick.AddListener(() =>
            {
                UIManager.Instance.ShowPanel<PlayerInfoPanel>();
            });
        
            singleButton.onClick.AddListener(() =>
            {
                UIManager.Instance.ShowPanel<SingleGamePanel>();
                UIManager.Instance.HidePanel<MainMenuPanel>();
            });
        
            multiplayerButton.onClick.AddListener(() =>
            {
                NetworkMgr.Instance.Connect();
            });
        
            exitButton.onClick.AddListener(Application.Quit);
        }

        private void OnDisable()
        {
            //取消订阅
            EventCenter.Instance.RemoveEventListener(E_EventType.PlayerInfoChanged, RefreshPlayerInfo);
        
            changePlayerInfoButton.onClick.RemoveAllListeners();
            singleButton.onClick.RemoveAllListeners();
            multiplayerButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        #endregion
    
        protected override void Init()
        {
        
        }

        #region 私有方法

        /// <summary>
        /// 把 PlayerData 显示到面板
        /// </summary>
        private void RefreshPlayerInfo()
        {
            PlayerData data = PlayerDataManager.Instance.playerData;
            playerHeadImage.sprite = data.headImage;
            playerName.text = data.playerName;
        }

        #endregion
    }
}
