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
        if (isOpen && itemEntity.item.state.Equals("Closed"))
        {
            isOpen = false;
            doorTransform.parent.position = doorTransform.parent.position - new Vector3(0f, 0f, .5f);
            doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, 90f, 0f);
        }

        if (!isOpen && itemEntity.item.state.Equals("Open"))
        {
            isOpen = true;
            doorTransform.parent.position = doorTransform.parent.position + new Vector3(0f, 0f, .5f);
            doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, -90f, 0f);
        }
    }

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        if (getInteractor.name.Equals(itemEntity.item.binder.Replace("_farm", "")))
        {
            if (isOpen)
            {
                isOpen = false;
                doorTransform.parent.position = doorTransform.parent.position - new Vector3(0f, 0f, .5f);
                doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, 90f, 0f);
                itemEntity.item.state = "Closed";
            }
            else
            {
                if (in_modified)
                {
                    itemEntity.item.state = itemEntity.item.state.Equals("Locked") ? "Unlocked" : "Locked";
                    getInteractor.toastNotifications.newNotification("Door is now " + itemEntity.item.state);
                }
                else
                {
                    if (!itemEntity.item.state.Equals("Locked"))
                    {
                        isOpen = true;
                        doorTransform.parent.position = doorTransform.parent.position + new Vector3(0f, 0f, .5f);
                        doorTransform.parent.eulerAngles = doorTransform.parent.eulerAngles + new Vector3(0f, -90f, 0f);
                        itemEntity.item.state = "Open";
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
        itemEntity.item.state = "Closed";
    }

    public void dayBeginAction()
    {
        itemEntity.item.state = "Open";
    }
}
