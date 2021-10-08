using Newtonsoft.Json;
using Socket.Quobject.SocketIoClientDotNet.Client;
//using Socket.Newtonsoft.Json;
//using Socket.Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static QSocket socket;
    private static string url;
    public static String Username { get; set; }
    public static String Password { get; set; }
    public static String Room { get; set; }
    public static String UserID { get; set; }
    public static bool local { get; set; }
    public static bool isConnected { get; set; }
    public static Entity loadedCharacter { get; set; }
    public static World loadedWorld { get; set; }
    public static Queue<characterListWrapper> characterNetworkUpdate = new Queue<characterListWrapper>();

    public static Queue<Dictionary<string, string>> serverResponse = new Queue<Dictionary<string, string>>();
    public static Queue<ItemWrapper> itemWrappers = new Queue<ItemWrapper>();
    public static Queue<gridIndexDtoWrapper> indexWrapper = new Queue<gridIndexDtoWrapper>();
    public static Queue<gridIndexPlantWrapper> areaIndexPlantWrapper = new Queue<gridIndexPlantWrapper>();
    public static Queue<allGridIndexPlantWrapper> listAreaIndexPlantWrapper = new Queue<allGridIndexPlantWrapper>();
    public static Queue<WorldListWrapper> worldListWrapper = new Queue<WorldListWrapper>();
    public static Queue<ItemExistanceWrapper> listOfCharacersItem = new Queue<ItemExistanceWrapper>();
    public static Queue<networkListReceiver<ItemExistanceDTOWrapper>> listOfStorageItem = new Queue<networkListReceiver<ItemExistanceDTOWrapper>>();
    public static Queue<plantDatabaseWrapper> listOfPlantDatabase = new Queue<plantDatabaseWrapper>();
    public static Queue<itemDatabaseWrapper> listOfItemDatabase = new Queue<itemDatabaseWrapper>();

    public static Queue<ActionWrapper> actionRetrieved = new Queue<ActionWrapper>();
    public static Stack<string> updateResponseInterpolation = new Stack<string>();
    private static JsonSerializerSettings settings;

    private static Queue<emitQueue> emitQueues = new Queue<emitQueue>();
    private static Queue<packetData> packetQueue = new Queue<packetData>();
    private float lastPacketSentTime = 0f;
    private float packetThreshold = 1f;


    public static Queue<List<PlantDTO>> plantDatabase = new Queue<List<PlantDTO>>();
    public static Queue<List<ItemDTO>> itemDatabase = new Queue<List<ItemDTO>>();
    public static Queue<WorldDTO> worldRetrieved = new Queue<WorldDTO>();

    public static Queue<characterListWrapper> listOfCharacters = new Queue<characterListWrapper>();
    public static Queue<List<EntityDTO>> listOfCharacter = new Queue<List<EntityDTO>>();
    public static Queue<List<WorldDTO>> listOfWorlds = new Queue<List<WorldDTO>>();
    public static Queue<List<ItemExistanceDTOWrapper>> listOfItems = new Queue<List<ItemExistanceDTOWrapper>>();
    public static Queue<string> serverAcknowledge = new Queue<string>();
    public static Queue<AreaDTO> areaConfig = new Queue<AreaDTO>();
    public static Queue<List<AreaIndexDTO>> listOfAreaIndexes = new Queue<List<AreaIndexDTO>>();
    public static Queue<List<AreaItemDTO>> listOfAreaItems = new Queue<List<AreaItemDTO>>();
    public static Queue<List<EntityExistanceDTO>> listOfAreaNPCs = new Queue<List<EntityExistanceDTO>>();
    public static Queue<List<AreaPlantDTO>> listOfAreaPlants = new Queue<List<AreaPlantDTO>>();
    public static Queue<AreaPlantDTO> areaPlants = new Queue<AreaPlantDTO>();
    public static Queue<ItemExistanceDTOWrapper> itemRetrieved = new Queue<ItemExistanceDTOWrapper>();

    public static bool debug = false;
    public static string state;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (emitQueues.Count > 0)
        {
            emitQueues.Dequeue().emitData();
            if (lastPacketSentTime - Time.time > packetThreshold)
                print("Lag");
            lastPacketSentTime = Time.time;
        }

        if (packetQueue.Count > 0)
            packetQueue.Dequeue().processPacket();
    }

    #region QSocket implementation
    public static void joinGame(string username, string password, string in_mode, string out_actual)
    {

        try
        {
            //            GameObject.Find("Listener").GetComponent<LoginServerListener>().alert.GetComponent<Alert>().Show("Connecting", 5f);
            socket = IO.Socket("http://35.212.249.77:26843");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        socket.On(QSocket.EVENT_CONNECT, () =>
        {
            try
            {
                settings = new JsonSerializerSettings();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                switch (in_mode)
                {
                    case "Login":
                        Dictionary<string, string> payload = new Dictionary<string, string>();
                        payload["Username"] = username;
                        payload["Password"] = password;
                        payload["Actual"] = out_actual;
                        payload["Action"] = "Login";
                        emitQueues.Enqueue(new emitQueue(socket, "Login", JsonConvert.SerializeObject(payload)));
                        //                        socket.Emit("Login", JsonConvert.SerializeObject(payload));
                        break;
                    case "Register":
                        Dictionary<string, string> create_payload = new Dictionary<string, string>();
                        create_payload["Username"] = username;
                        create_payload["Password"] = password;
                        create_payload["Actual"] = out_actual;
                        create_payload["Action"] = "Create user";
                        emitQueues.Enqueue(new emitQueue(socket, "Login", JsonConvert.SerializeObject(create_payload)));
                        //                      socket.Emit("Login", JsonConvert.SerializeObject(create_payload));
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        });

        socket.On("disconnect", () =>
        {
            Debug.Log("Disconnectiong");
        });

        //socket.On("Item", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    print(data);
        //    try
        //    {
        //        ItemWrapper json = JsonConvert.DeserializeObject<ItemWrapper>(data);
        //        itemWrappers.Enqueue(json);

        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log(e.StackTrace);
        //    }
        //});

        //socket.On("Area", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    try
        //    {
        //        gridIndexDtoWrapper json = JsonConvert.DeserializeObject<gridIndexDtoWrapper>(getData.ToString().Replace('`', '"'));
        //        indexWrapper.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //        print(e.Source);
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Load Plant", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        gridIndexPlantWrapper json = JsonConvert.DeserializeObject<gridIndexPlantWrapper>(getData.ToString().Replace('`', '"'));
        //        areaIndexPlantWrapper.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Load worlds", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        WorldListWrapper json = JsonConvert.DeserializeObject<WorldListWrapper>(getData.ToString().Replace('`', '"'));
        //        worldListWrapper.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Action Index", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    print(data);
        //    try
        //    {
        //        ActionWrapper json = JsonConvert.DeserializeObject<ActionWrapper>(data);
        //        actionRetrieved.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Load world", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        WorldDTO json = JsonConvert.DeserializeObject<WorldDTO>(data);
        //        worldRetrieved.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Load area plant", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    try
        //    {
        //        allGridIndexPlantWrapper json = JsonConvert.DeserializeObject<allGridIndexPlantWrapper>(data);
        //        listAreaIndexPlantWrapper.Enqueue(json);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //        print(e.StackTrace);
        //    }
        //});

        //socket.On("Character updates", (getData) =>
        //{
        //    //            string data = getData.ToString().Replace('`', '"');
        //    ////            print(data);
        //    //            try
        //    //            {
        //    //                characterListWrapper newWrapper = JsonConvert.DeserializeObject<characterListWrapper>(data);
        //    //                characterNetworkUpdate.Enqueue(newWrapper);
        //    //            }
        //    //            catch (Exception e)
        //    //            {
        //    //                print(e.StackTrace);
        //    //            }
        //});

        ////socket.On("Area Update", (getData) =>
        ////{
        ////    string data = getData.ToString().Replace('`', '"');
        ////    try
        ////    {
        ////        areaIndexDTO json = JsonConvert.DeserializeObject<areaIndexDTO>(getData.ToString().Replace('`', '"'));
        ////        gridIndexUpdator.Enqueue(json);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        print(e.StackTrace);
        ////    }
        ////});

        ////socket.On("Load area item", (getData) =>
        ////{
        ////    try
        ////    {
        ////        areaListItemWrapperDTO json = JsonConvert.DeserializeObject<areaListItemWrapperDTO>(getData.ToString().Replace('`', '"'));
        ////        areaItemWrapper.Enqueue(json);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        print(e.Message);
        ////    }
        ////});

        ////socket.On("Load area NPC", (getData) =>
        ////{
        ////    print(getData);

        ////    try
        ////    {
        ////        networkListReceiver<EntityDTOWrapper> json = JsonConvert.DeserializeObject<networkListReceiver<EntityDTOWrapper>>(getData.ToString().Replace('`', '"'));
        ////        listOfNPC.Enqueue(json);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        print(e.Message);
        ////    }
        ////});

        //socket.On("Action", (getData) =>
        //{
        //    Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(getData.ToString().Replace('`', '"'));
        //    serverResponse.Enqueue(json);
        //});

        //socket.On("Load plant database", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        plantDatabaseWrapper json = JsonConvert.DeserializeObject<plantDatabaseWrapper>(data);
        //        listOfPlantDatabase.Enqueue(json);

        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //    }
        //});

        //socket.On("Load item database", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        itemDatabaseWrapper json = JsonConvert.DeserializeObject<itemDatabaseWrapper>(data);
        //        listOfItemDatabase.Enqueue(json);
        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //    }
        //});

        ////socket.On("Load storage access", (getData) =>
        ////{
        ////    string data = getData.ToString().Replace('`', '"');
        ////    try { 
        ////        StorageItemListWrapper json = JsonConvert.DeserializeObject<StorageItemListWrapper>(data);
        ////        listOfStorageItem.Enqueue(json);
        ////    }
        ////    catch (Socket.Newtonsoft.Json.JsonException ex)
        ////    {
        ////        print("Json error");
        ////        print(ex.Data);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        print(e.Message);
        ////    }
        ////});

        ////socket.On("Get Item List", (getData) =>
        ////{
        ////    string data = getData.ToString().Replace('`', '"');

        ////    try
        ////    {
        ////        networkListReceiver<ItemExistanceDTOWrapper> newWrapper = JsonConvert.DeserializeObject<networkListReceiver<ItemExistanceDTOWrapper>>(data);
        ////        itemRetrieved.Enqueue(newWrapper);
        ////    }
        ////    catch (Socket.Newtonsoft.Json.JsonException ex)
        ////    {
        ////        print("Json error");
        ////        print(ex.Data);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        print(e.Message);
        ////    }
        ////});

        //socket.On("Transfer Item", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');

        //    try
        //    {
        //        ItemExistanceDTOWrapper newWrapper = JsonConvert.DeserializeObject<ItemExistanceDTOWrapper>(data);
        //        itemRetrieved.Enqueue(newWrapper);
        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //    }
        //});

        //socket.On("Get Item", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    print(data);
        //    try
        //    {
        //        ItemExistanceDTOWrapper newWrapper = JsonConvert.DeserializeObject<ItemExistanceDTOWrapper>(data);
        //        itemRetrieved.Enqueue(newWrapper);
        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //    }
        //});

        //socket.On("Load Items", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    print(data);
        //    try
        //    {
        //        ItemExistanceWrapper newWrapper = JsonConvert.DeserializeObject<ItemExistanceWrapper>(data);
        //        listOfCharacersItem.Enqueue(newWrapper);
        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //    }
        //});

        //socket.On("Load characters", (getData) =>
        //{
        //    string data = getData.ToString().Replace('`', '"');
        //    //            print(data);
        //    try
        //    {
        //        var newWrapper = JsonConvert.DeserializeObject<characterListWrapper>(data);
        //        listOfCharacters.Enqueue(newWrapper);
        //    }
        //    catch (JsonException ex)
        //    {
        //        print("Json error");
        //        print(ex.Data);
        //    }
        //    catch (Exception e)
        //    {
        //        print(e.Message);
        //        //print(e.StackTrace);
        //        //string entityName = data.Substring(62);
        //        //entityName = entityName.Substring(0,entityName.IndexOf('"'));
        //        //Dictionary<string, string> payload = new Dictionary<string, string>();
        //        //payload.Add("Username", Network.Username);
        //        //payload.Add("entityName", entityName);
        //        //payload.Add("Action", "Corrupted");
        //        //Network.doLogin(payload);
        //    }
        //});

        //socket.On("Mass Update", (getData) =>
        //{
        //    updateResponseInterpolation.Push(getData.ToString().Replace('`', '"'));
        //});

        socket.On("Get Data", (getData) =>
        {
            try
            {
                packetData new_packet = JsonConvert.DeserializeObject<packetData>(getData.ToString());
                packetQueue.Enqueue(new_packet);
            }
            catch (JsonException ex)
            {
                print("Json error");
                print(ex.Data);
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        });

    }

    /**
     *  Relay to everyone
     */
    public static void trade(string in_from_type, string in_from_name, string in_to_type, string in_to_name, string in_name, int in_quantity)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["fromType"] = in_from_type;
        payload["fromName"] = in_from_name;
        payload["toType"] = in_to_type;
        payload["toName"] = in_to_name;
        payload["item"] = in_name;
        payload["quantity"] = in_quantity.ToString();
        emitQueues.Enqueue(new emitQueue(socket, "Trade", JsonConvert.SerializeObject(payload)));
        //        socket.Emit("Trade", JsonConvert.SerializeObject(payload));
    }

    public static void doSave<T>(string in_mode, T in_data)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Data", JsonConvert.SerializeObject(in_data, settings));
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        emitQueues.Enqueue(new emitQueue(socket, "Save " + in_mode, JsonConvert.SerializeObject(payload)));
        //        socket.Emit("Save " + in_mode, JsonConvert.SerializeObject(payload));
    }

    public static void doAction<T>(string in_mode, T in_data)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Data", JsonConvert.SerializeObject(in_data, settings));
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        emitQueues.Enqueue(new emitQueue(socket, "Action " + in_mode, JsonConvert.SerializeObject(payload)));
    }

    public static void doSaveImmediate<T>(string in_mode, T in_data)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Data", JsonConvert.SerializeObject(in_data, settings));
        if (loadedWorld != null)
        {

            payload["worldName"] = Network.loadedWorld.worldName;
        }
        //        new emitQueue(socket, "Save " + in_mode, JsonConvert.SerializeObject(payload));

        socket.Emit("Save " + in_mode, JsonConvert.SerializeObject(payload));
    }

    public static void doLoad<T>(string in_mode, T in_data)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["worldName"] = Network.loadedWorld.worldName;
        payload.Add("Data", JsonConvert.SerializeObject(in_data, settings));
        emitQueues.Enqueue(new emitQueue(socket, "Load " + in_mode, JsonConvert.SerializeObject(payload)));

        //      socket.Emit("Load " + in_mode, JsonConvert.SerializeObject(payload));
    }
    public static void accessStorage(string in_mode, string in_data)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload.Add("Data", in_data);
        emitQueues.Enqueue(new emitQueue(socket, "Access " + in_mode, JsonConvert.SerializeObject(payload)));
        //        socket.Emit("Access " + in_mode, JsonConvert.SerializeObject(payload));
    }
    public static void doLoading(string in_input)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Username"] = loadedCharacter.entityName;
        payload["World"] = loadedWorld.worldName;
        payload["Action"] = in_input;
        emitQueues.Enqueue(new emitQueue(socket, "Loading", JsonConvert.SerializeObject(payload)));
        //        socket.Emit("Loading", JsonConvert.SerializeObject(payload));
    }
    public static void doShop(string in_target, string in_input)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["Target"] = in_target;
        payload["Username"] = loadedCharacter.entityName;
        payload["World"] = loadedWorld.worldName;
        payload["Action"] = in_input;
        emitQueues.Enqueue(new emitQueue(socket, "Shop", JsonConvert.SerializeObject(payload)));
        //        socket.Emit("Loading", JsonConvert.SerializeObject(payload));
    }
    public static void doLogin(Dictionary<string, string> in_payload)
    {
        emitQueues.Enqueue(new emitQueue(socket, "Login", JsonConvert.SerializeObject(in_payload)));
        //        socket.Emit("Login", JsonConvert.SerializeObject(in_payload));
    }
    #endregion

    public static void areaItemInteract(AreaItemDTO in_payload, string in_action)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["entity"] = loadedCharacter.entityName;
        payload["item"] = JsonConvert.SerializeObject(in_payload, settings);
        payload["input"] = in_action;
        payload["command"] = "Action";
        emitQueues.Enqueue(new emitQueue(socket, "Packet", JsonConvert.SerializeObject(payload)));
