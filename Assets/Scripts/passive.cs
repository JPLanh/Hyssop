using System.Collections.Generic;
using UnityEngine;

public class passive : MonoBehaviour, IServerListener
{
    [SerializeField] PlayerController currentPlayer;
    [SerializeField] TimeSystem timeSystem;
    [SerializeField] GridSystem currentGrid;

    static Dictionary<string, NonPlayerController> characterTracker = new Dictionary<string, NonPlayerController>();

    public void serverResponseListener()
    {

        if (Network.worldRetrieved.Count > 0)
        {
            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading teh world", 0);

            Network.loadedWorld = Network.worldRetrieved.Dequeue().getActual();
            timeSystem.currentWorld = Network.loadedWorld;

            if (Network.state.Equals("Loading"))
                Network.sendPacket(doCommands.load, "Config", currentGrid.areaName);
        }

        if (Network.areaConfig.Count > 0)
        {

            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading teh area you're in", 0);

            AreaDTO temp_wrapper = Network.areaConfig.Dequeue();
            currentGrid.loadAreaConfig(temp_wrapper);

            if (Network.state.Equals("Loading"))
                Network.sendPacket(doCommands.load, "Area Indexes", currentGrid.areaName);
            //            Network.loadAreaGrid(currentGrid.areaName, temp_wrapper.index, lastPayload.total);
            //            break;
        }

        if (Network.listOfAreaIndexes.Count > 0)
        {
            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading teh floor you stand on", 0);

            List<AreaIndexDTO> get_list = Network.listOfAreaIndexes.Dequeue();
            int count = 0;
            foreach (AreaIndexDTO it_areaIndex in get_list)
            {
                if (Network.state.Equals("Loading"))
                    currentPlayer.isLoading(true, "Loading teh floor you stand on", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaHelper(it_areaIndex);
                count++;
            }

            if (Network.state.Equals("Loading"))
                Network.sendPacket(doCommands.load, "Area Items", currentGrid.areaName);
        }

        if (Network.listOfAreaItems.Count > 0)
        {
            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading teh things", 0);

            List<AreaItemDTO> get_list = Network.listOfAreaItems.Dequeue();
            int count = 0;
            foreach (AreaItemDTO it_areaItem in get_list)
            {
                if (Network.state.Equals("Loading"))
                    currentPlayer.isLoading(true, "Loading teh things", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaItem(it_areaItem);
                count++;
            }

            if (Network.state.Equals("Loading"))
                Network.sendPacket(doCommands.load, "Area Plants", currentGrid.areaName);
        }

        if (Network.listOfAreaPlants.Count > 0)
        {
            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading mother's nature stoofs", 0);

            List<AreaPlantDTO> get_list = Network.listOfAreaPlants.Dequeue();
            int count = 0;
            foreach (AreaPlantDTO it_areaPlant in get_list)
            {
                if (Network.state.Equals("Loading"))
                    currentPlayer.isLoading(true, "Loading mother's nature stoofs", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaPlant(it_areaPlant);
                count++;
            }

            if (Network.state.Equals("Loading"))
                Network.sendPacket(doCommands.load, "Area NPCs", currentGrid.areaName);
        }

        if (Network.listOfAreaNPCs.Count > 0)
        {
            if (Network.state.Equals("Loading"))
                currentPlayer.isLoading(true, "Loading everywuns", 0);

            List<EntityExistanceDTO> get_list = Network.listOfAreaNPCs.Dequeue();
            int count = 0;
            foreach (EntityExistanceDTO it_areaNpc in get_list)
            {
                if (Network.state.Equals("Loading"))
                    currentPlayer.isLoading(true, "Loading everywun", (((float)count) / ((float)get_list.Count) * 100));
                currentGrid.loadAreaNPC(it_areaNpc);
                count++;
            }

            if (Network.state.Equals("Loading"))
            {
                currentPlayer.isLoading(false);
                Network.state = "";

            }
        }


        if (Network.areaPlants.Count > 0)
        {
            AreaPlantDTO it_areaPlant = Network.areaPlants.Dequeue();
            currentGrid.loadAreaPlant(it_areaPlant);
        }

        if (Network.serverAcknowledge.Count > 0)
        {
            string getResponse = Network.serverAcknowledge.Dequeue();
            print(getResponse);
            switch (getResponse)
            {
                case "Farm generated":
                    Network.sendPacket(doCommands.player, "Items");
                    break;
                case "Old day end":
                    currentPlayer.ts.setDayEnd();
                    break;
                case "New day begin":
                    currentPlayer.ts.setDayBegin();
                    currentPlayer.recover();
                    break;
            }
        }

        if (Network.serverResponse.Count > 0)
        {
            Dictionary<string, string> getResponse = Network.serverResponse.Dequeue();
            print(getResponse["action"]);
            switch (getResponse["action"])
            {
                case "Old day end":
                    currentPlayer.ts.setDayEnd();

                    break;
                case "New day begin":
                    currentPlayer.ts.setDayBegin();
                    currentPlayer.recover();
                    break;
            }
        }

        if (Network.actionRetrieved.Count > 0)
        {
            ActionWrapper get_action = Network.actionRetrieved.Dequeue();
            currentPlayer.actionProgression(get_action.action, 2);
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
                        if (it_entity.areaName.Equals(currentGrid.areaName))
                        {
                            it_entity.time = characterWrapper.time;
                            if (passive.characterTracker.TryGetValue(it_entity.entityName, out NonPlayerController out_npc))
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
                                    passive.characterTracker.Add(it_entity.entityName, out_NPC);
                                }
                            }
                        }
                        else
                        {
                            if (passive.characterTracker.TryGetValue(it_entity.entityName, out NonPlayerController out_npc))
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

        if (Network.itemWrappers.Count > 0)
        {
            ItemWrapper getItem = Network.itemWrappers.Dequeue();
            switch (getItem.Action)
            {
                case "New item":

                    //FIX THIS TOO
                    //                    currentPlayer.playerEntity.backpack.createItem(getItem.Item);
                    break;
            }
        }

        if (!currentPlayer.canvas.tradeMenu.gameObject.activeInHierarchy)
        {
            if (Network.itemRetrieved.Count > 0)
            {
                ItemExistanceDTOWrapper getItem = Network.itemRetrieved.Dequeue();
                print(getItem.ItemObj.itemName);
                currentPlayer.playerEntity.backpack.createItem(getItem);
            }
        }
    }

    public void resetState()
    {
        Network.state = "Loading";
    }


    // Start is called before the first frame update
    void Start()
    {
        Network.state = "Loading";
    }

    // Update is called once per frame
    void Update()
    {
        serverResponseListener();
    }
}
