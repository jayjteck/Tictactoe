using System;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string playerName;
    
        //玩家头像的索引（用于序列化，因为 Sprite无法被序列化）
        public int headImageIndex;
    
        [NonSerialized]
        public Sprite headImage;
    
    }
}
