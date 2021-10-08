using UnityEngine;

public class playerView : MonoBehaviour
{
    public LayerMask whatIsSelectable, whatIsInteractable;
    public GameObject groundSelection = null;
    public string cameraMode;


    public Vector3 focusPoint;

    public bool groundSelectable;

    public IInteractable selectInteractable;
    [SerializeField] PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        groundSelectable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitInteractable, 8f, whatIsInteractable))
        {
            if (hitInteractable.transform.TryGetComponent<IInteractable>(out IInteractable getInteractable))
            {
                selectInteractable = getInteractable;
            }
        }
        else
        {
            if (selectInteractable != null) selectInteractable = null;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitLower, 8f, whatIsSelectable))
            {
                focusPoint = hitLower.point;
                Debug.DrawLine(transform.position, hitLower.point);
                //selectgrid = getgrid.getindex(hitlower.point);
                //if (groundselectable)
                //{
                //    if (groundselection == null)
                //    {
                //        groundselection = instantiate(resources.load<gameobject>("select ground"), transform.position, quaternion.identity);
                //    }
                //    else
                //    {
                //        if (selectgrid != null)
                //        {
                //            groundselection.transform.position = hitlower.point;
                //            //                            groundselection.transform.position = new vector3(selectgrid.x, selectgrid.z, selectgrid.y);
                //        }
                //        else
                //        {
                //            disableselection();
                //        }
                //    }
                //}
                //else
                //{
                //    disableselection();
                //}
            }
            else
            {
                focusPoint = Vector3.zero;
            }
        }
    }

    public AreaIndex getGridIndex()
    {
        return playerController.currentGrid.getIndex(focusPoint);
    }
    public void disableSelection()
    {
        //        selectGrid = null;
        Destroy(groundSelection);
        groundSelection = null;

    }
}
