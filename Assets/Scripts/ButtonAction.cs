using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * Generic script to deal with buttons to put more dependencies on a central script instead of having to deal with several scripts.
 * 
 */
public class ButtonAction : MonoBehaviour
{
    public IActionListener listener;
    public string action;
    public Text text;

    public void OnClick()
    {
        listener.listen(action);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
