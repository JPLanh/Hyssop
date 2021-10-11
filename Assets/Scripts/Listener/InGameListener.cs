using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class InGameListener : MonoBehaviour, IServerListener
{
    [SerializeField] PlayerController currentPlayer;
    [SerializeField] TimeSystem timeSystem;
    [SerializeField] GridSystem currentGrid;

    static Dictionary<string, NonPlayerController> characterTracker = new Dictionary<string, NonPlayerController>();

    public void serverResponseListener()
    {

        if (Network.worldRetrieved.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh world", 0);

            Network.loadedWorld = Network.worldRetrieved.Dequeue().getActual();
            timeSystem.currentWorld = Network.loadedWorld;

            Network.sendPacket(doCommands.load, "Config", currentGrid.area.areaName);
        }

        if (Network.areaConfig.Count > 0)
        {
            currentPlayer.teleportToDestination();

            currentPlayer.isLoading(true, "Loading teh area you're in", 0);

            AreaDTO temp_wrapper = Network.areaConfig.Dequeue();
            currentGrid.loadAreaConfig(temp_wrapper);

            Network.sendPacket(doCommands.load, "Area Indexes", currentGrid.area.areaName);
        }

        if (Network.listOfAreaIndexes.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh floor you stand on", 0);

            List<AreaIndexDTO> get_list = Network.listOfAreaIndexes.Dequeue();
            int count = 0;
            foreach (AreaIndexDTO it_areaIndex in get_list)
            {
                currentPlayer.isLoading(true, "Loading teh floor you stand on", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaHelper(it_areaIndex);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area Items", currentGrid.area.areaName);
        }

        if (Network.listOfAreaItems.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading teh things", 0);

            List<AreaItemDTO> get_list = Network.listOfAreaItems.Dequeue();
            int count = 0;
            foreach (AreaItemDTO it_areaItem in get_list)
            {
                currentPlayer.isLoading(true, "Loading teh things", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaItem(it_areaItem);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area Plants", currentGrid.area.areaName);
        }

        if (Network.listOfAreaPlants.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading mother's nature stoofs", 0);

            List<AreaPlantDTO> get_list = Network.listOfAreaPlants.Dequeue();
            int count = 0;
            foreach (AreaPlantDTO it_areaPlant in get_list)
            {
                currentPlayer.isLoading(true, "Loading mother's nature stoofs", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaPlant(it_areaPlant);
                count++;
            }

            Network.sendPacket(doCommands.load, "Area NPCs", currentGrid.area.areaName);
        }

        if (Network.listOfAreaNPCs.Count > 0)
        {
            currentPlayer.isLoading(true, "Loading everywuns", 0);

            List<EntityExistanceDTO> get_list = Network.listOfAreaNPCs.Dequeue();
            int count = 0;
            foreach (EntityExistanceDTO it_areaNpc in get_list)
            {
                currentPlayer.isLoading(true, "Loading everywun", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaNPC(it_areaNpc);
                count++;
            }


            currentPlayer.isLoading(false);
            currentPlayer.loadTutorial();
        }


        if (Network.areaPlants.Count > 0)
        {
            AreaPlantDTO it_areaPlant = Network.areaPlants.Dequeue();
            currentGrid.loadAreaPlant(it_areaPlant);
        }

        if (Network.serverAcknowledge.Count > 0)
        {
            Dictionary<string, string> getResponse = Network.serverAcknowledge.Dequeue();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            switch (getResponse["action"])
            {
                case "Farm generated":
                    payload["entity"] = Network.loadedCharacter.entityName;
                    Network.sendPacket(doCommands.player, "Items", payload);
                    break;
                case "Old day end":
                    currentPlayer.ts.setDayEnd();
                    break;
                case "New day begin":
                    currentPlayer.ts.setDayBegin();
                    currentPlayer.recover();
                    break;
                case "Action event":
                    currentPlayer.actionProgression(getResponse["message"], 2);
                    break;
                case "Error":
                    currentPlayer.toastNotifications.newNotification(getResponse["message"]);
                    break;
            }  
        }

        if (Network.characterNetworkUpdate.Count > 0)
        {
            characterListWrapper characterWrapper = Network.characterNetworkUpdate.Dequeue();
            Network.characterNetworkUpdate.Clear();
            if (characterWrapper != null && characterWrapper.characterList != null)
            {
                foreach (EntityDTO it_entity in characterWrapper.characterList)
                {
                    if (!it_entity.entityName.Equals(Network.loadedCharacter.entityName))
                    {
                        if (it_entity.areaName.Equals(currentGrid.area.areaName))
                        {
                            it_entity.time = characterWrapper.time;
                            if (InGameListener.characterTracker.TryGetValue(it_entity.entityName, out NonPlayerController out_npc))
                            {
                                {
                                    out_npc.playerEntity = it_entity.getActual();
                                }
                            }
                            else
                            {
                                GameObject temp_object = Instantiate(Resources.Load<GameObject>("NPC"), it_entity.position, it_entity.rotation);
                                if (temp_object.TryGetComponent<NonPlayerController>(out NonPlayerController out_NPC))
                                {
                                    out_NPC.playerEntity = it_entity.getActual();
                                    out_NPC.currentObject = temp_object;
                                    InGameListener.characterTracker.Add(it_entity.entityName, out_NPC);
                                }
                            }
                        }
                        else
                        {
                            if (InGameListener.characterTracker.TryGetValue(it_entity.entityName, out NonPlayerController out_npc))
                            {
                                {
                                    Destroy(out_npc.currentObject);
                                    Destroy(out_npc.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        if (!currentPlayer.canvas.tradeMenu.gameObject.activeInHierarchy)
        {
            if (Network.itemRetrieved.Count > 0)
            {
                ItemExistanceDTOWrapper getItem = Network.itemRetrieved.Dequeue();
                print(currentPlayer.playerEntity.entityName + " , " + (getItem.binder.entityName));
                if (currentPlayer.playerEntity.entityName.Equals(getItem.binder.entityName))
                {
                    //                    currentPlayer.playerEntity.backpack.createItem(getItem);

                    ItemExistanceDTOWrapper has_item = currentPlayer.playerEntity.backpack.items.Find(x => x.ItemObj.itemName == getItem.ItemObj.itemName);
                    print(JsonConvert.SerializeObject(has_item, settings));
                    print(JsonConvert.SerializeObject(getItem, settings));
                    if (has_item != null)
                    {

                        if (getItem.ItemObj.quantity <= 0)
                        {
                            currentPlayer.playerEntity.backpack.items.Remove(has_item);
                        }
                        else
                        {
                            has_item.ItemObj.quantity = getItem.ItemObj.quantity;
                        }
                    }
                    else
                    {
                        currentPlayer.playerEntity.backpack.items.Add(getItem);
                    }

                }
            }
        }
    }

    public void resetState()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        serverResponseListener();
    }
}
