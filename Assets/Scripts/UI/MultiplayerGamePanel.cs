using Enum;
using Event;
using Game;
using TMPro;

namespace UI
{
    public class MultiplayerGamePanel : BasePanel
    {
        #region 公共变量
    
        public TextMeshProUGUI roundState;
    
        #endregion

        #region Unity生命周期函数

        private void OnEnable()
        {
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
            roundState.text = GameMgr.Instance.localPlayerType == E_PlayerType.X ? "你的回合" : "对手的回合";
        }

        #region 私有方法

        //玩家回合改变时，调用的函数
        private void OnTurnChanged(E_PlayerType playerType)
        {
            roundState.text = playerType == GameMgr.Instance.localPlayerType ? "你的回合" : "对手的回合";
        }
    
        // 游戏结束时，弹出 GameoverPanel
        private void OnGameOver(E_WinnerType winner)
        {
            UIManager.Instance.ShowPanel<GameoverPanel>();
        }

        #endregion
    
    
    }
}
