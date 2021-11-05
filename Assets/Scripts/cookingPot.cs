using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cookingPot : MonoBehaviour, IInteractable, ICanvas, IContainer
{
    public Storage storage = new Storage();
    [SerializeField] private CanvasHelper UICanvas;
    public GameObject cookingStatusUI;
    public PlayerController activePC;
    public ItemEntity itemEntity;

    [System.NonSerialized] public ItemExistanceDTOWrapper currentProduce;
    [System.NonSerialized] public List<ItemExistanceDTOWrapper> choppedProduce;
    private float cookingInterval = 5f;
    private float cookingTimer = 0;
    private float timeCooked = 0;
    public int cookingcounter = 0;
    private bool cookingDone = false;

    [Header("Stirring properties")]
    public int stirIndex = 0;
    public int mixStirValue = 0;
    public int minStirValue = -50;
    public int maxStirValue = 50;
    public int stirOverstirThreshold = 30;
    public int stirUnderstirThreshold = -10;
    public int stirValue = 0;

    [Header("Heating properties")]
    public bool fireOn = false;
    public int heatDelay = 0;
    public int heatIndex = 0;
    public int heatOvercookThreshold = 40;
    public int heatUndercookThreshold = 10;
    public int maxHeatValue = 50;
    public int minHeatValue = -50;
    public int heatValue = 0;


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
        activePC = getInteractor;
        if (getInteractor.playerEntity.getHolding() != null)
        {
            if (getInteractor.playerEntity.getHolding().ItemObj.itemType.Equals("Ingrediant"))
            {
                    Network.trade("Entity", getInteractor.playerEntity._id, "Storage", itemEntity.item.entityObj._id, getInteractor.playerEntity.getHolding()._id, 1);
                    getInteractor.toastNotifications.newNotification("You placed a " + getInteractor.playerEntity.getHolding().ItemObj.itemName + " into the cooking pot");
            }
            else
            {
                getInteractor.toastNotifications.newNotification("You can't put a " + getInteractor.playerEntity.getHolding().ItemObj.itemName + " into the cooking pot");
            }
        }
        else
        {
            //move produce into the chop board "storage"
            activePC = getInteractor;
            cookingStatusUI = Instantiate(Resources.Load<GameObject>("Cooking_UI"), new Vector3(0f, 0f, 0f), Quaternion.identity);
            //checkBoard(getInteractor);
            if (cookingStatusUI.TryGetComponent<cookingUI>(out cookingUI out_cook))
            {
                out_cook.cookingPot = this;
            }

            cookingStatusUI.transform.SetParent(UICanvas.transform);
            cookingStatusUI.transform.localPosition = new Vector3(0f, 0f, 0f);
            getInteractor.freeze();
            Cursor.lockState = CursorLockMode.None;
            getInteractor.playerEntity.state = "Cooking";
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
        if (fireOn)
        {
            cookingTimer -= Time.deltaTime;
            if (cookingTimer <= 0)
            {
                cookingTimer = cookingInterval;
                if (stirIndex > 0 && stirIndex <= maxStirValue)
                {
                    stirIndex++;
                }
                if (stirIndex <= 0 && stirIndex >= minStirValue)
                {
                    stirIndex--;
                }

                if (heatIndex > 0 && heatIndex <= maxHeatValue)
                {
                    heatIndex++;
                }
                if (heatIndex <= 0 && heatIndex >= minHeatValue)
                {
                    heatIndex--;
                }

                if (stirIndex >= stirUnderstirThreshold && stirValue <= stirOverstirThreshold)
                {
                    stirValue++;
                } else
                {
                    stirValue -= 2;
                }

                if (heatIndex >= heatUndercookThreshold && heatValue <= heatOvercookThreshold)
                {
                    heatValue++;
                }
                else
                {
                    heatValue -= 2;
                }

                timeCooked++;

                if (timeCooked >= cookingTimer * 10)
                {
                    heatOvercookThreshold = 0;
                    heatUndercookThreshold = 0;
                    stirOverstirThreshold = 0;
                    stirUnderstirThreshold = 0;
                    cookingDone = true;
                }
            }
        }
    }

    public void init()
    {
        if (activePC != null && cookingStatusUI != null)
        {
            if (cookingStatusUI.TryGetComponent<cookingUI>(out cookingUI out_cook))
            {
                out_cook.init();
            }
        }
    }

    public void setStorage(List<ItemExistanceDTOWrapper> in_item_list)
    {
        storage.inventory.items = in_item_list;
    }

    public void modifyStorage(ItemExistanceDTOWrapper in_item)
    {
        storage.inventory.refreshItem(in_item);
        init();
    }
}
