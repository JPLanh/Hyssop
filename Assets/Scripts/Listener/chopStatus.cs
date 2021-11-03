using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chopStatus : MonoBehaviour, IActionListener
{
    [SerializeField] private GameObject chopMarker;
    private int index = 0;
    private int indexMulti = 2;
    private static float fixTimer = .1f;
    private int maxMarker = 100;
    private int minMarker = 0;
    private float markerMove = 4.9f;
    private float spamTimer = fixTimer;
    private float startMarker = -247.5f;
    public ChoppingBoard choppingBoard;
    [SerializeField] private Transform progressBar;
    [SerializeField] private Transform chopListGameObject;
    [SerializeField] private Text onBoardText;
    public GameObject transfer_dialog_go;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    public void init()
    {
        refreshChoppedList();

        if (choppingBoard.currentProduce != null)
        {
            createButton("Remove chopping", choppingBoard.currentProduce.ItemObj.itemName, new Vector3(-190f, -27f, 0f), chopListGameObject);

            index = minMarker = 100 - choppingBoard.currentProduce.ItemObj.durability;
            chopMarker.transform.localPosition = new Vector3(startMarker + (index * markerMove), 0f, 0f);
            progressBar.localPosition = new Vector3(index * markerMove, 0f, 0f);
        }

        createButton("Close Chop", "Close", new Vector3(190f, -125f, 0f), chopListGameObject);
    }


    public void refreshChoppedList()
    {
        choppingBoard.refreshChoppedList();
        foreach (Transform it_chopItem in chopListGameObject)
        {
            Destroy(it_chopItem.gameObject);
        }
        choppingBoard.choppedProduce = new List<ItemExistanceDTOWrapper>();

        for (int it_counter = 0; it_counter < choppingBoard.storage.inventory.items.Count; it_counter++)
        {
            if (choppingBoard.storage.inventory.items[it_counter].ItemObj.itemType.Equals("Produce"))
                choppingBoard.currentProduce = choppingBoard.storage.inventory.items[it_counter];
            else
            {
                choppingBoard.choppedProduce.Add(choppingBoard.storage.inventory.items[it_counter]);
                createItemBox(choppingBoard.choppedProduce, choppingBoard.choppedProduce.Count - 1, choppingBoard.currentPlayer, chopListGameObject);
                createLabel(choppingBoard.choppedProduce, choppingBoard.choppedProduce.Count - 1, choppingBoard.currentPlayer, chopListGameObject);

            }
        }
    }

    public void createButton(string in_action, string in_button, Vector3 in_position, Transform objectList)
    {
        GameObject tmp_obj = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmp_obj.transform.SetParent(objectList);
        tmp_obj.transform.localPosition = in_position;
        if (tmp_obj.TryGetComponent<Hotbar>(out Hotbar out_hotbar))
        {
            out_hotbar.action = in_action;
            out_hotbar.listener = this;
            out_hotbar.hotbarText.text = in_button;
        }
    }
    private void close()
    {

        foreach (Transform it_chopItem in chopListGameObject)
        {
            Destroy(it_chopItem.gameObject);
        }

        choppingBoard.currentPlayer.unfreeze();
        choppingBoard.currentPlayer = null;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject);
    }

    private void tradeSelected(ItemExistanceDTOWrapper in_item, ITransferer in_from, ITransferer in_to)
    {
        if (transfer_dialog_go == null)
        {
            transfer_dialog_go = Instantiate(Resources.Load<GameObject>("Transfer_amt_dialog"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            transfer_dialog_go.transform.SetParent(transform);
            transfer_dialog_go.transform.localPosition = new Vector3(0f, 0f, 0f);
            if (transfer_dialog_go.TryGetComponent<Transder_Dialog>(out Transder_Dialog out_transfer_dialog))
            {
                out_transfer_dialog.transferLoad(in_item, in_from, in_to, choppingBoard.currentPlayer, "Transfer");
            }
        }
    }

    private void createItemBox(List<ItemExistanceDTOWrapper> in_items, int in_index, IActionListener in_listener, Transform in_list)
    {
        GameObject itemGet = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        itemGet.transform.SetParent(in_list);
        itemGet.transform.localPosition = new Vector3(-190f + 145f * (in_index / 2), -90f - 50f * (in_index % 2), 0f);
        itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        itemGet.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        {
            if (in_items[in_index] != null)
            {
                getHotbar.hotbarItem = in_items[in_index].ItemObj;
                getHotbar.action = "Select " + in_index;
                getHotbar.setActionListener(this);
                getHotbar.updateTexts();
                getHotbar.deselect();
            }
        }
    }

    private GameObject createLabel(List<ItemExistanceDTOWrapper> in_items, int in_index, IActionListener in_listener, Transform in_list)
    {
        GameObject tmpLabel = Instantiate(Resources.Load<GameObject>("Label"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpLabel.name = in_items[in_index].ItemObj.itemName;
        tmpLabel.transform.SetParent(in_list);
        tmpLabel.transform.localPosition = new Vector3(-55f + 145f * (in_index / 2), -90f - 50f * (in_index % 2), 0f);
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = (in_items[in_index].ItemObj.quantity).ToString();
        }

        return tmpLabel;
    }
    // Update is called once per frame
    void Update()
    {
        if (spamTimer >= 0)
        {
            spamTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && index>minMarker)
        {
            progressBar.localPosition = new Vector3(index * markerMove, 0f, 0f);
            string chopItem = null;
            if (index-minMarker <= 3){
                chopItem = "Thin " + choppingBoard.currentProduce.ItemObj.itemName + " Slice";
            } else
            {
                chopItem = "Thick " + choppingBoard.currentProduce.ItemObj.itemName + " Slice";
            }

            //ItemExistanceDTOWrapper out_item = items.Find(x => x.ItemObj.itemName.Equals(chopItem));
            //if (out_item != null)
            //{
            //    out_item.ItemObj.quantity += 1;
            //}


            choppingBoard.currentProduce.ItemObj.durability = 100 - (int)(100 * ((float)index / (float)maxMarker));
            if (choppingBoard.currentProduce.ItemObj.durability > 0)
            {
                Network.sendPacket<ItemExistanceDTOWrapper>(doCommands.item, "Save", choppingBoard.currentProduce);
            } else
            {
                Network.sendPacket<ItemExistanceDTOWrapper>(doCommands.item, "Remove", choppingBoard.currentProduce);
            }

            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["storage"] = choppingBoard.getID();
            payload["item"] = chopItem;
            payload["quantity"] = "1";
            Network.sendPacket(doCommands.storage, "Pickup", payload);
//            Network.itemUpdated(doCommands.playerItem, "Pickup Item", chopItem, 1);
            minMarker = index;

            if (minMarker == maxMarker)
            {
                print("Delete " + choppingBoard.currentProduce._id);
                Dictionary<string, string> remove_payload = new Dictionary<string, string>();
                remove_payload["data"] = choppingBoard.currentProduce._id;
                Network.sendPacket(doCommands.item, "Remove", remove_payload);
                choppingBoard.storage.inventory.items.Remove(choppingBoard.currentProduce);
                refreshChoppedList();
            }
        }

            if (Input.GetButtonDown("Move Right") || Input.GetButtonDown("Rotate Right"))
        {
            if (index < maxMarker)
            {
                spamTimer = fixTimer*2;
                index+=indexMulti;
                chopMarker.transform.localPosition = new Vector3(startMarker + (index * markerMove), 0f, 0f);
            }
        }

        if (Input.GetButtonDown("Move Left") || Input.GetButtonDown("Rotate Left"))
        {
                if (index > minMarker)
                {
                    spamTimer = fixTimer*2;
                    index -= indexMulti;
                    chopMarker.transform.localPosition = new Vector3(startMarker + (index * markerMove), 0f, 0f);
                }
        }

        if (Input.GetButton("Move Right") || Input.GetButton("Rotate Right"))
        {
            if (spamTimer <= 0f)
            {
                if (index < maxMarker)
                {
                    spamTimer = fixTimer;
                    index += indexMulti;
                    chopMarker.transform.localPosition = new Vector3(startMarker + (index * markerMove), 0f, 0f);
                }
            }
        }
        if (Input.GetButton("Move Left") || Input.GetButton("Rotate Left"))
        {
            if (spamTimer <= 0f)
            {
                if (index > minMarker)
                {
                    spamTimer = fixTimer;
                    index -= indexMulti;
                    chopMarker.transform.localPosition = new Vector3(startMarker + (index * markerMove), 0f, 0f);
                }
            }
        }
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
        string[] parsed = getAction.Split(' ');
        switch (parsed[0])
        {
            case "Close":
                close();
                break;
            case "Select":
                tradeSelected(choppingBoard.choppedProduce[int.Parse(parsed[1])], choppingBoard.itemEntity.item, choppingBoard.currentPlayer.playerEntity);
                choppingBoard.refreshChoppedList();
                break;
            case "Remove":
                tradeSelected(choppingBoard.currentProduce, choppingBoard.itemEntity.item, choppingBoard.currentPlayer.playerEntity);
                choppingBoard.refreshChoppedList();
                break;

        }
    }

    //public void serverResponseListener()
    //{

    //    if (Network.itemRetrieved.Count > 0)
    //    {
    //        ItemExistanceDTOWrapper new_wrapper = Network.itemRetrieved.Dequeue();
    //        if (new_wrapper.storageObj != null)
    //        {
    //            if (choppingBoard.storage.inventory.items[0]._id.Equals(new_wrapper.storageObj._id))
    //            {
    //                ItemExistanceDTOWrapper getItem = items.Find(x => x.ItemObj.itemName == new_wrapper.ItemObj.itemName);
    //                if (getItem != null)
    //                {
    //                    if (new_wrapper.ItemObj.quantity <= 0)
    //                    {
    //                        items.Remove(getItem);
    //                    }
    //                    else
    //                    {
    //                        getItem.ItemObj.quantity = new_wrapper.ItemObj.quantity;
    //                    }
    //                }
    //                else
    //                {
    //                    items.Add(new_wrapper);
    //                }
    //            }
    //        }
    //    }

    //    if (Network.listOfItems.Count > 0)
    //    {
    //        List<ItemExistanceDTOWrapper> temp_wrapper = Network.listOfItems.Dequeue();
    //        foreach (ItemExistanceDTOWrapper it_item in temp_wrapper)
    //        {
    //            focusStorage.storage.inventory.items.Add(it_item);
    //        }
    //        loadPanel("Right", focusStorage.storage.inventory);
    //    }
    //}
}
