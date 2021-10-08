using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] private GameObject step1;
    [SerializeField] private GameObject step2;
    [SerializeField] private GameObject step3;
    [SerializeField] private GameObject step4;
    public bool connectionCheck(AreaIndex in_from, AreaIndex in_to, string in_dir)
    {
        if (in_to.objectName.Equals(in_from.objectName))
        {
            step1.SetActive(false);
            step2.SetActive(false);
            step3.SetActive(false);
            step4.SetActive(false);
        }
        return true;
    }

    public bool connectionCut(AreaIndex in_from, AreaIndex in_to, string in_dir)
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
