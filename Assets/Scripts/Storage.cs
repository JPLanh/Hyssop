using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Storage
{
    public string areaName;
    public string storageName;
    public string storageType;
    public string state;
    public Backpack inventory;
    public Vector3 position;
    public Quaternion rotation;

    public void dayEnd()
    {
        if (storageType.Equals("Shipping Bin"))
        {
            for (int index = 0; index < inventory.items.Count; index++)
            {
                //Item it_item = inventory.items[index];
                //Item marketValue = DataCache.getItem(it_item.itemName);
                //if (marketValue.sellPrice > 0)
                //{
                //    //Tempoary, remove this later once fully implemented with resturant
                //    DataCache.itemCache[it_item.itemName].quantity += it_item.quantity;
                //    if (DataCache.produceToSeedMap.TryGetValue(it_item.itemName, out Item out_seed))
                //    {
                //        out_seed.quantity += it_item.quantity;
                //    }
                //    //End of temporary
                //    marketValue.quantity += it_item.quantity;
                //    Debug.Log(it_item.quantity + " " + it_item.itemName + " has been sold for " + marketValue.sellPrice + " each, totaling " + marketValue.sellPrice * it_item.quantity);
                //    inventory.createItem(storageName, "Silver", marketValue.sellPrice * it_item.quantity);
                //    DataCache.adjustMarketPrice(it_item);
                //    inventory.items.Remove(it_item);
                //}
            }
        }
    }

    public static void transfer(Storage in_from, Storage in_to, ItemExistanceDTOWrapper in_item)
    {
        if (in_to.inventory.createItem(in_item))
        {
            Debug.Log(in_item.ItemObj.itemName + " has been transfered from " + in_from.storageName + " to " + in_to.storageName);
        }
    }
}

[Serializable]
public class storageWrapper
{
    public ItemDTO itemObj;
    public Storage storageObj;
}

[Serializable]
public class StorageItemListWrapper
{
    public List<storageWrapper> storageList;
    public string Action;
    public int index;
    public int total;
}
[Serializable]
public class StorageWrapper
{
    public List<Storage> listOfStorage = new List<Storage>();
}
