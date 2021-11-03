using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Cache that holds game object to reduce the usage of GameObject.find();
 * 
 */
public class DataCache : MonoBehaviour
{
    private static Dictionary<string, GameObject> areaItems = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> areaStorage = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> areaChoppingBoard = new Dictionary<string, GameObject>();

    //Local stuff

    public static Dictionary<string, Plant> plantCache = new Dictionary<string, Plant>();
    public static Dictionary<string, Item> itemCache = new Dictionary<string, Item>();
    public static Dictionary<string, List<Plant>> inPlayPlants = new Dictionary<string, List<Plant>>();
    public static Dictionary<string, List<Storage>> inPlayStorages = new Dictionary<string, List<Storage>>();
    //    public static Dictionary<string, Item> inPlayPlayerItem = new Dictionary<string, Item>();
    public static Dictionary<string, List<Entity>> inPlayNPC = new Dictionary<string, List<Entity>>();
    public static Dictionary<string, List<Item>> inPlayAreaItem = new Dictionary<string, List<Item>>();

    public static Dictionary<string, Item> seedToProduceMap = new Dictionary<string, Item>();
    public static Dictionary<string, Item> produceToSeedMap = new Dictionary<string, Item>();

    public static List<ITimeListener> timeActionCache = new List<ITimeListener>();
    public static List<Entity> listOfCharacters = new List<Entity>();

    public static localConfig localConfig;

    public static Entity loadedCharacter;

    public static void addNewAreaItem(EntityExistanceDTO<ItemDTO> in_item, string item_type, GameObject in_go)
    {
        switch (item_type)
        {
            case "Storage":
                areaStorage.Add(in_item.entityObj._id, in_go);
                break;
            case "Chopping Board":
                areaChoppingBoard.Add(in_item.entityObj._id, in_go);
                break;
            default:
                areaItems.Add(in_item._id, in_go);
                break;
        }

    }

    public static void getAreaItemByID(string in_id, out GameObject out_go, out string out_itemType)
    {
        areaItems.TryGetValue(in_id, out out_go);
        out_itemType = "Area Item";
        if (out_go == null)
        {
            areaStorage.TryGetValue(in_id, out out_go);
            out_itemType = "Storage";
        }
        if (out_go == null)
        {
            areaChoppingBoard.TryGetValue(in_id, out out_go);
            out_itemType = "Chopping Board";
        }
    }
    public static void resetAll()
    {
        areaItems.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        //loadItem();
        //loadPlant();
        //loadCrops();
    }

    public void OnDestroy()
    {
        //saveItems();
        //savePlants();
        //saveAllCrops();
    }

    public static Entity load(string in_name)
    {
        //loadItem();
        //loadPlant();
        //loadCrops();
        //loadNPCList();
        return DataUtility.loadEntityPlayer(in_name);
    }

    public static void saveNewCharacter(Entity in_entity)
    {
        listOfCharacters.Add(in_entity);
        DataUtility.saveEntityPlayer(in_entity);
    }

    public static void deleteCharacter(Entity in_entity)
    {
        listOfCharacters.Remove(in_entity);
        DataUtility.deleteCharacter(in_entity.entityName);
    }
    public static void loadAllCharacter()
    {
        DataUtility.loadAllCharacter();
    }

    public static void loadItem()
    {
        ItemDAOWrapper loadedItemDao = DataUtility.loadDatabaseItems();
        foreach (ItemDAO it_item in loadedItemDao.listOfItems)
        {
            itemCache.Add(it_item.itemName, new Item(it_item));
        }
    }

