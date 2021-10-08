using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject[] toolbars;
    public GameObject[] pendingItem;
    public Hotbar currentSelect;
    public int currentIndex = -999;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void loadInventory()
    {
        if (toolbars.Length == 0)
        {
            toolbars = new GameObject[8];
            for (int index = 0; index < 8; index++)
            {
                toolbars[index] = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
                toolbars[index].transform.SetParent(transform);
                Vector3 tempPos = new Vector3(-135 + 40 * index, 25, 0);
                toolbars[index].transform.localPosition = tempPos;
                toolbars[index].transform.localScale = new Vector3(1f, 1f, 1f);
                toolbars[index].name = "Inventory " + index;
                if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
                {
                    getHotbar.action = "Select " + index;
                }
            }
        }
    }

    public void transferAllItemTo(Inventory in_inventory)
    {
        for (int index = 0; index < toolbars.Length; index++)
        {
            if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
            {
                if (in_inventory.toolbars[index].TryGetComponent<Hotbar>(out Hotbar getToHotbar))
                {
                    getToHotbar.hotbarItem = getHotbar.hotbarItem;
                }
            }
        }
    }

    public void updateToolbar()
    {
        for (int index = 0; index < 8; index++)
        {
            if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
            {
                getHotbar.updateTexts();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public Item getItemByIndex(int index)
    //{
    //    if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //    {
    //        return getHotbar.getItem();
    //    }
    //    return null;
    //}

    public ItemDTO getItemByName(string getItem)
    {
        foreach (GameObject eachItem in toolbars)
        {
            if (eachItem.TryGetComponent<Hotbar>(out Hotbar getHotbar))
            {
                if (getHotbar.getItem() != null)
                {
                    if (getHotbar.getItem().itemName.Equals(getItem))
                    {
                        return getHotbar.getItem();
                    }
                }
            }
        }
        return null;
    }


    ////Create one amount of the item
    //public bool createItem(string in_binder, string getItemName, string in_mode, IActionListener getListener)
    //{
    //    Item getItem = ItemFactory.createItem(getItemName);
    //    if (getItem == null) return false;
    //    getItem.binder = in_binder;
    //    getItem.quantity = 1;
    //    getItem.itemIndex = -1;

    //    return pickupItem("Inventory", getItem, getListener);
    //}

    ////Create a specified amount of the item
    //public bool createItem(string in_binder, string getItemName, string in_mode, int amount, IActionListener getListener)
    //{
    //    Item getItem = ItemFactory.createItem(getItemName);
    //    if (getItem == null) return false;
    //    getItem.binder = in_binder;
    //    getItem.quantity = amount;
    //    getItem.itemIndex = -1;

    //    return pickupItem("Inventory", getItem, getListener);
    //}

    //public bool loadItem(Item in_item, IActionListener in_listener)
    //{
    //    if (in_item.itemIndex >= 0)
    //    {
    //        if (toolbars[in_item.itemIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            //DataCache.createNewPlayerItem(in_item);
    //            return getHotbar.pickupItem(in_item, in_listener);
    //        }

    //        }
    //    return false;
    //}


    //    //Picks up all of the item
    //    public bool pickupItem(string getSlot, Item getItem, IActionListener getListener)
    //    {

    //        switch (getSlot)
    //        {
    //            case "Inventory":
    //                return assignItem(toolbars, getItem, getListener);
    //            case "Pending":
    //                return assignItem(pendingItem, getItem, getListener);
    //        }
    //        return false;
    ////        return pickupItem(getSlot, getItem.itemName, getItem.quantity, getItem.itemType, 0, getListener);
    //    }

    //public bool pickupItem(string getSlot, string getItem, int quantity, string type, int price, IActionListener getListener)
    //{
    //    switch (getSlot)
    //    {
    //        case "Inventory":
    //            return assignItem(toolbars, getItem, quantity, type, price, -1, getListener);
    //        case "Pending":
    //            return assignItem(pendingItem, getItem, quantity, type, price, -1, getListener);
    //    }
    //    return false;
    //}

    //public bool pickupItem(string getSlot, string getItem, int quantity, string type, int price, int slot, IActionListener getListener)
    //{
    //    switch (getSlot)
    //    {
    //        case "Inventory":
    //            return assignItem(toolbars, getItem, quantity, type, price, slot, getListener);
    //        case "Pending":
    //            return assignItem(pendingItem, getItem, quantity, type, price, slot, getListener);
    //    }
    //    return false;
    //}

    //private bool assignItem(GameObject[] getSlot, string getItem, int quantity, string type, int price, int slot, IActionListener getListener)
    //{
    //    int emptyIndex = slot;
    //    if (slot < 0)
    //    {
    //    for (int index = 0; index < toolbars.Length; index++)
    //    {
    //        if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            switch (getHotbar.itempickupCheck(getItem))
    //            {
    //                case "Empty":
    //                    if (emptyIndex == -1)
    //                        emptyIndex = index;
    //                    break;
    //                case "Same":
    //                    return getHotbar.pickupItem(getItem, quantity, type, index, price, getListener);
    //                case "Different":
    //                    continue;
    //            }
    //        }
    //    }
    //    }

    //    if (emptyIndex == -1)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        if (toolbars[emptyIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            return getHotbar.pickupItem(getItem, quantity, type, emptyIndex, price, getListener);
    //        }
    //        return false;
    //    }
    //}


    //    private bool assignItem(GameObject[] getSlot, Item in_item, IActionListener getListener)
    //    {
    //        if (in_item.itemIndex < 0)
    //        {
    //            for (int index = 0; index < toolbars.Length; index++)
    //            {
    //                if (toolbars[index].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //                {
    //                    switch (getHotbar.itempickupCheck(in_item.itemName))
    //                    {
    //                        case "Empty":
    //                            if (in_item.itemIndex == -1)
    //                                in_item.itemIndex = index;
    //                            break;
    //                        case "Same":
    //                            in_item.itemIndex = index;
    //                            //DataCache.createNewPlayerItem(in_item);
    //                            return getHotbar.pickupItem(in_item, getListener);
    ////                            return getHotbar.pickupItem(getItem, quantity, type, index, price, getListener);
    //                        case "Different":
    //                            continue;
    //                    }
    //                }
    //            }
    //        }

    //        if (in_item.itemIndex == -1)
    //        {
    //            return false;
    //        }
    //        else
    //        { 
    //            if (toolbars[in_item.itemIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //            {
    //                //DataCache.createNewPlayerItem(in_item);
    //                return getHotbar.pickupItem(in_item, getListener);
    //            }
    //            return false;
    //        }

    //    }

    //public Item getItem(int getIndex)
    //{
    //    if (toolbars[getIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //    {
    //        deselect();
    //        if (getHotbar.getItem() != null)
    //        {
    //            currentSelect = getHotbar;
    //            //getHotbar.select();
    //        }
    //        return getHotbar.getItem();
    //    }
    //    return null;
    //}

    public void select(int getIndex)
    {
        //print(getIndex + " , " + currentIndex);
        //if (getIndex == currentIndex)
        //{
        //    deselect();
        //}
        //else
        {
            if (toolbars[getIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
            {
                getHotbar.select();
                //deselect();
                //if (getHotbar.getItem() != null)
                //{
                //    currentIndex = getIndex;
                //}
            }
            else
            {
                deselect();
            }
        }
    }

    //public Item selectItem(int getIndex)
    //{
    //    if (getIndex == currentIndex)
    //    {
    //        deselect();
    //    }
    //    else
    //    {
    //        if (toolbars[getIndex].TryGetComponent<Hotbar>(out Hotbar getHotbar))
    //        {
    //            deselect();
    //            if (getHotbar.getItem() != null)
    //            {
    //                currentSelect = getHotbar;
    //                getHotbar.select();
    //                currentIndex = getIndex;
    //            }
    //                return getHotbar.getItem();
    //        } else
    //        {
    //            deselect();
    //        }
    //    }
    //    return null;
    //}

    public void deselect()
    {
        if (currentSelect != null)
        {
            currentSelect.deselect();
            currentSelect = null;
            currentIndex = -999;
        }
    }

    //public bool useItem(int quantity)
    //{
    //    bool getBool = currentSelect.useItem(quantity);
    //    if (!getBool) deselect();
    //    return getBool;
    //}

    //public Item getItemByItem(Item itemGet)
    //{
    //    return getItemByName(itemGet.itemName);
    //}

}

[Serializable]
public class listOfAreaItems
{
    public List<Item> listOfItems = new List<Item>();
}
[Serializable]
public class areaItemDAO
{
    public Item item;
    public Vector3 position;
    public Quaternion rotation;
}