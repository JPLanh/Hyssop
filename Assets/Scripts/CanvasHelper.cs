using UnityEngine;
using UnityEngine.UI;
/**
 * 
 * Place holder for all of the UI hub Game object, not the player UI
 * 
 */
public class CanvasHelper : MonoBehaviour
{
    public PlayerMenu listener;
    public TradeMenu tradeMenu;
    public progressBar progressBar;
    public Hub hub;
    public DialogBox dialogBox;
    public DiaglogBoxResponse responseBox;
    public Transform listOfResponses;
    public GameObject loadingScreen;
    public Text loadingText;
    public Text resourceText;
    public Text percentText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void newResponse(IActionListener in_listener, string in_response)
    {
        GameObject tempResponse = Instantiate(Resources.Load<GameObject>("Dialog Box Response"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tempResponse.transform.SetParent(listOfResponses);
        tempResponse.transform.localPosition = new Vector3(0f, 0f + listOfResponses.childCount * -55f, 0f);
        if (tempResponse.TryGetComponent<DiaglogBoxResponse>(out DiaglogBoxResponse out_response))
        {
            out_response.parentListener = in_listener;
            out_response.buttonLabel.text = in_response;
            out_response.action = in_response;
        }

    }
}
