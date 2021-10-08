using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Contains custom value for each entity to be used by the controllers
 * 
 */
[Serializable]
public class Entity
{
    public string entityName;
    public Vector3 position;
    public Quaternion rotation;
    public string areaName;
    public Backpack backpack = new Backpack();
    public int stamina;
    public int maxStamina;
    public string state;
    public string holding;
    public Int64 time;


    public string currentAnimal;
    public float primary_currentRed;
    public float primary_currentGreen;
    public float primary_currentBlue;
    public float secondary_currentRed;
    public float secondary_currentGreen;
    public float secondary_currentBlue;

    public void dayEnd()
    {
        //    if (state.Equals("General Shop"))
        //    {
        //        Item marketEcon = DataCache.itemCache["Silver"];

        //        //List of all item, including one the vendor does not have
        //        Dictionary<string, Item> inStock = new Dictionary<string, Item>();
        //        inStock.Add("Strawberry seed", null);
        //        inStock.Add("Grape seed", null);
        //        inStock.Add("Coffee bean", null);
        //        inStock.Add("Corn seed", null);
        //        inStock.Add("Wheat seed", null);
        //        inStock.Add("Carrot seed", null);
        //        inStock.Add("Potato tuber", null);
        //        inStock.Add("Lettuce seed", null);
        //        inStock.Add("Tomato seed", null);
        //        inStock.Add("Onion seed", null);
        //        inStock.Add("Green onion bulb", null);
        //        inStock.Add("Garlic clove", null);
        //        inStock.Add("Rice seedling", null);

        //        //populate the list
        //        foreach (Item it_item in backpack.items)
        //        {
        //            if (inStock.ContainsKey(it_item.itemName)) inStock[it_item.itemName] = it_item;
        //        }

        //        Item lowest = null;
        //        foreach (KeyValuePair<String, Item> it_item in inStock)
        //        {

        //            //If the shop keeper does not have the item, initialize it and also consider as the lowest
        //            if (it_item.Value == null)
        //            {
        //                inStock[it_item.Key] = new Item(it_item.Key, 0);
        //                purchaseItem(it_item.Value, marketEcon, 2);
        //            }
        //            else
        //            {
        //                purchaseItem(it_item.Value, marketEcon, 2);
        //            }

        //            if (lowest == null) lowest = it_item.Value;
        //            else
        //                lowest = (lowest.quantity > it_item.Value.quantity) ? it_item.Value : lowest;

        //        }
        //        purchaseItem(lowest, marketEcon, 10);
        //    }
    }

    //public void purchaseItem(Item in_item, Item in_market_silver, int in_amount)
    //{
    //    Item market_item = DataCache.itemCache[in_item.itemName];
    //    Item currency = backpack.items.Find(x => x.itemName.Equals("Silver"));
    //    if (market_item.quantity > in_amount)
    //    {
    //        int buyingAmount = 0;
    //        if (currency.quantity >= market_item.buyPrice * in_amount)
    //        {
    //            buyingAmount = in_amount;
    //        } else
    //        {
    //            buyingAmount = ((int)(currency.quantity / market_item.buyPrice)); 
    //        }

    //        if (backpack.createItem(entityName, in_item.itemName, buyingAmount))
    //        {
    //            market_item.quantity -= buyingAmount;
    //            in_market_silver.quantity += market_item.buyPrice * buyingAmount;
    //            currency.quantity -= market_item.buyPrice * buyingAmount;
    //            DataCache.adjustMarketPrice(in_item);
    //        }
    //    }
    //    else
    //    {
    //        int buyingAmount = 0;
    //        if (currency.quantity >= market_item.buyPrice * market_item.quantity)
    //        {
    //            buyingAmount = market_item.quantity;
    //        }
    //        else
    //        {
    //            buyingAmount = ((int)(currency.quantity / market_item.buyPrice));
    //        }

    //        if (backpack.createItem(entityName, in_item.itemName, buyingAmount))
    //        {
    //            market_item.quantity -= buyingAmount;
    //            in_market_silver.quantity += market_item.buyPrice * buyingAmount;
    //            currency.quantity -= market_item.buyPrice * buyingAmount;
    //            DataCache.adjustMarketPrice(in_item);
    //        }

    //}
    //}

    public ItemExistanceDTOWrapper getHolding()
    {
        ItemExistanceDTOWrapper itemGet = backpack.items.Find(x => x._id.Equals(holding));
        return itemGet;
    }
    public Dictionary<string, string> getDialog()
    {
        Dictionary<string, string> dialog = new Dictionary<string, string>();
        switch (entityName)
        {
            case "Trevik":
                dialog.Add("Prompt 1", "Hello Frwend, my name is Trevik and i am the general shop keeper here. I sell seeds for all your farming needs. What d'ya need?");
                dialog.Add("Prompt 1.1", "Buy seeds");
                dialog.Add("Prompt 1.2", "Good-bye");
                break;
            case "Izak":
                dialog.Add("Prompt 1", "Hello, I am Izak, the smith here. I can repair your equipments with the cost of your silver.");
                dialog.Add("Prompt 1.1", "Repair");
                dialog.Add("Prompt 1.2", "Good-bye");
                break;
        }
        return dialog;
    }
}

[Serializable]
public class EntityDTO
{
    public string entityName;
    public Vector3 position;
    public Quaternion rotation;
    public string areaName;
    public Backpack backpack = new Backpack();
    public int stamina;
    public int maxStamina;
    public string state;
    public string holding;
    public string currentAnimal;
    public float primary_currentRed;
    public float primary_currentGreen;
    public float primary_currentBlue;
    public float secondary_currentRed;
    public float secondary_currentGreen;
    public float secondary_currentBlue;
    [System.NonSerialized]
    public Int64 time;

    public Entity getActual()
    {
        Entity temp_entity = new Entity();
        temp_entity.entityName = entityName;
        temp_entity.position = position;
        temp_entity.rotation = rotation;
        temp_entity.areaName = areaName;
        temp_entity.backpack = backpack;
        temp_entity.stamina = stamina;
        temp_entity.maxStamina = maxStamina;
        temp_entity.state = state;
        temp_entity.holding = holding;
        temp_entity.time = time;
        temp_entity.currentAnimal = currentAnimal;
        temp_entity.primary_currentBlue = primary_currentBlue;
        temp_entity.primary_currentGreen = primary_currentGreen;
        temp_entity.primary_currentRed = primary_currentRed;
        temp_entity.secondary_currentBlue = secondary_currentBlue;
        temp_entity.secondary_currentGreen = secondary_currentGreen;
        temp_entity.secondary_currentRed = secondary_currentRed;
        return temp_entity;
    }
}
[Serializable]
public class EntityWrapper
{
    public List<Entity> listOfNPC = new List<Entity>();
}
public class EntityDTOWrapper
{
    public EntityDTO entityObj;
}