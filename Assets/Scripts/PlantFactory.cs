using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantFactory : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Transform plantList;
    [SerializeField] TimeSystem ts;



    private void Start()
    {
        //addToJson("Coffee cherry", "Coffee bean", 0, 5, 0, 6);
        //addToJson("Corn", "Corn seed", 0, 5, 0, 6);
        //addToJson("Wheat", "Wheat seed", 0, 5, 0, 6);
        //addToJson("Carrot", "Carrot seed", 0, 5, 0, 6);
        //addToJson("Potato", "Potato tuber", 0, 5, 0, 6);
        //addToJson("Lettuce", "Lettuce seed", 0, 5, 0, 6);
        //addToJson("Cabbage", "Cabbage seed", 0, 5, 0, 6);
        //addToJson("Tomato", "Tomato seed", 0, 5, 0, 6);
        //addToJson("Green onion", "Green onion bulb", 0, 5, 0, 6);
        //addToJson("Garlic", "Garlic clove", 0, 5, 0, 6);
        //addToJson("Rice", "Rice seedling", 0, 5, 0, 6 );
        //DataCache.savePlants();
    }

    void OnApplicationQuit()
    {

        DataCache.saveAllCrops();
    }

    public static void addToJson(string in_plant, string in_seed, int in_dayPass, int in_dayReq, int in_deathDayPass, int in_deathDayReq)
    {
        DataCache.plantCache.Add(in_seed, new Plant(in_plant, in_seed, in_dayPass, in_dayReq, in_deathDayPass, in_deathDayReq));
    }


    public static Plant retrievePlant(string getName)
    {
        return new Plant(DataCache.getPlant(getName));
    }


    public static void newCrop(Plant in_plant)
    {
        if (DataCache.inPlayPlants.ContainsKey(in_plant.areaName))
        {
            DataCache.inPlayPlants[in_plant.areaName].Add(in_plant);
        }
        else
        {
            List<Plant> temp_list = new List<Plant>();
            temp_list.Add(in_plant);
            DataCache.inPlayPlants.Add(in_plant.areaName, temp_list);
        }
        //string UIDKey = in_plant.areaName + " " + in_plant.x + " " + in_plant.y + " " + in_plant.z;
        ////        print(UIDKey + " , " + in_plant);
        //DataCache.inPlayPlants.Add(UIDKey, in_plant);
    }

    public static void removeCrop(Plant in_plant)
    {
        //Fix this
        print(in_plant.areaName);
        DataCache.inPlayPlants[in_plant.areaName].Remove(in_plant);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void checkCrops()
    {
        foreach (KeyValuePair<string, List<Plant>> it_list in DataCache.inPlayPlants)
        {
            foreach (Plant it_plant in it_list.Value)
            {
                string UIDKey = it_plant.areaName + " " + it_plant.x + " " + it_plant.y + " " + it_plant.z;
            }

        }
    }
    public static void dayFinished(GridSystem in_grid)
    {
        foreach (KeyValuePair<string, List<Plant>> it_list in DataCache.inPlayPlants)
        {
            foreach (Plant it_plant in it_list.Value)
            {
                if (it_plant.isWatered)
                {
                    it_plant.dayPassed += 1;
                }
                else
                {
                    it_plant.deathDayPassed += 1;
                }

                it_plant.isWatered = false;

                if (it_plant.deathDayPassed >= it_plant.deathDayRequired)
                {
                    it_plant.state = "Dead";
                }
                if (it_plant.dayPassed >= it_plant.dayRequired)
                {
                    it_plant.state = "Grown";
                }
                //if (in_grid.areaName.Equals(it_plant.areaName))
                //{
                //    if (in_grid.currentGrid[it_plant.x, it_plant.y, it_plant.z].index.TryGetComponent<Soil>(out Soil out_soil)){
                //        out_soil.loadPlant(it_plant);
                //    }
                //}
            }
        }
    }

    public void dayBegin()
    {
        throw new NotImplementedException();
    }


    //public static void LoadJson()
    //{
    //    if (listOfItems == null)
    //    {
    //        listOfItems = DataUtility.loadItems();
    //    }
    //}

    //public static void writeJson()
    //{
    //    DataUtility.saveItems(listOfItems);
    //}

    //public static void addToJson(string getName, string getType)
    //{
    //    LoadJson();
    //    listOfItems.listOfItems.Add(new ItemDAO(getName, getType));
    //    writeJson();
    //}
}


[Serializable]
public class PlantDAOWrapper
{
    public List<Plant> listOfPlants = new List<Plant>();
}