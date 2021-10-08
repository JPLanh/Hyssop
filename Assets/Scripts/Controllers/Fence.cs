using UnityEngine;

/**
 * 
 * Controller for fences
 * 
 */
public class Fence : MonoBehaviour, IConnectable, IStructure, IPickable
{
    private string type = "Fence";
    private GameObject connectedObject;
    [SerializeField] private GameObject southeastConnector;
    [SerializeField] private GameObject southConnector;
    [SerializeField] private GameObject southwestConnector;
    [SerializeField] private GameObject westConnector;
    [SerializeField] private GameObject northwestConnector;
    [SerializeField] private GameObject northConnector;
    [SerializeField] private GameObject northeastConnector;
    [SerializeField] private GameObject eastConnector;



    public bool connectionCheck(AreaIndex in_from, AreaIndex in_to, string in_dir)
    {
        if (in_to.index.TryGetComponent<IConnectable>(out IConnectable out_iconnectable))
        {
            if (in_to.index.TryGetComponent<IStructure>(out IStructure out_istructure))
            {
                connectAdjacentType(type);
                connectAdjacentDirection(in_dir, true);
                out_istructure.connectAdjacentType(out_istructure.getStructureType());

                out_istructure.connectAdjacentDirection(getOppositeDirection(in_dir), true);
                return true;
            }
        }
        return false;
    }


    public bool connectionCut(AreaIndex in_from, AreaIndex in_to, string in_dir)
    {
        if (in_to.index.TryGetComponent<IConnectable>(out IConnectable out_iconnectable))
        {
            if (in_to.index.TryGetComponent<IStructure>(out IStructure out_istructure))
            {
                connectAdjacentType(type);
                connectAdjacentDirection(in_dir, false);
                out_istructure.connectAdjacentType(out_istructure.getStructureType());

                out_istructure.connectAdjacentDirection(getOppositeDirection(in_dir), false);
                return true;
            }
        }
        return false;
    }


    private string getOppositeDirection(string in_dir)
    {
        switch (in_dir)
        {

            case "South":
                return "North";
            case "Southeast":
                return "Northwest";
            case "Southwest":
                return "Northeast";
            case "North":
                return "South";
            case "Northeast":
                return "Southwest";
            case "Northwest":
                return "Southeast";
            case "West":
                return "East";
            case "East":
                return "West";
        }
        return null;
    }

    public void connectAdjacentType(string in_type)
    {
        switch (in_type)
        {
            case "Fence":
                break;
        }
    }
    public void connectAdjacentDirection(string in_dir, bool in_connection)
    {
        switch (in_dir)
        {
            case "South":
                southConnector.SetActive(in_connection);
                break;
            case "Southeast":
                southeastConnector.SetActive(in_connection);
                break;
            case "Southwest":
                southwestConnector.SetActive(in_connection);
                break;
            case "North":
                northConnector.SetActive(in_connection);
                break;
            case "Northeast":
                northeastConnector.SetActive(in_connection);
                break;
            case "Northwest":
                northwestConnector.SetActive(in_connection);
                break;
            case "West":
                westConnector.SetActive(in_connection);
                break;
            case "East":
                eastConnector.SetActive(in_connection);
                break;
        }
    }
    public string getStructureType()
    {
        return type;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void pickupCheck(AreaIndex in_index, GridSystem in_grid, out string out_item, out string out_state)
    {
        out_item = null;
        out_state = null;
        if (in_index.pickable)
        {
            in_grid.buildCheck(in_index, false);
            Destroy(gameObject);
            in_grid.unloadIndex(in_index);
            AreaIndex topIndex = in_grid.getIndex(in_index.x, in_index.y, in_index.z + 1);
            in_grid.buildCheck(topIndex, false);
            Destroy(topIndex.index);
            in_grid.unloadIndex(topIndex);
            out_item = gameObject.name;
        }
    }

}
