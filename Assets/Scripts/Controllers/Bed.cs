using UnityEngine;
using System.Collections.Generic;
/**
 * 
 * Controller for beds
 * 
 */
public class Bed : MonoBehaviour, IInteractable
{

    public TimeSystem ts;

    public void interact(PlayerController getInteractor, bool in_modified)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["data"] = "New day";
        Network.sendPacket(doCommands.action, "New day", payload);
        //if (ts.newDay())
        //{
        //    getInteractor.recover();
        //}
    }

    public void reaction(PlayerController getInteractor)
    {
        throw new System.NotImplementedException();
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