    public static void saveItems()
    {
        ItemDAOWrapper newWrapper = new ItemDAOWrapper();
        if (itemCache.Count > 0)
        {
            foreach (KeyValuePair<string, Item> it_item in itemCache)
            {
                newWrapper.listOfItems.Add(new ItemDAO(it_item.Value));

            }
            DataUtility.saveDatabaseItems(newWrapper);
        }
    }
    public static void adjustMarketPrice(Item in_item)
    {
        //y = ab^x
        //float a = Mathf.Pow(itemCache[in_item.itemName].priceRoof, ((float)itemCache[in_item.itemName].itemRoof/ ((float)itemCache[in_item.itemName].itemRoof-1)));
        //float b = itemCache[in_item.itemName].priceRoof / a;
        //in_item.buyPrice = (int) (a*Mathf.Pow(b, (float)in_item.quantity));
        //in_item.sellPrice = (int)(a * Mathf.Pow(b, (float)in_item.quantity));
    }

    public static void loadPlant()
    {
        PlantDAOWrapper loadedItemDao = DataUtility.loadDatabasePlants();
        foreach (Plant it_item in loadedItemDao.listOfPlants)
        {
            plantCache.Add(it_item.seedName, it_item);
        }
    }

    public static void loadItemMappings()
    {
        foreach (KeyValuePair<string, Plant> it_item in plantCache)
        {
            seedToProduceMap.Add(it_item.Value.seedName, itemCache[it_item.Value.plantName]);
            produceToSeedMap.Add(it_item.Value.plantName, itemCache[it_item.Value.seedName]);
        }

    }

    public static void loadAllPlants()
    {
        DataUtility.loadAllEntityPlants();
    }

    public static void loadAllStorages()
    {
        DataUtility.loadAllStorages();
    }

    public static void saveAllStorage()
    {
        DataUtility.saveAllStorage();
    }
    public static void saveAllCrops()
    {
        DataUtility.saveAllEntityPlants();
        //foreach (KeyValuePair<string, List<Plant>> it_list in inPlayPlants)
        //{
        //    PlantDAOWrapper newWrapper = new PlantDAOWrapper();
        //    foreach (Plant it_plant in it_list.Value)
        //    {
        //        newWrapper.listOfPlants.Add(it_plant);
        //    }
        //    DataUtility.saveEntityPlants(newWrapper);
        //}
    }

    public static void saveAllNPC()
    {
        DataUtility.saveAllNPC();
    }

    public static void loadAllNPC()
    {
        DataUtility.loadAllNPC();
    }

    public static void savePlants()
    {
        PlantDAOWrapper newWrapper = new PlantDAOWrapper();
        if (plantCache.Count > 0)
        {
            foreach (KeyValuePair<string, Plant> it_plant in plantCache)
            {
                newWrapper.listOfPlants.Add(it_plant.Value);

            }
            DataUtility.saveDatabasePlants(newWrapper);
        }
    }

    public static Item getItem(string in_item)
    {
        return itemCache[in_item];
    }

    public static Plant getPlant(string in_plant)
    {
        return plantCache[in_plant];
    }

    public static void loadAllAreaItem()
    {
        DataUtility.loadAllAreaItem();
    }

    //public static void createNewPlayerItem(Item in_item)
    //{
    //    if (inPlayPlayerItem.ContainsKey(in_item.binder + " " + in_item.mode + " " + in_item.itemName)){
    //        inPlayPlayerItem[in_item.binder + " " + in_item.mode + " " + in_item.itemName].quantity += in_item.quantity;
    //    } else 
    //        inPlayPlayerItem.Add(in_item.binder + " " + in_item.mode + " " + in_item.itemName, in_item);
    //}

    public static void createNewAreaItem(Item in_item)
    {
        if (inPlayAreaItem.TryGetValue(in_item.binder, out List<Item> listOfitems))
        {
            listOfitems.Add(in_item);
        }
        else
        {
            List<Item> tmp_list = new List<Item>();
            tmp_list.Add(in_item);
            inPlayAreaItem.Add(in_item.binder, tmp_list);
        }
    }

    public static void saveAreaItems(string in_name)
    {
        ItemListWrapper tmp_wrapper = new ItemListWrapper();
        foreach (KeyValuePair<string, List<Item>> it_list in inPlayAreaItem)
        {
            if (it_list.Key.Contains(in_name))
            {
                foreach (Item it_item in it_list.Value)
                {
                    tmp_wrapper.listOfItems.Add(it_item);
                }
            }
        }
        DataUtility.saveAreaItem(in_name, tmp_wrapper);
    }

