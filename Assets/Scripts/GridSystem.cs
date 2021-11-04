using Socket.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Core script for each area and indices
 * 
 */
public class GridSystem : MonoBehaviour
{
    //public string areaName;
    //public int width;
    //public int height;
    //public int length;
    //public bool buildable;

    public Area area = new Area();

    [Header("Game Object Lists")]
    [SerializeField] private GameObject gameObjectList;
    [SerializeField] private Transform itemList;
    [SerializeField] private Transform plantList;
    [SerializeField] private Transform NPCList;
    [SerializeField] private CanvasHelper canvasUI;


    private int size = 1;
    public GameObject getGridType;
    public GameObject plane;

    public TimeSystem ts;

    public AreaIndex[,,] currentGrid;

    [SerializeField] private GridSystem tempGrid;
    public gridIndexDaoWrapper tempWrapperHolder;

    void Start()
    {
    }
    #region init

    //Buffer rendering 
    public bool loadPreloadGrid()
    {
        unloadGrid();
        loadArea(tempGrid);
        loadLocalPlants();
        loadItemInArea();
        spawnNPCInArea();
        loadStorageInArea();
        return true;
    }
     
    //Load current area that is designed by the area name variable
    public bool loadArea()
    {
        if (Network.isConnected)
        {
            Network.sendPacket(doCommands.load, "World");
        }
        else
        {
            gridIndexDaoWrapper getAreaIndices = DataUtility.loadArea(area.areaName);
            if (loadConfig(area.areaName, getAreaIndices.length, getAreaIndices.width, getAreaIndices.height, getAreaIndices.buildable))
            {
                loadAreaHelper(getAreaIndices.listOfgrids);
                loadLocalPlants();
                loadItemInArea();
                spawnNPCInArea();
                loadStorageInArea();
                return true;
            }
        }
        return false;
    }

    //Preload the area without anything else
    public void loadAreaConfig(AreaDTO getAreaIdices)
    {
        loadConfig(area.areaName, getAreaIdices.length, getAreaIdices.width, getAreaIdices.height, getAreaIdices.buildable);
        generateIndexes();
    }

    public void loadAreaItem(EntityExistanceDTO<ItemDTO> it_itemDTO)
    {
        GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(it_itemDTO.entityObj.itemName), it_itemDTO.position, Quaternion.identity);
        temp_Obj.transform.eulerAngles = new Vector2(it_itemDTO.rotation.x, it_itemDTO.rotation.y);
        temp_Obj.name = it_itemDTO._id;
        temp_Obj.transform.SetParent(gameObjectList.transform);
        if (temp_Obj.TryGetComponent<ItemEntity>(out ItemEntity out_itemEntity))
        {
            out_itemEntity.item = it_itemDTO;
        }
        if (temp_Obj.TryGetComponent<Bed>(out Bed out_bed))
        {
            out_bed.ts = ts;
        }
        if (temp_Obj.TryGetComponent<ITimeListener>(out ITimeListener out_timeListener))
        {
            DataCache.timeActionCache.Add(out_timeListener);
        }
        if (temp_Obj.TryGetComponent<ICanvas>(out ICanvas out_canvas))
        {
            out_canvas.getCanvas(canvasUI);
        }

