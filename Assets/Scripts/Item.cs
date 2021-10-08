using Socket.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string _id { get; set; }
    public string binder { get; set; }
    [JsonProperty("itemName")] public string itemName { get; set; }
    [JsonProperty("quantity")] public int quantity { get; set; }
    [JsonProperty("itemType")] public string itemType { get; set; }
    public int durability { get; set; }
    [JsonProperty("maxDurability")] public int maxDurability { get; set; }
    public int capacity { get; set; }
    [JsonProperty("maxCapacity")] public int maxCapacity { get; set; }
    public int itemIndex { get; set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public string state { get; set; }
    [JsonConstructor]
    public Item()
    {

    }
    public Item(ItemDAO in_item)
    {
        itemName = in_item.itemName;
        itemType = in_item.itemType;
        maxDurability = in_item.maxDurability;
        maxCapacity = in_item.maxCapacity;
        durability = in_item.maxDurability;
        capacity = in_item.maxCapacity;
        quantity = in_item.quantity;
    }
    public Item(ItemDTO in_item)
    {
        _id = in_item._id;
        itemName = in_item.itemName;
        itemType = in_item.itemType;
        maxDurability = in_item.maxDurability;
        maxCapacity = in_item.maxCapacity;
        durability = in_item.maxDurability;
        capacity = in_item.maxCapacity;
        quantity = in_item.quantity;
        state = in_item.state;
        binder = in_item.binder;
    }

    public Item(Item in_Item)
    {
        _id = in_Item._id;
        itemName = in_Item.itemName;
        quantity = in_Item.quantity;
        itemType = in_Item.itemType;
        itemIndex = in_Item.itemIndex;
        durability = in_Item.durability;
        capacity = in_Item.capacity;
        maxDurability = in_Item.maxDurability;
        maxCapacity = in_Item.maxCapacity;

    }

    public Item(string getItem, string getType)
    {
        itemName = getItem;
        itemType = getType;
    }

    public Item(string in_item, int in_amount)
    {
        itemName = in_item;
        quantity = in_amount;
    }

    public ItemDTO getDTO()
    {
        ItemDTO temp_DTO = new ItemDTO();
        temp_DTO._id = _id;
        temp_DTO.itemName = itemName;
        temp_DTO.itemType = itemType;
        temp_DTO.maxDurability = maxDurability;
        temp_DTO.maxCapacity = maxCapacity;
        temp_DTO.durability = durability;
        temp_DTO.capacity = capacity;
        temp_DTO.quantity = quantity;
        temp_DTO.state = state;
        temp_DTO.binder = binder;
        return temp_DTO;
    }
    override
    public string ToString()
    {
        return itemName + " , " + quantity + " , " + itemType;
    }

    public string getJson()
    {
        return JsonUtility.ToJson(this);
    }
}

[Serializable]
public class ItemDTO
{
    public string _id { get; set; }
    public string binder { get; set; }
    public string itemName { get; set; }
    public int quantity { get; set; }
    public string itemType { get; set; }
    public int durability { get; set; }
    public int maxDurability { get; set; }
    public int capacity { get; set; }
    public int maxCapacity { get; set; }
    public int itemIndex { get; set; }
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public string state { get; set; }

    public Item getActual()
    {
        Item temp_item = new Item();
        temp_item._id = _id;
        temp_item.binder = binder;
        temp_item.itemName = itemName;
        temp_item.quantity = quantity;
        temp_item.itemType = itemType;
        temp_item.durability = durability;
        temp_item.maxDurability = maxDurability;
        temp_item.capacity = capacity;
        temp_item.maxCapacity = maxCapacity;
        temp_item.position = position;
        temp_item.rotation = rotation;
        temp_item.state = state;
        return temp_item;
    }
}
[Serializable]
public class ItemExistanceDTOWrapper
{
    public string _id;
    public Entity binder;
    public ItemDTO ItemObj;
    public itemMarket itemMarketObj;
    public ItemDTO storageObj;
}
[Serializable]
public class ItemExistanceWrapper
{
    public List<ItemExistanceDTOWrapper> itemList;
    public string Action;
    public int index;
    public int total;
}

public class itemMarket
{
    public int buyPrice;
    public int sellPrice;
    public float priceRoof;
    public float itemRoof;
}