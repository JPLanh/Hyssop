using UnityEngine;

/**
 * 
 * Controller for the UI Gauge
 * 
 */
public class Gauges : MonoBehaviour
{

    [SerializeField] private Transform gaugeLevel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateGauge(float in_stamina)
    {
        gaugeLevel.localPosition = new Vector3(-200 * in_stamina, 0f, 0f);
    }
}
