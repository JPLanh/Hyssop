using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class World
{
    public string worldName;
    [Range(0.0f, 1.0f)] public float time;
    public float fullDayLength;
    [Range(0.0f, 1.0f)] public float startTime;
    public float timeRate;
    public bool timeStopped;
    //public Vector3 noon;
    public string owner;
    public bool isDayEnd;

    [Header("Time Configs")]
    public int dayEndHour;
    public int dayEndMinute;
    public int dayBeginHour;
    public int dayBeginMinute;

    public string Action;


    public int getMinute()
    {
        return (int)(time * 1440);
    }

    public string getTime()
    {
        return string.Format("{0,2:D2}", ((int)(getMinute() / 60)) % 12) + ":" + string.Format("{0,2:D2}", ((int)(getMinute() % 60))) + " " + (((int)(getMinute() / 60)) >= 12 ? "PM" : "AM");
    }

    public WorldDTO getDTO()
    {
        WorldDTO temp_world = new WorldDTO();
        temp_world.worldName = worldName;
        temp_world.time = time;
        temp_world.fullDayLength = fullDayLength;
        temp_world.startTime = startTime;
        temp_world.timeRate = timeRate;
        temp_world.timeStopped = timeStopped;
        //      temp_world.noon = noon;
        temp_world.owner = owner;
        temp_world.isDayEnd = isDayEnd;

        temp_world.dayEndHour = dayEndHour;
        temp_world.dayEndMinute = dayEndMinute;
        temp_world.dayBeginHour = dayBeginHour;
        temp_world.dayBeginMinute = dayBeginMinute;
        return temp_world;

    }
}


[Serializable]
public class WorldListWrapper
{
    public List<WorldDTO> worldList = new List<WorldDTO>();
    public int index;
    public int total;
    public string Action;
}