    public static void addNewNPC(string in_area, Entity in_npc)
    {
        if (inPlayNPC.ContainsKey(in_area))
        {
            inPlayNPC[in_area].Add(in_npc);
        }
        else
        {
            List<Entity> temp_list = new List<Entity>();
            temp_list.Add(in_npc);
            inPlayNPC[in_area] = temp_list;
        }
    }

    public static void addNewStorage(string in_area, Storage in_storage)
    {
        if (inPlayStorages.ContainsKey(in_area))
        {
            inPlayStorages[in_area].Add(in_storage);
        }
        else
        {
            List<Storage> temp_list = new List<Storage>();
            temp_list.Add(in_storage);
            inPlayStorages[in_area] = temp_list;
        }
    }

    public static void addNewPlant(string in_area, Plant in_plant)
    {
        if (inPlayPlants.ContainsKey(in_area))
        {
            inPlayPlants[in_area].Add(in_plant);
        }
        else
        {
            List<Plant> temp_list = new List<Plant>();
            temp_list.Add(in_plant);
            inPlayPlants[in_area] = temp_list;
        }
    }

    //public static void saveEntityItems(string in_name, Inventory in_player, Inventory in_creator)
    //{
    //    ItemWrapper tmp_wrapper = new ItemWrapper();
    //    foreach (GameObject go in in_player.toolbars)
    //    {
    //        if (go.TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            if (getHotbar.getItem() != null)
    //                tmp_wrapper.listOfItems.Add(new Item(getHotbar.getItem()));
    //        }
    //    }
    //    DataUtility.saveEntityItem(in_name, "Player", tmp_wrapper);

    //    tmp_wrapper = new ItemWrapper();
    //    foreach (GameObject go in in_creator.toolbars)
    //    {
    //        if (go.TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            if (getHotbar.getItem() != null)
    //                tmp_wrapper.listOfItems.Add(new Item(getHotbar.getItem()));
    //        }
    //    }
    //    DataUtility.saveEntityItem(in_name, "Creator", tmp_wrapper);

    //foreach (KeyValuePair<string, Item> it_item in inPlayPlayerItem)
    //{
    //    if (it_item.Key.Contains(in_name) && it_item.Key.Contains("Player"))
    //    {
    //        tmp_wrapper.listOfItems.Add(it_item.Value);
    //    }
    //}
    //DataUtility.saveEntityItem(in_name, "Player", tmp_wrapper);

    //tmp_wrapper = new ItemWrapper();
    //foreach (KeyValuePair<string, Item> it_item in inPlayPlayerItem)
    //{
    //    if (it_item.Key.Contains(in_name) && it_item.Key.Contains("Creator"))
    //    {
    //        tmp_wrapper.listOfItems.Add(it_item.Value);
    //    }
    //}
    //DataUtility.saveEntityItem(in_name, "Creator", tmp_wrapper);
    //}
    //Obsolete
    //public static void saveEntityItems(string in_name)
    //{
    //    ItemWrapper tmp_wrapper = new ItemWrapper();
    //    foreach (KeyValuePair<string, Item> it_item in inPlayPlayerItem)
    //    {
    //        if (it_item.Key.Contains(in_name) && it_item.Key.Contains("Player"))
    //        {
    //            tmp_wrapper.listOfItems.Add(it_item.Value);
    //        }
    //    }
    //    DataUtility.saveEntityItem(in_name, "Player", tmp_wrapper);

    //    tmp_wrapper = new ItemWrapper();
    //    foreach (KeyValuePair<string, Item> it_item in inPlayPlayerItem)
    //    {
    //        if (it_item.Key.Contains(in_name) && it_item.Key.Contains("Creator"))
    //        {
    //            tmp_wrapper.listOfItems.Add(it_item.Value);
    //        }
    //    }
    //    DataUtility.saveEntityItem(in_name, "Creator", tmp_wrapper);
    //}


