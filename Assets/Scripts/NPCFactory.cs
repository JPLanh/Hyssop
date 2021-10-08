using UnityEngine;


/**
 * 
 * NPC Factory
 * 
 * manages all npc
 * 
 */
public class NPCFactory : MonoBehaviour
{
    void OnApplicationQuit()
    {
        DataCache.saveAllNPC();
    }

    //Create a new data for the NPC and insert it into the cache
    public static Entity createNewNPC(string in_name, Vector3 in_position, Quaternion in_rotation, string in_type, string in_area)
    {
        Entity new_npc = new Entity();
        new_npc.areaName = in_area;
        new_npc.entityName = in_name;
        new_npc.state = in_type;
        new_npc.position = in_position;
        new_npc.rotation = in_rotation;
        new_npc.backpack = new Backpack();
        DataCache.addNewNPC(in_area, new_npc);
        return new_npc;
    }

    //Instantiate a new NPC into the area
    public static GameObject generateNewNPC(Vector3 in_position, Quaternion in_rotation, string in_name, string in_type, string in_area)
    {
        Entity temp_NPC = createNewNPC(in_name, in_position, in_rotation, in_type, in_area);
        GameObject temp_Obj = Instantiate(Resources.Load<GameObject>("NPC"), in_position, in_rotation);
        if (temp_Obj.TryGetComponent<NPCEntity>(out NPCEntity out_entity))
        {
            out_entity.npc = temp_NPC;

        }

        temp_Obj.name = in_name;
        switch (in_type)
        {
            case "Smithery":
            case "Shop":
                Shop temp_shop = temp_Obj.AddComponent<Shop>();
                temp_shop.currentNPC = temp_NPC;
                break;
            case "General Shop":
                Shop temp_gen_shop = temp_Obj.AddComponent<Shop>();
                temp_gen_shop.currentNPC = temp_NPC;
                out_entity.npc.backpack.size = 20;
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Strawberry seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Grape seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Coffee bean", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Corn seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Wheat seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Carrot seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Potato tuber", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Lettuce seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Cabbage seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Tomato seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Green onion bulb", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Onion seed", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Rice seedling", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Garlic clove", 10);
                temp_NPC.backpack.createItem(temp_NPC.entityName, "Silver", 2500);
                //            currentToolbar.createItem(name, "Basic Shovel", playerState, this);
                //        pickupItem("Seed", 25, "Seed", false, 2);
                //inventory.pickupItem("Inventory", "Seed", 25, "Seed", 2);
                //        inventory.createItem("Strawberry Seed", 25, this);
                //        inventory.createItem("Grape Seed", 25, this);
                //int value = Random.Range(3, 7);
                //inventory.adjustPrice("Strawberry Seed", value);
                //value = Random.Range(3, 7);
                //inventory.adjustPrice("Grape Seed", value);
                break;
        }

        return temp_Obj;
    }

    //Load an exisisting NPC into the area
    public static GameObject loadNPC(Entity in_npc, string in_area)
    {
        GameObject temp_Obj = Instantiate(Resources.Load<GameObject>("NPC"), in_npc.position, in_npc.rotation);
        temp_Obj.name = in_npc.entityName;
        if (temp_Obj.TryGetComponent<NPCEntity>(out NPCEntity out_entity))
        {
            out_entity.npc = in_npc;

            switch (in_npc.state)
            {
                case "Smithery":
                case "Shop":
                    Shop temp_shop = temp_Obj.AddComponent<Shop>();
                    temp_shop.currentNPC = out_entity.npc;
                    break;
                case "General Shop":
                    Shop temp_gen_shop = temp_Obj.AddComponent<Shop>();
                    temp_gen_shop.currentNPC = out_entity.npc;

                    //                    out_entity.npc.backpack.loadInventory();


                    //            currentToolbar.createItem(name, "Basic Shovel", playerState, this);
                    //        pickupItem("Seed", 25, "Seed", false, 2);
                    //inventory.pickupItem("Inventory", "Seed", 25, "Seed", 2);
                    //        inventory.createItem("Strawberry Seed", 25, this);
                    //        inventory.createItem("Grape Seed", 25, this);
                    //int value = Random.Range(3, 7);
                    //inventory.adjustPrice("Strawberry Seed", value);
                    //value = Random.Range(3, 7);
                    //inventory.adjustPrice("Grape Seed", value);
                    break;
            }
        }
        return temp_Obj;

    }

}