        if (temp_Obj.TryGetComponent<StorageEntity>(out StorageEntity out_storageEntity))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["storageID"] = it_itemDTO.entityObj._id;
            Network.sendPacket(doCommands.storage, "Access", payload);
            DataCache.addNewAreaItem(it_itemDTO, "Storage", temp_Obj);
            print(it_itemDTO.entityObj._id + ": Storage");
        }
        if (temp_Obj.TryGetComponent<ChoppingBoard>(out ChoppingBoard out_choppingBoard))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["storageID"] = it_itemDTO.entityObj._id;
            Network.sendPacket(doCommands.storage, "Access", payload);
            DataCache.addNewAreaItem(it_itemDTO, "Chopping Board", temp_Obj);
            print(it_itemDTO.entityObj._id + ": Chopping Board");
        }
        if (temp_Obj.TryGetComponent<cookingPot>(out cookingPot out_cookingPot))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["storageID"] = it_itemDTO.entityObj._id;
            Network.sendPacket(doCommands.storage, "Access", payload);
            DataCache.addNewAreaItem(it_itemDTO, "Cooking Pot", temp_Obj);
            print(it_itemDTO.entityObj._id + ": Cooking Pot");
        }
    }

    public void loadAreaNPC(EntityExistanceDTO<EntityDTO> it_dto)
    {
            GameObject new_npc = Instantiate(Resources.Load<GameObject>("NPC"), it_dto.position, Quaternion.identity);
            avatarProperties new_npc_properties = new avatarProperties(it_dto.entityObj.currentAnimal);
            new_npc_properties.primary_currentRed = it_dto.entityObj.primary_currentRed;
            new_npc_properties.primary_currentGreen = it_dto.entityObj.primary_currentGreen;
            new_npc_properties.primary_currentBlue = it_dto.entityObj.primary_currentBlue;
            new_npc_properties.secondary_currentBlue = it_dto.entityObj.secondary_currentBlue;
            new_npc_properties.secondary_currentRed = it_dto.entityObj.secondary_currentRed;
            new_npc_properties.secondary_currentGreen = it_dto.entityObj.secondary_currentGreen;
            new_npc_properties.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/" + it_dto.entityObj.currentAnimal), new Vector3(0f, 0f, 0f), Quaternion.identity);
            new_npc_properties.currentAvatar.transform.SetParent(new_npc.transform);
            new_npc_properties.currentAvatar.transform.localPosition = new Vector3(0f, 0f, 0f);
            new_npc.transform.SetParent(NPCList);
            new_npc.name = it_dto._id;
            if (new_npc_properties.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
            {
                new_npc_properties.current_avatarEntity = out_avatarEntity;
        }
            new_npc.transform.eulerAngles = new Vector2(0, it_dto.rotation.x);
        new_npc_properties.current_avatarEntity.setAllColor(new Color(new_npc_properties.primary_currentRed / 255f, new_npc_properties.primary_currentGreen / 255f, new_npc_properties.primary_currentBlue / 255f), "Primary");
            new_npc_properties.current_avatarEntity.setAllColor(new Color(new_npc_properties.secondary_currentRed / 255f, new_npc_properties.secondary_currentGreen / 255f, new_npc_properties.secondary_currentBlue / 255f), "Secondary");
            shopCheck(new_npc, it_dto);
    }

    private void shopCheck(GameObject in_GO, EntityExistanceDTO<EntityDTO> in_entity)
    {
        switch (in_entity.entityObj.entityName)
        {
            case "Izak":
            case "Trevik":
                Shop temp_shop = in_GO.AddComponent<Shop>();
                temp_shop.currentNPC = in_entity.entityObj.getActual();
                break;

        }
    }

    //Local preload
    public GridSystem preloadGrid(string in_name)
    {
        tempGrid.area.areaName = in_name;

        tempGrid.tempWrapperHolder = DataUtility.loadArea(in_name);
        if (tempGrid.loadConfig(in_name, tempGrid.tempWrapperHolder.length, tempGrid.tempWrapperHolder.width, tempGrid.tempWrapperHolder.height, tempGrid.tempWrapperHolder.buildable))
        {
            return tempGrid;
        }
        else
        {
            return null;
        }

    }

    public void createNewGrid(int in_length, int in_width, int in_height, string in_name)
    {
        unloadGrid();
        area.width = in_width + 2;
        area.height = in_height;
        area.length = in_length + 2;
        area.areaName = in_name;

        currentGrid = new AreaIndex[area.length, area.width, area.height];
    }

    public void generateEmptyGrid(string in_mode, int in_length, int in_width, int in_height, string in_name)
    {
        createNewGrid(in_length, in_width, in_height, in_name);
        generateIndexes();
        generateBorder();
        saveArea(in_mode);
    }

    //Local saving of area, will need to come back to this when implementing offline
    public void saveArea(string in_mode)
    {
        if (Network.isConnected)
        {
            //            Network.doSave("Area", new gridIndexDaoWrapper(length, width, height, currentGrid, buildable));
        }
        else
        {
            DataUtility.saveArea(area.length, area.width, area.height, currentGrid, area.areaName, area.buildable);
        }
    }

    public bool loadConfig(string in_area, int in_length, int in_width, int in_height, bool in_buildable)
    {
        unloadGrid();
        area.areaName = in_area;
        area.width = in_width;
        area.height = in_height;
        area.length = in_length;
        area.buildable = in_buildable;

        currentGrid = new AreaIndex[area.length, area.width, area.height];
        return true;
    }


    public bool loadArea(GridSystem in_grid)
    {

        area.areaName = in_grid.area.areaName;
        area.width = in_grid.area.width;
        area.height = in_grid.area.height;
        area.length = in_grid.area.length;
        area.buildable = in_grid.area.buildable;

        currentGrid = new AreaIndex[area.length, area.width, area.height];

        loadAreaHelper(in_grid.tempWrapperHolder.listOfgrids);
        return true;
    }

    public void loadLocalPlants()
    {
        if (DataCache.inPlayPlants.ContainsKey(area.areaName))
        {
            foreach (Plant it_plant in DataCache.inPlayPlants[area.areaName])
            {
                {
                    if (currentGrid[it_plant.x, it_plant.y, it_plant.z].index.TryGetComponent<Soil>(out Soil out_soil))
                    {
                        out_soil.loadPlant(it_plant);
                    }
                }
            }
        }
    }

    public void unloadGrid()
    {
        if (gameObjectList != null)
        {
            foreach (Transform obj in gameObjectList.transform)
            {
                Destroy(obj.gameObject);
            }
            foreach (Transform obj in NPCList)
            {
                Destroy(obj.gameObject);
            }
            area.areaName = null;
        }
    }

    #endregion


    #region generators
    public void generateIndexes()
    {

        //print(length + " , " + width + " , " + height);
        for (int x = 0; x < area.length; x++)
        {
            for (int y = 0; y < area.width; y++)
            {
                for (int z = 0; z < area.height; z++)
                {
                    //print(x + " , " + y + " , " + z);
                    currentGrid[x, y, z] = new AreaIndex(x, y, z, null, null, false, false);
                    //Debug.DrawLine(getGridAtLocationOffset(x - 1, y - 1, z - 1), getGridAtLocationOffset(x - 1, y, z - 1), Color.red, 100f);
                    //Debug.DrawLine(getGridAtLocationOffset(x - 1, y - 1, z - 1), getGridAtLocationOffset(x, y - 1, z - 1), Color.red, 100f);
                    //Debug.DrawLine(getGridAtLocationOffset(x - 1, y - 1, z - 1), getGridAtLocationOffset(x - 1, y - 1, z), Color.red, 100f);
                }
                //Debug.DrawLine(getGridAtLocationOffset(x - 1, y - 1, 0), getGridAtLocationOffset(x - 1, y, 0), Color.red, 100f);
                //Debug.DrawLine(getGridAtLocationOffset(x - 1, y - 1, 0), getGridAtLocationOffset(x, y - 1, 0), Color.red, 100f);
            }
        }
    }

    public void unloadIndex(AreaIndex in_index)
    {
        in_index.index = null;
        in_index.objectName = null;
        in_index.state = null;
    }

    public void generateBorder()
    {
        for (int row = 0; row < area.length; row++)
        {
            if (row == 0 || row == area.length - 1)
            {
                for (int column = 0; column < area.width; column++)
                {
                    generateObject(currentGrid[row, column, 0], "Border", false, false);
                    generateObject(currentGrid[row, column, 2], "Border", false, false);
                }
            }
            else
            {
                generateObject(currentGrid[row, 0, 0], "Border", false, false);
                generateObject(currentGrid[row, 0, 2], "Border", false, false);
                generateObject(currentGrid[row, area.width - 1, 0], "Border", false, false);
                generateObject(currentGrid[row, area.width - 1, 2], "Border", false, false);
            }
        }
    }

    public void generateParameter(string in_input)
    {
        for (int row = 1; row < area.length - 1; row++)
        {
            if (row == 1 || row == area.length - 2)
            {
                for (int column = 2; column < area.width - 2; column++)
                {
                    generateObject(currentGrid[row, column, 0], in_input, false, false);
                    generateObject(currentGrid[row, column, 1], in_input, false, false);
                }
            }
            else
            {
                generateObject(currentGrid[row, 1, 0], in_input, false, false);
                generateObject(currentGrid[row, 1, 1], in_input, false, false);

                generateObject(currentGrid[row, area.width - 2, 0], in_input, false, false);
                generateObject(currentGrid[row, area.width - 2, 1], in_input, false, false);
            }
        }
    }
    /*
     * Y + 1 == West
     * Y - 1 == East
     * X + 1 == North
     * X - 1 == South
     */

    #endregion


    private void loadAreaHelper(List<AreaIndex> in_loadArea)
    {
        foreach (AreaIndex gi in in_loadArea)
        {
            //            print(gi.x + " , " + gi.y + " , " + gi.z + " , " + gi.objectName);
            currentGrid[gi.x, gi.y, gi.z] = new AreaIndex(gi.x, gi.y, gi.z, null, gi.state, gi.destructable, gi.pickable);
            if (gi.z == 0)
            {
                //Debug.DrawLine(getGridAtLocationOffset(gi.x - 1, gi.y - 1, gi.z), getGridAtLocationOffset(gi.x - 1, gi.y, gi.z), Color.red, 100f);
                //Debug.DrawLine(getGridAtLocationOffset(gi.x - 1, gi.y - 1, gi.z), getGridAtLocationOffset(gi.x, gi.y - 1, gi.z), Color.red, 100f);

            }
            if (!gi.objectName.Equals(""))
            {
                generateObject(currentGrid[gi.x, gi.y, gi.z], gi.objectName, gi.destructable, gi.pickable);
                if (!gi.state.Equals(""))
                {
                    if (currentGrid[gi.x, gi.y, gi.z].index.TryGetComponent<Soil>(out Soil getSoil))
                    {
                        getSoil.currentGridIndex = currentGrid[gi.x, gi.y, gi.z];
                        getSoil.setState(gi.state);
                    }
                }
            }
        }
    }

    public void loadAreaPlant(AreaPlantDTO temp_plant)
    {
        //Error NullReferenceException: Object reference not set to an instance of an object
        if (currentGrid[temp_plant.index.x, temp_plant.index.y, temp_plant.index.z].index.TryGetComponent<Soil>(out Soil out_soil))
        {
            if (out_soil.plant == null || string.IsNullOrEmpty(out_soil.plant.plantName))
            {
                Plant get_plant = temp_plant.getPlant();
                out_soil.loadPlant(get_plant);
                if (DataCache.inPlayPlants.TryGetValue(area.areaName, out List<Plant> out_list))
                {
                    out_list.Add(get_plant);
                }
                else
                {
                    List<Plant> new_list = new List<Plant>();
                    new_list.Add(get_plant);
                    DataCache.inPlayPlants.Add(area.areaName, new_list);
                }
            }
            else
            {
                if (temp_plant.plantName.Equals(out_soil.plant.plantName))
                {
                    out_soil.plant.updatePlant(temp_plant);
                }
                else
                {
                }
            }
        }

    }

    public void loadAreaHelper(AreaIndexDTO gi)
    {
        if (gi.state.Equals("Clear"))
        {
            removeObjectAtIndex(currentGrid[gi.x, gi.y, gi.z]);
            currentGrid[gi.x, gi.y, gi.z] = null;
        }
        else
        {
            currentGrid[gi.x, gi.y, gi.z] = new AreaIndex(gi.x, gi.y, gi.z, null, gi.state, gi.destructable, gi.pickable);
            if (gi.z == 0)
            {
                //Debug.DrawLine(getGridAtLocationOffset(gi.x - 1, gi.y - 1, gi.z), getGridAtLocationOffset(gi.x - 1, gi.y, gi.z), Color.red, 100f);
                //Debug.DrawLine(getGridAtLocationOffset(gi.x - 1, gi.y - 1, gi.z), getGridAtLocationOffset(gi.x, gi.y - 1, gi.z), Color.red, 100f);

            }
            if (!gi.objectName.Equals(""))
            {
                generateObject(currentGrid[gi.x, gi.y, gi.z], gi.objectName, gi.destructable, gi.pickable);
                if (currentGrid[gi.x, gi.y, gi.z].index.TryGetComponent<Soil>(out Soil getSoil))
                {
                    getSoil.currentGridIndex = currentGrid[gi.x, gi.y, gi.z];
                    getSoil.setState(gi.state);
                }
            }
        }

    }

    //Local generation NPC in the area, in creator mode
    public void spawnNewNPC(Vector3 in_position, Vector3 in_rotation, string in_name, string in_type)
    {
        GameObject temp_npc = NPCFactory.generateNewNPC(in_position, in_rotation, in_name, in_type, area.areaName);
        temp_npc.transform.SetParent(NPCList);
    }

    //Local generation Storage in the area, in creator mode
    public GameObject spawnNewStorage(Vector3 in_position, Quaternion in_rotation, string in_name, string in_type)
    {
        GameObject temp_storage = StorageFactory.generateNewStorage(in_position, in_rotation, in_name, in_type, area.areaName);
        temp_storage.transform.SetParent(itemList);
        return temp_storage;
    }

    //Local spawning of npc into the area
    public void spawnNPCInArea()
    {
        if (DataCache.inPlayNPC.ContainsKey(area.areaName))
        {
            foreach (Entity it_npc in DataCache.inPlayNPC[area.areaName])
            {
                GameObject temp_npc = NPCFactory.loadNPC(it_npc, area.areaName);
                temp_npc.transform.SetParent(NPCList);
            }
        }
    }

    //Local spawning of storage into the area
    public void loadStorageInArea()
    {
        if (DataCache.inPlayStorages.ContainsKey(area.areaName))
        {
            foreach (Storage it_storage in DataCache.inPlayStorages[area.areaName])
            {
                GameObject temp_npc = StorageFactory.loadStorage(it_storage, area.areaName);
                temp_npc.transform.SetParent(NPCList);
            }
        }
    }

    public Vector3 getGridAtLocation(int getX, int getY, int getZ)
    {
        return new Vector3(getX, getZ, getY) * size;
        //        return new Vector3(getX, 5, getY) * size + new Vector3(-395f, 0, -395f);
    }

    public Vector3 getGridAtLocationOffset(int getX, int getY, int getZ)
    {
        return new Vector3(getX + .5f, getZ, getY + .5f) * size;
        //        return new Vector3(getX, 5, getY) * size + new Vector3(-395f, 0, -395f);
    }

    public AreaIndex getIndex(Vector3 worldPosition)
    {
        int x, y, z;
        getXY(worldPosition, out x, out y, out z);
        if (x >= 0 && x < area.length && y >= 0 && y < area.width && z >= 0 && z < area.height)
            return currentGrid[x, y, z];
        else
            return null;
    }

    public AreaIndex getIndex(int in_x, int in_y, int in_z)
    {
        return currentGrid[in_x, in_y, in_z];
    }

    public void getXY(Vector3 worldPosition, out int x, out int y, out int z)
    {
        x = Mathf.FloorToInt((worldPosition.x + .5f) / size);
        y = Mathf.FloorToInt((worldPosition.z + .5f) / size);
        z = Mathf.FloorToInt((worldPosition.y + .5f) / size);
    }

    public Dictionary<string, AreaIndex> getNeighbors(AreaIndex in_currentIndex)
    {
        Dictionary<string, AreaIndex> neighbors = new Dictionary<string, AreaIndex>();
        if (in_currentIndex.y + 1 < area.width && in_currentIndex.y + 1 >= 0)
        {
            neighbors.Add("West", currentGrid[in_currentIndex.x, in_currentIndex.y + 1, in_currentIndex.z]);
            if (in_currentIndex.x + 1 < area.length)
                neighbors.Add("Northwest", currentGrid[in_currentIndex.x + 1, in_currentIndex.y + 1, in_currentIndex.z]);
            if (in_currentIndex.x - 1 >= 0)
                neighbors.Add("Southwest", currentGrid[in_currentIndex.x - 1, in_currentIndex.y + 1, in_currentIndex.z]);
        }
        if (in_currentIndex.y - 1 >= 0 && in_currentIndex.y - 1 < area.width)
        {
            neighbors.Add("East", currentGrid[in_currentIndex.x, in_currentIndex.y - 1, in_currentIndex.z]);
            if (in_currentIndex.x + 1 < area.length)
                neighbors.Add("Northeast", currentGrid[in_currentIndex.x + 1, in_currentIndex.y - 1, in_currentIndex.z]);
            if (in_currentIndex.x - 1 >= 0)
                neighbors.Add("Southeast", currentGrid[in_currentIndex.x - 1, in_currentIndex.y - 1, in_currentIndex.z]);
        }
        if (in_currentIndex.x + 1 < area.length)
            neighbors.Add("North", currentGrid[in_currentIndex.x + 1, in_currentIndex.y, in_currentIndex.z]);
        if (in_currentIndex.x - 1 >= 0)
            neighbors.Add("South", currentGrid[in_currentIndex.x - 1, in_currentIndex.y, in_currentIndex.z]);

        return neighbors;
    }

    public GameObject generateObjectHelper(AreaIndex in_index, string in_object)
    {
        Vector3 tempPos = getGridAtLocation(in_index.x, in_index.y, in_index.z);
        GameObject tempObj = Instantiate(Resources.Load<GameObject>(in_object), tempPos, Quaternion.identity);
        tempObj.name = tempObj.name.Replace("(Clone)", "");
        tempObj.transform.SetParent(gameObjectList.transform);
        return tempObj;
    }

    public void generateObject(AreaIndex in_index, string in_object, bool in_destructable, bool in_pickable)
    {
        removeObjectAtIndex(in_index);
        in_index.index = generateObjectHelper(in_index, in_object);
        if (in_index.index.TryGetComponent<Bed>(out Bed out_bed))
        {
            print("Ts set");
            out_bed.ts = ts;
        }

        in_index.objectName = in_object;
        in_index.destructable = in_destructable;
        in_index.pickable = in_pickable;

        buildCheck(in_index, true);
    }

    public void spawnNPC(Vector3 in_position, string in_name, string in_type)
    {
        ////GameObject tmp_obj = generateObjectHelper(in_index, in_type);
        //Entity tmp_item = NPCFactory.createNewNPC(in_name, in_type); //ItemFactory.createItem(in_item);
        //tmp_item.binder = areaName;
        //DataCache.createNewAreaItem(tmp_item);
        //GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(tmp_item.itemName), position, rotation);
        //temp_Obj.name = temp_Obj.name.Replace("(Clone)", "");
        //temp_Obj.transform.SetParent(gameObjectList.transform);
        //tmp_item.position = temp_Obj.transform.position;
        //tmp_item.rotation = temp_Obj.transform.rotation;
        //temp_Obj.name = in_item;
        //tmp_obj.name = in_name;
        //if (tmp_obj.TryGetComponent<NPC>(out NPC out_npc))
        //{
        //    out_npc.position = tmp_obj.transform.position;
        //    out_npc.rotation = tmp_obj.transform.rotation;
        //    out_npc.areaName = areaName;
        //    out_npc.newNPC();
        //    out_npc.save();
        //}
    }

    public Item spawnItem(string in_item, Vector3 position, Quaternion rotation)
    {
        //ItemExistanceDTOWrapper tmp_item = ItemFactory.createItem(in_item);
        //tmp_item.binder = areaName;
        //DataCache.createNewAreaItem(tmp_item);
        //GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(tmp_item.itemName), position, rotation);
        //temp_Obj.name = temp_Obj.name.Replace("(Clone)", "");
        //temp_Obj.transform.SetParent(gameObjectList.transform);
        //tmp_item.position = temp_Obj.transform.position;
        //tmp_item.rotation = temp_Obj.transform.rotation;
        //temp_Obj.name = in_item;
        //if (tmp_item.itemType.Equals("Door"))
        //{
        //    tmp_item.state = "Open";
        //}
        //if (temp_Obj.TryGetComponent<ItemEntity>(out ItemEntity out_itemEntity))
        //{
        //    out_itemEntity.item = tmp_item;
        //}
        //if (temp_Obj.TryGetComponent<Bed>(out Bed out_bed))
        //{
        //    out_bed.ts = ts;
        //}
        //DataCache.saveAreaItems(areaName);
        //return tmp_item;
        return null;
    }

    public void loadItemInArea()
    {
        //if (DataCache.inPlayAreaItem != null)
        //{
        //    if (DataCache.inPlayAreaItem.TryGetValue(area.areaName, out List<Item> out_list))
        //    {
        //        foreach (Item it_item in out_list)
        //        {
        //            GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(it_item.itemName), it_item.position, it_item.rotation);
        //            temp_Obj.name = temp_Obj.name.Replace("(Clone)", "");
        //            temp_Obj.transform.SetParent(gameObjectList.transform);
        //            if (temp_Obj.TryGetComponent<ItemEntity>(out ItemEntity out_itemEntity))
        //            {
        //                out_itemEntity.item = it_item;
        //            }
        //            if (temp_Obj.TryGetComponent<Bed>(out Bed out_bed))
        //            {
        //                out_bed.ts = ts;
        //            }
        //        }
        //    }
        //}
    }

    public void removeObjectAtIndex(AreaIndex in_index)
    {
        if (in_index != null)
        {
            Destroy(in_index.index);
        }
        in_index.objectName = null;
        in_index.index = null;
    }

    public void buildCheck(AreaIndex in_index, bool connect)
    {
        if (connect)
        {
            if (in_index.index.TryGetComponent<IConnectable>(out IConnectable out_connectable))
            {
                foreach (KeyValuePair<string, AreaIndex> out_dict in getNeighbors(in_index))
                {
                    if (out_dict.Value != null)
                    {
                        if (out_dict.Value.index != null)
                        {
                            out_connectable.connectionCheck(in_index, out_dict.Value, out_dict.Key);
                            //                if (out_dict.Value.index.TryGetComponent<IStructure>(out IStructure getCurrentStructure))

                        }

                    }
                }
            }
        }
        else
        {
            if (in_index.index.TryGetComponent<IConnectable>(out IConnectable out_connectable))
            {
                foreach (KeyValuePair<string, AreaIndex> out_dict in getNeighbors(in_index))
                {
                    if (out_dict.Value != null)
                    {
                        if (out_dict.Value.index != null)
                        {
                            out_connectable.connectionCut(in_index, out_dict.Value, out_dict.Key);
                            //                if (out_dict.Value.index.TryGetComponent<IStructure>(out IStructure getCurrentStructure))

                        }

                    }
                }
            }

        }
    }

}

