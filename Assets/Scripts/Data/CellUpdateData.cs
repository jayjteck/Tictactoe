using System.Collections;
using System.Collections.Generic;
using Enum;
using UnityEngine;

public struct CellUpdateData
{
    public int cellIndex;
    public E_PlayerType player;
    
    public CellUpdateData(int index, E_PlayerType player)
    {
        cellIndex = index;
        this.player = player;
    }
}
