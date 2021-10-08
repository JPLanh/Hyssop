using UnityEngine;

public class WorldResource : MonoBehaviour
{
    public string type;
    public float durability { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool damage(float in_dmg)
    {
        durability -= in_dmg;
        if (durability <= 0) return true;
        else return false;
    }
}
