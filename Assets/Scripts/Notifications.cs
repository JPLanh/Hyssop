using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    public GameObject toastNotifications;
    private Vector3 notifPosition;
    public float aliveTimer;
    [SerializeField]
    private Text notifText;

    // Start is called before the first frame update
    void Start()
    {
        aliveTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, notifPosition, Time.deltaTime * 50f);
        if (Time.time > aliveTimer + 5f)
        {
            Destroy(this.gameObject);
            toastNotifications.GetComponent<ToastNotifications>().updateNotifList = true;
        }
    }

    public void updateMessage(string getMsg)
    {
        notifText.text = getMsg;
    }

    public void refreshNotification(int getIndex)
    {
        notifPosition = new Vector3(0f, 27.5f * getIndex, 0f);
    }
}
