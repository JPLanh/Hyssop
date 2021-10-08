using UnityEngine;

public class Road : MonoBehaviour, IPickable
{
    public void pickupCheck(AreaIndex in_index, GridSystem in_grid, out string out_item, out string out_state)
    {
        out_item = null;
        out_state = null;
        if (in_index.pickable)
        {
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
