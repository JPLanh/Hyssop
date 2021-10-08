using UnityEngine;

public class StorageFactory : MonoBehaviour
{

    void OnApplicationQuit()
    {
        DataCache.saveAllStorage();
    }

    //Create a new data for the NPC and insert it into the cache
    public static Storage createNewStorage(string in_name, Vector3 in_position, Quaternion in_rotation, string in_type, string in_area)
    {
        Storage new_storage = new Storage();
        new_storage.areaName = in_area;
        new_storage.storageType = in_type;
        new_storage.inventory = new Backpack();
        new_storage.position = in_position;
        new_storage.rotation = in_rotation;
        new_storage.state = "Unlocked";
        new_storage.storageName = in_name;
        DataCache.addNewStorage(in_area, new_storage);
        return new_storage;
    }

    //Instantiate a new NPC into the area
    public static GameObject generateNewStorage(Vector3 in_position, Quaternion in_rotation, string in_name, string in_type, string in_area)
    {
        Storage temp_storage = createNewStorage(in_name, in_position, in_rotation, in_type, in_area);
        GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(in_name), in_position, in_rotation);
        if (temp_Obj.TryGetComponent<StorageEntity>(out StorageEntity out_entity))
        {
            out_entity.storage = temp_storage;

        }

        temp_Obj.name = in_name;
        return temp_Obj;
    }

    //Load an exisisting NPC into the area
    public static GameObject loadStorage(Storage in_storage, string in_area)
    {
        GameObject temp_Obj = Instantiate(Resources.Load<GameObject>(in_storage.storageName), in_storage.position, in_storage.rotation);
        temp_Obj.name = in_storage.storageName;
        if (temp_Obj.TryGetComponent<StorageEntity>(out StorageEntity out_entity))
        {
            out_entity.storage = in_storage;
        }
        return temp_Obj;
    }

    public void dayBegin()
    {
        throw new System.NotImplementedException();
    }
}