using Enum;
using Event;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Cell : MonoBehaviour
    {
        #region 公共变量

        //格子索引（第几个格子）
        [Range(0, 8)] 
        public int cellIndex; 
    
        #endregion
    
        #region 私有变量

        private Button  _buttonCell;
        private Image  _imageCell;

        #endregion
    
        #region Unity生命周期函数

        private void Awake()
        {
            _buttonCell = GetComponent<Button>();
            _imageCell = GetComponent<Image>();
        }
    
        private void OnEnable()
        {
            //订阅按钮点击事件
            _buttonCell.onClick.AddListener(() =>
            {
                EventCenter.Instance.EventTrigger<int>(E_EventType.PlayerMove, cellIndex);
            });
        
            // 订阅"格子图片更新"事件，由事件驱动 UI刷新
            EventCenter.Instance.AddEventListener<CellUpdateData>(E_EventType.CellSpriteUpdate, OnCellSpriteUpdate);
        
            // 订阅"重开游戏"事件
            EventCenter.Instance.AddEventListener(E_EventType.GameRestart, OnGameRestart);
        }

        private void Start()
        {
            //初始化图片
            _imageCell.sprite = Resources.Load<Sprite>("9Sliced");
        }

        private void OnDisable()
        {
            //取消订阅
            _buttonCell.onClick.RemoveAllListeners();
        
            EventCenter.Instance.RemoveEventListener<CellUpdateData>(E_EventType.CellSpriteUpdate, OnCellSpriteUpdate);
        
            EventCenter.Instance.RemoveEventListener(E_EventType.GameRestart, OnGameRestart);
        }

        #endregion
    
        #region 私有方法
    
        //格子图片更新方法
        private void OnCellSpriteUpdate(CellUpdateData data)
        {
            //不是我这个格子，忽略
            if (data.cellIndex != cellIndex) 
                return; 
        
            switch (data.player)
            {
                case E_PlayerType.X:
                    _imageCell.sprite = Resources.Load<Sprite>("刺猬");
                    break;
                case E_PlayerType.O:
                    _imageCell.sprite = Resources.Load<Sprite>("R-C");
                    break;
            }
        }
    
        //重开游戏后，设置初始图片的方法
        private void OnGameRestart()
        {
            _imageCell.sprite = Resources.Load<Sprite>("9Sliced");
        }
    
        #endregion
    }
}