//        emitQueues.Enqueue(new emitQueue(socket, "Update area item", JsonConvert.SerializeObject(in_payload, settings)));
    }

    public static void itemUpdated(string in_command, string in_action, string in_item, int in_amount)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["command"] = in_command;
        payload["input"] = in_action;
        payload["item"] = in_item;
        payload["quantity"] = in_amount.ToString();
        payload["entity"] = loadedCharacter.entityName;
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        emitQueues.Enqueue(new emitQueue(socket, "Packet", JsonConvert.SerializeObject(payload)));
    }
    public static void sendPacket<T>(string in_command, string in_action, T in_wrapper)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["command"] = in_command;
        payload["input"] = in_action;
        payload["username"] = Username;
        if (loadedCharacter != null)
        {
            payload["entity"] = loadedCharacter.entityName;
        }
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        payload["data"] = JsonConvert.SerializeObject(in_wrapper);

        emitQueues.Enqueue(new emitQueue(socket, "Packet", JsonConvert.SerializeObject(payload)));
    }

    public static void sendPacket(string in_command, string in_action)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["command"] = in_command;
        payload["input"] = in_action;
        payload["username"] = Username;
        if (loadedCharacter != null)
        {
            payload["entity"] = loadedCharacter.entityName;
        }
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        emitQueues.Enqueue(new emitQueue(socket, "Packet", JsonConvert.SerializeObject(payload)));
    }
    public static void sendPacket(string in_command, string in_action, string in_area)
    {
        Dictionary<string, string> payload = new Dictionary<string, string>();
        payload["command"] = in_command;
        payload["areaName"] = in_area;
        payload["input"] = in_action;
        payload["username"] = Username;
        if (loadedCharacter != null)
        {
            payload["entity"] = loadedCharacter.entityName;
        }
        if (loadedWorld != null)
        {
            payload["worldName"] = Network.loadedWorld.worldName;
        }
        emitQueues.Enqueue(new emitQueue(socket, "Packet", JsonConvert.SerializeObject(payload)));
    }

}
[Serializable]
public class characterListWrapper
{
    public List<EntityDTO> characterList;
    public int total;
    public Int64 time;
    public string Action;
    public int index;
}

