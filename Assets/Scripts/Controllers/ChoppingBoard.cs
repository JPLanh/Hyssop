using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoppingBoard : MonoBehaviour, IInteractable, ICanvas, IContainer
{
    public Storage storage = new Storage();
    [SerializeField] private CanvasHelper UICanvas;
    public GameObject choppingStatusUI;
    public PlayerController activePC;
    public ItemEntity itemEntity;
    [System.NonSerialized] public ItemExistanceDTOWrapper currentProduce;
    [System.NonSerialized] public List<ItemExistanceDTOWrapper> choppedProduce;

    public void getCanvas(CanvasHelper in_canvas)
    {
        UICanvas = in_canvas;
    }

    public void getPlayer(PlayerController in_player)
    {
        activePC = in_player;
    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (getInteractor.playerEntity.getHolding() != null)
        {
            if (getInteractor.playerEntity.getHolding().ItemObj.itemType.Equals("Produce"))
            {
                if (currentProduce == null)
                {
                    Network.trade("Entity", getInteractor.playerEntity._id, "Storage", itemEntity.item.entityObj._id, getInteractor.playerEntity.getHolding()._id, 1);
                    getInteractor.toastNotifications.newNotification("You placed a " + getInteractor.playerEntity.getHolding().ItemObj.itemName + " on the chopping board");
                }
                else
                {
                    getInteractor.toastNotifications.newNotification("There's a " + currentProduce.ItemObj.itemName + " already on the chopping board");
                }
            } else
            {
                getInteractor.toastNotifications.newNotification("Can't place the " + currentProduce.ItemObj.itemName + " on the chopping board");
            }
        } else
        {
            //move produce into the chop board "storage"
            activePC = getInteractor;
            choppingStatusUI = Instantiate(Resources.Load<GameObject>("Chop Status"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            //checkBoard(getInteractor);
            if (choppingStatusUI.TryGetComponent<chopStatus>(out chopStatus out_chop))
            {
                out_chop.choppingBoard = this;
            }

            choppingStatusUI.transform.SetParent(UICanvas.transform);
            choppingStatusUI.transform.localPosition = new Vector3(0f, 0f, 0f);
            getInteractor.freeze();
            Cursor.lockState = CursorLockMode.None;
            getInteractor.playerEntity.state = "Chopping";
        }
    }


    public void refreshChoppedList()
    {
        choppedProduce = new List<ItemExistanceDTOWrapper>();
        currentProduce = null;

        for (int it_counter = 0; it_counter < storage.inventory.items.Count; it_counter++)
        {
            if (storage.inventory.items[it_counter].ItemObj.itemType.Equals("Produce"))
                currentProduce = storage.inventory.items[it_counter];
            else
            {
                choppedProduce.Add(storage.inventory.items[it_counter]);

            }
        }
    }

    private void checkBoard(PlayerController in_interactor)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["storageID"] = itemEntity.item.entityObj._id;
        Network.sendPacket(doCommands.storage, "Access", payload);

    }

    public string getID()
    {
        return itemEntity.item.entityObj._id;
    }
    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        storage.inventory.size = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init()
    {
        if (activePC != null && choppingStatusUI != null)
        {
            if (choppingStatusUI.TryGetComponent<chopStatus>(out chopStatus out_chop))
            {
                out_chop.init();
            }
        }
    }

    public void setStorage(List<ItemExistanceDTOWrapper> in_item_list)
    {
        storage.inventory.items = in_item_list;
        refreshChoppedList();
    }

    public void modifyStorage(ItemExistanceDTOWrapper in_item)
    {
        storage.inventory.refreshItem(in_item);
        if (in_item.ItemObj.itemType.Equals("Produce"))
        {
            currentProduce = in_item;
        }
        init();
    }
}
