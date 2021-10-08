using UnityEngine;
using UnityEngine.UI;

/*
 * 
 * Controller for a dialog box
 * 
 */
public class DialogBox : MonoBehaviour
{
    public Text speakerName;
    public Text messageText;
    public string message;
    private int index;
    [SerializeField] float delayPerText;
    public float textPerSecond;
    public Transform listOfResponse;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (index != message.Length)
        {
            if (textPerSecond == 0)
            {
                index = message.Length;
                messageText.text = message.Substring(0, index - 1);
            }
            if (delayPerText >= textPerSecond)
            {
                delayPerText = 0;
                index++;
                messageText.text = message.Substring(0, index - 1);
            }
            delayPerText += Time.deltaTime;

        }
    }

    public void newMessage(string in_name, string in_message)
    {
        gameObject.SetActive(true);
        speakerName.text = in_name;
        message = in_message;
        index = 0;
    }

    public void dialogClickResponse()
    {
        gameObject.SetActive(false);
    }

    public bool isClicked()
    {
        if (index != message.Length)
        {
            index = message.Length;
            messageText.text = message.Substring(0, index);
            return false;
        }
        else
        {
            if (listOfResponse.childCount <= 0)
            {
                dialogClickResponse();
                return true;
            }
        }
        return false;
    }
}
