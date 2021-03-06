using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class TradeMenu : MonoBehaviour, IActionListener
{
    public GameObject currentMenu;
    public GameObject leftObjectList;
    public GameObject rightObjectList;

    //    public backpack focusInventory;
    public StorageEntity focusStorage;
    public PlayerController currentPlayer;
    public string menuState;
    public GameObject transfer_dialog_go;
    public GameObject canvas_go;

    public GameObject newButton;
    private bool modifier;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
//        if (gameObject.activeInHierarchy) serverResponseListener();
        if (Input.GetButtonDown("Modifier")) modifier = true;
        if (Input.GetButtonUp("Modifier")) modifier = false;
    }

    public void init()
    {
        switch (currentPlayer.playerEntity.state)
        {
            default:
                menuState = "Start";
                break;
        }
        mainMenu();
    }

    private void mainMenu()
    {
        close();
        currentMenu.SetActive(true);
        switch (menuState)
        {
            case "Start":
                loadPanel("Left", currentPlayer.playerEntity.backpack);
                //if (!Network.isConnected) 
                    loadPanel("Right", focusStorage.storage.inventory);
                break;
        }
    }
    public void close()
    {
        foreach (Transform menuButton in leftObjectList.transform)
        {
            Destroy(menuButton.gameObject);
        }
        foreach (Transform menuButton in rightObjectList.transform)
        {
            Destroy(menuButton.gameObject);
        }

        currentMenu.SetActive(false);
    }

    private void loadPanel(string in_panel, Backpack in_backpack)
    {
        Transform temp_panel = null;

        if (in_panel.Equals("Left"))
            temp_panel = leftObjectList.transform;
        else if (in_panel.Equals("Right"))
            temp_panel = rightObjectList.transform;
        else
            print("Error Panel side");

        foreach (Transform it_obj in temp_panel)
        {
            Destroy(it_obj.gameObject);
        }
        for (int counter = 0; counter < in_backpack.items.Count; counter++)
        {
            createItemBox(in_backpack, counter, this, temp_panel, in_panel);
            createLabel(in_backpack, counter, this, temp_panel, in_panel);
        }
    }
    private void createItemBox(Backpack in_backpack, int in_index, IActionListener in_listener, Transform in_list, string in_side)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(in_list);
        itemGet.transform.localPosition = new Vector3(-85f + 145f * (in_index / 7), 155f - 50f * (in_index % 7), 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            if (in_backpack.items[in_index] != null)
            {
                getHotbar.hotbarItem = in_backpack.items[in_index].ItemObj;
                getHotbar.action = "Select " + in_side + " " + in_index;
                getHotbar.setActionListener(in_listener);
                getHotbar.updateTexts();
                getHotbar.deselect();
            }
        }
    }

    private GameObject createLabel(Backpack in_backpack, int in_index, IActionListener in_listener, Transform in_list, string in_side)
    {
        GameObject tmpLabel = Instantiate(Resources.Load<GameObject>("Label"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpLabel.name = in_backpack.items[in_index].ItemObj.itemName;
        tmpLabel.transform.SetParent(in_list);
        tmpLabel.transform.localPosition = new Vector3(50f + 145f * (in_index / 7), 155f - 50f * (in_index % 7), 0f);
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = (in_backpack.items[in_index].ItemObj.quantity).ToString();
        }

        return tmpLabel;
    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    public IActionListener getActionListener()
    {
        throw new System.NotImplementedException();
    }


    private bool trade(Backpack in_from, Backpack in_to, ItemExistanceDTOWrapper in_item)
    {

        ItemExistanceDTOWrapper hasItem = in_to.items.Find(x => x.ItemObj.itemName.Equals(in_item.ItemObj.itemName));

        if (in_to.items.Count < in_to.size || hasItem != null)
        {
            if (modifier)
            {
                in_to.createItem("Storage", in_item.ItemObj.itemName, in_item.ItemObj.quantity);
                in_from.modifyItem(in_item, in_item.ItemObj.quantity);
                mainMenu();

            }
            else
            {
                in_to.createItem("Storage", in_item.ItemObj.itemName, 1);
                in_from.modifyItem(in_item, 1);
                mainMenu();
            }
            return true;
        }
        else
        {
            currentPlayer.toastNotifications.newNotification("Inventory is full");
            return false;
        }
    }
    public void listen(string getAction)
    {
        string[] parser = getAction.Split(' ');
        switch (parser[0])
        {
            case "Select":
                Dictionary<string, string> payload = new Dictionary<string, string>();
                if (parser[1].Equals("Left"))
                {
                    if (!Network.isConnected) trade(currentPlayer.playerEntity.backpack, focusStorage.storage.inventory, currentPlayer.playerEntity.backpack.items[int.Parse(parser[2])]);
                    else
                    {
                        tradeSelected(currentPlayer.playerEntity.backpack.items[int.Parse(parser[2])], currentPlayer.playerEntity, focusStorage.itemEntity.item);
//                        Network.trade("Entity", currentPlayer.playerEntity.entityName, "Storage", focusStorage.itemEntity.item.itemObj._id, currentPlayer.playerEntity.backpack.items[int.Parse(parser[2])]._id,
//                            modifier ? currentPlayer.playerEntity.backpack.items[int.Parse(parser[2])].ItemObj.quantity : 1);

                    }

                }
                else if (parser[1].Equals("Right"))
                {
                    if (currentPlayer.playerEntity.backpack.spaceAvailable(focusStorage.storage.inventory.items[int.Parse(parser[2])]))
                    {
                        if (!Network.isConnected) 
                            trade(focusStorage.storage.inventory, currentPlayer.playerEntity.backpack, focusStorage.storage.inventory.items[int.Parse(parser[2])]);
                        else
                        {
                            tradeSelected(focusStorage.storage.inventory.items[int.Parse(parser[2])], focusStorage.itemEntity.item, currentPlayer.playerEntity);
                            //                            Network.trade("Storage", focusStorage.itemEntity.item.itemObj._id, "Entity", currentPlayer.playerEntity.entityName, focusStorage.storage.inventory.items[int.Parse(parser[2])]._id,
                            //                                modifier ? focusStorage.storage.inventory.items[int.Parse(parser[2])].ItemObj.quantity : 1);
                        }
                    }
                    else
                    {
                        currentPlayer.toastNotifications.newNotification("You have no more bag space available for any new items.");
                    }
                }
                break;
        }
    }

    public void serverResponseListener()
    {
        if (Network.itemRetrieved.Count > 0)
        {
            ItemExistanceDTOWrapper new_wrapper = Network.itemRetrieved.Dequeue();
            if (new_wrapper.storageObj != null)
            {
                if (focusStorage.itemEntity.item.entityObj._id.Equals(new_wrapper.storageObj._id))
                {
                    ItemExistanceDTOWrapper getItem = focusStorage.storage.inventory.items.Find(x => x.ItemObj.itemName == new_wrapper.ItemObj.itemName);
                    if (getItem != null)
                    {
                        if (new_wrapper.ItemObj.quantity <= 0)
                        {
                            focusStorage.storage.inventory.items.Remove(getItem);
                        }
                        else
                        {
                            getItem.ItemObj.quantity = new_wrapper.ItemObj.quantity;
                        }
                    }
                    else
                    {
                        focusStorage.storage.inventory.items.Add(new_wrapper);
                    }
                    loadPanel("Right", focusStorage.storage.inventory);
                }
            }

            if (new_wrapper.binder != null)
            {
                if (currentPlayer.playerEntity.entityName.Equals(new_wrapper.binder.entityName))
                {
                    ItemExistanceDTOWrapper getItem = currentPlayer.playerEntity.backpack.items.Find(x => x.ItemObj.itemName == new_wrapper.ItemObj.itemName);
                    if (getItem != null)
                    {

                        if (new_wrapper.ItemObj.quantity <= 0)
                        {
                            currentPlayer.playerEntity.backpack.items.Remove(getItem);
                        }
                        else
                        {
                            getItem.ItemObj.quantity = new_wrapper.ItemObj.quantity;
                        }
                    }
                    else
                    {
                        currentPlayer.playerEntity.backpack.items.Add(new_wrapper);
                    }
                    loadPanel("Left", currentPlayer.playerEntity.backpack);
                }

            }
        }

        if (Network.listOfItems.Count > 0)
        {
            List<ItemExistanceDTOWrapper> temp_wrapper = Network.listOfItems.Dequeue();
            foreach (ItemExistanceDTOWrapper it_item in temp_wrapper)
            {
                focusStorage.storage.inventory.items.Add(it_item);
            }
            loadPanel("Right", focusStorage.storage.inventory);
        }
    }
    private void tradeSelected(ItemExistanceDTOWrapper in_item, ITransferer in_from, ITransferer in_to)
    {
        if (transfer_dialog_go == null)
        {
            transfer_dialog_go = Instantiate(Resources.Load<GameObject>("Transfer_amt_dialog"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            transfer_dialog_go.transform.SetParent(canvas_go.transform);
            transfer_dialog_go.transform.localPosition = new Vector3(0f, 0f, 0f);
            if (transfer_dialog_go.TryGetComponent<Transder_Dialog>(out Transder_Dialog out_transfer_dialog))
            {
                out_transfer_dialog.transferLoad(in_item, in_from, in_to, currentPlayer, "Transfer");
            }
        }
    }
}
