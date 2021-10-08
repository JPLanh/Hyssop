using UnityEngine;

public class ShippingBinUI : MonoBehaviour
{
    public IMenu container;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool transferTo(PlayerController activePC, Item getItem, int amount)
    {
        if (getItem != null && container.getAcceptable().Contains(getItem.itemType))
        {
            //activePC.currentToolbar.select(getItem.itemIndex);
            //            container.getInventory().pickupItem("Inventory", getItem, container.getActionListener());
            //            container.getInventory().pickupItem("Inventory", getItem.itemName, amount, getItem.itemType, getItem.tradeValue);
            //activePC.currentToolbar.useItem(amount);
            activePC.deselectHotbar();
        }
        return true;
    }
}
