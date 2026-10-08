using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class HintPanel : BasePanel
    {
        #region 公开字段

        public TextMeshProUGUI  hintText;
        public Button exitButton;

        #endregion
    
        protected override void Init()
        {
            exitButton.onClick.AddListener(() => UIManager.Instance.HidePanel<HintPanel>());
        }

    }
}
