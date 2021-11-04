using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class InGameListener : MonoBehaviour, IServerListener
{
    [SerializeField] PlayerController currentPlayer;
    [SerializeField] TimeSystem timeSystem;
    [SerializeField] GridSystem currentGrid;

    public GameObject npcList;
    static public Dictionary<string, NonPlayerController> characterTracker = new Dictionary<string, NonPlayerController>();

    public void serverResponseListener()
    {

        if (Network.worldRetrieved.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh world", 0);

            Network.loadedWorld = Network.worldRetrieved.Dequeue().getActual();
            timeSystem.currentWorld = Network.loadedWorld;

            Network.sendPacket(doCommands.load, "Config", currentGrid.area.areaName);
        }

        if (Network.areaConfig.Count > 0)
        {
            currentPlayer.teleportToDestination();
            DataCache.resetAll();

            currentPlayer.isLoading(true, "Loading teh area you're in", 0);


            AreaDTO temp_wrapper = Network.areaConfig.Dequeue();
            currentGrid.loadAreaConfig(temp_wrapper);

            Network.sendPacket(doCommands.load, "Area Indexes", currentGrid.area.areaName);
        }

        if (Network.listOfAreaIndexes.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh floor you stand on", 0);

            List<AreaIndexDTO> get_list = Network.listOfAreaIndexes.Dequeue();
            int count = 0;
            foreach (AreaIndexDTO it_areaIndex in get_list)
            {
                currentPlayer.isLoading(true, "Loading teh floor you stand on", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaHelper(it_areaIndex);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area Items", currentGrid.area.areaName);
        }

        if (Network.listOfAreaItems.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh things", 0);

            List<EntityExistanceDTO<ItemDTO>> get_list = Network.listOfAreaItems.Dequeue();
            int count = 0;
            foreach (EntityExistanceDTO<ItemDTO> it_areaItem in get_list)
            {
                currentPlayer.isLoading(true, "Loading teh things", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaItem(it_areaItem);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area Plants", currentGrid.area.areaName);
        }

        if (Network.listOfAreaPlants.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading mother's nature stoofs", 0);

            List<AreaPlantDTO> get_list = Network.listOfAreaPlants.Dequeue();
            int count = 0;
            foreach (AreaPlantDTO it_areaPlant in get_list)
            {
                currentPlayer.isLoading(true, "Loading mother's nature stoofs", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaPlant(it_areaPlant);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area NPCs", currentGrid.area.areaName);
        }

        if (Network.listOfAreaNPCs.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading everywuns", 0);

            List<EntityExistanceDTO<EntityDTO>> get_list = Network.listOfAreaNPCs.Dequeue();
            int count = 0;
            foreach (EntityExistanceDTO<EntityDTO> it_areaNpc in get_list)
            {
                currentPlayer.isLoading(true, "Loading everywun", (((float)count) / ((float)get_list.Count) * 100));
                //                currentGrid.loadAreaNPC(it_areaNpc);
                count++;
            }
            currentPlayer.isLoading(false);
            currentPlayer.loadTutorial();
        }


        if (Network.areaPlants.Count > 0)
        {
            AreaPlantDTO it_areaPlant = Network.areaPlants.Dequeue();
            currentGrid.loadAreaPlant(it_areaPlant);
        }

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
                case "Old day end":
                    currentPlayer.ts.setDayEnd();
                    break;
                case "New day begin":
                    currentPlayer.ts.setDayBegin();
                    currentPlayer.recover();
                    break;
                case "Action event":
                    currentPlayer.actionProgression(getResponse["message"], 2);
                    break;
                case "Error":
                    currentPlayer.toastNotifications.newNotification(getResponse["message"]);
                    break;
            }
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        if (Network.listOfUpdatedPlayers.Count > 0)
        {
            List<EntityExistanceDTO<EntityDTO>> entity_list = Network.listOfUpdatedPlayers.Dequeue();
            foreach (EntityExistanceDTO<EntityDTO> it_entity in entity_list)
            {
                if (it_entity.areaObj.areaName.Equals(currentGrid.area.areaName))
                {
                    if (characterTracker.TryGetValue(it_entity._id, out NonPlayerController out_npc))
                    {
                        out_npc.playerEntity = it_entity;
                        out_npc.lastUpdate = Time.time;
                        out_npc.currentObject.transform.position = it_entity.position;
                        out_npc.currentObject.transform.eulerAngles = new Vector2(0, it_entity.rotation.x);
                    }
                    else
                    {
                        GameObject temp_object = Instantiate(Resources.Load<GameObject>("NPC"), it_entity.position, Quaternion.identity);
                        if (temp_object.TryGetComponent<NonPlayerController>(out NonPlayerController out_NPC))
                        {
                            out_NPC.currentObject = temp_object;
                            temp_object.transform.SetParent(npcList.transform);
                            out_NPC.playerEntity = it_entity;
                            out_NPC.load();
                            out_NPC.lastUpdate = Time.time;
                            InGameListener.characterTracker.Add(it_entity._id, out_NPC);
                        }
                    }
                }
            }
        }

        if (Network.listOfItems.Count > 0)
        {
            List<ItemExistanceDTOWrapper> temp_wrapper = Network.listOfItems.Dequeue();
            if (temp_wrapper.Count > 0)
            {
                DataCache.getAreaItemByID(temp_wrapper[0].storageObj._id, out GameObject out_access_go, out string out_access_itemType);
                print(out_access_itemType);
                switch (out_access_itemType)
                {
                    case "Storage":
                        //                    print(it_item.storageObj._id + ": load " + out_itemType + " " + it_item.ItemObj._id);
                        out_access_go.TryGetComponent<StorageEntity>(out StorageEntity out_storage);
                        //                        out_storage.storage.inventory.createItem(it_item);
                        out_storage.storage.inventory.items = temp_wrapper;
                        //                    out_storage.storage.inventory.items.Add(it_item);
                        break;
                    case "Chopping Board":
                        //                    print(it_item.storageObj._id + ": load " + out_itemType + " " + it_item.ItemObj._id);
                        out_access_go.TryGetComponent<ChoppingBoard>(out ChoppingBoard out_choppingBoard);
                        out_choppingBoard.storage.inventory.items = temp_wrapper;
                        //                    out_choppingBoard.onBoard = it_item;
                        break;
                    case "Cooking Pot":
                        //                    print(it_item.storageObj._id + ": load " + out_itemType + " " + it_item.ItemObj._id);
                        out_access_go.TryGetComponent<cookingPot>(out cookingPot out_cookingPot);
                        out_cookingPot.storage.inventory.items = temp_wrapper;
                        //                    out_choppingBoard.onBoard = it_item;
                        break;
                }
            }
            //foreach (ItemExistanceDTOWrapper it_item in temp_wrapper)
            //{
            //    DataCache.getAreaItemByID(it_item.storageObj._id, out GameObject out_go, out string out_itemType);
            //}
        }

        //if (!currentPlayer.canvas.tradeMenu.gameObject.activeInHierarchy)
        //{
        if (Network.itemRetrieved.Count > 0)
        {
            ItemExistanceDTOWrapper getItem = Network.itemRetrieved.Peek();
            print("inGameListener-ServerResponse: " + getItem.ItemObj.itemName);
            if (getItem.binder != null)
            {
                getItem = Network.itemRetrieved.Dequeue();
                print("InGameListener : itemRetrieved] - " + currentPlayer.playerEntity.entityName + " , " + getItem.binder.entityName);
                if (currentPlayer.playerEntity.entityName.Equals(getItem.binder.entityName))
                {
                    //                    currentPlayer.playerEntity.backpack.createItem(getItem);

                    ItemExistanceDTOWrapper has_item = currentPlayer.playerEntity.backpack.items.Find(x => x.ItemObj.itemName == getItem.ItemObj.itemName);
                    //print(JsonConvert.SerializeObject(has_item, settings));
                    //print(JsonConvert.SerializeObject(getItem, settings));
                    if (has_item != null)
                    {

                        if (getItem.ItemObj.quantity <= 0)
                        {
                            currentPlayer.playerEntity.backpack.items.Remove(has_item);
                        }
                        else
                        {
                            has_item.ItemObj.quantity = getItem.ItemObj.quantity;
                        }
                    }
                    else
                    {
                        currentPlayer.playerEntity.backpack.items.Add(getItem);
                    }
                    if (currentPlayer.canvas.tradeMenu.gameObject.activeInHierarchy)
                    {
                        currentPlayer.canvas.tradeMenu.init();
                    }
                }
            }
            else if (getItem.storageObj != null)
            {
                getItem = Network.itemRetrieved.Dequeue();
                DataCache.getAreaItemByID(getItem.storageObj._id, out GameObject out_go, out string out_itemType);
                switch (out_itemType)
                {
                    case "Storage":
                        out_go.TryGetComponent<StorageEntity>(out StorageEntity out_storage);
                        out_storage.storage.inventory.refreshItem(getItem);
                        if (currentPlayer.canvas.tradeMenu.focusStorage.itemEntity.item.entityObj._id.Equals(getItem.storageObj._id))
                        {
                            currentPlayer.canvas.tradeMenu.init();
                        }
                        break;
                    case "Chopping Board":
                        out_go.TryGetComponent<ChoppingBoard>(out ChoppingBoard out_choppingBoard);
                        out_choppingBoard.storage.inventory.refreshItem(getItem);

                        if (out_choppingBoard.choppingStatusUI != null && out_choppingBoard.itemEntity.item.entityObj._id.Equals(getItem.storageObj._id))
                        {
                            if (out_choppingBoard.choppingStatusUI.TryGetComponent<chopStatus>(out chopStatus out_chop))
                            {
                                out_chop.init();
                            }
                        }
                        break;
                    case "Cooking Pot":
                        out_go.TryGetComponent<cookingPot>(out cookingPot out_cookingPot);
                        out_cookingPot.storage.inventory.refreshItem(getItem);

                        if (out_cookingPot.cookingStatusUI != null && out_cookingPot.itemEntity.item.entityObj._id.Equals(getItem.storageObj._id))
                        {
                            if (out_cookingPot.cookingStatusUI.TryGetComponent<cookingUI>(out cookingUI out_cooking))
                            {
                                out_cooking.init();
                            }
                        }
                        break;
                }
            }
        }
        //}
    }

    public void resetState()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        serverResponseListener();
    }

    public static void removeCharacter(string in_ID)
    {
        if (characterTracker.TryGetValue(in_ID, out NonPlayerController out_npc))
        {
            Destroy(out_npc.currentObject);
            characterTracker.Remove(in_ID);
        }
    }
}
