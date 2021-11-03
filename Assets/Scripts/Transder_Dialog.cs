using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Transder_Dialog : MonoBehaviour, IActionListener
{
    [SerializeField] private Hotbar actionButton;
    [SerializeField] private Hotbar actionAllButton;
    [SerializeField] private Hotbar cancelButton;
    [SerializeField] private Text actionText;
    [SerializeField] private Text actionAllText;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemQuantity;
//    [SerializeField] private Text itemSliderAmount;
    [SerializeField] private InputField itemQuantityIF;
    [SerializeField] private SliderListener slider;
    private string state;
    private ItemExistanceDTOWrapper item;
    private ITransferer fromType;
    private ITransferer toType;
    private PlayerController currentPlayer;
    //private string fromType;
    //private string fromName;
    //private string toType;
    //private string toName;


    public void transferLoad(ItemExistanceDTOWrapper in_item, ITransferer in_from, ITransferer in_to, PlayerController in_player, string in_state)
    {
        fromType = in_from;
        toType = in_to;
        item = in_item;
        state = in_state;
        currentPlayer = in_player;
        itemName.text = item.ItemObj.itemName;
        itemQuantity.text = " / " + item.ItemObj.quantity;
        itemQuantityIF.text = "1";
        slider.getAction = "Amount";
        slider.listener = this;
        slider.configSlider(0, item.ItemObj.quantity);
        switch (in_state)
        {
            case "Sell":
                actionText.text = "Sell";
                actionButton.action = "Sell";
                actionAllText.text = "Sell All";
                actionAllButton.action = "Sell All";
                break;
            case "Transfer":
                actionText.text = "Transfer";
                actionButton.action = "Transfer";
                actionAllText.text = "Transfer All";
                actionAllButton.action = "Transfer All";
                break;
        }
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
            case "Amount":
                itemQuantityIF.text = getAction.Replace("Amount ", "").Split('.')[0];
                break;
            case "Transfer":
                if (getAction.Equals("Transfer"))
                {
                    Network.trade(fromType.getType(), fromType.getID(), toType.getType(), toType.getID(), item._id, Int32.Parse(itemQuantityIF.text));
                } else if (getAction.Equals("Transfer All"))
                {
                    Network.trade(fromType.getType(), fromType.getID(), toType.getType(), toType.getID(), item._id, item.ItemObj.quantity);
                }
                Destroy(gameObject);
                break;
            case "Cancel":
                Destroy(gameObject);
                break;
            case "Sell":
                if (getAction.Equals("Sell"))
                {
                    sellItem(Int32.Parse(itemQuantityIF.text));
                    //Network.trade(fromType.getType(), fromType.getID(), toType.getType(), toType.getID(), item._id, Int32.Parse(itemQuantityIF.text));
                }
                else if (getAction.Equals("Sell All"))
                {
                    sellItem(item.ItemObj.quantity);
//                    Network.trade(fromType.getType(), fromType.getID(), toType.getType(), toType.getID(), item._id, item.ItemObj.quantity);
                }
                Destroy(gameObject);
                //                sellItem(item, amount);
                break;
        }
    }

    public void sellItem(int amount)
    {
        if (fromType.getInventory().spaceAvailable(item))
        {
            ItemExistanceDTOWrapper currencyTrade = fromType.getInventory().getItemByName("Silver");
            if (currencyTrade != null)
            {
                int totalCost = item.itemMarketObj.buyPrice * amount;
                if (currencyTrade.ItemObj.quantity >= totalCost)
                {
                    //if (currentPlayer.playerEntity.backpack.createItem(currentPlayer.playerEntity.entityName, tradeItem.ItemObj.itemName, 1))
                    //{

                    Network.trade("Entity", toType.getID(), "Entity", fromType.getID(), item._id, amount);
                    //                                currentPlayer.playerEntity.backpack.localCreateItem(tradeItem.ItemObj.itemName, 1);
                    //                            tradeItem.ItemObj.quantity -= 1;
                    //                            currentPlayer.playerEntity.backpack.localCreateItem(currentPlayer.playerEntity.entityName, tradeItem.ItemObj.itemName, 1);
                    Network.trade("Entity", fromType.getID(), "Entity", toType.getID(), currencyTrade._id, totalCost);
                    //                                currentPlayer.playerEntity.backpack.localCreateItem(currencyTrade.ItemObj.itemName, -tradeItem.itemMarketObj.buyPrice);
                    //                            currentPlayer.playerEntity.backpack.localModifyItem(currencyTrade, tradeItem.itemMarketObj.buyPrice);
                    //                                DataCache.adjustMarketPrice(tradeItem);
                    currentPlayer.mainMenu.updateMenu();
                    //}
                    //else
                    //{
                    //    currentPlayer.toastNotifications.newNotification("Your bag is full");
                    //}
                }
                else
                {
                    currentPlayer.toastNotifications.newNotification("You do not have enough Silver");
                }
            }
            else
            {
                currentPlayer.toastNotifications.newNotification("You do not have any Silver");
            }
        }
        else
        {
            currentPlayer.toastNotifications.newNotification("You have no more bag space available for any new items.");
        }
    }
    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        actionAllButton.listener = this;
        actionButton.listener = this;
        cancelButton.listener = this;
        itemQuantityIF.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        Int32.TryParse(itemQuantityIF.text, out int out_num);
        slider.sliderObj.value = out_num;
        if (out_num > item.ItemObj.quantity)
        {
            itemQuantityIF.text = item.ItemObj.quantity.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
