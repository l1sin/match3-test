using System;

[Serializable]
public class Progress
{
    public int LevelsComplete;
    public LevelInfo[] LevelData;

    public Progress()
    {
        LevelData = new LevelInfo[15];
    }
}

[Serializable]
public struct LevelInfo
{
    public int Stars;
    public int HighScore;
}
