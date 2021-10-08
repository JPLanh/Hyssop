using UnityEngine;

public class Soil : MonoBehaviour, IPickable
{
    public Plant plant;
    [SerializeField] private GameObject mound;
    [SerializeField] private GameObject watered;
    public GameObject plantObj { get; set; }
    public PlantEntity plantEntity;
    [SerializeField] private GameObject witheredObj;
    public AreaIndex currentGridIndex;

    //private float initTimer;
    //private float growthTimer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!string.IsNullOrEmpty(plant.plantName))
        {
            if (!mound.activeInHierarchy)
                mound.SetActive(true);
            if (plant.isWatered && !watered.activeInHierarchy)
                watered.SetActive(true);
            if (!plant.isWatered && watered.activeInHierarchy)
                watered.SetActive(false);
        }
        else
        {
            if (mound.activeInHierarchy)
                mound.SetActive(false);
        }

        if (plantObj != null && !string.IsNullOrEmpty(plant.plantName)) growthCheck();
    }

    public void growthCheck()
    {
        if (plant.deathDayPassed < plant.deathDayRequired)
        {
            float growthInterval = ((float)(plantEntity.development_cycle.Count - 2)) / ((float)plant.dayRequired);
            int currentGrowth = (int)(1 + growthInterval * plant.dayPassed);
            if (currentGrowth != plantEntity.development_index)
            {
                plantEntity.development_cycle[plantEntity.development_index].SetActive(false);
                plantEntity.development_index = currentGrowth;
                plantEntity.development_cycle[plantEntity.development_index].SetActive(true);
            }
        }
        else
        {
            if (!plantEntity.development_cycle[0].activeInHierarchy)
            {
                plantEntity.development_cycle[plantEntity.development_index].SetActive(false);
                plantEntity.development_index = 0;
                plantEntity.development_cycle[plantEntity.development_index].SetActive(true);
            }
        }
    }
    public void setState(string in_state)
    {
        //if (in_state.Equals("Growing"))
        //{
        //    seed = in_state.Replace(" Seed", "");
        //    plantSeed(in_state);
        //}
        //if (in_state.Contains("Grown"))
        //{
        //    seed = in_state.Replace(" Grown", "");
        //    plantGrow();
        //}
    }


    public void loadPlant(Plant in_plant)
    {
        plant = in_plant;
        plantObj = Instantiate(Resources.Load<GameObject>("Plants/" + in_plant.plantName), new Vector3(in_plant.x, in_plant.y, in_plant.z), Quaternion.identity);
        if (plantObj.TryGetComponent<PlantEntity>(out PlantEntity out_PE))
        {
            plantEntity = out_PE;
        }
        plantObj.transform.SetParent(transform);
        plantObj.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void loadPlant(PlantDTO in_plant)
    {
        plant = new Plant(in_plant);
        switch (plant.state)
        {
            case "Growing":
                mound.SetActive(true);
                break;
            case "Grown":
                plantObj.SetActive(true);
                break;
            case "Dead":
                witheredObj.SetActive(true);
                break;
        }
        if (in_plant.isWatered)
            watered.SetActive(true);
        else
            watered.SetActive(false);
    }

    public bool plantSeed(GridSystem in_area, AreaIndex in_grid, Plant in_plant)
    {
        if (plant.state.Equals(""))
        {
            in_plant.x = in_grid.x;
            in_plant.y = in_grid.y;
            in_plant.z = in_grid.z;
            in_plant.areaName = in_area.areaName;
            in_plant.state = "Growing";
            plant = in_plant;
            PlantFactory.newCrop(in_plant);
            mound.SetActive(true);
            return true;
        }
        else
            return false;
    }

    public bool water()
    {
        if (!plant.isWatered)
        {
            plant.isWatered = true;
            watered.SetActive(true);
            DataCache.saveAllCrops();
        }
        return true;
    }
    private void plantGrow()
    {
        mound.SetActive(false);
        plantObj.SetActive(true);
        plant.state = "Grown";
    }
    private void plantDeath()
    {
        mound.SetActive(false);
        plantObj.SetActive(false);
        witheredObj.SetActive(true);
        plant.state = "Dead";
    }

    public void pickupCheck(AreaIndex in_index, GridSystem in_grid, out string out_item, out string out_state)
    {
        out_item = null;
        out_state = null;

        if (!string.IsNullOrEmpty(plant.plantName))
        {
            out_item = plant.plantName;
            if (plant.deathDayPassed >= plant.deathDayRequired)
            {
                out_state = "Dead";
            }
            else
            {
                if (plant.dayPassed >= plant.dayRequired)
                {
                    out_state = "Grown";
                }
            }
        }
    }

    public string pickup(string in_plant)
    {
        throw new System.NotImplementedException();
    }
}
