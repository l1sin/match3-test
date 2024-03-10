using System;

[Serializable]
public class Progress
{
    public float SFXVolume;
    public float MusicVolume;
    public int LevelsComplete;
    public LevelInfo[] LevelData;

    public Progress()
    {
        SFXVolume = 0.5f;
        MusicVolume = 0.5f;
        LevelData = new LevelInfo[15];
    }
}

[Serializable]
public struct LevelInfo
{
    public int Stars;
    public int HighScore;
}