    //public static void newNPC(NPC in_npc)
    //{
    //    if (inPlayNPC.TryGetValue(in_npc.areaName, out List<npcDao> listOfNPC)){
    //        listOfNPC.Add(new npcDao(in_npc));
    //    } else
    //    {
    //        List<npcDao> tmp_list = new List<npcDao>();
    //        tmp_list.Add(new npcDao(in_npc));
    //        inPlayNPC.Add(in_npc.areaName, tmp_list);
    //    }
    //}

    //public static void saveNPC()
    //{
    //    NPCWrapper tmp_wrapper = new NPCWrapper();
    //    foreach (KeyValuePair<string, List<npcDao>> it_list in inPlayNPC)
    //    {
    //        foreach(npcDao it_npc in it_list.Value)
    //        {
    //            tmp_wrapper.listOfNPC.Add(it_npc);
    //        }
    //    }
    //        DataUtility.saveNPC(tmp_wrapper);
    //}

    //public static void loadNPCList()
    //{
    //    NPCWrapper tmp_wrapper = DataUtility.loadNPC();
    //    if (tmp_wrapper != null)
    //    {
    //        foreach (npcDao it_npc in tmp_wrapper.listOfNPC)
    //        {
    //            if (inPlayNPC.TryGetValue(it_npc.areaName, out List<npcDao> listOfNPC)){
    //                listOfNPC.Add(it_npc);
    //            } else
    //            {
    //                List<npcDao> tmp_list = new List<npcDao>();
    //                tmp_list.Add(it_npc);
    //                inPlayNPC.Add(it_npc.areaName, tmp_list);
    //            }
    //        }
    //    }
    //}

    //public static List<npcDao> getListOfNPCAtArea(string in_area)
    //{
    //    if (inPlayNPC.TryGetValue(in_area, out List<npcDao> listOfNPC))
    //    {
    //        return inPlayNPC[in_area];
    //    } else
    //    {
    //        return new List<npcDao>();
    //    }
    //}
    // Update is called once per frame
    void Update()
    {

    }


    public static void dayFinished()
    {
        storageDayEnd();
        npcDayEnd();
    }

    private static void storageDayEnd()
    {
        foreach (KeyValuePair<string, List<Storage>> out_list in inPlayStorages)
        {
            Storage isMail = null;
            Storage isBin = null;
            foreach (Storage out_storage in out_list.Value)
            {
                if (out_storage.storageType.Equals("Shipping Bin"))
                {
                    isBin = out_storage;
                }
                else if (out_storage.storageType.Equals("Mailbox"))
                {
                    isMail = out_storage;
                }
                out_storage.dayEnd();
            }

            if (isBin != null)
            {
                foreach (ItemExistanceDTOWrapper it_item in isBin.inventory.items)
                {
                    Storage.transfer(isBin, isMail, it_item);
                }
                isBin.inventory.items.Clear();
            }
        }
    }

    private static void npcDayEnd()
    {
        foreach (KeyValuePair<string, List<Entity>> out_list in inPlayNPC)
        {
            foreach (Entity out_entity in out_list.Value)
            {
                out_entity.dayEnd();
            }
        }
    }

    public void dayBegin()
    {
        throw new System.NotImplementedException();
    }
}


[Serializable]
public class itemDatabaseWrapper
{
    public List<ItemDTO> itemList;
    public string Action;
    public int index;
    public int total;
}
[Serializable]
public class itemDatabase
{
    public string itemName;
    public string itemType;
    public int maxDurability;
    public int maxCapacity;
}

[Serializable]
public class plantDatabaseWrapper
{
    public List<PlantDTO> plantList;
    public string Action;
    public int index;
    public int total;
}
public class plantDatabase
{
    public string plantName;
    public string seedName;
    public int dayRequired;
    public int deathDayRequired;
}