public class Area
{
    public string areaName;
    public int length;
    public int width;
    public int height;
    public bool buildable;

    public AreaDTO toDTO()
    {
        AreaDTO newWrapper = new AreaDTO();
        newWrapper.areaName = areaName;
        newWrapper.length = length;
        newWrapper.width = width;
        newWrapper.height = height;
        newWrapper.buildable = buildable;
        return newWrapper;
    }
}
[Serializable]
public class AreaIndex
{
    public int x, y, z;
    public string objectName;
    public bool destructable;
    public bool pickable;
    public string state;

    [System.NonSerialized] public GameObject index;

    //public AreaIndex(int in_x, int in_y, int in_z, GameObject in_obj, string in_state, bool in_destructable, bool in_pickable)
    //{
    //    x = in_x;
    //    y = in_y;
    //    z = in_z;
    //    index = in_obj;
    //    destructable = in_destructable;
    //    state = in_state;
    //    pickable = in_pickable;
    //}
    [JsonConstructor]
    public AreaIndex(int in_x, int in_y, int in_z, string in_name, string in_state, bool in_destructable, bool in_pickable)
    {
        x = in_x;
        y = in_y;
        z = in_z;
        objectName = in_name;
        destructable = in_destructable;
        state = in_state;
        pickable = in_pickable;
    }

