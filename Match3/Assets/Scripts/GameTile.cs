using System;
using UnityEngine;

[Serializable]
public class GameTile: IComparable<GameTile>
{
    public Entity CurrentEntity;
    public Vector3Int Pos;
    public Obstacle CurrentObstacle;

    public GameTile(Vector3Int pos)
    {
        Pos = pos;
    }

    public int CompareTo(GameTile other)
    {
        if (Pos.y > other.Pos.y) return 1;
        else if (Pos.y < other.Pos.y) return -1;
        else
        {
            if (Pos.x > other.Pos.x) return 1;
            else if (Pos.x < other.Pos.x) return -1;
            else return 0;
        };
    }
}
