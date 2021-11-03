using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IActionListener, IDayNightCycle
{
    public CanvasHelper canvas;
    public Entity playerEntity;
    public EntityExistanceDTO<Entity> entityUpdater;
    public PlayerMenu mainMenu;
    public Transform itemDrag;
    public GridSystem currentGrid;
    public Animator entityAnimation;
    public ToastNotifications toastNotifications;
    public Item holding;
    public Transform cameraCenter;

    public TimeSystem ts;

    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float gravity = 20f;
    public Vector3 moveDirection;
//    public Vector3 rotation = Vector3.zero;
    [SerializeField]
    private float lookSensativity = 10f;
    private float lookXLimit = 50f;
    [SerializeField]
    private Transform userHead;

    private float recoveryTimer;
    public playerView playerVision;
    public CharacterController characterController;
    public avatarProperties current_avatar;

    [Header("Equipments")]
    public Text holdingText;

    private Vector3 teleportPosition = Vector3.zero;
    private bool canRotate;
    private bool canMove;
    public bool isPaused;

    public GameObject indexSelector;

    private bool debug = true;
    private float updateBroadcastTimer = 0;
    private float updateBroadcastInterval = .05f;

    private float rotateAmount = 2.5f;

    void Start()
    {
        characterController.enabled = false;
        canvas.listener.currentPlayer = this;
        mainMenu = canvas.listener;
        holdingText = canvas.hub.itemText;
        ts.currentArea = currentGrid;

        if (canvas.TryGetComponent<CanvasHelper>(out CanvasHelper out_canvas))
        {
            out_canvas.listener.currentPlayer = this;
        }

        if (Network.isConnected)
        {
            playerEntity = Network.loadedCharacter.entityObj.getActual();
        }
        else
        {
            playerEntity = DataCache.loadedCharacter;
        }

        load(playerEntity);
        currentGrid.loadArea();

        characterController.enabled = true;
        canvas.hub.playerName.text = playerEntity.entityName;
        canMove = false;
        canRotate = false;
    }

    public void recover()
    {
        playerEntity.stamina = playerEntity.maxStamina;
    }

    void OnApplicationQuit()
    {
        if (Network.isConnected)
        {
            saveAndDC();
            Network.socket.Disconnect();

        }
        else
        {
            save();
            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
            DataCache.saveAreaItems(currentGrid.area.areaName);
        }
    }

    public void load(Entity in_entity)
    {
        name = in_entity.entityName;
        characterController.enabled = false;
        transform.position = Network.loadedCharacter.position;
        characterController.enabled = true;
        //        transform.rotation = Network.loadedCharacter.rotation;
        playerVision.fpsRotation = Network.loadedCharacter.rotation;
        playerEntity = in_entity;
        currentGrid.area.areaName = Network.loadedCharacter.areaObj.areaName;


        current_avatar = new avatarProperties(in_entity.currentAnimal);
        current_avatar.primary_currentRed = in_entity.primary_currentRed;
        current_avatar.primary_currentGreen = in_entity.primary_currentGreen;
        current_avatar.primary_currentBlue = in_entity.primary_currentBlue;
        current_avatar.secondary_currentBlue = in_entity.secondary_currentBlue;
        current_avatar.secondary_currentRed = in_entity.secondary_currentRed;
        current_avatar.secondary_currentGreen = in_entity.secondary_currentGreen;
        current_avatar.currentAvatar = Instantiate(Resources.Load<GameObject>("Avatar/" + in_entity.currentAnimal), new Vector3(0f, 0f, 0f), Quaternion.identity);
        current_avatar.currentAvatar.transform.SetParent(transform);
        current_avatar.currentAvatar.transform.localPosition = new Vector3(-0f, -0f, -0f);
        current_avatar.currentAvatar.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        entityUpdater._id = Network.loadedCharacter._id;
        entityUpdater.entityObj = playerEntity;
        entityUpdater.areaObj = currentGrid.area;
        

        if (current_avatar.currentAvatar.TryGetComponent<AvatarEntity>(out AvatarEntity out_avatarEntity))
        {
            current_avatar.current_avatarEntity = out_avatarEntity;
            entityAnimation = out_avatarEntity.animator;
        }
        initFirstPersonView();
        current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.primary_currentRed / 255f, current_avatar.primary_currentGreen / 255f, current_avatar.primary_currentBlue / 255f), "Primary");
        current_avatar.current_avatarEntity.setAllColor(new Color(current_avatar.secondary_currentRed / 255f, current_avatar.secondary_currentGreen / 255f, current_avatar.secondary_currentBlue / 255f), "Secondary");


        if (playerEntity.state.Equals("New"))
        {
            playerEntity.backpack.size = 8;
            playerEntity.backpack.createItem(name, "Basic Shovel", 1);
            playerEntity.backpack.createItem(name, "Basic Axe", 1);
            playerEntity.backpack.createItem(name, "Basic Pickaxe", 1);
            playerEntity.backpack.createItem(name, "Small Watering Can", 1);
            playerEntity.backpack.createItem(name, "Coffee bean", 10);
            playerEntity.backpack.createItem(name, "Onion", 10);
            playerEntity.backpack.createItem(name, "Silver", 100);
            holdingText.text = "Nothing";
        }
        else
        {
            if (playerEntity.holding == null || holding.itemName == null)
            {
                holdingText.text = "Nothing";
            }
            else
            {
                holdingText.text = playerEntity.getHolding().ItemObj.itemName.Equals("") ? "Nothing" : playerEntity.getHolding().ItemObj.itemName;

            }
            menuToggle(false);
        }
    }

    private void initFirstPersonView()
    {

        cameraCenter.SetParent(current_avatar.current_avatarEntity.avatar_head_camera);
        cameraCenter.transform.localPosition = new Vector3(0f, 0f, 0f);
        userHead.SetParent(cameraCenter);
        userHead.localPosition = new Vector3(0f, 0f, .45f);
        userHead.localRotation = Quaternion.Euler(0f, 0f, 0f);

        playerVision.transform.localPosition = new Vector3(0f, 0f, 0f);
        playerVision.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        playerVision.cameraMode = "First Person";
    }

    private void initThirdPersonView()
    {
        playerVision.tpsRotation = new Vector3(0f, playerVision.fpsRotation.y, 0f);
        cameraCenter.SetParent(current_avatar.current_avatarEntity.avatar_head_camera);
        cameraCenter.transform.localPosition = new Vector3(0f, 0f, 0f);
        userHead.SetParent(cameraCenter);
        userHead.localRotation = Quaternion.Euler(playerVision.fpsRotation.y, playerVision.fpsRotation.x, 0);
        userHead.localRotation = Quaternion.Euler(0f, 0f, 0f);

        playerVision.transform.localPosition = new Vector3(0f, 0f, -2f);
        playerVision.cameraMode = "Third Person";
    }
    public void saveAndDC()
    {

        //entityUpdater.position = transform.position;
        //entityUpdater.rotation = playerVision.fpsRotation;
        //updateBroadcastTimer = updateBroadcastInterval;
        //entityUpdater.areaObj = currentGrid.area;
        //Network.doUpdate(entityUpdater);

        entityUpdater.position = CustomUtilities.vector3Rounder(transform.position);
        entityUpdater.rotation = CustomUtilities.vector3Rounder(playerVision.fpsRotation);
        entityUpdater.areaObj = currentGrid.area;
        //            playerEntity.rotation = Quaternion.identity;
        if (!Network.isConnected)
        {
            DataUtility.saveEntityPlayer(playerEntity);
        }
        else
        {
            Network.doSaveImmediate("Entity", entityUpdater);
        }
    }
    public void save()
    {
        playerEntity.position = CustomUtilities.vector3Rounder(transform.position);
        playerEntity.rotation = CustomUtilities.vector3Rounder(playerVision.fpsRotation);
        //            playerEntity.rotation = Quaternion.identity;
        playerEntity.areaName = currentGrid.area.areaName;
        if (!Network.isConnected)
        {
            DataUtility.saveEntityPlayer(playerEntity);
        }
        else
        {
            Network.doSaveImmediate("Entity", entityUpdater);
        }
    }

    void OnGUI()
    {
        if (debug)
        {
            if (!canvas.loadingScreen.activeInHierarchy)
            {
                GUI.Label(new Rect(10, 100, 300, 20), "Currently At: " + currentGrid.getIndex(transform.position));
                if (playerVision.focusPoint != Vector3.zero) GUI.Label(new Rect(10, 120, 300, 20), "Looking At: " + playerVision.focusPoint.x + " , " + playerVision.focusPoint.z + " , " + playerVision.focusPoint.y);
                if (playerVision.focusPoint != Vector3.zero && playerVision.getGridIndex() != null) GUI.Label(new Rect(10, 140, 300, 20), "Index Select: " + playerVision.getGridIndex().x + " , " + playerVision.getGridIndex().y + " , " + playerVision.getGridIndex().z);
                GUI.Label(new Rect(10, 160, 300, 20), currentGrid.area.areaName + " _ Time: " + ts.currentWorld.time);
                if (playerEntity.getHolding() != null)
                {
                    if (playerEntity.getHolding().ItemObj.itemName != null)
                    {
                        if (playerEntity.getHolding().ItemObj.itemName.Equals("Placement Manipulator"))
                        {
                            if (playerVision.focusPoint != Vector3.zero) GUI.Label(new Rect(10, 180, 300, 20), "Pickable: " + playerVision.getGridIndex().pickable + " , Destructable: " + playerVision.getGridIndex().destructable);
                        }
                    }
                }
                string dir = "";
                if (transform.localEulerAngles.y > 22.5 && transform.localEulerAngles.y <= 67.5) dir = "Northwest";
                else if (transform.localEulerAngles.y > 67.5 && transform.localEulerAngles.y <= 112.5) dir = "North";
                else if (transform.localEulerAngles.y > 112.5 && transform.localEulerAngles.y <= 157.5) dir = "Northeast";
                else if (transform.localEulerAngles.y > 157.5 && transform.localEulerAngles.y <= 202.5) dir = "east";
                else if (transform.localEulerAngles.y > 202.5 && transform.localEulerAngles.y <= 247.5) dir = "Southeast";
                else if (transform.localEulerAngles.y > 247.5 && transform.localEulerAngles.y <= 292.5) dir = "South";
                else if (transform.localEulerAngles.y > 292.5 && transform.localEulerAngles.y <= 337.5) dir = "Southwest";
                else if (transform.localEulerAngles.y > 337.5 || transform.localEulerAngles.y <= 22.5) dir = "west";
                GUI.Label(new Rect(10, 200, 300, 20), "Angle: " + transform.localEulerAngles.y);
                GUI.Label(new Rect(10, 220, 300, 20), "Facing: " + dir);
            
            }
        }
    }
    private void FixedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        uiControlls();
        visionControl();
        movement();
        keyboardAction();
        networkUpdates();
    }

    private void networkUpdates()
    {
        updateBroadcastTimer -= Time.deltaTime;
        if (updateBroadcastTimer < 0)
        {

            if (transform.position != entityUpdater.position ||
                playerVision.fpsRotation != entityUpdater.rotation)
            {
                entityUpdater.position = transform.position;
                entityUpdater.rotation = playerVision.fpsRotation;
                updateBroadcastTimer = updateBroadcastInterval;
                entityUpdater.areaObj = currentGrid.area;
                Network.doUpdate(entityUpdater);

            }
        }
    }

    private void uiControlls()
    {

            canvas.hub.staminaGauge.updateGauge(((float)(playerEntity.maxStamina - playerEntity.stamina) / playerEntity.maxStamina));
        if (playerEntity.getHolding() == null)
        {
            holdingText.text = "Nothing";
            playerEntity.holding = "";
        }
            if (!string.IsNullOrEmpty(playerEntity.holding) && playerEntity.getHolding().ItemObj.itemName != null && !playerEntity.getHolding().ItemObj.itemName.Equals(""))
        {
            holdingText.text = playerEntity.getHolding().ItemObj.itemName.Equals("") ? "Nothing" : playerEntity.getHolding().ItemObj.itemName;
            if (playerEntity.getHolding().ItemObj.maxDurability > 0)
                {
                    if (!canvas.hub.durabilityGameObject.activeInHierarchy) canvas.hub.durabilityGameObject.SetActive(true);
                    canvas.hub.durabilityGauges.updateGauge(((float)(playerEntity.getHolding().ItemObj.maxDurability - playerEntity.getHolding().ItemObj.durability) / playerEntity.getHolding().ItemObj.maxDurability));
                }
                else
                {
                    if (canvas.hub.durabilityGameObject.activeInHierarchy) canvas.hub.durabilityGameObject.SetActive(false);

                }


                if (playerEntity.getHolding().ItemObj.maxCapacity > 0)
                {
                    if (!canvas.hub.capacityGameObject.activeInHierarchy) canvas.hub.capacityGameObject.SetActive(true);
                    canvas.hub.capacityGauge.updateGauge(((float)(playerEntity.getHolding().ItemObj.maxCapacity - playerEntity.getHolding().ItemObj.capacity) / playerEntity.getHolding().ItemObj.maxCapacity));
                }
                else
                {
                    if (canvas.hub.capacityGameObject.activeInHierarchy) canvas.hub.capacityGameObject.SetActive(false);
                }
            }
            else
            {
                if (canvas.hub.capacityGameObject.activeInHierarchy) canvas.hub.capacityGameObject.SetActive(false);
                if (canvas.hub.durabilityGameObject.activeInHierarchy) canvas.hub.durabilityGameObject.SetActive(false);
            }


            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= 3)
            {
                playerEntity.stamina = playerEntity.stamina + 3 < playerEntity.maxStamina ? playerEntity.stamina + 3 : playerEntity.maxStamina;
                recoveryTimer = 0;
            }

            canvas.hub.time.text = string.Format("{0,2:D2}", 1 + ((int)(ts.getMinute() / 60)) % 12) + ":" + string.Format("{0,2:D2}", ((int)(ts.getMinute() % 60))) + " " + (1 + ((int)(ts.getMinute() / 60)) >= 12 ? "PM" : "AM");
        
    }

    private void keyboardAction()
    {
        if (!isPaused)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                useHand();
            }

            if (Input.GetButton("Rotate Left"))
            {
                rotateAvatar("Left");
            }

            if (Input.GetButton("Rotate Right"))
            {
                rotateAvatar("Right");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (Input.GetButton("Modifier"))
                    interact(true);
                else
                    interact(false);
            }


            if (Input.GetButtonDown("Drop"))
            {
                dropItem();
            }

        }
        else
        {
            if (canvas.dialogBox.gameObject.activeInHierarchy)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (dialogClick())
                    {
                        menuToggle(false);
                    }
                }
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && canRotate)
        {
            if (Vector3.Distance(playerVision.transform.position, userHead.transform.position) <= 2f)
            {
                //cameraCenter.transform.localPosition = new Vector3(0f, 2f, .35f);
                //playerVision.transform.localPosition = new Vector3(0f, 0f, 0f);
                //rotation = Vector3.zero;
                if (!playerVision.cameraMode.Equals("First Person"))
                    initFirstPersonView();
            }
            else
            {
                playerVision.transform.position = Vector3.MoveTowards(playerVision.transform.position, userHead.transform.position, 15 * Time.deltaTime);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (!playerVision.cameraMode.Equals("Third Person"))
            {
                initThirdPersonView();
            }
            playerVision.transform.position = Vector3.MoveTowards(playerVision.transform.position, userHead.transform.position, -15 * Time.deltaTime);
            //if (Vector3.Distance(playerVision.transform.position, userHead.transform.position) <= 2f)
            //{
            //    cameraCenter.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //    playerVision.transform.localPosition = new Vector3(0f, 0f, -2f);
            //}
            //else
            //{
            //}


        }
        //if (Input.GetButtonDown("Cancel"))
        //{
        //    switch (playerEntity.state)
        //    {
        //        case "Chopping":                    
        //            break;
        //        default:
        //            accessMenu();
        //            break;
        //    }
        //    Cursor.lockState = CursorLockMode.Locked;
        //}

        if (Input.GetButtonDown("Menu"))
        {
            if ((canvas.listener.listOfInputFields != null && canvas.listener.listOfInputFields.Count <= 0) || (canvas.listener.listOfInputFields == null))
                accessInventory();
        }
    }
    private bool dialogClick()
    {
        return canvas.dialogBox.isClicked();
    }
    public void creatorMode()
    {
        playerEntity.state = "Creator";
    }

    public void playerMode()
    {
        playerEntity.state = "Player";
    }

    public void dropItem()
    {
        if (playerEntity.getHolding().ItemObj != null)
            playerEntity.getHolding().ItemObj.quantity -= 1;

        if (playerEntity.getHolding().ItemObj.quantity <= 0)
        {
            playerEntity.backpack.items.Remove(playerEntity.getHolding());
        }

    }

    public void freeze()
    {
        canMove = false;
        canRotate = false;
        isPaused = true;
    }

    public void unfreeze()
    {
        canMove = true;
        canRotate = true;
        isPaused = false;
    }

    public void rotateAvatar(string in_direction)
    {
        switch (in_direction)
        {
            case "Left":
                transform.Rotate(0f, -rotateAmount, 0f);
                if (playerVision.cameraMode.Equals("Third Person"))
                    playerVision.fpsRotation.x += -rotateAmount;
                break;
            case "Right":
                transform.Rotate(0f, rotateAmount, 0f);
                if (playerVision.cameraMode.Equals("Third Person"))
                    playerVision.fpsRotation.x += rotateAmount;
                break;
        }
    }

    public void isLoading(bool in_load, string in_message, float in_percent)
    {
        if (in_load)
        {
            canvas.loadingScreen.SetActive(true);
            canvas.resourceText.text = in_message;
            canvas.percentText.text = ((int)in_percent).ToString() + " %";
            freeze();
        }
        else
        {
            canvas.loadingScreen.SetActive(false);
            unfreeze();
        }
    }
    public void isLoading(bool in_load, float in_percent)
    {
        if (in_load)
        {
            canvas.loadingScreen.SetActive(true);
            canvas.percentText.text = ((int)in_percent).ToString() + " %";
            freeze();
        }
        else
        {
            canvas.loadingScreen.SetActive(false);
            unfreeze();
        }
    }
    public void isLoading(bool in_load, string in_message)
    {
        if (in_load)
        {
            canvas.loadingScreen.SetActive(true);
            canvas.resourceText.text = in_message;
            freeze();
        }
        else
        {
            canvas.loadingScreen.SetActive(false);
            unfreeze();
        }
    }
    public void isLoading(bool in_load)
    {
        if (in_load)
        {
            canvas.loadingScreen.SetActive(true);
            freeze();
        }
        else
        {
            canvas.loadingScreen.SetActive(false);
            unfreeze();
        }
    }

    private void movement()
    {

        if (canMove)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = movementSpeed * Input.GetAxis("Vertical");
            float curSpeedY = movementSpeed * Input.GetAxis("Horizontal");
            if (curSpeedX != 0 || curSpeedY != 0)
                entityAnimation.SetBool("isWalking", true);
            else
                entityAnimation.SetBool("isWalking", false);
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void visionControl()
    {
        if (canRotate)
        {
            switch (playerVision.cameraMode)
            {
                case "First Person":
                    playerVision.fpsRotation.x += Input.GetAxis("Mouse X") * lookSensativity;
                    playerVision.fpsRotation.z = 0;

                    playerVision.fpsRotation.y += -Input.GetAxis("Mouse Y") * lookSensativity;
                    playerVision.fpsRotation.y = Mathf.Clamp(playerVision.fpsRotation.y, -lookXLimit, lookXLimit);
                    transform.eulerAngles = new Vector2(0, playerVision.fpsRotation.x);
                    userHead.localRotation = Quaternion.Euler(playerVision.fpsRotation.y, 0, 0);
                    break;
                default:
                    playerVision.tpsRotation.x += Input.GetAxis("Mouse X") * lookSensativity;
                    playerVision.tpsRotation.z = 0;

                    playerVision.tpsRotation.y += -Input.GetAxis("Mouse Y") * lookSensativity;
                    userHead.localRotation = Quaternion.Euler(playerVision.tpsRotation.y, playerVision.tpsRotation.x, 0);
                    playerVision.transform.LookAt(cameraCenter.position);
                    break;
            }
            AreaIndex get_grid = playerVision.getGridIndex();
            if (playerVision.focusPoint != Vector3.zero && playerVision.getGridIndex() != null)
            {
                if (!indexSelector.activeInHierarchy) indexSelector.SetActive(true);
                indexSelector.transform.position = currentGrid.getGridAtLocation(get_grid.x, get_grid.y, get_grid.z) + new Vector3(0f, .15f, 0f);
            }
            else
            if (indexSelector.activeInHierarchy) indexSelector.SetActive(false);

        }
    }

    private void useHand()
    {
        if (playerVision.focusPoint != Vector3.zero && !isPaused)
        {
            groundInteract();
        }

    }

    public void interact(bool is_modified)
    {
        if (playerVision.selectInteractable != null)
        {
            playerVision.selectInteractable.interact(this, is_modified);
        }
    }

    public void accessMenu()
    {
        if (isPaused)
        {
            menuToggle(false);
        }
        else
        {
            playerEntity.state = "Player";
            menuToggle(true);
        }
    }

    private void accessInventory()
    {
        if (isPaused)
            menuToggle(false);
        else
        {
            playerEntity.state = "Inventory";
            menuToggle(true);

        }
    }

    public void accessTrade()
    {
        if (isPaused)
        {
            tradeToggle(false);
        }
        else
        {
            playerEntity.state = "Player";
            tradeToggle(true);
        }
    }


    public void actionProgression(string in_action, int in_seconds)
    {
        if (currentGrid.area.buildable)
        {
            if (playerEntity.stamina < 5)
            {
                toastNotifications.newNotification("You do not have enough stamina.");
            }
            else
            {
                if (!string.IsNullOrEmpty(playerEntity.holding) && (playerEntity.getHolding() != null || !playerEntity.getHolding().ItemObj.itemName.Equals("Nothing")))
                {
                    if (playerEntity.getHolding().ItemObj.maxDurability > 0 && playerEntity.getHolding().ItemObj.durability <= 0)
                    {
                        toastNotifications.newNotification("Your " + playerEntity.getHolding().ItemObj.itemName + " is broken.");
                    }
                    else if (playerEntity.getHolding().ItemObj.maxCapacity > 0 && playerEntity.getHolding().ItemObj.capacity <= 0)
                    {
                        toastNotifications.newNotification("Your " + playerEntity.getHolding().ItemObj.itemName + " is empty.");
                    }
                    else
                    {
                        canvas.progressBar.activate(in_action, in_seconds, this);
                    }
                }
                else
                {
                    canvas.progressBar.activate(in_action, in_seconds, this);
                }
            }
        }
        else
            toastNotifications.newNotification("Unable to perform that action here");
    }


    private void useHolding(int in_durability, int in_stamina)
    {
        playerEntity.stamina -= in_stamina;
        playerEntity.getHolding().ItemObj.durability -= in_durability;
        ItemExistanceDTOWrapper toUpdate = playerEntity.getHolding();
        Network.sendPacket<ItemExistanceDTOWrapper>(doCommands.item, "Save", toUpdate);
//        Network.doSave("Item", toUpdate);
    }

    public void deselectHotbar()
    {
        holdingText.text = null;
        playerVision.disableSelection();
        playerEntity.holding = null;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            string getState = currentGrid.getIndex(transform.position).state;
            string[] parser = getState.Split(' ');
            switch (parser[0])
            {
                case "Teleport":
                    teleport(int.Parse(parser[1]), int.Parse(parser[2]), int.Parse(parser[3]),
                        getState.Replace(parser[0], "")
                        .Replace(parser[1], "")
                        .Replace(parser[2], "")
                        .Replace(parser[3], "").Trim());
                    break;
                case "Tutorial":
                    switch (getState.Replace(parser[0] + " ", ""))
                    {
                        case "Exit House":
                            newMessage("Jimmy", "Well then, being the eager one you are, lets start with the core of this game, FARMING yay....... Bring up your menu with the [M] key, and select your Basic Shovel. Whatever equiptment you have on you, you'll see the detail on the top left of your screen. If the Durability of the equipment hits 0, the equipment is unsuable. There's some equipment that has a capacity (such as watering can), and capacity too if at 0, it will not be usable. Try first plowing the grass area with your basic shovel, and then plant a strawberry seed and then water it. Water will evaporate at the end of each day. Different seeds have different amount of day they wither, or amount of day they will fully grow. When you're done exit your farm.");
                            currentGrid.currentGrid[12, 11, 0].state = "";
                            currentGrid.currentGrid[12, 10, 0].state = "";
                            currentGrid.currentGrid[12, 11, 1].objectName = "";
                            currentGrid.currentGrid[12, 10, 1].objectName = "";
                            Destroy(currentGrid.currentGrid[12, 11, 1].index);
                            Destroy(currentGrid.currentGrid[12, 10, 1].index);

                            currentGrid.currentGrid[12, 11, 1].index = null;
                            currentGrid.currentGrid[12, 10, 1].index = null;
                            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                            break;
                        case "Exit Farm":
                            newMessage("Jimmy", "As you exit the farm, there's a cube to your right, that's your shipping bin. Place all item to be sold into the shipping bin and the collector will collect the item at the of the day. When the new day beings, money will be sent to your mail on the right. The price fluctuate as you sell more an other factor. Same goes with npc that sells thing. More they have, the cheaper, but the less they have they become more expensive. The circle thing is the well where you can refil water in your watering can. Durability can be restored by the smith, and seed can be bought from the general shop keep. I think that's about it, i'm going to shut up now.");
                            currentGrid.currentGrid[2, 11, 1].state = "";
                            currentGrid.currentGrid[2, 10, 1].state = "";
                            Destroy(currentGrid.currentGrid[2, 11, 1].index);
                            Destroy(currentGrid.currentGrid[2, 10, 1].index);
                            currentGrid.currentGrid[2, 11, 1].objectName = "";
                            currentGrid.currentGrid[2, 10, 1].objectName = "";

                            currentGrid.currentGrid[2, 11, 1].index = null;
                            currentGrid.currentGrid[2, 10, 1].index = null;
                            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                            break;

                    }
                    break;
            }
        }
    }

    public bool teleport(int in_x, int in_y, int in_z, string in_area)
    {
        string tempArea = null;
        if (in_area.Equals("(Player Farm)"))
        {
            tempArea = name + "_farm";
        }
        else
        {
            tempArea = in_area;
        }

        print(in_area + " , " + in_x + " , " + in_y);
        if (Network.isConnected)
        {
            teleportPosition = currentGrid.getGridAtLocation(in_x, in_y, in_z);
            currentGrid.area.areaName = tempArea;
            Dictionary<string, string> payload = new Dictionary<string, string>();
            payload["areaName"] = tempArea;
            payload["x"] = in_x.ToString();
            payload["y"] = in_y.ToString();
            payload["z"] = in_z.ToString();
            payload["_id"] = Network.loadedCharacter._id; 
            Network.sendPacket(doCommands.action, "Teleport", payload);
//            Network.doLoad<gridIndexWrapper>("Area", newWrapper);
            return false;
        }
        else
        {
            GridSystem tempGrid = currentGrid.preloadGrid(tempArea);
            if (tempGrid == null)
            {
                toastNotifications.newNotification(tempArea + " does not exists");
                return false;
            }
            if ((in_x > 0 && in_x < tempGrid.area.length) && (in_y > 0 && in_y < tempGrid.area.width && (in_z >= 0 && in_z < tempGrid.area.height)))
            {
                currentGrid.loadPreloadGrid();
                characterController.enabled = false;
                transform.position = currentGrid.getGridAtLocation(in_x, in_y, in_z);
                characterController.enabled = true;
                return true;
            }
            else
            {
                toastNotifications.newNotification("Destination is out of boundary");
            }
            return false;
        }
    }

    public bool teleportHelper(GridSystem in_grid)
    {
        print(teleportPosition);
        print(in_grid.area.length + " , " + in_grid.area.width + " , " + in_grid.area.height);
        if (in_grid == null)
        {
            toastNotifications.newNotification(in_grid.area.areaName + " does not exists");
            return false;
        }
        if ((teleportPosition.x >= 0 && teleportPosition.x < in_grid.area.length) && (teleportPosition.y >= 0 && teleportPosition.y < in_grid.area.height && (teleportPosition.z >= 0 && teleportPosition.z < in_grid.area.width)))
        {
            //            currentGrid.loadPreloadGrid();
            //            string temp_area = currentGrid.areaName;
            //            currentGrid.unloadGrid();
            //           currentGrid.areaName = temp_area;
            currentGrid.loadArea();
            characterController.enabled = false;
            transform.position = teleportPosition;
            characterController.enabled = true;
            return true;
        }
        else
        {
            toastNotifications.newNotification("Destination is out of boundary");
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(cameraCenter.transform.position, userHead.transform.position);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(userHead.transform.position, playerVision.transform.position);
        //Gizmos.DrawLine(stepHigh.transform.position, stepHigh.transform.position + stepHigh.transform.forward * .5f);
    }

    public void menuToggle(bool in_toggle)
    {

        canvas.listener.gameObject.SetActive(in_toggle);
        if (!in_toggle)
        {
            mainMenu.close();
            mainMenu.menuState = null;
            canvas.tradeMenu.close();
            canvas.tradeMenu.menuState = null;
            Cursor.lockState = CursorLockMode.Locked;
            canRotate = true;
            canMove = true;
            isPaused = false;

        }
        else
        {
            canvas.tradeMenu.close();
            canvas.tradeMenu.menuState = null;
            mainMenu.init();
            Cursor.lockState = CursorLockMode.None;
            canRotate = false;
            canMove = false;
            isPaused = true;

        }
    }
    public void tradeToggle(bool in_toggle)
    {
        canvas.tradeMenu.gameObject.SetActive(in_toggle);

        if (!in_toggle)
        {
            mainMenu.close();
            mainMenu.menuState = null;
            canvas.tradeMenu.close();
            canvas.tradeMenu.menuState = null;
            Cursor.lockState = CursorLockMode.Locked;
            canRotate = true;
            canMove = true;
            isPaused = false;

        }
        else
        {
            canvas.tradeMenu.init();
            Cursor.lockState = CursorLockMode.None;
            mainMenu.close();
            mainMenu.menuState = null;
            canRotate = false;
            canMove = false;
            isPaused = true;

        }
    }

    public void setActionListener(IActionListener listener)
    {
        throw new System.NotImplementedException();
    }

    public IActionListener getActionListener()
    {
        return this;
    }

    private void groundInteract()
    {
        if (!isPaused)
        {
            //fix this when getholding is null
            if (string.IsNullOrEmpty(playerEntity.holding) || playerEntity.getHolding().ItemObj.itemName.Equals(""))
            {

                if (playerVision.focusPoint != Vector3.zero && playerVision.getGridIndex().index != null)
                {
                    if (playerVision.getGridIndex().index.TryGetComponent<IPickable>(out IPickable out_pick))
                    {
                        out_pick.pickupCheck(playerVision.getGridIndex(), currentGrid, out string in_item, out string in_state);
                        if (in_state != null)
                        {
                            if (Network.isConnected)
                            {
                                ActionWrapper new_action = new ActionWrapper();
                                new_action.areaName = currentGrid.area.areaName;
                                new_action.index = playerVision.getGridIndex();
                                Network.sendPacket(doCommands.action, "Pick vegetation", new_action);
                                //                                Network.doAction<ActionWrapper>("Index", new_action);
                            }
                            else
                            {
                                if (in_state.Equals("Grown"))
                                {
                                    current_avatar.current_avatarEntity.animator.SetBool("isPlanting", true);
                                    actionProgression("Harvesting " + in_item, 2);
                                }
                                else if (in_state.Equals("Dead"))
                                {
                                    current_avatar.current_avatarEntity.animator.SetBool("isPlanting", true);
                                    actionProgression("Removing " + in_item, 2);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                switch (playerEntity.getHolding().ItemObj.itemType)
                {
                    case "Bed":
                        currentGrid.generateObject(playerVision.getGridIndex(), playerEntity.getHolding().ItemObj.itemName, true, true);
                        playerEntity.backpack.modifyItem(playerEntity.getHolding(), 1);
                        currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                        break;
                    case "Path":
                    case "Slab":
                    case "Stair":
                    case "Wall":
                    case "Floor":
                    case "Trigger":
                        if (playerVision.getGridIndex().index == null)
                        {
                            currentGrid.generateObject(playerVision.getGridIndex(), playerEntity.getHolding().ItemObj.itemName, true, true);
                            playerEntity.backpack.modifyItem(playerEntity.getHolding(), 1);
                            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                        }
                        break;
                    case "Fence":
                        if (playerVision.getGridIndex().index == null)
                        {
                            currentGrid.generateObject(playerVision.getGridIndex(), "Wooden Fence", true, true);
                            AreaIndex topIndex = currentGrid.getIndex(playerVision.getGridIndex().x, playerVision.getGridIndex().y, playerVision.getGridIndex().z + 1);
                            currentGrid.generateObject(topIndex, "Wooden Fence", true, true);
                            playerEntity.backpack.modifyItem(playerEntity.getHolding(), 1);
                            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                        }
                        break;
                    case "Placement Modifier Tool":
                        if (playerVision.getGridIndex().index != null)
                        {
                            playerVision.getGridIndex().pickable = playerVision.getGridIndex().pickable ? false : true;
                        }
                        break;
                    case "Action Modifier Tool":
                        if (playerVision.getGridIndex().index != null)
                        {
                            if (playerVision.getGridIndex().index.TryGetComponent<IActions>(out IActions out_action))
                            {
                                out_action.modifyAction(playerVision.getGridIndex(), mainMenu);
                                Cursor.lockState = CursorLockMode.None;
                                canRotate = false;
                                canMove = false;
                                isPaused = true;
                            }
                        }
                        break;

                    case "Pickaxe":
                        if (playerVision.getGridIndex().index != null)
                        {
                            if (playerVision.getGridIndex().index.TryGetComponent<WorldResource>(out WorldResource getResource))
                            {
                                if (getResource.type.Equals("Stone"))
                                {
                                    if (getResource.damage(25))
                                    {
                                        Destroy(playerVision.getGridIndex().index);
                                        playerVision.getGridIndex().index = null;
                                    };
                                }
                            }
                        }
                        break;
                    case "Shovel":
                        if (playerVision.getGridIndex().index == null)
                        {

                            if (playerVision.getGridIndex().index == null)
                            {
                                actionProgression("Plowing Land", 3);
                                current_avatar.current_avatarEntity.animator.SetBool("isPlowing", true);
                            }
                        }
                        break;
                    case "Seed":
                        if (playerVision.getGridIndex().index != null)
                        {
                            if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil getSoil))
                            {
                                if (string.IsNullOrEmpty(getSoil.plant.plantName))
                                {
                                    actionProgression("Planting " + playerEntity.getHolding().ItemObj.itemName, 3);
                                    current_avatar.current_avatarEntity.animator.SetBool("isPlanting", true);
                                }
                            }
                        }
                        break;
                    case "Watering Can":
                        if (playerVision.getGridIndex().index != null)
                        {
                            if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil getSoil))
                            {
                                if (playerEntity.getHolding().ItemObj.capacity > 0)
                                {
                                    current_avatar.current_avatarEntity.animator.SetBool("isWatering", true);
                                    actionProgression("Watering", 3);
                                }
                                else
                                {
                                    toastNotifications.newNotification("Your " + playerEntity.getHolding().ItemObj.itemName + " has no more water.");
                                }
                            }
                            else if (playerVision.getGridIndex().index.TryGetComponent<Well>(out Well out_well))
                            {
                                actionProgression("Refilling", (int)(playerEntity.getHolding().ItemObj.maxCapacity / 10));
                            }
                        }

                        break;
                }
            }
        }
    }

    public void doAction(string in_action)
    {
        string[] parser = in_action.Split(' ');

        AreaIndexDTO newWrapper = null;
        if (parser[0].Equals("Planting"))
        {
            if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil getSoil))
            {
                if (Network.isConnected)
                {
                        newWrapper = playerVision.getGridIndex().toDTO();
                        newWrapper.areaObj = currentGrid.area.toDTO();
                        newWrapper.state = "Plant " + playerEntity.getHolding().ItemObj.itemName;
                        playerEntity.stamina -= 5;
                        current_avatar.current_avatarEntity.animator.SetBool("isPlanting", false);
                        playerEntity.backpack.modifyItem(playerEntity.getHolding(), 1);
                        Network.sendPacket<AreaIndexDTO>(doCommands.index, "Plant", newWrapper);
                    
                }
                else
                {
                    if (getSoil.plantSeed(currentGrid, playerVision.getGridIndex(), PlantFactory.retrievePlant(playerEntity.getHolding().ItemObj.itemName)))
                    {
                        getSoil.currentGridIndex = playerVision.getGridIndex();
                        playerVision.getGridIndex().state = playerEntity.getHolding().ItemObj.itemName;
                        playerEntity.stamina -= 5;
                        playerEntity.backpack.modifyItem(playerEntity.getHolding(), 1);
                    }
                }
            }
        }
        else if (parser[0].Equals("Removing"))
        {
            string temp_pickup = in_action.Replace(parser[0] + " ", "");
            if (temp_pickup != null || !temp_pickup.Equals("Nothing"))
            {

                if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil out_soil))
                {
                    PlantFactory.removeCrop(out_soil.plant);


                    currentGrid.removeObjectAtIndex(playerVision.getGridIndex());
                    newWrapper = playerVision.getGridIndex().toDTO();
                    newWrapper.areaObj = currentGrid.area.toDTO();
                    newWrapper.state = "Harvest";
                    current_avatar.current_avatarEntity.animator.SetBool("isPlanting", false);
                    Network.sendPacket<AreaIndexDTO>(doCommands.index, "Harvest", newWrapper);
                }
            }
        }
        else if (parser[0].Equals("Harvesting"))
        {
            string temp_pickup = in_action.Replace(parser[0] + " ", "");
            if (temp_pickup != null || !temp_pickup.Equals("Nothing"))
            {
                if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil out_soil))
                {
                    if (Network.isConnected)
                    {
                        DataCache.plantCache.TryGetValue(temp_pickup, out Plant out_plant);

                        playerEntity.backpack.createItem(name, out_plant.plantName, Random.Range(2, 4));
                        currentGrid.removeObjectAtIndex(playerVision.getGridIndex());
                        newWrapper = playerVision.getGridIndex().toDTO();
                        newWrapper.areaObj = currentGrid.area.toDTO();
                        newWrapper.state = "Harvest";
                        current_avatar.current_avatarEntity.animator.SetBool("isPlanting", false);
                        Network.sendPacket<AreaIndexDTO>(doCommands.index, "Harvest", newWrapper);
                    }
                    else
                    {
                        if (playerEntity.backpack.createItem(name, temp_pickup, Random.Range(2, 4)))
                        {
                            //                        PlantFactory.removeCrop(out_soil.plant);
                            Destroy(playerVision.getGridIndex().index);
                            playerVision.getGridIndex().index = null;
                            playerVision.getGridIndex().objectName = null;
                            currentGrid.saveArea(Network.isConnected ? "Online" : "Offline");
                        }
                        else
                        {
                            toastNotifications.newNotification("Your bag is full");
                        }
                    }

                }
            }
        }
        else
        {
            switch (in_action)
            {
                case "Plowing Land":
                        currentGrid.generateObject(playerVision.getGridIndex(), "Soil", true, true);
                        newWrapper = playerVision.getGridIndex().toDTO();
                        newWrapper.areaObj = currentGrid.area.toDTO();
                        newWrapper.state = "Plow";
                        current_avatar.current_avatarEntity.animator.SetBool("isPlowing", false);
                        Network.sendPacket<AreaIndexDTO>(doCommands.index, "Plow", newWrapper);
                        useHolding(2, 5);
                    break;
                case "Watering":
                    if (playerVision.getGridIndex().index.TryGetComponent<Soil>(out Soil getSoil))
                    {
                        playerEntity.getHolding().ItemObj.capacity -= 1;
                        newWrapper = playerVision.getGridIndex().toDTO();
                        newWrapper.areaObj = currentGrid.area.toDTO();
                        newWrapper.state = "Water";
                        current_avatar.current_avatarEntity.animator.SetBool("isWatering", false);
                        Network.sendPacket<AreaIndexDTO>(doCommands.index, "Water", newWrapper);
                        useHolding(2, 5);
                    }
                    break;
                case "Refilling":
                    playerVision.selectInteractable.reaction(this);
                    current_avatar.current_avatarEntity.animator.SetBool("isWatering", false);
                    useHolding(1, 5);
                    break;
            }
        }
    }
    public void listen(string getAction)
    {
        string[] parsed = getAction.Split(' ');
        if (parsed[0].Equals("Select"))
        {
            switch (playerEntity.state)
            {
                case "Inventory":
                    playerEntity.holding = playerEntity.backpack.items[int.Parse(parsed[1])]._id;
                    playerEntity.backpack.indexSelected = int.Parse(parsed[1]);
                    mainMenu.init();
                    holdingText.text = playerEntity.getHolding().ItemObj.itemName;
                    break;
                case "Smithery":
                    ItemExistanceDTOWrapper toRepair = playerEntity.backpack.items[int.Parse(parsed[1])];
                    int repairAmount = toRepair.ItemObj.maxDurability - toRepair.ItemObj.durability;
                    ItemExistanceDTOWrapper currency = playerEntity.backpack.items.Find(x => x.ItemObj.itemName.Equals("Silver"));
                    if (currency.ItemObj.quantity >= repairAmount && toRepair.ItemObj.durability != 0)
                    {
                        toRepair.ItemObj.durability = toRepair.ItemObj.maxDurability;
                        currency.ItemObj.quantity -= repairAmount;
                        Network.trade("Entity", playerEntity.entityName, "NPC", mainMenu.focusShop.currentNPC._id, currency._id, repairAmount);

                        Network.sendPacket(doCommands.item, "Save", toRepair);
                    }
                    mainMenu.updateMenu();
                    break;
            }
        }
        else if (parsed[0].Equals("Unselect"))
        {
            playerEntity.holding = null;
            playerEntity.backpack.indexSelected = -1;
            mainMenu.init();
            holdingText.text = "Nothing";
        }
        else if (parsed[0].Equals("Hover"))
        {
            switch (playerEntity.state)
            {
                case "Smithery":
                    ItemDTO hoverItem = playerEntity.backpack.items[int.Parse(parsed[1])].ItemObj;
                    string dialogResponse = null;
                    float itemDurability = (float)hoverItem.durability / (float)hoverItem.maxDurability;
                    if (itemDurability == 1)
                    {
                        dialogResponse = " looks fine";
                    }
                    else if (itemDurability > .50f && itemDurability < 1f)
                    {
                        dialogResponse = " looks like it has been worn down a bit and may cost " + (hoverItem.maxDurability - hoverItem.durability) + " silver to repair.";
                    }
                    else if (itemDurability >= .1f && itemDurability <= .50f)
                    {
                        dialogResponse = " looks like it is about to break and may cost " + (hoverItem.maxDurability - hoverItem.durability) + " silver to repair.";
                    }
                    else
                    {
                        dialogResponse = " is damage beyond repair and I won't be able to repair it sadly :(.";
                    }
                    newMessage(canvas.dialogBox.speakerName.text, "Looks like your " + playerEntity.backpack.items[int.Parse(parsed[1])].ItemObj.itemName + dialogResponse);
                    break;
            }
        }
        else
        {
            switch (getAction)
            {
                case "Good-bye":
                    canvas.dialogBox.textPerSecond = Configurations.textSpeed;
                    continueMessage();
                    menuToggle(false);
                    break;
                case "Repair":
                    foreach (Transform it_child in canvas.listOfResponses)
                    {
                        Destroy(it_child.gameObject);
                    }
                    playerEntity.state = "Smithery";
                    newResponse("Good-bye");
                    menuToggle(true);
                    break;
                case "Buy seeds":
                    foreach (Transform it_child in canvas.listOfResponses)
                    {
                        Destroy(it_child.gameObject);
                    }
                    playerEntity.state = "General Shop";
                    newResponse("Good-bye");
                    menuToggle(true);
                    break;

            }

            //            selectHotbar(int.Parse(parsed[1]));
        }
    }

    public void dayFinished()
    {
    }

    public void dayBegin()
    {
    }

    public void newMessage(string in_name, string in_message)
    {
        Cursor.lockState = CursorLockMode.None;
        canvas.dialogBox.newMessage(in_name, in_message);
        isPaused = true;
        canMove = false;
        canRotate = false;
    }

    public void newResponse(string in_response)
    {
        canvas.newResponse(this, in_response);

    }

    public void continueMessage()
    {
        canvas.dialogBox.dialogClickResponse();
        foreach (Transform it_child in canvas.listOfResponses)
        {
            Destroy(it_child.gameObject);
        }
        isPaused = false;
        canMove = true;
        canRotate = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void teleportToDestination()
    {
        if (teleportPosition != Vector3.zero)
        transform.position = teleportPosition;
    }

    public void loadTutorial()
    {
        if (playerEntity.state.Equals("New"))
        {
            playerEntity.state = "Player";
            newMessage("Jimmy", "Hi ho there! Welcome to the alpha version of this game, Move with WASD, and control your camera with the mouse (FPS logic much?). Anyways, M button will bring up your inventory and ESC will bring up setting menu. IT's not fully implemented but that's where you can go into the builder mode to build up your place.. it's clunky but i will finish it later. Lets go outside and proceed with the game.");
            save();
        }
    }
}
