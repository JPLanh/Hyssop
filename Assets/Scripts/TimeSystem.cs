using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSystem : MonoBehaviour
{
    public World currentWorld;

    //[Range(0.0f, 1.0f)] public float dayEnd;
    //[Range(0.0f, 1.0f)] public float dayBegin;
    public Vector3 noon;

    public Text timeText;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;


    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Settings")]
    public AnimationCurve lightingIntensityMultipler;
    public AnimationCurve reflectionIntensityMultipler;

    [Header("Groups")]
    [SerializeField] private GameObject containerGroup;
    [SerializeField] private GameObject plotGroup;
    [SerializeField] private GameObject farmerGroups;

    public GridSystem currentArea;
    public bool isClosed = false;

    private void OnDestroy()
    {
        localConfig tmp_calendar = new localConfig();
        tmp_calendar.time = currentWorld.time;
        tmp_calendar.timeStop = currentWorld.timeStopped;
        tmp_calendar.isLoaded = true;
        DataUtility.saveCalendar(tmp_calendar);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!Network.isConnected)
        {
            currentWorld.timeRate = 1.0f / currentWorld.fullDayLength;
            localConfig tmp_calendar = DataUtility.loadCalendar();
            if (tmp_calendar.isLoaded)
            {
                currentWorld.time = tmp_calendar.time;
                currentWorld.timeStopped = tmp_calendar.timeStop;
            }
        }

        InvokeRepeating("timerTick", 0f, 1f);

    }

    public void setDayEnd()
    {
        currentWorld.time = ((float)((currentWorld.dayEndHour) * 60) / 1440);
        Network.sendPacket<WorldDTO>(doCommands.world, "Save", currentWorld.getDTO());
//        Network.doSave<World>("World", currentWorld);
    }
    public void setDayBegin()
    {
        currentWorld.time = ((float)((currentWorld.dayBeginHour) * 60) / 1440);
        Network.sendPacket<WorldDTO>(doCommands.world, "Save", currentWorld.getDTO());
        //        Network.doSave<World>("World", currentWorld);
    }
    void timerTick()
    {
        currentWorld.time += (1f / 1440);
    }

    public int getMinute()
    {
        return (int)(currentWorld.time * 1440);
    }
    private void endOfDay()
    {

        if (currentWorld.time >= 1.0f)
        {
            currentWorld.time = 0f;
        }

        if (currentWorld.time >= (((float)currentWorld.dayEndHour * 60f) / 1440f) && !currentWorld.isDayEnd)
        {

            currentWorld.isDayEnd = true;
            PlantFactory.dayFinished(currentArea);


            foreach (ITimeListener getListener in DataCache.timeActionCache)
            {
                getListener.dayEndAction();
            }
        }


        if (!Network.isConnected)
        {
            if
                ((((int)(getMinute() / 60)) >= currentWorld.dayEndHour) && (((int)(getMinute() % 60)) >= currentWorld.dayEndMinute) && !currentWorld.isDayEnd)
            {
                if (DataCache.inPlayAreaItem.TryGetValue("Central Hub", out List<Item> out_area))
                {
                    List<Item> out_item = out_area.FindAll(x => x.itemName.Equals("Wooden Door"));
                    foreach (Item out_door in out_item)
                    {
                        out_door.state = "Closed";
                    }
                }
                dayEndEvent();
                currentWorld.isDayEnd = true;
            }

            if (currentWorld.time >= 1.0f)
            {
                currentWorld.time = 0f;
            }
        }
    }

    private void dayEndEvent()
    {
        PlantFactory.dayFinished(currentArea);
        DataCache.dayFinished();
        DataCache.saveAllCrops();
    }
    private void beginOfDay()
    {
        if
        (
            currentWorld.time >= ((float)currentWorld.dayBeginHour * 60f) / 1440f &&
            currentWorld.time < ((float)currentWorld.dayEndHour * 60f) / 1440f &&
            currentWorld.isDayEnd
        )
        {

            currentWorld.isDayEnd = false;
            print("Begin Day");
            foreach (ITimeListener getListener in DataCache.timeActionCache)
            {
                getListener.dayBeginAction();
            }
        }
    }

    public void setTime(float getServerTime)
    {
        currentWorld.time = (getServerTime % 600) / 600;
    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(currentWorld.worldName))
        {
            endOfDay();
            beginOfDay();
        }

        sun.transform.eulerAngles = (currentWorld.time - .25f) * noon * 4.0f;
        sun.intensity = sunIntensity.Evaluate(currentWorld.time);
        sun.color = sunColor.Evaluate(currentWorld.time);
        if (sun.intensity <= 0 && sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
            sun.gameObject.SetActive(true);

        moon.transform.eulerAngles = (currentWorld.time - .75f) * noon * 8.0f;
        moon.intensity = moonIntensity.Evaluate(currentWorld.time);
        moon.color = moonColor.Evaluate(currentWorld.time);
        if (moon.intensity <= 0 && moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
            moon.gameObject.SetActive(true);

        RenderSettings.ambientIntensity = lightingIntensityMultipler.Evaluate(currentWorld.time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultipler.Evaluate(currentWorld.time);
    }
}
