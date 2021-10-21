using Socket.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IActionListener, IServerListener
{

    [SerializeField] Transform objectList;
    public Dictionary<string, inputField> listOfInputFields = new Dictionary<string, inputField>();
    private List<InputField> tabbableField = new List<InputField>();
    private Dictionary<string, SliderListener> listOfSliders = new Dictionary<string, SliderListener>();
    [SerializeField] ToastNotifications toast;
    private int tabIndex;
    private string focusButton;
    public avatarProperties current_avatar;

    public List<EntityExistanceDTO<EntityDTO>> allChars;
    public List<World> allWorld;

    void OnApplicationQuit()
    {
        Network.socket.Disconnect();
    }

    public IActionListener getActionListener()
    {
        throw new System.NotImplementedException();
    }


    private void onlineCreate()
    {

        listOfInputFields.TryGetValue("Username", out inputField out_username);
        listOfInputFields.TryGetValue("Password", out inputField out_password);
        if (string.IsNullOrEmpty(out_username.inputValue.text) || string.IsNullOrEmpty(out_username.inputValue.text))
        {
            toast.newNotification("Missing a required field");
        }
        else
        {
            var newEncrypt = SimpleAESEncryption.Encrypt(out_username.inputValue.text, "obIt6RrZTP+omkHxuXgadQ==", out_password.inputValue.text);
            Network.joinGame(out_username.inputValue.text, newEncrypt.EncryptedText, "Register", out_password.inputValue.text);
        }
    }
    private void login()
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        listOfInputFields.TryGetValue("Username", out inputField out_username);
        listOfInputFields.TryGetValue("Password", out inputField out_password);
        if (string.IsNullOrEmpty(out_username.inputValue.text) || string.IsNullOrEmpty(out_username.inputValue.text))
        {
            toast.newNotification("Missing a required field");
        }
        else
        {
            var newEncrypt = SimpleAESEncryption.Encrypt(out_username.inputValue.text, "obIt6RrZTP+omkHxuXgadQ==", out_password.inputValue.text);
            payload["Username"] = out_username.inputValue.text;
            payload["Password"] = newEncrypt.EncryptedText;
            Network.joinGame(out_username.inputValue.text, newEncrypt.EncryptedText, "Login", out_password.inputValue.text);
        }
    }
    private void registerNewUser()
    {
        clearObjectList();
        createInputField("Username", new Vector3(100f, 50f, 0f), objectList);
        createInputField("Password", new Vector3(100f, 0f, 0f), objectList);
        createButton("Register Create", "Register", new Vector3(0f, -260f, 0f), objectList);
        createButton("Offline Back", "Back", new Vector3(150f, -260f, 0f), objectList);

        focusButton = "Register Create";
        if (tabbableField != null && tabbableField.Count > 0)
        {
            tabbableField[0].ActivateInputField();
        }
    }
    private void loadOnline()
    {
        clearObjectList();
        createInputField("Username", new Vector3(100f, 50f, 0f), objectList);
        createInputField("Password", new Vector3(100f, 0f, 0f), objectList);
        createButton("Register Login", "Login", new Vector3(-150f, -260f, 0f), objectList);
        createButton("Register New", "Register", new Vector3(0f, -260f, 0f), objectList);
        createButton("Offline Back", "Back", new Vector3(150f, -260f, 0f), objectList);
        focusButton = "Register Login";
        if (tabbableField != null && tabbableField.Count > 0)
        {
            tabbableField[0].ActivateInputField();
        }

    }

    private void loadOffline()
    {
        clearObjectList();
        int charCount = 0;
        foreach (Entity it_entity in DataCache.listOfCharacters)
        {
            createButton("Offline " + charCount, it_entity.entityName, new Vector3(-185f, 40f - (50f * charCount), 0f), objectList);
            charCount++;
        }
        for (int remaining = charCount; remaining < 5; remaining++)
        {
            createButton("Offline " + charCount, "{New Character}", new Vector3(-185f, 40f - (50f * charCount), 0f), objectList);
            charCount++;
        }

        createButton("Offline Back", "Back", new Vector3(-190f, -260f, 0f), objectList);
    }

    private void createNewCharacter(string in_mode)
    {
        Entity playerEntity = new Entity();
        EntityExistanceDTO<Entity> temp_entity = new EntityExistanceDTO<Entity>();
        listOfInputFields.TryGetValue("Character name", out inputField out_name);
        playerEntity.entityName = out_name.inputValue.text;
        playerEntity.backpack.size = 8;
        playerEntity.state = "New";
        playerEntity.areaName = out_name.inputValue.text + "_farm";

        playerEntity.stamina = 100;
        playerEntity.maxStamina = 100;

        playerEntity.position = new Vector3(19f, 0f, 8f);
        playerEntity.rotation = new Vector3(0f, 0f, 0f);
//        playerEntity.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerEntity.backpack = new Backpack();
        playerEntity.holding = null;

        playerEntity.currentAnimal = current_avatar.currentAnimal;
        playerEntity.primary_currentRed = current_avatar.primary_currentRed;
        playerEntity.primary_currentGreen = current_avatar.primary_currentGreen;
        playerEntity.primary_currentBlue = current_avatar.primary_currentBlue;
        playerEntity.secondary_currentRed = current_avatar.secondary_currentRed;
        playerEntity.secondary_currentGreen = current_avatar.secondary_currentGreen;
        playerEntity.secondary_currentBlue = current_avatar.secondary_currentBlue;

        temp_entity.entityObj = playerEntity;
        temp_entity.position = new Vector3(19f, 0f, 8f);
        temp_entity.rotation = new Vector3(0f, 0f, 0f);
//        temp_entity.rotation = Quaternion.Euler(0f, 0f, 0f);

        if (in_mode.Equals("Offline"))
        {
            DataCache.saveNewCharacter(playerEntity);
            loadOffline();
        }
        else if (in_mode.Equals("Online"))
        {

            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("Action", "Create character");
            payload.Add("Username", Network.Username);
            payload.Add("Character", JsonConvert.SerializeObject(temp_entity, settings));

            Network.sendPacket(doCommands.character, "Create", payload);
            //            Network.doLogin(payload);
        }
    }

    private void createNewWorld()
    {
        World new_world = new World();
        listOfInputFields.TryGetValue("World Name", out inputField out_name);
        if (string.IsNullOrEmpty(out_name.inputValue.text))
        {
            toast.newNotification("Please input a name for the world");
        }
        else
        {
            new_world.worldName = out_name.inputValue.text;
            new_world.owner = Network.Username;
            new_world.fullDayLength = 1440;
            new_world.timeRate = 1.0f / new_world.fullDayLength;
            //        new_world.noon = CustomUtilities.vector3Rounder(new Vector3(90f, 0f, 0f));
            new_world.dayBeginHour = 8;
            new_world.dayEndHour = 18;

            Network.sendPacket(doCommands.world, "Create", new_world);
            //Network.doSave<World>("World", new_world);
        }
    }

    private void registerNewWorld()
    {
        clearObjectList();
        createInputField("World Name", new Vector3(100f, 50f, 0f), objectList);
        createButton("World Create", "Create", new Vector3(0f, -260f, 0f), objectList);
        createButton("World Back", "Back", new Vector3(150f, -260f, 0f), objectList);

        focusButton = "Register Create";
        if (tabbableField != null && tabbableField.Count > 0)
        {
            tabbableField[0].ActivateInputField();
        }
    }
    private void deleteCharacter(string in_mode, int in_index)
    {
        if (in_mode.Equals("Offline"))
        {
            Entity getEntity = DataCache.listOfCharacters[in_index];
            DataCache.deleteCharacter(getEntity);
            loadOffline();
        }
        else if (in_mode.Equals("Online"))
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload.Add("Username", Network.Username);
            payload.Add("EntityName", allChars[in_index].entityObj.entityName);
            payload.Add("Action", "Delete");
            Network.doLogin(payload);
        }
    }

    private void selectCharacterAtIndex(string in_mode, int in_index)
    {
        if (in_mode.Equals("Offline"))
        {
            if (in_index < DataCache.listOfCharacters.Count)
            {
                Entity getEntity = DataCache.listOfCharacters[in_index];
                if (getEntity != null)
                {
                    createLabel("Name: " + getEntity.entityName, new Vector3(0f, 50f, 0f), this);
                    createLabel("Area: " + getEntity.areaName, new Vector3(0f, 25f, 0f), this);
                    createButton("Offline Play " + in_index, "Load", new Vector3(0f, -260f, 0f), objectList);
                    createButton("Offline Delete " + in_index, "Delete", new Vector3(150f, -260f, 0f), objectList);
                }
            }
            else
            {
                createInputField("Character name", new Vector3(100f, 50f, 0f), objectList);
                createButton("Offline New " + in_index, "Create", new Vector3(0f, -260f, 0f), objectList);
                focusButton = "Offline New " + in_index;

                if (tabbableField != null && tabbableField.Count > 0)
                {
                    tabbableField[0].ActivateInputField();
                }
            }
        }
        else if (in_mode.Equals("Online"))
        {
            if (in_index < allChars.Count)
            {
                EntityExistanceDTO<EntityDTO> getEntity = allChars[in_index];
                if (getEntity != null)
                {
                    createLabel("Name: " + getEntity.entityObj.entityName, new Vector3(0f, 50f, 0f), this);
                    createLabel("Area: " +  (getEntity.areaObj != null ? getEntity.areaObj.areaName : "The Void"), new Vector3(0f, 25f, 0f), this);
                    createButton("Online Play " + in_index, "Load", new Vector3(0f, -260f, 0f), objectList);
                    createButton("Online Delete " + in_index, "Delete", new Vector3(150f, -260f, 0f), objectList);

                    current_avatar = new avatarProperties(getEntity.entityObj.currentAnimal);
                    current_avatar.primary_currentRed = getEntity.entityObj.primary_currentRed;
                    current_avatar.primary_currentGreen = getEntity.entityObj.primary_currentGreen;
                    current_avatar.primary_currentBlue = getEntity.entityObj.primary_currentBlue;
                    current_avatar.secondary_currentBlue = getEntity.entityObj.secondary_currentBlue;
                    current_avatar.secondary_currentRed = getEntity.entityObj.secondary_currentRed;
                    current_avatar.secondary_currentGreen = getEntity.entityObj.secondary_currentGreen;
                    current_avatar.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/" + getEntity.entityObj.currentAnimal), new Vector3(0f, 0f, 0f), Quaternion.identity);
                    current_avatar.currentAvatar.transform.SetParent(objectList);
                    current_avatar.currentAvatar.transform.localPosition = new Vector3(-637.5f, -363f, -6f);
                    current_avatar.currentAvatar.transform.rotation = Quaternion.Euler(0f, 133f, 0f);
                    current_avatar.currentAvatar.transform.localScale = new Vector3(.85f, .85f, .85f);
                    if (current_avatar.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
                    {
                        current_avatar.current_avatarEntity = out_avatarEntity;
                    }
                    current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.primary_currentRed / 255f, current_avatar.primary_currentGreen / 255f, current_avatar.primary_currentBlue / 255f), "Primary");
                    current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.secondary_currentRed / 255f, current_avatar.secondary_currentGreen / 255f, current_avatar.secondary_currentBlue / 255f), "Secondary");

                }

            }
            else
            {
                current_avatar = new avatarProperties("Bear");
                current_avatar.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/Bear"), new Vector3(0f, 0f, 0f), Quaternion.identity);
                current_avatar.currentAvatar.transform.SetParent(objectList);
                current_avatar.currentAvatar.transform.localPosition = new Vector3(-637.5f, -363f, -6f);
                current_avatar.currentAvatar.transform.rotation = Quaternion.Euler(0f, 133f, 0f);
                current_avatar.currentAvatar.transform.localScale = new Vector3(.85f, .85f, .85f);
                if (current_avatar.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
                {
                    current_avatar.current_avatarEntity = out_avatarEntity;
                }
                current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.primary_currentRed / 255f, current_avatar.primary_currentGreen / 255f, current_avatar.primary_currentBlue / 255f), "Primary");
                current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.secondary_currentRed / 255f, current_avatar.secondary_currentGreen / 255f, current_avatar.secondary_currentBlue / 255f), "Secondary");

                createInputField("Character name", new Vector3(100f, 50f, 0f), objectList);
                createButton("Online New " + in_index, "Create", new Vector3(0f, -260f, 0f), objectList);
                createLabel("Avatar", new Vector3(-35f, -15f, 0f), this);
                createButton("Character Bear", "Bear", new Vector3(-70f, -50f, 0f), objectList);
                createButton("Character Bunny", "Bunny", new Vector3(40f, -50f, 0f), objectList);
                createButton("Character Cat", "Cat", new Vector3(155f, -50f, 0f), objectList);
                createButton("Character Dog", "Dog", new Vector3(-15f, -95f, 0f), objectList);
                createButton("Character Fox", "Fox", new Vector3(95, -95f, 0f), objectList);
                createLabel("Color:", new Vector3(-35f, -130f, 0f), this);
                createLabel("Red:", new Vector3(-20f, -150f, 0f), this);
                createSlider("Red", new Vector3(35f, -150f, 0f), 0, 255, objectList);
                createLabel("Blue:", new Vector3(-20f, -175f, 0f), this);
                createSlider("Blue", new Vector3(35f, -175f, 0f), 0, 255, objectList);
                createLabel("Green:", new Vector3(-20f, -200f, 0f), this);
                createSlider("Green", new Vector3(35f, -200f, 0f), 0, 255, objectList);
                createButton("Color Primary", "Primary Color", new Vector3(180f, -155f, 0f), objectList);
                createButton("Color Secondary", "Secondary Color", new Vector3(180f, -200f, 0f), objectList);
                focusButton = "Online New " + in_index;

                if (tabbableField != null && tabbableField.Count > 0)
                {
                    tabbableField[0].ActivateInputField();
                }
            }

        }
    }

    private void selectWorldAtIndex(int in_index)
    {
        if (in_index < allWorld.Count)
        {
            World getWorld = allWorld[in_index];
            if (getWorld != null)
            {
                createLabel("Name: " + getWorld.worldName, new Vector3(0f, 50f, 0f), this);
                createLabel("Time: " + getWorld.getTime(), new Vector3(0f, 25f, 0f), this);
                createLabel("Owner: " + getWorld.owner, new Vector3(0f, 0f, 0f), this);
                createButton("World Play " + in_index, "Load", new Vector3(0f, -260f, 0f), objectList);
                if (Network.Username.Equals(getWorld.owner)) createButton("Online Delete " + in_index, "Delete", new Vector3(150f, -260f, 0f), objectList);

            }
        }
    }

    private void loadAllWorld()
    {
        clearObjectList();
        int charCount = 0;
        foreach (World it_world in allWorld)
        {
            createButton("World Enter " + charCount, it_world.worldName, new Vector3(-185f, 40f - (50f * charCount), 0f), objectList);
            charCount++;
        }
        createButton("World Character", "Back", new Vector3(-190f, -260f, 0f), objectList);
        createButton("World New", "New World", new Vector3(0, -260f, 0f), objectList);
    }


    private void loadWorld(int in_index)
    {
        World getWorld;
        getWorld = allWorld[in_index];
        if (getWorld != null)
        {
            Network.loadedWorld = getWorld;
            SceneManager.LoadScene("LoadResources");
        }

    }

    private GameObject createLabel(string in_string, Vector3 in_position, IActionListener in_listener)
    {
        GameObject tmpLabel = Instantiate(Resources.Load<GameObject>("Label"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpLabel.name = "Label";
        tmpLabel.transform.SetParent(objectList);
        tmpLabel.transform.localPosition = in_position;
        if (tmpLabel.TryGetComponent<inputField>(out inputField out_inputField))
        {
            out_inputField.inputLabel.text = in_string;
        }

        return tmpLabel;
    }

    private void loadCharacter(string in_mode, int in_index)
    {
        EntityExistanceDTO<EntityDTO> getEntity;
        if (Network.isConnected)
        {
            getEntity = allChars[in_index];
            if (getEntity != null)
            {
                Network.loadedCharacter = getEntity;
                Configurations.isHost = true;
                Configurations.textSpeed = .005f;
                Network.sendPacket(doCommands.world, "All worlds");
                //                Network.doLogin("Load all world");
                //                SceneManager.LoadScene("LoadResources");
            }
        }
        else
        {
            //getEntity = DataCache.listOfCharacters[in_index];
            //if (getEntity != null)
            //{
            //    DataCache.loadedCharacter = getEntity;
            //    Configurations.isHost = true;
            //    Configurations.textSpeed = .005f;
            //    SceneManager.LoadScene("LoadResources");
            //}
        }

    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        DataUtility.instantiateLocal();
        DataCache.loadAllCharacter();
        loadMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabIndex = (tabIndex + 1) % tabbableField.Count;
            tabbableField[tabIndex].ActivateInputField();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            listen(focusButton);
        }
        serverResponseListener();
    }

    private void loadMainMenu()
    {
        clearObjectList();
        //        createButton("Select offline", "Offline", new Vector3(0f, 0f, 0f), objectList);
        createButton("Select online", "Online", new Vector3(0f, -50f, 0f), objectList);
        //        createButton("Select lan", "Lan", new Vector3(0f, -100f, 0f), objectList);
    }

    public void generateAvatarPreview(string in_animal, bool in_color)
    {
        if (!current_avatar.currentAnimal.Equals(in_animal))
        {
            if (current_avatar.currentAvatar != null) Destroy(current_avatar.currentAvatar);
            current_avatar.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/" + in_animal), new Vector3(0f, 0f, 0f), Quaternion.identity);
            current_avatar.currentAvatar.transform.SetParent(objectList);
            current_avatar.currentAvatar.transform.localPosition = new Vector3(-637.5f, -363f, -6f);
            current_avatar.currentAvatar.transform.rotation = Quaternion.Euler(0f, 133f, 0f);
            current_avatar.currentAvatar.transform.localScale = new Vector3(.85f, .85f, .85f);
            if (current_avatar.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
            {
                current_avatar.current_avatarEntity = out_avatarEntity;
            }
            //            currentAvatar.transform.localPosition = in_position;
        }
        if (in_color)
        {
            if (current_avatar.colorType.Equals("Primary"))
            {
                current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.primary_currentRed / 255f, current_avatar.primary_currentGreen / 255f, current_avatar.primary_currentBlue / 255f), "Primary");
            }
            if (current_avatar.colorType.Equals("Secondary"))
            {
                current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.secondary_currentRed / 255f, current_avatar.secondary_currentGreen / 255f, current_avatar.secondary_currentBlue / 255f), "Secondary");
            }
        }
    }
    private void clearObjectList()
    {
        foreach (Transform it_object in objectList)
        {
            Destroy(it_object.gameObject);
        }
        listOfInputFields.Clear();
        listOfSliders.Clear();
        tabbableField.Clear();
        tabIndex = 0;
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

    public void createSlider(string in_action, Vector3 in_position, int in_min, int in_max, Transform objectList)
    {
        GameObject tmp_obj = Instantiate(Resources.Load<GameObject>("Slider"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmp_obj.transform.SetParent(objectList);
        tmp_obj.transform.localPosition = in_position;
        if (tmp_obj.TryGetComponent<SliderListener>(out SliderListener out_slider))
        {
            listOfSliders.Add(in_action, out_slider);
            out_slider.getAction = in_action;
            out_slider.listener = this;
            out_slider.configSlider(in_min, in_max);
        }
    }

    private GameObject createInputField(string in_textName, Vector3 in_position, Transform objectList)
    {
        GameObject tmpField = Instantiate(Resources.Load<GameObject>("InputField"), new Vector3(0f, 0f, 0f), Quaternion.identity);
        tmpField.name = in_textName;
        tmpField.transform.SetParent(objectList);
        tmpField.transform.localPosition = in_position;
        if (tmpField.TryGetComponent<inputField>(out inputField in_inputField))
        {
            in_inputField.inputLabel.text = in_textName;
            listOfInputFields.Add(in_textName, in_inputField);
            tabbableField.Add(in_inputField.inputObject);

        }
        return tmpField;

    }

    private void loadAllCharacter()
    {
        clearObjectList();
        int charCount = 0;
        foreach (EntityExistanceDTO<EntityDTO> it_entity in allChars)
        {
            createButton("Online " + charCount, it_entity.entityObj.entityName, new Vector3(-185f, 40f - (50f * charCount), 0f), objectList);
            charCount++;
        }
        for (int remaining = charCount; remaining < 5; remaining++)
        {
            createButton("Online " + charCount, "{New Character}", new Vector3(-185f, 40f - (50f * charCount), 0f), objectList);
            charCount++;
        }

        createButton("Online Back", "Back", new Vector3(-190f, -260f, 0f), objectList);
    }



    public void listen(string getAction)
    {
        string[] parser = getAction.Split(' ');
        switch (parser[0])
        {
            case "Color":
                current_avatar.colorType = parser[1];
                if (parser[1].Equals("Primary"))
                {
                    listOfSliders["Red"].sliderObj.value = current_avatar.primary_currentRed;
                    listOfSliders["Blue"].sliderObj.value = current_avatar.primary_currentBlue;
                    listOfSliders["Green"].sliderObj.value = current_avatar.primary_currentGreen;

                }
                else if (parser[1].Equals("Secondary"))
                {
                    listOfSliders["Red"].sliderObj.value = current_avatar.secondary_currentRed;
                    listOfSliders["Blue"].sliderObj.value = current_avatar.secondary_currentBlue;
                    listOfSliders["Green"].sliderObj.value = current_avatar.secondary_currentGreen;
                }
                break;
            case "Character":
                generateAvatarPreview(parser[1], false);
                current_avatar.currentAnimal = parser[1];
                break;
            case "Red":
                if (current_avatar.colorType.Equals("Primary")) current_avatar.primary_currentRed = float.Parse(parser[1]);
                if (current_avatar.colorType.Equals("Secondary")) current_avatar.secondary_currentRed = float.Parse(parser[1]);
                generateAvatarPreview(current_avatar.currentAnimal, true);
                break;
            case "Blue":
                if (current_avatar.colorType.Equals("Primary")) current_avatar.primary_currentBlue = float.Parse(parser[1]);
                if (current_avatar.colorType.Equals("Secondary")) current_avatar.secondary_currentBlue = float.Parse(parser[1]);
                generateAvatarPreview(current_avatar.currentAnimal, true);
                break;
            case "Green":
                if (current_avatar.colorType.Equals("Primary")) current_avatar.primary_currentGreen = float.Parse(parser[1]);
                if (current_avatar.colorType.Equals("Secondary")) current_avatar.secondary_currentGreen = float.Parse(parser[1]);
                generateAvatarPreview(current_avatar.currentAnimal, true);
                break;
            case "World":
                switch (parser[1])
                {
                    case "New":
                        registerNewWorld();
                        break;
                    case "Create":
                        createNewWorld();
                        break;
                    case "Character":
                        clearObjectList();
                        loadAllCharacter();
                        break;
                    case "Play":
                        loadWorld(int.Parse(parser[2]));
                        break;
                    case "Back":
                        clearObjectList();
                        loadAllWorld();
                        break;
                    case "Enter":
                        loadAllWorld();
                        selectWorldAtIndex(int.Parse(parser[2]));
                        break;
                }
                break;
            case "Register":
                switch (parser[1])
                {
                    case "Create":
                        onlineCreate();
                        break;
                    case "Login":
                        login();
                        break;
                    case "Back":
                        loadOnline();
                        break;
                    case "New":
                        registerNewUser();
                        break;
                }
                break;
            case "Select":
                if (parser[1].Equals("offline"))
                {
                    loadOffline();
                }
                else if (parser[1].Equals("online"))
                {
                    loadOnline();
                }
                break;
            case "Online":
                switch (parser[1])
                {
                    case "Delete":
                        deleteCharacter("Online", int.Parse(parser[2]));
                        break;
                    case "New":
                        allChars.Clear();
                        createNewCharacter("Online");
                        break;
                    case "Play":
                        loadCharacter("Online", int.Parse(parser[2]));
                        break;
                    case "Back":
                        clearObjectList();
                        loadMainMenu();
                        break;
                    default:
                        loadAllCharacter();
                        selectCharacterAtIndex(parser[0], int.Parse(parser[1]));
                        break;
                }
                break;
            case "Offline":
                switch (parser[1])
                {
                    case "Back":
                        loadMainMenu();
                        break;
                    case "New":
                        createNewCharacter("Offline");
                        break;
                    case "Delete":
                        deleteCharacter("Offline", int.Parse(parser[2]));
                        break;
                    case "Play":
                        loadCharacter("Offline", int.Parse(parser[2]));
                        break;
                    default:
                        loadOffline();
                        selectCharacterAtIndex(parser[0], int.Parse(parser[1]));
                        break;
                }
                break;
        }
    }

    public void serverResponseListener()
    {
        if (Network.listOfCharacter.Count > 0)
        {
            List<EntityExistanceDTO<EntityDTO>> temp_wrapper = Network.listOfCharacter.Dequeue();
            allChars.Clear();
            allChars = temp_wrapper;
            //foreach (CharacterAccountDTO it_entity in temp_wrapper)
            //{
            //    allChars.Add(it_entity);
            //}
            loadAllCharacter();
        }

        if (Network.listOfWorlds.Count > 0)
        {
            List<WorldDTO> temp_wrapper = Network.listOfWorlds.Dequeue();
            allWorld.Clear();
            foreach (WorldDTO it_world in temp_wrapper)
            {
                allWorld.Add(it_world.getActual());
            }
            loadAllWorld();
        }

        if (Network.serverAcknowledge.Count > 0)
        {
            Dictionary<string, string> getAction = Network.serverAcknowledge.Dequeue();
            switch (getAction["action"])
            {
                case "Welcome":

                    listOfInputFields.TryGetValue("Username", out inputField out_username);
                    Network.isConnected = true;
                    Network.Username = out_username.inputValue.text;
                    Network.sendPacket(doCommands.character, "All Characters");
                    break;
                case "Denied":
                    toast.newNotification("Invalid login credentials");
                    break;
                case "Created User":
                    loadOnline();
                    toast.newNotification("Successfully created, please login");
                    break;
                case "Failed user creation":
                    toast.newNotification("Username already exists, please choose another");
                    break;

            }
        }
    }
}

[Serializable]
public class avatarProperties
{
    public GameObject currentAvatar;
    public string currentAnimal;
    public AvatarEntity current_avatarEntity;
    public float primary_currentRed;
    public float primary_currentGreen;
    public float primary_currentBlue;
    public float secondary_currentRed;
    public float secondary_currentGreen;
    public float secondary_currentBlue;
    public string colorType;
    public bool changed;

    public avatarProperties(string in_animal)
    {
        currentAnimal = in_animal;
        colorType = "Primary";
        primary_currentBlue = UnityEngine.Random.Range(0, 255);
        primary_currentGreen = UnityEngine.Random.Range(0, 255);
        primary_currentRed = UnityEngine.Random.Range(0, 255);
        secondary_currentBlue = UnityEngine.Random.Range(0, 255);
        secondary_currentGreen = UnityEngine.Random.Range(0, 255);
        secondary_currentRed = UnityEngine.Random.Range(0, 255);


    }
}