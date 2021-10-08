using System;

/*
 * 
 * Configuration settings for the game
 * 
 */
public static class Configurations
{
    public static localConfig currentConfigs;
    public static bool isHost;
    public static float textSpeed;
}


[Serializable]
public class localConfig
{
    public float time;
    public bool timeStop;
    public bool isLoaded;
    public string IV;
}