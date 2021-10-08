using System;
using System.Collections.Generic;

[Serializable]
public class Plant
{
    public int x, y, z;
    public string areaName;
    public string plantName;
    public string seedName;
    public string state;
    public int dayPassed;
    public int dayRequired;
    public int deathDayPassed;
    public int deathDayRequired;
    public bool isWatered;

    public Plant(string in_plant, string in_state, int in_dayPass, int in_dayReq, int in_deathDayPass, int in_deathDayReq, bool watered)
    {
        plantName = in_plant;
        state = in_state;
        dayPassed = in_dayPass;
        dayRequired = in_dayReq;
        deathDayPassed = in_deathDayPass;
        deathDayRequired = in_deathDayReq;
        isWatered = watered;
    }

    public Plant(Plant in_plant)
    {

        plantName = in_plant.plantName;
        seedName = in_plant.seedName;
        state = in_plant.state;
        dayPassed = in_plant.dayPassed;
        dayRequired = in_plant.dayRequired;
        deathDayPassed = in_plant.deathDayPassed;
        deathDayRequired = in_plant.deathDayRequired;
        isWatered = in_plant.isWatered;
    }
    public Plant(PlantDTO in_plant)
    {

        plantName = in_plant.plantName;
        state = in_plant.state;
        seedName = in_plant.seedName;
        dayPassed = in_plant.dayPassed;
        dayRequired = in_plant.dayRequired;
        deathDayPassed = in_plant.deathDayPassed;
        deathDayRequired = in_plant.deathDayRequired;
        isWatered = in_plant.isWatered;
    }

    public Plant(string in_plant, string in_seed, int in_dayPass, int in_dayReq, int in_deathDayPass, int in_deathDayReq)
    {
        plantName = in_plant;
        seedName = in_seed;
        dayPassed = in_dayPass;
        dayRequired = in_dayReq;
        deathDayPassed = in_deathDayPass;
        deathDayRequired = in_deathDayReq;
    }

    public void updatePlant(AreaPlantDTO in_plant)
    {
        plantName = in_plant.plantName;
        state = in_plant.state;
        dayPassed = in_plant.dayPassed;
        dayRequired = in_plant.dayRequired;
        deathDayPassed = in_plant.deathDayPassed;
        deathDayRequired = in_plant.deathDayRequired;
        isWatered = in_plant.isWatered;
    }

    public Plant(){}

    override
    public string ToString()
    {
        return areaName + " " + x + " " + y + " " + z;
    }
}
[Serializable]
public class PlantDTO
{
    public string plantName;
    public string seedName;
    public string state;
    public int dayPassed;
    public int dayRequired;
    public int deathDayPassed;
    public int deathDayRequired;
    public bool isWatered;

    public Plant getActual()
    {
        return new Plant(this);

    }
}
[Serializable]
public class gridIndexPlantWrapper
{
    public AreaIndex index;
    public PlantDTO plant;
    public string plantName;
    public string state;
    public int dayPassed;
    public int dayRequired;
    public int deathDayPassed;
    public int deathDayRequired;
    public bool isWatered;

    public Plant getPlant()
    {
        if (plant != null && !string.IsNullOrEmpty(plant.plantName))
            return new Plant(plant);
        else
            return new Plant(plantName, state, dayPassed, dayRequired, deathDayPassed, deathDayRequired, isWatered);
    }
}

[Serializable]
public class allGridIndexPlantWrapper
{
    public List<gridIndexPlantWrapper> listOfPlants;
    public int index;
    public int total;
    public string Action;
}