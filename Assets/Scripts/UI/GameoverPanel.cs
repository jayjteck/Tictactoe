using Enum;
using Event;
using Game;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class GameoverPanel : BasePanel
    {
        public TextMeshProUGUI winnerText;
        public Button exitButton;
        public Button restartButton;

        #region Unity生命周期函数

        private void OnEnable()
        {
            EventCenter.Instance.AddEventListener(E_EventType.GameRestart, OnGameRestart);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.GameRestart, OnGameRestart);
        }

        #endregion
    
        protected override void Init()
        {
            // 退出游戏：离开房间
            exitButton.onClick.AddListener(() => NetworkMgr.Instance.LeaveRoom());
        
            // 再来一局：请求重开
            restartButton.onClick.AddListener(() => NetworkMgr.Instance.RequestRestart());
        
            // 面板是 GameOver 之后才显示，收不到 GameOver 事件，所以直接读结果
            ShowResult(GameMgr.Instance.Winner);
        }

        #region 私有方法

        //显示游戏结果
        private void ShowResult(E_WinnerType winner)
        {
            if (winner == E_WinnerType.Dogfall)
            {
                winnerText.text = "平局！";
            }
            else
            {
                E_PlayerType winPlayer = (winner == E_WinnerType.X) ? E_PlayerType.X : E_PlayerType.O;
                winnerText.text = (winPlayer == GameMgr.Instance.localPlayerType) ? "你赢了！" : "你输了！";
            }
        }
    
        //GameRestart事件的回调函数
        private void OnGameRestart()
        {
            UIManager.Instance.HidePanel<GameoverPanel>();
        }

        #endregion
    }
}
