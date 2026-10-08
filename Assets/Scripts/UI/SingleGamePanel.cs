using Enum;
using Event;
using Game;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class SingleGamePanel : BasePanel
    {
        #region 公共变量
    
        public TextMeshProUGUI roundState;
        public Button restartButton;
        public Button exitButton;
    
        #endregion

        #region Unity生命周期函数

        private void OnEnable()
        {
            // 进入单机模式：把游戏模式切回本地，并重置上一局（可能是联网对局）遗留的棋盘状态
            // 否则打完多人后 currentGameMode 仍是 Online，isGameOver 仍是 true，点格子会被直接 return
            GameMgr.Instance.currentGameMode = E_GameMode.Local;
            GameMgr.Instance.Restart();

            EventCenter.Instance.AddEventListener<E_PlayerType>(E_EventType.TurnChanged, OnTurnChanged);
            EventCenter.Instance.AddEventListener<E_WinnerType>(E_EventType.GameOver, OnGameOver);
        }

        private void OnDisable()
        {
            EventCenter.Instance.RemoveEventListener<E_PlayerType>(E_EventType.TurnChanged, OnTurnChanged);
            EventCenter.Instance.RemoveEventListener<E_WinnerType>(E_EventType.GameOver, OnGameOver);
        }

        #endregion
    
        protected override void Init()
        {
            roundState.text = "玩家X的回合";
        
            restartButton.onClick.AddListener(GameMgr.Instance.Restart);
        
            exitButton.onClick.AddListener(() =>
            {
                UIManager.Instance.ShowPanel<MainMenuPanel>();
                UIManager.Instance.HidePanel<SingleGamePanel>();
                GameMgr.Instance.Restart();
            });
        }

        //玩家回合改变时，调用的函数
        private void OnTurnChanged(E_PlayerType playerType)
        {
            roundState.text = $"玩家{playerType}的回合";
        }
    
        // 游戏结束时，弹出 GameoverPanel
        private void OnGameOver(E_WinnerType winner)
        {
            roundState.text = $"玩家{winner}胜利";
        }
    }
}
