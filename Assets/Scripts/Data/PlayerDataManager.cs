using System.IO;
using SingletonBase;
using UnityEngine;

namespace Data
{
    public class PlayerDataManager : SingletonBase<PlayerDataManager>
    {
        #region 公共字段

        public PlayerData playerData;
        public int nowHeadIndex;

        #endregion
    
        #region 私有字段

        private string savePath = $"{Application.persistentDataPath}/playerData.json";
    
        //头像资源路径
        private static readonly string[] HeadPath =
        {
            "PlayerHeadSprite/佐伊",
            "PlayerHeadSprite/刺猬",
            "PlayerHeadSprite/妮蔻",
            "PlayerHeadSprite/安妮",
            "PlayerHeadSprite/悠米",
        };

        #endregion

        private PlayerDataManager()
        {
            playerData = new PlayerData();
            Load();
        }

        #region 公共方法

        /// <summary>
        /// 根据索引获取头像（越界自动回退到第 0个）
        /// </summary>
        public Sprite GetHeadSprite(int index)
        {
            index = Mathf.Clamp(index, 0, HeadPath.Length - 1);
            return Resources.Load<Sprite>(HeadPath[index]);
        }
    
        /// <summary>
        /// 设置头像索引，并同步更新内存中的 Sprite
        /// </summary>
        public void SetHead(int index)
        {
            nowHeadIndex = Mathf.Clamp(index, 0, HeadPath.Length - 1);
            playerData.headImage = GetHeadSprite(nowHeadIndex);
        }
    
        /// <summary>
        /// 从磁盘读取存档，写入内存中的 playerData（没有存档就用默认值）
        /// </summary>
        public void Load()
        {
            //如果本地存在保存的数据
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                PlayerData save = JsonUtility.FromJson<PlayerData>(json);
                playerData.playerName = save.playerName;
                SetHead(save.headImageIndex);
            }
            else
            {
                playerData.playerName = "新玩家";
                SetHead(0);
            }
        }
    
        /// <summary>
        /// 把内存中的 playerData 保存到磁盘
        /// </summary>
        public void Save()
        {
            PlayerData save = new PlayerData
            {
                playerName = playerData.playerName,
                headImageIndex = nowHeadIndex,
            };

            File.WriteAllText(savePath, JsonUtility.ToJson(save, true));
        }

        #endregion
    }
}
