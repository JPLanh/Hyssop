using UnityEngine;

/**
 * 
 * Triggers an event based on the action provided to this index
 * 
 */
public class TriggerCube : MonoBehaviour, IActions, IPickable
{

    public void execute(AreaIndex in_grid)
    {
        throw new System.NotImplementedException();
    }

    public void modifyAction(AreaIndex in_grid, mainMenuListener in_mainMenu)
    {
        in_mainMenu.modifyAction();
        in_mainMenu.selectedGridIndex = in_grid;
        in_mainMenu.gameObject.SetActive(true);

    }

    public void pickupCheck(AreaIndex in_index, GridSystem in_grid, out string out_item, out string out_state)
    {
        out_item = null;
        out_state = null;
        if (in_index.pickable)
        {
            //            in_grid.buildCheck(in_index, false);
            Destroy(gameObject);
            in_grid.unloadIndex(in_index);
            out_item = gameObject.name;
        }
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
