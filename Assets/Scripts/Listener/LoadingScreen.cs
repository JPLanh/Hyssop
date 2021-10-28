using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour, IServerListener
{
    [SerializeField] Text prompt;
    [SerializeField] private GridSystem gridSystem;

    void OnApplicationQuit()
    {
        Network.socket.Disconnect();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (!Network.isConnected)
        {
            prompt.text = "Loading item from database";
            DataCache.loadItem();

            prompt.text = "Loading plants from database";
            DataCache.loadPlant();

            prompt.text = "Loading item mappings";
            DataCache.loadItemMappings();

            prompt.text = "Updating Market Price";
            foreach (KeyValuePair<string, Item> it_item in DataCache.itemCache)
            {
                DataCache.adjustMarketPrice(it_item.Value);
            }


            if (!DataUtility.areaDataExists())
            {
            }
            else
            {

                prompt.text = "Loading Entities";
                DataCache.loadAllNPC();

                DataCache.loadAllAreaItem();

                DataCache.loadAllStorages();

                prompt.text = "Loading planted crops";
                DataCache.loadAllPlants();

            }

            if (!DataUtility.areaExists("Central Hub"))
            {
                AreaGenerator.generateCentralHub("Offline", gridSystem, 50, 25, 10, "Central Hub");
            }

            if (!DataUtility.areaExists(DataCache.loadedCharacter.entityName + "_farm"))
            {
                AreaGenerator.generateBasicPlayerFarm("Offline", gridSystem, 25, 50, 10, DataCache.loadedCharacter.entityName + "_farm");
            }
            SceneManager.LoadScene("MainGame");
        }
        else
        {
            //Load online version
            //            areaGenerator.generateCentralHub("Online", gridSystem, 50, 25, 10, "Central Hub");
            //Network.doLoading("Check Farm");
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["entityID"] = Network.loadedCharacter._id;
            Network.sendPacket(doCommands.preload, "Generate farm", payload);
            //print(Network.loadedCharacter._id);
            //print(Network.loadedCharacter.areaObj);
            //print(Network.loadedCharacter.areaObj.areaName);
        }


    }

    // Update is called once per frame
    void Update()
    {
        serverResponseListener();
    }


    public void serverResponseListener()
    {

        if (Network.serverAcknowledge.Count > 0)
        {
            Dictionary<string, string> getResponse = Network.serverAcknowledge.Dequeue();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            switch (getResponse["action"])
            {
                case "Farm generated":
                    payload["entity"] = Network.loadedCharacter.entityObj.entityName;
                    Network.sendPacket(doCommands.player, "Items", payload);
                    break;
            }
        }

        if (Network.areaConfig.Count > 0)
        {
            AreaDTO get_area = Network.areaConfig.Dequeue();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["entity"] = Network.loadedCharacter.entityObj.entityName;
            Network.sendPacket(doCommands.player, "Items", payload);
            if (Network.loadedCharacter.areaObj == null)
                Network.loadedCharacter.areaObj = get_area.getActual();
        }

        if (Network.characterQueue.Count > 0)
        {
            Network.loadedCharacter = Network.characterQueue.Dequeue();
        }

        if (Network.listOfItems.Count > 0)
        {
            List<ItemExistanceDTOWrapper> temp_wrapper = Network.listOfItems.Dequeue();

            foreach (ItemExistanceDTOWrapper it_item in temp_wrapper)
            {
                Network.loadedCharacter.entityObj.backpack.items.Add(it_item);
            }
            Network.sendPacket(doCommands.database, "Items");
        }

        if (Network.itemDatabase.Count > 0)
        {
            List<ItemDTO> temp_wrapper = Network.itemDatabase.Dequeue();
            foreach (ItemDTO it_item in temp_wrapper)
            {
                DataCache.itemCache.Add(it_item.itemName, it_item.getActual());
            }
            Network.sendPacket(doCommands.database, "Plants");
        }

        if (Network.plantDatabase.Count > 0)
        {
            List<PlantDTO> temp_wrapper = Network.plantDatabase.Dequeue();
            foreach (PlantDTO it_plant in temp_wrapper)
            {
                DataCache.plantCache.Add(it_plant.seedName, it_plant.getActual());
            }

            SceneManager.LoadScene("MainGame");
        }

        //if (Network.listOfCharacersItem.Count > 0)
        //{
        //    ItemExistanceWrapper temp_wrapper = Network.listOfCharacersItem.Dequeue();
        //    if (temp_wrapper.Action.Equals("Complete")) Network.doLoading("Get item database");
        //    else
        //    {
        //        foreach (ItemExistanceDTOWrapper it_item in temp_wrapper.itemList)
        //        {
        //            Network.loadedCharacter.backpack.items.Add(it_item);
        //        }
        //    }
        //}

        //if (Network.listOfItemDatabase.Count > 0)
        //{
        //    //            print("list of Item");
        //    itemDatabaseWrapper temp_wrapper = Network.listOfItemDatabase.Dequeue();
        //    if (temp_wrapper.Action.Equals("Complete")) Network.doLoading("Get plant database");
        //    else
        //    {
        //        foreach (ItemDTO it_item in temp_wrapper.itemList)
        //        {
        //            DataCache.itemCache.Add(it_item.itemName, it_item.getActual());
        //        }
        //    }
        //}

        //if (Network.listOfPlantDatabase.Count > 0)
        //{
        //    //            print("list of Plant");
        //    plantDatabaseWrapper temp_wrapper = Network.listOfPlantDatabase.Dequeue();
        //    if (temp_wrapper.Action.Equals("Complete")) SceneManager.LoadScene("MainGame");
        //    else
        //    {
        //        foreach (PlantDTO it_item in temp_wrapper.plantList)
        //        {
        //            DataCache.plantCache.Add(it_item.seedName, it_item.getActual());
        //        }
        //    }
        //}

        //if (Network.serverResponse.Count > 0)
        //{
        //    Dictionary<string, string> getResponse = Network.serverResponse.Dequeue();

        //    switch (getResponse["Action"])
        //    {
        //        case "Farm generated":
        //            Network.doLoading("Get Inventory");
        //            break;
        //        case "Load items":
        //            Network.doLoading("Get item database");
        //            break;
        //        case "Load plants":
        //            Network.doLoading("Get plant database");
        //            break;
        //        case "Loaded Successful":
        //            SceneManager.LoadScene("MainGame");
        //            break;
        //    }
        //}
    }
}
