using UnityEngine;
/**
 * 
 * Controller for the doors
 * 
 */
public class Door : MonoBehaviour, IInteractable, ITimeListener
{
    public bool isOpen = false;
    public ItemEntity itemEntity;
    public Transform doorTransform;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && itemEntity.item.entityObj.state.Equals("Closed"))
        {
            isOpen = false;
            doorTransform.parent.position = doorTransform.parent.position - new Vector3(0f, 0f, .5f);
            doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, 90f, 0f);
        }

        if (!isOpen && itemEntity.item.entityObj.state.Equals("Open"))
        {
            isOpen = true;
            doorTransform.parent.position = doorTransform.parent.position + new Vector3(0f, 0f, .5f);
            doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, -90f, 0f);
        }
    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (getInteractor.name.Equals(itemEntity.item.areaObj.areaName.Replace("_farm", "")))
        {
            if (isOpen)
            {
                isOpen = false;
                doorTransform.parent.position = doorTransform.parent.position - new Vector3(0f, 0f, .5f);
                doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, 90f, 0f);
                itemEntity.item.entityObj.state = "Closed";
            }
            else
            {
                if (in_modified)
                {
                    itemEntity.item.entityObj.state = itemEntity.item.entityObj.state.Equals("Locked") ? "Unlocked" : "Locked";
                    getInteractor.toastNotifications.newNotification("Door is now " + itemEntity.item.entityObj.state);
                }
                else
                {
                    if (!itemEntity.item.entityObj.state.Equals("Locked"))
                    {
                        isOpen = true;
                        doorTransform.parent.position = doorTransform.parent.position + new Vector3(0f, 0f, .5f);
                        doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, -90f, 0f);
                        itemEntity.item.entityObj.state = "Open";
                    }
                }
            }
        }
    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
    }

    public void dayEndAction()
    {
        itemEntity.item.entityObj.state = "Closed";
    }

    public void dayBeginAction()
    {
        itemEntity.item.entityObj.state = "Open";
    }
}
