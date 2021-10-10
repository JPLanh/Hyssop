using System;
using UnityEngine;

/**
 * 
 * Wrapper class for all DTO
 * 
 */

[Serializable]
public class AreaDTO
{
    public WorldDTO worldObj;
    public string areaName;
    public int length;
    public int width;
    public int height;
    public bool buildable;
}

[Serializable]
public class WorldDTO
{
    public string worldName;
    [Range(0.0f, 1.0f)] public float time;
    public float fullDayLength;
    [Range(0.0f, 1.0f)] public float startTime;
    public float timeRate;
    public bool timeStopped;
    public bool isDayEnd;
    //    public Vector3 noon;
    public string owner;

    [Header("Time Configs")]
    public int dayEndHour;
    public int dayEndMinute;
    public int dayBeginHour;
    public int dayBeginMinute;

    public string Action;

    public World getActual()
    {
        World temp_world = new World();
        temp_world.worldName = worldName;
        temp_world.time = time;
        temp_world.fullDayLength = fullDayLength;
        temp_world.startTime = startTime;
        temp_world.timeRate = timeRate;
        temp_world.timeStopped = timeStopped;
        temp_world.isDayEnd = isDayEnd;
        //temp_world.noon = noon;
        temp_world.owner = owner;
        temp_world.Action = Action;

        temp_world.dayEndHour = dayEndHour;
        temp_world.dayEndMinute = dayEndMinute;
        temp_world.dayBeginHour = dayBeginHour;
        temp_world.dayBeginMinute = dayBeginMinute;

        return temp_world;
    }
}


[Serializable]
public class AreaIndexDTO
{
    public AreaDTO areaObj;
    public int x, y, z;
    public string objectName;
    public bool destructable;
    public bool pickable;
    public string state;

    public AreaIndex getActual()
    {
        return new AreaIndex(x, y, z, objectName, state, destructable, pickable);
    }
}

[Serializable]
public class AreaItemDTO
{
    public string _id;
    public ItemDTO itemObj;
    public Vector3 position;
    public Quaternion rotation;
    public AreaDTO areaObj;
}

[Serializable]
public class EntityExistanceDTO
{
    public string _id;
    public EntityDTO entityObj;
    public AreaDTO areaObj;
}

[Serializable]
public class AreaPlantDTO
{
    public AreaIndexDTO index;
    public string plantName;
    public string state;
    public int dayPassed;
    public int dayRequired;
    public int deathDayPassed;
    public int deathDayRequired;
    public bool isWatered;

    public Plant getPlant()
    {
        Plant new_plant = new Plant();
        new_plant.x = index.x;
        new_plant.y = index.y;
        new_plant.z = index.z;
        new_plant.areaName = index.areaObj.areaName;
        new_plant.plantName = plantName;
        new_plant.state = state;
        new_plant.dayPassed = dayPassed;
        new_plant.dayRequired = dayRequired;
        new_plant.deathDayPassed = deathDayPassed;
        new_plant.deathDayRequired = deathDayRequired;
        new_plant.isWatered = isWatered;
        return new_plant;
    }
}