using System;
using UnityEngine;

public class GameTile
{
    public Entity CurrentEntity;
    public Vector3Int Pos;

    public GameTile(Vector3Int pos)
    {
        Pos = pos;
    }
}
