using Game;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class MultiplayerMenuPanel : BasePanel
    {
        #region 公开字段

        public TMP_InputField roomNameInput;
        public Button creatRoomButton;
        public Button joinRoomButton;
        public Button exitButton;

        #endregion

        #region 私有字段

        private string _roomName;

        #endregion

        #region Unity生命周期函数

        private void OnEnable()
        {
            roomNameInput.onValueChanged.AddListener((strValue) =>
            {
                _roomName = strValue;
            });
        
            creatRoomButton.onClick.AddListener(() =>
            {
                NetworkMgr.Instance.CreateRoom(_roomName);
            });
        
            joinRoomButton.onClick.AddListener(()=>
            {
                NetworkMgr.Instance.JoinRoom(_roomName);
            });
        
            exitButton.onClick.AddListener(() =>
            {
                NetworkMgr.Instance.DisConnect();
            });
        }

        private void OnDisable()
        {
            roomNameInput.onValueChanged.RemoveAllListeners();
            creatRoomButton.onClick.RemoveAllListeners();
            joinRoomButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        #endregion
    
        protected override void Init()
        {
        
        }

    }
}
