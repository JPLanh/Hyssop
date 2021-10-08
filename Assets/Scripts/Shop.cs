using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable, IMenu, IActionListener
{
    PlayerController activePC;
    public Entity currentNPC;
    ArrayList acceptable;
    public string prevPlayerState;
    GameObject exitMenu;


    public void accessMenu(PlayerController getPC)
    {
        //Display the UI
        //for (int index = 0; index < inventory.toolbars.Length; index++)
        //{
        //    GameObject itemGet = inventory.toolbars[index];
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

        //Display the Exit Button
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

    public ArrayList getAcceptable()
    {
        return acceptable;
    }

    public Backpack getInventory()
    {
        return currentNPC.backpack;
    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (!getInteractor.isPaused)
        {
            activePC = getInteractor;
            prevPlayerState = getInteractor.playerEntity.state;
            getInteractor.playerEntity.state = currentNPC.state;
            getInteractor.mainMenu.focusShop = this;
            Dictionary<string, string> list_Dialog = currentNPC.getDialog();
            string prompt = "Prompt 1";
            getInteractor.newMessage(currentNPC.entityName, list_Dialog[prompt]);
            string[] promptParser = prompt.Split(' ');
            foreach (KeyValuePair<string, string> it_prompt in list_Dialog)
            {
                string[] responseParser = it_prompt.Key.Split('.');
                if (responseParser[0].Equals(prompt) && responseParser.Length != 1)
                {

                    getInteractor.newResponse(it_prompt.Value);
                }
            }

            //foreach (ItemExistanceDTOWrapper it_item in currentNPC.backpack.items)
            //{
            //    DataCache.adjustMarketPrice(it_item);
            //}
        }
    }


    public IActionListener getActionListener()
    {
        return this;
    }

    public void listen(string getAction)
    {
        //string[] parseAction = getAction.Split(' ');
        //switch (parseAction[0])
        //{
        //    case "Hover":
        //        Item hoverItem = currentNPC.backpack.items[int.Parse(parseAction[1])];
        //        activePC.newMessage(currentNPC.entityName, "The price for " + hoverItem.itemName + " goes for " + hoverItem.buyPrice + " silvers, and I currently have " + hoverItem.quantity);
        //        break;
        //    case "Select":
        //        Item tradeItem = currentNPC.backpack.items[int.Parse(parseAction[1])];
        //        if (tradeItem != null)
        //        {
        //            Item currencyTrade = activePC.playerEntity.backpack.getItemByName("Silver");
        //            if (currencyTrade != null)
        //            {
        //                if (currencyTrade.quantity >= tradeItem.buyPrice)
        //                {
        //                    if (activePC.playerEntity.backpack.createItem(activePC.playerEntity.entityName, tradeItem.itemName, 1))
        //                    {
        //                        currentNPC.backpack.modifyItem(tradeItem, 1);
        //                        currentNPC.backpack.createItem(currentNPC.entityName, "Silver", tradeItem.buyPrice);
        //                        currencyTrade.quantity -= tradeItem.buyPrice;
        //                        DataCache.adjustMarketPrice(tradeItem);
        //                        activePC.mainMenu.updateMenu();

        //                    }
        //                    else
        //                    {
        //                        activePC.toastNotifications.newNotification("Your bag is full");
        //                    }
        //                }
        //                else
        //                {
        //                    activePC.toastNotifications.newNotification("You do not have enough Silver");
        //                }
        //            }
        //            else
        //            {
        //                activePC.toastNotifications.newNotification("You do not have any Silver");
        //            }
        //        }
        //        break;
        //    default:
        //        //if (getAction.Equals("Exit Menu"))
        //        //{
        //        //    foreach (GameObject itemGet in currentNPC.backpack)
        //        //    {
        //        //        itemGet.transform.SetParent(transform);
        //        //        activePC.menuToggle(true);
        //        //    }
        //        //    Destroy(activePC.mainMenu);
        //        //}
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
        acceptable.Add("Money");

        //        pickupItem("Seed", 25, "Seed", false, 2);
        //inventory.pickupItem("Inventory", "Seed", 25, "Seed", 2);
        //        inventory.createItem("Strawberry Seed", 25, this);
        //        inventory.createItem("Grape Seed", 25, this);
        //int value = Random.Range(3, 7);
        //inventory.adjustPrice("Strawberry Seed", value);
        //value = Random.Range(3, 7);
        //inventory.adjustPrice("Grape Seed", value);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }
}
