using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerImageItem : MonoBehaviour
    {
        public Image headImage;
        public TextMeshProUGUI nameText;

        /// <summary>
        /// 设置玩家名字
        /// </summary>
        public void SetName(string playerName)
        {
            if (nameText != null)
                nameText.text = playerName;
        }

        /// <summary>
        /// 根据索引设置头像
        /// </summary>
        public void SetHead(int index)
        {
            if (headImage != null)
                headImage.sprite = PlayerDataManager.Instance.GetHeadSprite(index);
        }
    }
}
