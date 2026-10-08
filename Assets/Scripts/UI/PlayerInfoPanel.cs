using Data;
using Enum;
using Event;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfoPanel : BasePanel
    {
        #region 公开字段

        public Image  nowPlayerHeadImage;
        public Button playerHeadButton1;
        public Button playerHeadButton2;
        public Button playerHeadButton3;
        public Button playerHeadButton4;
        public Button playerHeadButton5;
        public TMP_InputField  playerName;
        public Button exitButton;

        #endregion
    

        #region 私有字段

        // 本次编辑中选中的头像索引
        private int _selectedHeadIndex;   

        #endregion
    
        #region Unity生命周期函数

        private void OnEnable()
        {
            // 打开面板时，用当前数据填充 UI
            PlayerData data = PlayerDataManager.Instance.playerData;
            playerName.text = data.playerName;
            _selectedHeadIndex = PlayerDataManager.Instance.nowHeadIndex;
            nowPlayerHeadImage.sprite = data.headImage;

            playerHeadButton1.onClick.AddListener(() => SelectHead(0));
            playerHeadButton2.onClick.AddListener(() => SelectHead(1));
            playerHeadButton3.onClick.AddListener(() => SelectHead(2));
            playerHeadButton4.onClick.AddListener(() => SelectHead(3));
            playerHeadButton5.onClick.AddListener(() => SelectHead(4));

            exitButton.onClick.AddListener(OnExit);
        }

        private void OnDisable()
        {
            playerHeadButton1.onClick.RemoveAllListeners();
            playerHeadButton2.onClick.RemoveAllListeners();
            playerHeadButton3.onClick.RemoveAllListeners();
            playerHeadButton4.onClick.RemoveAllListeners();
            playerHeadButton5.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        #endregion
    
    
        protected override void Init()
        {
        
        }

        #region 私有方法

        /// <summary>
        /// 点头像按钮，预览对应头像
        /// </summary>
        private void SelectHead(int index)
        {
            _selectedHeadIndex = index;
            nowPlayerHeadImage.sprite = PlayerDataManager.Instance.GetHeadSprite(index);
        }

        /// <summary>
        /// 点退出按钮时，写回数据并保存到磁盘
        /// </summary>
        private void OnExit()
        {
            string name = playerName.text;
            if (string.IsNullOrEmpty(name))
                name = "Player";

            PlayerDataManager.Instance.playerData.playerName = name;
            PlayerDataManager.Instance.SetHead(_selectedHeadIndex);
            PlayerDataManager.Instance.Save();

            // 通知其他面板刷新显示
            EventCenter.Instance.EventTrigger(E_EventType.PlayerInfoChanged);
        
            UIManager.Instance.HidePanel<PlayerInfoPanel>();
        }

        #endregion
    }
}
