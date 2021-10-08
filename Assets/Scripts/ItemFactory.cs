using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private Transform itemList;

    private void OnApplicationQuit()
    {
        DataCache.saveItems();

    }
    private void Start()
    {
        //addToJson("Coffee cherry", "Produce");
        //addToJson("Corn", "Produce");
        //addToJson("Wheat", "Produce");
        //addToJson("Carrot", "Produce");
        //addToJson("Potato", "Produce");
        //addToJson("Lettuce", "Produce");
        //addToJson("Cabbage", "Produce");
        //addToJson("Tomato", "Produce");
        //addToJson("Onion", "Produce");
        //addToJson("Green Onion", "Produce");
        //addToJson("Garlic", "Produce");
        //addToJson("Rice", "Produce");


        //addToJson("Coffee bean", "Seed");
        //addToJson("Corn seed", "Seed");
        //addToJson("Wheat seed", "Seed");
        //addToJson("Carrot seed", "Seed");
        //addToJson("Potato tuber", "Seed");
        //addToJson("Lettuce seed", "Seed");
        //addToJson("Cabbage seed", "Seed");
        //addToJson("Tomato seed", "Seed");
        //addToJson("Onion seed", "Seed");
        //addToJson("Green onion bulb", "Seed");
        //addToJson("Garlic clove", "Seed");
        //addToJson("Rice seedling", "Seed");
        //DataCache.saveItems();

    }

    public static void addToJson(string getName, string getType)
    {
        DataCache.itemCache.Add(getName, new Item(getName, getType));
    }


    public static ItemExistanceDTOWrapper createItem(string getName)
    {
        ItemDTO temp_obj = DataCache.getItem(getName).getDTO();
        ItemExistanceDTOWrapper new_itemEntity = new ItemExistanceDTOWrapper();
        new_itemEntity.ItemObj = temp_obj;
        return new_itemEntity;
    }

    public static void newItem(string in_name, Item in_item)
    {

    }
}

[Serializable]
public class ItemDAOWrapper
{
    public List<ItemDAO> listOfItems = new List<ItemDAO>();
}

[Serializable]
public class ItemListWrapper
{
    public List<Item> listOfItems = new List<Item>();
}

[Serializable]
public class ItemDAO
{
    public string itemName;
    public string itemType;
    public int maxDurability;
    public int maxCapacity;
    public int quantity;

    public ItemDAO(Item in_item)
    {
        itemName = in_item.itemName;
        itemType = in_item.itemType;
        maxDurability = in_item.durability;
        maxCapacity = in_item.capacity;
        quantity = in_item.quantity;
    }

    public ItemDAO() { }

    public string getJson()
    {
        return JsonUtility.ToJson(this);
    }
}