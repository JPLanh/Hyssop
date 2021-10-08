using UnityEngine;

public class ToastNotifications : MonoBehaviour
{
    //    private float nextTimer = 0f;
    public bool updateNotifList = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (updateNotifList)
            refreshNotifs();
    }

    public void newNotification(string getMessage)
    {
        GameObject newNotif = Instantiate(Resources.Load<GameObject>("Notifications"), transform.position, Quaternion.identity);
        newNotif.transform.SetParent(this.transform);
        newNotif.GetComponent<Notifications>().toastNotifications = this.gameObject;
        newNotif.transform.localPosition = new Vector3(0f, 27.5f * (this.transform.childCount - 1), 0f);
        newNotif.GetComponent<Notifications>().updateMessage(getMessage);
        refreshNotifs();
    }

    public void refreshNotifs()
    {
        bool maxSize = false;
        if (this.transform.childCount > 10)
            maxSize = true;

        int count = 0;
        foreach (Transform notifs in this.transform)
        {
            if (maxSize)
            {
                maxSize = false;
                Destroy(notifs.gameObject);
                print("Destroy obj");
            }
            notifs.GetComponent<Notifications>().refreshNotification(count);
            count += 1;
        }
    }
}