    override
    public string ToString()
    {
        return x + " , " + y + " , " + z;
    }

    public AreaIndexDTO toDTO()
    {
        AreaIndexDTO newWrapper = new AreaIndexDTO();
        newWrapper.x = x;
        newWrapper.y = y;
        newWrapper.z = z;
        newWrapper.objectName = objectName;
        newWrapper.destructable = destructable;
        newWrapper.pickable = pickable;
        newWrapper.state = state;
        return newWrapper;
    }
}

[Serializable]
public class gridIndexWrapper
{
    public string state;
    public string areaName;
    public AreaIndex Data;
}

[Serializable]
public class areaItemWrapper
{
    public AreaIndex index;
    public Item item;
}

[Serializable]
public class gridIndexDaoWrapper
{
    public int length, width, height;
    public List<AreaIndex> listOfgrids = new List<AreaIndex>();
    public bool buildable;
    public string areaName;

    public gridIndexDaoWrapper() { }
    public gridIndexDaoWrapper(int in_length, int in_width, int in_height, AreaIndex[,,] in_grid, bool is_buildable)
    {
        length = in_length;
        width = in_width;
        height = in_height;
        buildable = is_buildable;

        foreach (AreaIndex gi in in_grid)
        {
            listOfgrids.Add(gi);
        }
    }
}
[Serializable]
public class gridIndexDtoWrapper
{
    public int length, width, height;
    public List<AreaIndexDTO> areaIndices = new List<AreaIndexDTO>();
    public bool buildable;
    public int index;
    public int total;
    public string areaName;
    public string Action;

    public gridIndexDaoWrapper getDAO()
    {
        gridIndexDaoWrapper new_wrapper = new gridIndexDaoWrapper();
        new_wrapper.length = length;
        new_wrapper.width = width;
        new_wrapper.height = height;
        new_wrapper.areaName = areaName;
        return new_wrapper;
    }
}

[Serializable]
public class ActionWrapper
{
    public string action;
    public AreaIndex index;
    public string areaName;
}
