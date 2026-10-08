using Game;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class LobbyPanel : BasePanel
    {
        public TextMeshProUGUI roomNameText;
        public Button startButton;
        public Button exitButton;

        #region Unity生命周期函数

        private void OnEnable()
        {
            roomNameText.text = $"房间名：{PhotonNetwork.CurrentRoom.Name}";
        
            startButton.onClick.AddListener(() =>
            {
                NetworkMgr.Instance.StartGame();
            });
        
            exitButton.onClick.AddListener(() =>
            {
                NetworkMgr.Instance.LeaveRoom();
            });
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        #endregion
    
        protected override void Init()
        {
        
        }
    }
}
