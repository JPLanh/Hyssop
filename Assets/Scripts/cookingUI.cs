using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cookingUI : MonoBehaviour, IActionListener
{
    [SerializeField] private Transform cookCursor;
    [SerializeField] private Transform ladle;
    [SerializeField] private Transform ladleParent;
    private float cookTime;
    public cookingPot cookingPot;
    [SerializeField] private Transform objectGameList;

    [Header("Ladle Properties")]
    [SerializeField] private Transform stirMeter;
    [SerializeField] private Transform minStirMeter;
    [SerializeField] private Transform maxStirMeter;
    private bool holding = false;
    private Vector3 currentVectorRotation;

    [Header("Heat Properties")]
    [SerializeField] private Transform heatMeter;
    [SerializeField] private Transform minHeatMeter;
    [SerializeField] private Transform maxHeatMeter;

    // Start is called before the first frame update

    void Start()
    {
        init();
    }

    public void init()
    {
        refreshChoppedList();

        createButton("Close Chop", "Close", new Vector3(190f, -190f, 0f), objectGameList);
    }


    public void refreshChoppedList()
    {
        foreach (Transform it_chopItem in objectGameList)
        {
            Destroy(it_chopItem.gameObject);
        }

        cookingPot.cookingcounter = 0;
        for (int ingrediantIndex = 0; ingrediantIndex < cookingPot.storage.inventory.items.Count; ingrediantIndex++)
        {
            for(int it_item_count = 0; it_item_count < cookingPot.storage.inventory.items[ingrediantIndex].ItemObj.quantity; it_item_count++)
            {
                createLabel(cookingPot.storage.inventory.items[ingrediantIndex].ItemObj.itemName, cookingPot.cookingcounter, this, objectGameList);
                cookingPot.cookingcounter++;
            }
        }
        print(cookingPot.cookingcounter);
    }

    private void refreshUI()
    {
        stirMeter.localPosition = new Vector3(cookingPot.stirIndex * 3, 0f, 0f);
        minStirMeter.localPosition = new Vector3(cookingPot.stirUnderstirThreshold * 3, 0f, 0f);
        maxStirMeter.localPosition = new Vector3(cookingPot.stirOverstirThreshold * 3, 0f, 0f);

        heatMeter.localPosition = new Vector3(0f, cookingPot.heatIndex * 3, 0f);
        minHeatMeter.localPosition = new Vector3(0f, cookingPot.heatUndercookThreshold * 3, 0f);
        maxHeatMeter.localPosition = new Vector3(0f, cookingPot.heatOvercookThreshold * 3, 0f);

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            holding = true;
            cookCursor.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cookCursor.localPosition = new Vector3(cookCursor.localPosition.x, cookCursor.localPosition.y, 0f);
            ladleParent.LookAt(cookCursor);
            currentVectorRotation = ladleParent.localEulerAngles;
        }

        if (Input.GetButtonUp("Fire1"))
        {
            holding = false;
        }


        if (Input.GetButtonDown("Jump"))
        {
            cookingPot.fireOn = cookingPot.fireOn ? false : true;
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            cookingPot.heatDelay += 1;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            cookingPot.heatDelay -= 1;
        }
        
        if (cookingPot.heatDelay > 10)
        {
            cookingPot.heatDelay = 0;
            if (cookingPot.heatIndex < cookingPot.maxHeatValue)
                cookingPot.heatIndex += 1;
        }
        if (cookingPot.heatDelay < -10)
        {
            cookingPot.heatDelay = 0;
            if (cookingPot.heatIndex > cookingPot.minHeatValue)
                cookingPot.heatIndex -= 1;

        }

        refreshUI();

        if (holding)
        {
            cookCursor.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            cookCursor.localPosition = new Vector3(cookCursor.localPosition.x, cookCursor.localPosition.y, 0f);
            ladleParent.LookAt(cookCursor);

            //            print(currentVectorRotation.x % 90 + " , " + transform.localEulerAngles.x % 90);

            getAngle(currentVectorRotation, ladleParent.localEulerAngles, out string out_direction, out float out_difference);
            if (out_difference != 0)
            {
                if (out_difference > -25 && out_difference < 25)
                {
                    cookingPot.mixStirValue += (int)out_difference;
                    currentVectorRotation = ladleParent.localEulerAngles;
                }

                if (cookingPot.mixStirValue >= 100)
                {
                    if (cookingPot.stirIndex < cookingPot.maxStirValue)
                    {
                        cookingPot.stirIndex += 1;
                    }
                    cookingPot.mixStirValue = 0;
                }
                if (cookingPot.mixStirValue <= -100)
                {
                    if (cookingPot.stirIndex > cookingPot.minStirValue)
                    {
                        cookingPot.stirIndex -= 1;
                    }
                    cookingPot.mixStirValue = 0;
                }
            }
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
                lv_difference = ladleParent.localEulerAngles.x - currentVectorRotation.x;
                if (lv_difference > 1f)
                    lv_direction = "CW";
                else if (lv_difference < -1f)
                    lv_direction = "CCW";
            }
            else if (in_new.y > 180)
            {
                lv_difference = currentVectorRotation.x - ladleParent.localEulerAngles.x;
                if (lv_difference > 1f)
                    lv_direction = "CW";
                else if (lv_difference < -1f)
                    lv_direction = "CCW";
            }
        }
        else
        {
            currentVectorRotation = ladleParent.localEulerAngles;
        }

        if (string.IsNullOrEmpty(lv_direction))
        {
            lv_difference = 0f;
        }
        out_direction = lv_direction;
        out_difference = lv_difference;
    }

    public void createButton(string in_action, string in_button, Vector3 in_position, Transform objectList)
    {
        GameObject tmp_obj = Instantiate(Resources.Load<GameObject>("Toolbar"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmp_obj.transform.SetParent(objectList);
        tmp_obj.transform.localPosition = in_position;
        if (tmp_obj.TryGetComponent<Hotbar>(out Hotbar out_hotbar))
        {
            out_hotbar.action = in_action;
            out_hotbar.listener = this;
            out_hotbar.hotbarText.text = in_button;
        }
    }


    private GameObject createLabel(string in_text, int in_index, IActionListener in_listener, Transform in_list)
    {
        GameObject tmpLabel = Instantiate(Resources.Load<GameObject>("Label"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpLabel.name = in_text;
        tmpLabel.transform.SetParent(in_list);
        tmpLabel.transform.localPosition = new Vector3(-285f + 145f * (in_index / 15), 150f - 20f * (in_index % 15), 0f);
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = in_text;
        }

        return tmpLabel;
    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    public IActionListener getActionListener()
    {
        throw new System.NotImplementedException();
    }

    public void listen(string getAction)
    {
        string[] parsed = getAction.Split(' ');
        switch (parsed[0])
        {
            case "Close":
                close();
                break;
            //case "Select":
            //    tradeSelected(choppingBoard.choppedProduce[int.Parse(parsed[1])], choppingBoard.itemEntity.item, choppingBoard.currentPlayer.playerEntity);
            //    choppingBoard.refreshChoppedList();
            //    break;
            //case "Remove":
            //    tradeSelected(choppingBoard.currentProduce, choppingBoard.itemEntity.item, choppingBoard.currentPlayer.playerEntity);
            //    choppingBoard.refreshChoppedList();
            //    break;

        }
    }

    private void close()
    {

        foreach (Transform it_chopItem in objectGameList)
        {
            Destroy(it_chopItem.gameObject);
        }

        cookingPot.activePC.unfreeze();
        cookingPot.activePC = null;
        Cursor.lockState = CursorLockMode.Locked;
        Destroy(gameObject);
    }
}
