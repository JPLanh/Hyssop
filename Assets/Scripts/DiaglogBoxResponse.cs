using UnityEngine;
using UnityEngine.UI;

/*
 * 
 * Generic script that allow the player to interact with the dialog box with the person they are conversing with.
 * 
 */
public class DiaglogBoxResponse : MonoBehaviour
{
    public IActionListener parentListener;
    public Text buttonLabel;
    public string action;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClick()
    {
        parentListener.listen(action);
    }
}
