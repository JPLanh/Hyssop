using UnityEngine;

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
        Network.sendPacket(doCommands.action, "New day");
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
