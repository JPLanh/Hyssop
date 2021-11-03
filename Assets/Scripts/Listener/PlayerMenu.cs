using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour, IActionListener, IServerListener
{
    public GameObject currentMenu;

    public PlayerController currentPlayer;
    public GameObject newButton;
    public GameObject newInputField;
    public GameObject newLabel;
    [SerializeField] private GameObject menuOption;
    public AreaIndex selectedGridIndex;
    public Dictionary<string, inputField> listOfInputFields;
    private List<InputField> tabbableField;

    public Shop focusShop;
    public Backpack focusBackpack;

    private int tabIndex;
    private string focusButton;
    private List<ItemExistanceDTOWrapper> shopList = new List<ItemExistanceDTOWrapper>();

    public GameObject transfer_dialog_go;
    public GameObject canvas_go;
    public string menuState;

    // Start is called before the first frame update
    void Start()
    {
        listOfInputFields = new Dictionary<string, inputField>();
        tabbableField = new List<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabIndex = (tabIndex + 1) % tabbableField.Count;
            tabbableField[tabIndex].ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            listen(focusButton);
        }
        serverResponseListener();
    }

    private void teleportModification()
    {
        listOfInputFields.TryGetValue("Area name", out inputField out_areaname);
        listOfInputFields.TryGetValue("X", out inputField out_x);
        listOfInputFields.TryGetValue("Y", out inputField out_y);
        listOfInputFields.TryGetValue("Z", out inputField out_z);

        selectedGridIndex.state = "Teleport"
            + " " + out_x.inputValue.text
            + " " + out_y.inputValue.text
            + " " + out_z.inputValue.text
            + " " + out_areaname.inputValue.text;
        menuState = "Modify Action";
    }

    private GameObject createLabel(string in_buttonName, Vector3 position)
    {
        GameObject tmpLabel = Instantiate(newLabel) as GameObject;
        tmpLabel.name = in_buttonName;
        tmpLabel.transform.SetParent(menuOption.transform);
        tmpLabel.transform.localPosition = position;
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = in_buttonName;
        }

        return tmpLabel;
    }
    private GameObject createButton(string in_buttonName, string in_action, bool in_focus)
    {
        GameObject tmpButton = Instantiate(newButton) as GameObject;
        tmpButton.name = in_buttonName;
        tmpButton.transform.SetParent(menuOption.transform);
        if (tmpButton.TryGetComponent<ButtonAction>(out ButtonAction out_button))
        {
            out_button.action = in_action;
            out_button.listener = this;
            out_button.text.text = in_buttonName;
        }

        if (in_focus) focusButton = in_action;
        return tmpButton;
    }

    private GameObject createInputField(string in_textName)
    {
        GameObject tmpField = Instantiate(newInputField) as GameObject;
        tmpField.name = in_textName;
        tmpField.transform.SetParent(menuOption.transform);
        if (tmpField.TryGetComponent<inputField>(out inputField in_inputField))
        {
            in_inputField.inputLabel.text = in_textName;
            listOfInputFields.Add(in_textName, in_inputField);
            tabbableField.Add(in_inputField.inputObject);

        }
        return tmpField;

    }

    public void init()
    {
        switch (currentPlayer.playerEntity.state)
        {
            case "Creator":
                menuState = "Creator Mode";
                break;
            case "Modify Action":
                menuState = "Modify Action";
                break;
            case "General Shop":
                menuState = "Shop Menu";
                break;
            case "Inventory":
                menuState = "Access Inventory";
                break;
            case "Smithery":
                menuState = "Repair Menu";
                break;
            default:
                menuState = "Start";
                break;
        }
        mainMenu();
    }

    public void modifyAction()
    {
        menuState = "Modify Action";
        mainMenu();
    }

    private void accessInventory()
    {
        createEmptyItemBox(-1);
        for (int index = 0; index < currentPlayer.playerEntity.backpack.items.Count; index++)
        {
            createItemBoxOffset(currentPlayer.playerEntity.backpack, index, currentPlayer);
            createLabelOffset(currentPlayer.playerEntity.backpack, index, currentPlayer);

        }
    }
    private void createEmptyItemBox(int in_index)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(menuOption.transform);
        itemGet.transform.localPosition = new Vector3(-180f, 160f, 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            getHotbar.action = "Unselect items";
            getHotbar.setActionListener(currentPlayer);
            if (currentPlayer.playerEntity.backpack.indexSelected == -1)
                getHotbar.select();
            else
                getHotbar.deselect();
        }
    }
    private void createItemBoxOffset(Backpack in_backpack, int in_index, IActionListener in_listener)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(menuOption.transform);
        itemGet.transform.localPosition = new Vector3(-180f + 145f * ((in_index + 1) / 7), 160f - 50f * ((in_index + 1) % 7), 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            if (in_backpack.items[in_index] != null)
            {
                getHotbar.hotbarItem = in_backpack.items[in_index].ItemObj;
                getHotbar.action = "Select " + in_index;
                getHotbar.setActionListener(in_listener);
                getHotbar.updateTexts();
                if (in_backpack.indexSelected == in_index)
                    getHotbar.select();
                else
                    getHotbar.deselect();
            }
        }
    }
    private void createShopItemBox(ItemDTO in_item, string in_subText, int in_index, IActionListener in_listener)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(menuOption.transform);
        itemGet.transform.localPosition = new Vector3(-180f + 145f * ((in_index) / 7), 160f - 50f * ((in_index) % 7), 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            if (in_item != null)
            {
                getHotbar.hotbarItem = in_item;
                getHotbar.action = "Select " + in_index;
                getHotbar.hover = "Hover " + in_index;
                getHotbar.setActionListener(in_listener);
                //createLabel(in_subText, new Vector3(-100f + 50f * ((in_slot) % 5), 75f - 75f * ((in_slot) / 5), 0f));
                getHotbar.updateTexts();
            }
        }
    }

    private void createShopItemBox(ItemDTO in_item, string in_subText, int in_index)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(menuOption.transform);
        itemGet.transform.localPosition = new Vector3(-180f + 145f * ((in_index) / 7), 160f - 50f * ((in_index) % 7), 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            if (in_item != null)
            {
                getHotbar.hotbarItem = in_item;
                getHotbar.action = "Select " + in_index;
                getHotbar.hover = "Hover " + in_index;
                getHotbar.setActionListener(this);
                //createLabel(in_subText, new Vector3(-100f + 50f * ((in_slot) % 5), 75f - 75f * ((in_slot) / 5), 0f));
                getHotbar.updateTexts();
            }
        }
    }

    private GameObject createLabelOffset(Backpack in_backpack, int in_index, IActionListener in_listener)
    {
        GameObject tmpLabel = Instantiate(Resources.Load<GameObject>("Label"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpLabel.name = in_backpack.items[in_index].ItemObj.itemName;
        tmpLabel.transform.SetParent(menuOption.transform);
        tmpLabel.transform.localPosition = new Vector3(-45f + 145f * ((in_index + 1) / 7), 160f - 50f * ((in_index + 1) % 7), 0f);
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = (in_backpack.items[in_index].ItemObj.quantity).ToString();
        }

        return tmpLabel;
    }

    public void teleportPlayer()
    {
        listOfInputFields.TryGetValue("Area name", out inputField out_areaname);
        listOfInputFields.TryGetValue("X", out inputField out_x);
        listOfInputFields.TryGetValue("Y", out inputField out_y);
        listOfInputFields.TryGetValue("Z", out inputField out_z);

        int x = int.Parse(out_x.inputValue.text);
        int y = int.Parse(out_y.inputValue.text);
        int z = int.Parse(out_z.inputValue.text);

        currentPlayer.teleport(int.Parse(out_x.inputValue.text),
            int.Parse(out_y.inputValue.text),
            int.Parse(out_z.inputValue.text),
            out_areaname.inputValue.text);

        menuState = "Creator Mode";
        mainMenu();
    }

    //public void spawnNPC(string in_type)
    //{
    //    listOfInputFields.TryGetValue("NPC name", out inputField out_areaname);
    //    currentPlayer.currentGrid.spawnNPC(currentPlayer.playerVision.getGridIndex(), out_areaname.inputValue.text, "General Shop Keeper");

    //}

    private void createNewArea()
    {
        listOfInputFields.TryGetValue("Area name", out inputField out_areaname);
        listOfInputFields.TryGetValue("Length", out inputField out_length);
        listOfInputFields.TryGetValue("Width", out inputField out_width);
        listOfInputFields.TryGetValue("Height", out inputField out_height);
        currentPlayer.currentGrid.generateEmptyGrid("Offline", int.Parse(out_length.inputValue.text), int.Parse(out_width.inputValue.text), int.Parse(out_height.inputValue.text), out_areaname.inputValue.text);
        currentPlayer.toastNotifications.newNotification(out_areaname.inputValue.text + " has been created");
    }

    private void mainMenu()
    {
        close();
        currentMenu.SetActive(true);
        switch (menuState)
        {
            case "Start":
                createButton("Creator Mode", "Enter Creator Mode", false).transform.localPosition = new Vector3(0f, 170f, 0f);
                createButton("Save Person", "Save Person", false).transform.localPosition = new Vector3(0f, 170f - (35f * 1), 0f);
                createButton("Exit Menu", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Creator Mode":
                createButton("New Area", "New Area", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 0), 0f);
                createButton("Pause/Unpause Time", "Toggle timestop", false).transform.localPosition = new Vector3(75f, 170f - (35f * 5), 0f);
                createButton("Configure Area", "Configure Area", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 1), 0f);
                createButton("Save Area", "Save Area", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 2), 0f);
                createButton("Generate Items", "Generate Item", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 3), 0f);
                createButton("Generate NPC", "NPC Generation Option", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 4), 0f);
                createButton("Teleport to area", "Teleport Option", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 5), 0f);
                createButton("Switch day end", "Day end", false).transform.localPosition = new Vector3(75f, 170f - (35f * 0), 0f);
                createButton("Switch day begin", "Day begin", false).transform.localPosition = new Vector3(75f, 170f - (35f * 1), 0f);
                createButton("Exit Creator Mode", "Exit Creator Mode", false).transform.localPosition = new Vector3(0f, -170f + (35f * 1), 0f);
                createButton("Exit Menu", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Generate Item":
                createButton("Onion", "Create Onion", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 0), 0f);
                createButton("Wooden Floor", "Create Wooden Floor", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 1), 0f);
                createButton("Wooden Slab", "Create Wooden Slab", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 2), 0f);
                createButton("Wooden Stair", "Create Wooden Stair", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 3), 0f);
                createButton("Wooden Wall", "Create Wooden Wall", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 4), 0f);
                createButton("Stone Path", "Create Stone Path", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 5), 0f);
                createButton("Area Connector", "Create Area Connector", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 6), 0f);
                createButton("Action Modifier", "Create Action Modifier", false).transform.localPosition = new Vector3(-75f, 170f - (35f * 7), 0f);
                createButton("Placement Manipulator", "Create Placement Manipulator", false).transform.localPosition = new Vector3(75f, 170f - (35f * 0), 0f);
                createButton("Glass", "Create Glass", false).transform.localPosition = new Vector3(75f, 170f - (35f * 1), 0f);
                createButton("Basic Bed", "Create Basic Bed", false).transform.localPosition = new Vector3(75f, 170f - (35f * 3), 0f);
                createButton("Back", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f + 35f, 0f);
                createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Modify Action":
                createButton("Teleport", "Teleport modification", false).transform.localPosition = new Vector3(0f, 170f, 0f);

                createButton("Cancel", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Modify Action - Teleport":
                createInputField("Area name").transform.localPosition = new Vector3(25f, 180f, 0f);
                createInputField("X").transform.localPosition = new Vector3(25f, 180f - (55f * 1), 0f);
                createInputField("Y").transform.localPosition = new Vector3(25f, 180f - (55f * 2), 0f);
                createInputField("Z").transform.localPosition = new Vector3(25f, 180f - (55f * 3), 0f);
                createButton("Apply", "Apply teleport modification", true).transform.localPosition = new Vector3(0f, -100f, 0f);
                createButton("Back", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "New Area":
                createInputField("Area name").transform.localPosition = new Vector3(25f, 180f, 0f);
                createInputField("Length").transform.localPosition = new Vector3(25f, 180f - (55f * 1), 0f);
                createInputField("Width").transform.localPosition = new Vector3(25f, 180f - (55f * 2), 0f);
                createInputField("Height").transform.localPosition = new Vector3(25f, 180f - (55f * 3), 0f);
                createButton("Create", "Generate New Area", true).transform.localPosition = new Vector3(0f, -100f, 0f);
                createButton("Cancel", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Teleport":
                createInputField("Area name").transform.localPosition = new Vector3(25f, 180f, 0f);
                createInputField("X").transform.localPosition = new Vector3(25f, 180f - (55f * 1), 0f);
                createInputField("Y").transform.localPosition = new Vector3(25f, 180f - (55f * 2), 0f);
                createInputField("Z").transform.localPosition = new Vector3(25f, 180f - (55f * 3), 0f);
                createButton("Teleport", "Teleport", true).transform.localPosition = new Vector3(0f, -100f, 0f);
                createButton("Back", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Configure Area":
                createButton("Generate Borders", "Generate Border", false).transform.localPosition = new Vector3(0f, 170f - (35f * 0), 0f);
                createButton("Generate Lights", "Generate Light", false).transform.localPosition = new Vector3(0f, 170f - (35f * 1), 0f);
                createButton("Cancel", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
                break;
            case "Shop Menu":
                createShop();
                break;
            case "Generate NPC":
                npcModification();
                break;
            case "Access Inventory":
                accessInventory();
                break;
            case "Repair Menu":
                repairMenu();
                break;

        }

        if (tabbableField != null && tabbableField.Count > 0)
        {
            tabbableField[0].ActivateInputField();
        }
    }

    private void repairMenu()
    {
        currentPlayer.canvas.dialogBox.textPerSecond = 0f;
        int counter = 0;
        Backpack selectedBackpack = currentPlayer.playerEntity.backpack;
        for (int index = 0; index < selectedBackpack.items.Count; index++)
        {
            if (!selectedBackpack.items[index].ItemObj.itemName.Equals("Silver"))
            {
                createShopItemBox(selectedBackpack.items[index].ItemObj, (selectedBackpack.items[index].ItemObj.maxDurability - selectedBackpack.items[index].ItemObj.durability).ToString(), index, currentPlayer);
                counter++;
            }
        }

    }

    public void npcModification()
    {

        createInputField("NPC name").transform.localPosition = new Vector3(25f, 180f, 0f);
        createButton("Spawn as Villager", "Spawn Villager", false).transform.localPosition = new Vector3(-75f, -170f + 70f, 0f);
        createButton("Spawn as Vendor", "Spawn Vendor", false).transform.localPosition = new Vector3(75f, -170f + 70f, 0f);
        createButton("Back", "Modify Action", false).transform.localPosition = new Vector3(0f, -170f + 35f, 0f);
        createButton("Exit", "Exit Menu", false).transform.localPosition = new Vector3(0f, -170f, 0f);
    }

    public void createShop()
    {
        currentPlayer.canvas.dialogBox.textPerSecond = 0f;
        if (!Network.isConnected)
        {
            int counter = 0;
            for (int index = 0; index < focusShop.currentNPC.backpack.items.Count; index++)
            {
                if (!focusShop.currentNPC.backpack.items[index].ItemObj.itemName.Equals("Silver"))
                {
                    //                    createShopItemBox(focusShop.currentNPC.backpack.items[index], focusShop.currentNPC.backpack.items[index].buyPrice.ToString(), index, focusShop);
                    counter++;
                }
            }
        }
        else
        {
            shopList = new List<ItemExistanceDTOWrapper>();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["entity"] = focusShop.currentNPC.entityName;
            Network.sendPacket(doCommands.player, "Items", payload);
//            Network.doShop(focusShop.currentNPC.entityName, "Get item database");
        }
    }

    public void updateMenu()
    {
        mainMenu();
    }
    public void close()
    {
        foreach (Transform menuButton in menuOption.transform)
        {
            Destroy(menuButton.gameObject);
        }

        if (currentPlayer.playerEntity.state.Equals("Shop"))
        {
            currentPlayer.playerEntity.state = "Player";//focusShop.prevPlayerState;
        }

        if (listOfInputFields != null)
            listOfInputFields.Clear();
        if (tabbableField != null)
            tabbableField.Clear();
        currentMenu.SetActive(false);
    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    public IActionListener getActionListener()
    {
        throw new System.NotImplementedException();
    }

    public void listen(string getAction)
    {
        string[] parser = getAction.Split(' ');
        switch (parser[0])
        {
            //case "Create":
            //    //currentPlayer.currentToolbar.createItem(currentPlayer.name, in_action.Replace(parser[0] + " ", ""), currentPlayer.playerState, currentPlayer);
            //    break;
            //case "Spawn":
            //    //                spawnNPC(parser[1]);
            //    break;
            case "Hover":
                ItemExistanceDTOWrapper hoverItem = shopList[int.Parse(parser[1])];
                currentPlayer.newMessage(focusShop.currentNPC.entityName, "The price for " + hoverItem.ItemObj.itemName + " goes for " + hoverItem.itemMarketObj.buyPrice + " silvers, and I currently have " + hoverItem.ItemObj.quantity);
                break;
            case "Select":
                ItemExistanceDTOWrapper tradeItem = shopList[int.Parse(parser[1])];
                if (tradeItem != null)
                {
                    tradeSelected(tradeItem);
                }
                break;
            default:
                switch (getAction)
                {
                    case "Enter Creator Mode":
                        menuState = "Creator Mode";
                        currentPlayer.creatorMode();
                        mainMenu();
                        break;
                    case "Exit Creator Mode":
                        menuState = "Start";
                        currentPlayer.playerMode();
                        mainMenu();
                        break;
                    case "Exit Menu":
                        close();
                        currentPlayer.accessMenu();
                        break;
                    case "Generate Item":
                        menuState = "Generate Item";
                        mainMenu();
                        break;
                    case "Teleport modification":
                        menuState = "Modify Action - Teleport";
                        mainMenu();
                        break;
                    case "Apply teleport modification":
                        teleportModification();
                        currentPlayer.toastNotifications.newNotification("Teleportation modification has been applied");
                        mainMenu();
                        break;
                    case "Save Area":
                        currentPlayer.currentGrid.saveArea("Offline");
                        currentPlayer.toastNotifications.newNotification(currentPlayer.currentGrid.area.areaName + " has been saved");
                        break;
                    case "Save Person":
                        currentPlayer.save();
                        currentPlayer.toastNotifications.newNotification(currentPlayer.name + " has been saved");
                        break;
                    case "New Area":
                        menuState = "New Area";
                        mainMenu();
                        break;
                    case "Generate New Area":
                        createNewArea();
                        break;
                    case "Teleport Option":
                        menuState = "Teleport";
                        mainMenu();
                        break;
                    case "Teleport":
                        teleportPlayer();
                        break;
                    case "Configure Area":
                        menuState = "Configure Area";
                        mainMenu();
                        break;
                    case "Generate Border":
                        currentPlayer.currentGrid.generateParameter("Wooden Fence");
                        currentPlayer.toastNotifications.newNotification("Border has been generated");
                        break;
                    case "NPC Generation Option":
                        menuState = "Generate NPC";
                        mainMenu();
                        break;
                    case "Toggle timestop":
                        currentPlayer.ts.currentWorld.timeStopped = currentPlayer.ts.currentWorld.timeStopped ? false : true;
                        currentPlayer.toastNotifications.newNotification("Time has been " + (currentPlayer.ts.currentWorld.timeStopped ? "Paused" : "Unpaused"));
                        break;
                    case "Generate Light":
                        currentPlayer.currentGrid.spawnItem("Torch", currentPlayer.playerVision.focusPoint, currentPlayer.transform.rotation);
                        break;
                    case "Day end":
                        currentPlayer.ts.setDayEnd();
                        break;
                    case "Day begin":
                        currentPlayer.ts.setDayBegin();
                        break;
                }
                break;
        }
    }

    public void serverResponseListener()
    {
        if (Network.listOfItems.Count > 0)
        {
            List<ItemExistanceDTOWrapper> temp_wrapper = Network.listOfItems.Dequeue();
            foreach (ItemExistanceDTOWrapper it_itemObj in temp_wrapper)
            {
                if (!it_itemObj.ItemObj.itemType.Equals("Money"))
                {
                    createShopItemBox(it_itemObj.ItemObj, it_itemObj.itemMarketObj.buyPrice.ToString(), shopList.Count);
                    shopList.Add(it_itemObj);
                }
            }
        }
    }

    private void tradeSelected(ItemExistanceDTOWrapper in_item)
    {
        transfer_dialog_go = Instantiate(Resources.Load<GameObject>("Transfer_amt_dialog"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        transfer_dialog_go.transform.SetParent(canvas_go.transform);
        transfer_dialog_go.transform.localPosition = new Vector3(0f, 0f, 0f);
        if (transfer_dialog_go.TryGetComponent<Transder_Dialog>(out Transder_Dialog out_transfer_dialog)){
            out_transfer_dialog.transferLoad(in_item, currentPlayer.playerEntity, focusShop.currentNPC,  currentPlayer, "Sell");
        }
    }
}
