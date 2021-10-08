using System.Collections;
using UnityEngine;

public class mail : MonoBehaviour, IMenu, IInteractable, IActionListener, IDayNightCycle
{
    public Backpack inventory = new Backpack();
    private PlayerController activePC;
    [SerializeField] private GameObject exitMenu;
    private ArrayList acceptable;

    // Start is called before the first frame update
    void Start()
    {
        acceptable = new ArrayList();
        acceptable.Add("Money");
        inventory.size = 25;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (!getInteractor.isPaused)
        {
            activePC = getInteractor;
            //            prevPlayerState = getInteractor.playerEntity.state;
            getInteractor.playerEntity.state = "Storage";
            //            getInteractor.mainMenu.focusShop = this;
            getInteractor.menuToggle(true);
            //            getInteractor.mainMenu.init();
        }

    }

    public void accessMenu(PlayerController getPC)
    {
        //Display the UI
        //for (int index = 0; index < mailInventory.toolbars.Length; index++)
        //{
        //    GameObject itemGet = mailInventory.toolbars[index];
        //    itemGet.transform.SetParent(getPC.mainMenu.transform);
        //    itemGet.transform.localPosition = new Vector3(-100f + 50f * (index % 5), 100f - 50f * (index / 5), 0f);
        //    itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        //    if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //    {
        //        if (getHotbar.getItem() != null)
        //        {
        //            getHotbar.setActionListener(this);
        //        }
        //    }
        //}
        //int counter = 0;
        //foreach (GameObject itemGet in mailInventory.toolbars)
        //{

        //    itemGet.transform.SetParent(getPC.mainMenu.transform);
        //    itemGet.transform.localPosition = new Vector3(-100f + 50f * (counter % 5), 100f - 50f * (counter / 5), 0f);
        //    itemGet.transform.localScale = new Vector3(1f, 1f, 1f);
        //    if (itemGet.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //    {
        //        if (getHotbar.getItem() != null)
        //        {
        //            getHotbar.setActionListener(this, "Transfer " + itemGet.name);
        //            print("Got item");
        //        }
        //    }
        //    counter++;
        //}

        //Display the Exit Button
        exitMenu = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        exitMenu.transform.SetParent(getPC.mainMenu.transform);
        exitMenu.transform.localPosition = new Vector3(112f, 140f, 0f);
        exitMenu.transform.localScale = new Vector3(.5f, .5f, 1f);
        if (exitMenu.TryGetComponent<Hotbar>(out Hotbar getExitButton))
        {
            getExitButton.action = "Exit Menu";
            getExitButton.setActionListener(this);
        }
    }

    public Backpack getInventory()
    {
        return null;
    }

    public IActionListener getActionListener()
    {
        return this;
    }

    public void listen(string getAction)
    {
        //        string[] parseAction = getAction.Split(' ');
        //        switch (parseAction[0])
        //        {
        //            case "Select":
        ////                Item getItem = mailInventory.getItem(parseAction[1]);
        //                GameObject itemSelect = mailInventory.toolbars[int.Parse(parseAction[1])];
        //                if (itemSelect.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //                {
        //                    if (getHotbar.getItem() != null)
        //                    {
        //                        //activePC.currentToolbar.pickupItem("Inventory", getHotbar.getItem(), this);
        //                        mailInventory.selectItem(int.Parse(parseAction[1]));
        //                        mailInventory.useItem(getHotbar.getItem().quantity);
        //                        mailInventory.deselect();
        //                    }
        //                }
        //                break;
        //            default:
        //                if (getAction.Equals("Exit Menu"))
        //                {
        //                    foreach (GameObject itemGet in mailInventory.toolbars)
        //                    {
        //                        itemGet.transform.SetParent(transform);
        //                        activePC.activeMode();
        //                    }
        //                    Destroy(activePC.mainMenu);
        //                }
        //                break;
        //        }

    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    public bool transfer(Item getItem)
    {
        throw new System.NotImplementedException();
    }

    public ArrayList getAcceptable()
    {
        return acceptable;
    }

    public void dayFinished()
    {
    }

    public void dayBegin()
    {
        //foreach (GameObject inventoryIndex in mailInventory.pendingItem)
        //{
        //    if (inventoryIndex.TryGetComponent<Hotbar>(out Hotbar getHotbar))
        //    {
        //        if (getHotbar.getItem() != null)
        //        {
        //            //if (mailInventory.createItem("Silver", getHotbar.getItem().quantity * 5, this))
        //            //{
        //            //    print("Sold");
        //            //    getHotbar.removeItem();
        //            //}
        //        }
        //    }
        //}

    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }
}
