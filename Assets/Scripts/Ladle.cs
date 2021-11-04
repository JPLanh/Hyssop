using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladle : MonoBehaviour
{
    [SerializeField] private Transform cookCursor;
    
    [SerializeField] private Transform stirMeter;
    [SerializeField] private Transform minMeter;
    [SerializeField] private Transform maxMeter;
    private int index = 0;
    private int mixValue = 0;
    private int minValue = -50;
    private int maxValue = 50;

    private int heatIndex = 0;
    private int heatMax = 100;
    private int heatMin = 0;

    private bool holding = false;
    private Vector3 currentVectorRotation;
    public string stirDir = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        stirMeter.localPosition = new Vector3(index*3, 0f, 0f);
        if (Input.GetButtonDown("Fire1"))
        {
            holding = true;
            cookCursor.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cookCursor.localPosition = new Vector3(cookCursor.localPosition.x, cookCursor.localPosition.y, 0f);
            transform.parent.transform.LookAt(cookCursor);
            currentVectorRotation = transform.parent.transform.localEulerAngles;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            holding = false;
        }

        if (Input.GetAxis("Vertical") > 0)
        {

        }

        if (holding)
        {
            cookCursor.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cookCursor.localPosition = new Vector3(cookCursor.localPosition.x, cookCursor.localPosition.y, 0f);
            transform.parent.transform.LookAt(cookCursor);

//            print(currentVectorRotation.x % 90 + " , " + transform.parent.transform.localEulerAngles.x % 90);

            getAngle(currentVectorRotation, transform.parent.transform.localEulerAngles, out string out_direction, out float out_difference);
            if (out_difference != 0)
            {
                if (out_difference > -10 && out_difference < 10)
                {
                    mixValue += (int)out_difference;
                    currentVectorRotation = transform.parent.transform.localEulerAngles;
                    print(mixValue);
                }
                
                if (minValue >= 100)
                {
                    if (index < maxValue)
                    {
                        index += 1;
                    }
                    mixValue = 0;
                }
                if (mixValue <= -100)
                {
                    if (index > minValue)
                    {
                        index -= 1;
                    }
                    mixValue = 0;
                }
            }

            //float difAngle = Quaternion.Angle(currentRotation, transform.parent.rotation);
            //if (difAngle > 1 && difAngle < 5)
            //{
            //    print(currentVectorRotation + " , " + transform.parent.transform.localEulerAngles);
            //    currentRotation = transform.parent.rotation;
            //    currentVectorRotation = transform.parent.transform.localEulerAngles;
            //} else if (difAngle >= 5)
            //{
            //    if (difAngle > 100) {
            //        print("Too fast " + difAngle);
            //        holding = false;
            //    }
            //}
        }
    }

    private void getAngle(Vector3 in_current, Vector3 in_new, out string out_direction, out float out_difference)
    {
        string lv_direction = null;
        float lv_difference = 0f;

        if ((in_new.y < 180 && in_current.y < 180) || (in_new.y > 180 && in_current.y > 180))
        {
            if (in_new.y < 180)
            { 
                lv_difference = transform.parent.transform.localEulerAngles.x - currentVectorRotation.x;
                if (lv_difference > 1f)
                    lv_direction = "CW";
                else if (lv_difference < -1f)
                    lv_direction = "CCW";
            } else if (in_new.y > 180)
            {
                lv_difference = currentVectorRotation.x - transform.parent.transform.localEulerAngles.x;
                if (lv_difference > 1f)
                    lv_direction = "CW";
                else if (lv_difference < -1f)
                    lv_direction = "CCW";
            }
        }
        else
        {
            currentVectorRotation = transform.parent.transform.localEulerAngles;
        }

        if (string.IsNullOrEmpty(lv_direction))
        {
            lv_difference = 0f;
        }
        out_direction = lv_direction;
        out_difference = lv_difference;
    }

}
