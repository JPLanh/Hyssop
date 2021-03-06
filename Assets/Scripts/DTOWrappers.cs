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

    public Area getActual()
    {
        Area temp_area = new Area();
        temp_area.areaName = areaName;
        temp_area.length = length;
        temp_area.width = width;
        temp_area.height = height;
        temp_area.buildable = buildable;
        return temp_area;
    }
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
public class EntityDTO
{
    public string _id;
    public string entityName;
    public Vector3 position;
    public Vector3 rotation;
    public string areaName;
    public Backpack backpack = new Backpack();
    public string occupation;
    public int stamina;
    public int maxStamina;
    public string state;
    public string holding;
    public string currentAnimal;
    public float primary_currentRed;
    public float primary_currentGreen;
    public float primary_currentBlue;
    public float secondary_currentRed;
    public float secondary_currentGreen;
    public float secondary_currentBlue;
    [System.NonSerialized]
    public Int64 time;

    public Entity getActual()
    {
        Entity temp_entity = new Entity();
        temp_entity._id = _id;
        temp_entity.entityName = entityName;
        temp_entity.position = position;
        temp_entity.rotation = rotation;
        temp_entity.areaName = areaName;
        temp_entity.backpack = backpack;
        temp_entity.occupation = occupation;
        temp_entity.stamina = stamina;
        temp_entity.maxStamina = maxStamina;
        temp_entity.state = state;
        temp_entity.holding = holding;
        temp_entity.time = time;
        temp_entity.currentAnimal = currentAnimal;
        temp_entity.primary_currentBlue = primary_currentBlue;
        temp_entity.primary_currentGreen = primary_currentGreen;
        temp_entity.primary_currentRed = primary_currentRed;
        temp_entity.secondary_currentBlue = secondary_currentBlue;
        temp_entity.secondary_currentGreen = secondary_currentGreen;
        temp_entity.secondary_currentRed = secondary_currentRed;
        return temp_entity;
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
public class AreaItemDTO : ITransferer
{
    public string _id;
    public ItemDTO entityObj;
    public Vector3 position;
    public Quaternion rotation;
    public AreaDTO areaObj;

    public string getID()
    {
        return _id;
    }

    public Backpack getInventory()
    {
        throw new NotImplementedException();
    }

    public string getType()
    {
        return "Storage";
    }
}

[Serializable]
public class EntityExistanceDTO<T> : ITransferer
{
    public string _id;
    public T entityObj;
    public Vector3 position;
    public Vector3 rotation;
    public Area areaObj;

    public string getID()
    {
        if (typeof(T) == typeof(EntityDTO))
        {
            return _id;
        }
        else if (typeof(T) == typeof(Entity))
        {
            return _id;
        }
        else if (typeof(T) == typeof(ItemDTO))
        {
            return (entityObj as ItemDTO)._id;
        }
        Debug.LogError("DTOWrapper ID is null");
        return null;
    }

    public Backpack getInventory()
    {
        if (typeof(T) == typeof(EntityDTO))
        {
            return (entityObj as EntityDTO).backpack;
        } else if (typeof(T) == typeof(Entity))
        {
            return (entityObj as Entity).backpack;
        }
        Debug.LogError("DTOWrapper Inventory is null");
        return null;
    }

    public string getType()
    {
        if (typeof(T) == typeof(EntityDTO))
        {
            return "Entity";
        }
        else if (typeof(T) == typeof(Entity))
        {
            return "Entity";
        }
        if (typeof(T) == typeof(ItemDTO))
        {
            return "Storage";
        }
        Debug.LogError("DTOWrapper Type is null");
        return null;
    }
}

[Serializable]
public class CharacterAccountDTO
{
    public EntityDTO entityObj;
    public AreaDTO areaObj;
    public Vector3 position;
    public Quaternion rotation;
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