using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageEntity : MonoBehaviour, IInteractable, IMenu
{
    public Storage storage = new Storage();
    //    public backpack storage = new backpack();
    public ItemEntity itemEntity;
    [SerializeField] private GameObject exitMenu;
    private PlayerController activePC;
    private ArrayList acceptable;
    public Storage deliverPoint;



    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (!getInteractor.isPaused)
        {
            activePC = getInteractor;
            getInteractor.canvas.tradeMenu.currentPlayer = getInteractor;
            getInteractor.canvas.tradeMenu.focusStorage = this;
            getInteractor.accessTrade();
            storage.inventory.items = new List<ItemExistanceDTOWrapper>();
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["storageID"] = itemEntity.item.itemObj._id;
            Network.sendPacket(doCommands.storage, "Access", payload);
//            Network.accessStorage("Storage", name);
        }
    }

    public IActionListener getActionListener()
    {
        return this;
    }

    public void listen(string getAction)
    {
        //print(getAction);
        //string[] parseAction = getAction.Split(' ');
        //switch (parseAction[0])
        //{
        //    case "Select":
        //        Item getItem = toolbars.getItem(int.Parse(parseAction[1]));
        //        if (getItem != null)
        //        {
        //            //activePC.currentToolbar.pickupItem("Inventory", getItem, activePC.getActionListener());
        //            toolbars.selectItem(int.Parse(parseAction[1]));
        //            toolbars.useItem(getItem.quantity);
        //            toolbars.deselect();
        //        }
        //        //GameObject itemSelect = toolbars.toolbars[int.Parse(parseAction[1])];
        //        //if (itemSelect.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //        //{
        //        //    if (getHotbar.getItem() != null)
        //        //    {
        //        //        //activePC.toolbars.pickupItem("Inventory", getHotbar.getItem());
        //        //        //toolbars.selectItem(int.Parse(parseAction[1]));
        //        //        //toolbars.useItem(getHotbar.getItem().quantity);
        //        //        //toolbars.deselect();
        //        //    }
        //        //}
        //        break;
        //    default:
        //        if (getAction.Equals("Exit Menu"))
        //        {
        //            foreach (GameObject itemGet in toolbars.toolbars)
        //            {
        //                itemGet.transform.SetParent(transform);
        //                activePC.activeMode();
        //            }
        //            Destroy(activePC.mainMenu);
        //        }
        //        break;
        //}
    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        acceptable = new ArrayList();
        acceptable.Add("Ingrediants");
        storage.inventory.size = 10;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void accessMenu(PlayerController getPC)
    {
        //Display the UI
        //int counter = 0;
        //foreach (GameObject itemGet in toolbars.toolbars)
        //{
        //    itemGet.transform.SetParent(getPC.mainMenu.transform);
        //    itemGet.transform.localPosition = new Vector3(-100f + 50f * (counter % 5), 100f - 50f * (counter / 5), 0f);
        //    itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        //    if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //    {
        //        if (getHotbar.getItem() != null)
        //            getHotbar.setActionListener(this);
        //    }
        //    counter++;
        //}

        ////Display the Exit Button
        //exitMenu = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        //exitMenu.transform.SetParent(getPC.mainMenu.transform);
        //exitMenu.transform.localPosition = new Vector3(112f, 140f, 0f);
        //exitMenu.transform.localScale = new Vector3(.5f, .5f, 1f);
        //if (exitMenu.TryGetComponent<Hotbar>(out Hotbar getExitButton))
        //{
        //    getExitButton.action = "Exit Menu";
        //    getExitButton.setActionListener(this);
        //}
    }

    public Backpack getInventory()
    {
        return null;
    }

    public ArrayList getAcceptable()
    {
        return acceptable;
    }

    public void dayBegin()
    {
    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }
}
