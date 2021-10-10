using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Controller for an entity's inventory
 * 
 */
[Serializable]
public class Backpack
{
    public int size;
    public int indexSelected;
    [System.NonSerialized] public List<ItemExistanceDTOWrapper> items = new List<ItemExistanceDTOWrapper>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool createItem(ItemExistanceDTOWrapper in_item)
    {
        return addItem(in_item);
    }
    //public bool createItem(Item in_item)
    //{
    //    return addItem(in_item);
    //}

    public bool localCreateItem(string getItemName, int in_amount)
    {
        ItemExistanceDTOWrapper getItem = ItemFactory.createItem(getItemName);
        if (getItem == null) return false;
        getItem.ItemObj.quantity = in_amount;
        getItem.ItemObj.itemIndex = -1;

        return addItem(getItem);
    }

    public bool createItem(string in_binder, string getItemName, int in_amount)
    {
        ItemExistanceDTOWrapper getItem = ItemFactory.createItem(getItemName);
        if (getItem == null) return false;
        getItem.ItemObj.quantity = in_amount;
        getItem.ItemObj.itemIndex = -1;



        if (Network.isConnected)
        {
            Debug.Log(getItemName);
            ItemExistanceDTOWrapper out_item = items.Find(x => x.ItemObj.itemName.Equals(getItemName));
            if (out_item != null)
            {
                Debug.Log(out_item.ItemObj.quantity + " , " + in_amount);
                out_item.ItemObj.quantity += in_amount;
                Debug.Log(out_item.ItemObj.quantity + " , " + in_amount);
            }
            Network.itemUpdated(doCommands.playerItem, "Pickup Item", getItemName, in_amount);
//            Network.getNewItem(getItemName, in_amount);
            return false;
        }

        return addItem(getItem);
    }

    private bool addItem(ItemExistanceDTOWrapper in_item)
    {
        ItemExistanceDTOWrapper out_item = items.Find(x => x.ItemObj.itemName.Equals(in_item.ItemObj.itemName));
        if (out_item != null)
        {
            Debug.Log(out_item.ItemObj.quantity + " , " + in_item.ItemObj.quantity);
            out_item.ItemObj.quantity += in_item.ItemObj.quantity;
            return true;
        }
        else
        {
            if (items.Count < size)
            {
                items.Add(in_item);
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public bool modifyItem(ItemExistanceDTOWrapper in_item, int in_quantity)
    {
        in_item.ItemObj.quantity -= in_quantity;
        if (Network.isConnected)
        {
            Network.itemUpdated(doCommands.playerItem, "Pickup Item", in_item.ItemObj.itemName, -in_quantity);
//            Network.getNewItem(in_item.ItemObj.itemName, -in_quantity);
        }
        if (in_item.ItemObj.quantity < 1)
        {
            items.Remove(in_item);
            return false;
        }
        return true;
    }

    public bool localModifyItem(ItemExistanceDTOWrapper in_item, int in_quantity)
    {
        in_item.ItemObj.quantity -= in_quantity;
        if (in_item.ItemObj.quantity < 1)
        {
            items.Remove(in_item);
            return false;
        }
        return true;
    }

    public ItemExistanceDTOWrapper getItemByName(string in_item)
    {
        return items.Find(x => x.ItemObj.itemName.Equals(in_item));
    }
}