public class ItemWrapper
{
    public ItemDTO Item;
    public string Action;
}

class emitQueue
{
    public string label;
    public string data;
    public QSocket socket;

    public emitQueue(QSocket in_socket, string in_label, string in_data)
    {
        label = in_label;
        data = in_data;
        socket = in_socket;
    }

    public void emitData()
    {
        socket.Emit(label, data);
    }
}

[Serializable]
public class networkListReceiver<T>
{
    public List<T> objectList;
    public string Action;
    public string type;
    public int index;
    public int total;
}

/**
 *  Process the data that were retrieved from the server, and enqueue them to the appropriate queue.
 * 
 */
public class packetData
{
    public string type;
    public string data;

    public void processPacket()
    {
        data = data.Replace('`', '"');
        if (Network.debug)
        {
            Debug.Log(type);
            Debug.Log(data);

        }

        try
        {
            switch (type)
            {
                case "Acknowledge":
                    processPacketHelper<string>(Network.serverAcknowledge);
                    break;
                case "Character list":
                    processPacketHelper<List<EntityDTO>>(Network.listOfCharacter);
                    break;
                case "World list":
                    processPacketHelper<List<WorldDTO>>(Network.listOfWorlds);
                    break;
                case "Item list":
                    processPacketHelper<List<ItemExistanceDTOWrapper>>(Network.listOfItems);
                    break;
                case "Plant database":
                    processPacketHelper<List<PlantDTO>>(Network.plantDatabase);
                    break;
                case "Item database":
                    processPacketHelper<List<ItemDTO>>(Network.itemDatabase);
                    break;
                case "World":
                    processPacketHelper<WorldDTO>(Network.worldRetrieved);
                    break;
                case "Area":
                    processPacketHelper<AreaDTO>(Network.areaConfig);
                    break;
                case "Area Indexes":
                    processPacketHelper<List<AreaIndexDTO>>(Network.listOfAreaIndexes);
                    break;
                case "Area Items":
                    processPacketHelper<List<AreaItemDTO>>(Network.listOfAreaItems);
                    break;
                case "Area Plants":
                    processPacketHelper<List<AreaPlantDTO>>(Network.listOfAreaPlants);
                    break;
                case "Area Plant":
                    processPacketHelper<AreaPlantDTO>(Network.areaPlants);
                    break;
                case "Area NPCs":
                    processPacketHelper<List<EntityExistanceDTO>>(Network.listOfAreaNPCs);
                    break;
                case "Item":
                    processPacketHelper<ItemExistanceDTOWrapper>(Network.itemRetrieved);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(data);
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
    }

    private void processPacketHelper<T>(Queue<T> in_queue)
    {
        in_queue.Enqueue(JsonConvert.DeserializeObject<T>(data));
    }
}


public static class doCommands
{
    public static String mainMenu { get { return "Main Menu"; } }
    public static String player { get { return "Player"; } }
    public static String database { get { return "Database"; } }
    public static String preload { get { return "Preload"; } }
    public static String load { get { return "Load"; } }
    public static String action { get { return "Action"; } }
    public static String playerItem { get { return "Player Item"; } }
    public static String entityItem { get { return "Entity Item"; } }
    public static String area { get { return "Area"; } }
}