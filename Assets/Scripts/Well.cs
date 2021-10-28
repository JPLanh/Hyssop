using UnityEngine;

public class Well : MonoBehaviour, IInteractable
{
    [SerializeField] private AreaItemDTO itemEntity;
    private float counterTime = 0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Network.isConnected)
        {
            if (itemEntity.entityObj.capacity < itemEntity.entityObj.maxCapacity)
            {
                counterTime += Time.deltaTime;
                if (counterTime > 1)
                {
                    itemEntity.entityObj.durability += 5;
                    counterTime = 0f;
                }
                if (itemEntity.entityObj.durability >= itemEntity.entityObj.maxDurability)
                {
                    itemEntity.entityObj.capacity = itemEntity.entityObj.capacity + itemEntity.entityObj.quantity < itemEntity.entityObj.maxCapacity ?
                        itemEntity.entityObj.capacity + itemEntity.entityObj.quantity :
                        itemEntity.entityObj.maxCapacity;
                    itemEntity.entityObj.durability = 0;
                }
            }
        }
    }


    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (getInteractor.playerEntity.holding != null && getInteractor.playerEntity.getHolding().ItemObj.itemType != null)
        {
            if (getInteractor.playerEntity.getHolding().ItemObj.itemType.Equals("Watering Can"))
            {
                getInteractor.canvas.progressBar.activate("Refilling", (int)(getInteractor.playerEntity.getHolding().ItemObj.maxCapacity / 10), getInteractor);

            }

        }

    }

    public void reaction(PlayerController getInteractor)
    {
//        int max_amount = getInteractor.playerEntity.getHolding().ItemObj.maxCapacity - getInteractor.playerEntity.getHolding().ItemObj.capacity;
//        int amountFill = itemEntity.itemObj.capacity >= max_amount ? max_amount : itemEntity.itemObj.capacity;
//        itemEntity.itemObj.capacity -= amountFill;
        getInteractor.playerEntity.getHolding().ItemObj.capacity = getInteractor.playerEntity.getHolding().ItemObj.maxCapacity;
//        print(itemEntity.itemObj.capacity + " , " + amountFill);
        //if (amountFill != 0)
        {
            //AreaItemDTO item_update = new AreaItemDTO();
            //item_update.itemObj = itemEntity.item.getDTO();
            //item_update._id = itemEntity.name;
            //item_update.areaName = getInteractor.currentGrid.areaName;
            //item_update.position = CustomUtilities.vector3Rounder(transform.position);
            //item_update.rotation = CustomUtilities.vector3Rounder(transform.rotation);
//            Network.areaItemInteract(getInteractor.playerEntity.getHolding(), "refill water");
        }
    }
}
