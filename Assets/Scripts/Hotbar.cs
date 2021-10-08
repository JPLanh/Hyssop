using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour, IActionListener
{

    public Text hotbarText;
    [SerializeField] private Text hotbarQuantity;
    [SerializeField] private Image borderColor;
    public ItemDTO hotbarItem;

    [SerializeField] private MenuSelectionBox selectionBoxAction;

    public string action;
    public string hover;
    public IActionListener listener;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void removeItem()
    {
        print(hotbarItem.itemName + " has been sold");
        hotbarItem = null;
        hotbarText.text = null;
        hotbarQuantity.text = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateTexts()
    {
        if (hotbarItem != null)
        {
            //            hotbarQuantity.text = hotbarItem.quantity.ToString();
            hotbarText.text = hotbarItem.itemName;
        }
        else
        {
            hotbarQuantity.text = null;
            hotbarText.text = null;
        }
    }

    //public bool pickupItem(string getItem, int quantity, string type, int getIndex, int price, IActionListener getListener)
    //{
    //    if (hotbarItem == null)
    //        hotbarItem = new Item(getItem, quantity, type, getIndex, price);
    //    else
    //    {
    //        if (hotbarItem.itemName.Equals(getItem))
    //        {
    //            hotbarItem.quantity += quantity;
    //        }
    //    }
    //    listener = getListener;
    //    updateTexts();
    //    return true;
    //}

    public bool pickupItem(ItemExistanceDTOWrapper in_item, IActionListener getListener)
    {
        if (hotbarItem == null || string.IsNullOrEmpty(hotbarItem.itemName))
            hotbarItem = in_item.ItemObj;
        else
        {
            if (hotbarItem.itemName.Equals(in_item.ItemObj.itemName))
            {
                hotbarItem.quantity += in_item.ItemObj.quantity;
            }
        }
        listener = getListener;
        updateTexts();
        return true;
    }

    public void select()
    {
        borderColor.color = new Color(0, 150f / 255f, 255f / 255f, 255f / 255f);
    }

    public void deselect()
    {
        borderColor.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);
    }

    public ItemDTO getItem()
    {
        return hotbarItem;
    }

    public bool useItem(int amount)
    {
        hotbarItem.quantity -= amount;
        if (hotbarItem.quantity <= 0)
        {
            hotbarItem = null;
            updateTexts();
            return false;
        }
        else
        {
            updateTexts();
            return true;
        }
    }

    public string itempickupCheck(string getItem)
    {
        if (hotbarItem == null || string.IsNullOrEmpty(hotbarItem.itemName))
        {
            return "Empty";
        }
        else if (hotbarItem.itemName.Equals(getItem))
        {
            return "Same";
        }
        else
        {
            return "Different";
        }
    }

    public void setActionListener(IActionListener listener)
    {
        this.listener = listener;
    }


    public IActionListener getActionListener()
    {
        return listener;
    }

    public void listen(string getAction)
    {
        throw new System.NotImplementedException();
    }

    public void doAction()
    {
        if (!action.Equals("") && listener != null)
            listener.listen(action);
    }

    public void doHover()
    {
        if (!action.Equals("") && listener != null)
            listener.listen(hover);

    }
}
