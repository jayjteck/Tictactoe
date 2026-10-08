namespace Enum
{
    /// <summary>
    /// 事件类型（即：事件名，需要新事件，就往该枚举中添加）
    /// </summary>
    public enum E_EventType
    {
        /// <summary>
        /// 落子事件，参数为 int（0-8 的格子索引）
        /// </summary>
        PlayerMove,
    
        /// <summary>
        /// 游戏结束事件，参数为 WinnerType
        /// </summary>
        GameOver,
    
        /// <summary>
        /// 回合切换事件，参数为 PlayerType（当前轮到谁）
        /// </summary>
        TurnChanged,
    
        /// <summary>
        /// 游戏重开事件，无参数
        /// </summary>
        GameRestart,
    
        /// <summary>
        /// 格子图片更新事件，参数为 (cellIndex: int, playerType: PlayerType) 
        /// </summary>
        CellSpriteUpdate,
    
        /// <summary>
        /// 联网模式，我移动了事件
        /// </summary>
        OnlineMyMoveSent,
    
        /// <summary>
        /// 玩家信息已修改（改名字/头像后触发，无参数）
        /// </summary>
        PlayerInfoChanged,
    
    }
}
