using UnityEngine;
using UnityEngine.UI;

public class SliderListener : MonoBehaviour
{
    public Slider sliderObj;
    public IActionListener listener;
    public string getAction;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void sliderListener()
    {
        listener.listen(getAction + " " + sliderObj.value);
    }

    public void configSlider(int in_min, int in_max)
    {
        sliderObj.minValue = in_min;
        sliderObj.maxValue = in_max;
    }
}
