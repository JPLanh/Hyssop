using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/**
 * 
 *  Local configurations
 * 
 *  Read files on the local hard drive, primarily designated for local single player
 */
public static class DataUtility
{

    #region variables
    static string dir = "Json/";
    static string areaDir = "Area/";
    static string playerDir = "Player/";
    static string databaseDir = "Database/";

    static string extension = ".dat";
    static string itemJson = "Items";
    static string npcJson = "NPC";
    static string plantJson = "Plants";
    static string localJson = "local";
    static string storageJson = "Storage";

    //    public static bool secureData = true;
    private static string const_pass = "Unicorn Theoriest";
    #endregion

    #region generic
    public static localConfig instantiateLocal()
    {
        string directory = dir;
        string fileName = localJson;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(directory + "/" + localJson + extension))
        {
            localConfig in_local = new localConfig();
            var encryption = SimpleAESEncryption.Encrypt(const_pass, const_pass);
            in_local.IV = encryption.IV;
            in_local.time = ((float)(8 * 60) / 1440);
            in_local.timeStop = false;
            in_local.isLoaded = true;
            File.WriteAllText(directory + localJson + extension, JsonUtility.ToJson(in_local));
            DataCache.localConfig = in_local;
            return in_local;
        }
        else
        {
            string getJson = File.ReadAllText(directory + localJson + extension);
            DataCache.localConfig = JsonUtility.FromJson<localConfig>(getJson);
        }
        return null;
    }

    private static void writeData(string in_data, string in_path, string in_file, bool in_secure)
    {
        if (!Directory.Exists(in_path))
        {
            Directory.CreateDirectory(in_path);
        }
        if (in_secure)
        {
            var encryption = SimpleAESEncryption.Encrypt(in_data, DataCache.localConfig.IV, const_pass);
            File.WriteAllText(in_path + in_file + extension, encryption.EncryptedText);
        }
        else
        {
            File.WriteAllText(in_path + in_file + extension, in_data);
        }
    }

    private static string readData(string in_path, string in_file, bool in_secure)
    {
        if (!Directory.Exists(in_path))
        {
            Directory.CreateDirectory(in_path);
        }
        if (File.Exists(in_path + in_file + extension))
        {
            string getJson = File.ReadAllText(in_path + in_file + extension);
            if (in_secure)
            {
                Debug.Log(DataCache.localConfig.IV + " , " + const_pass);
                return SimpleAESEncryption.Decrypt(getJson, DataCache.localConfig.IV, const_pass);
            }
            else
            {
                return getJson;
            }
        }
        return null;

    }

    #endregion

    #region Area
    public static void saveArea(int in_length, int in_width, int in_height, AreaIndex[,,] in_grid, string in_name, bool is_buildable)
    {
        String directory = dir + areaDir + in_name + "/";
        string fileName = "Main";
        string plainText = JsonUtility.ToJson(new gridIndexDaoWrapper(in_length, in_width, in_height, in_grid, is_buildable), false);
        writeData(plainText, directory, fileName, false);
    }

    public static gridIndexDaoWrapper loadArea(string in_name)
    {
        string directory = dir + areaDir + in_name + "/";
        string file = "Main";
        return JsonUtility.FromJson<gridIndexDaoWrapper>(readData(directory, file, false));
    }
    public static bool areaDataExists()
    {
        string directory = dir + areaDir;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            return false;
        }
        return true;
    }

    public static bool areaExists(string in_area)
    {
        string directory = dir + areaDir + in_area;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            return false;
        }
        return true;
    }
    #endregion

    #region database Items
    public static void saveDatabaseItems(ItemDAOWrapper listOfItems)
    {
        string directory = dir + databaseDir;
        string fileName = itemJson;

        string plainText = JsonUtility.ToJson(listOfItems);
        writeData(plainText, directory, fileName, false);
    }

    public static ItemDAOWrapper loadDatabaseItems()
    {
        string directory = dir + databaseDir;
        string fileName = itemJson;

        string getJson = readData(directory, fileName, false);
        return JsonUtility.FromJson<ItemDAOWrapper>(getJson);
    }
    #endregion

    #region database plant
    public static void saveDatabasePlants(PlantDAOWrapper in_plants)
    {
        string directory = dir + databaseDir;
        string fileName = plantJson;

        string getJson = JsonUtility.ToJson(in_plants);
        writeData(getJson, directory, fileName, false);
    }

    public static PlantDAOWrapper loadDatabasePlants()
    {
        string directory = dir + databaseDir;
        string fileName = plantJson;

        return JsonUtility.FromJson<PlantDAOWrapper>(readData(directory, fileName, false));
    }
    #endregion

    #region player entity
    public static void saveEntityPlayer(Entity in_entity)
    {
        string directory = dir + playerDir;
        string fileName = in_entity.entityName;

        string getJson = JsonUtility.ToJson(in_entity);
        writeData(getJson, directory, fileName, false);
    }

    public static Entity loadEntityPlayer(string in_name)
    {
        string directory = dir + playerDir;
        string fileName = in_name;

        return JsonUtility.FromJson<Entity>(readData(directory, fileName, false));
    }
    public static void loadAllCharacter()
    {
        Dictionary<string, EntityWrapper> listOfWrapper = new Dictionary<string, EntityWrapper>();
        string rootDirectory = dir + playerDir;
        DataCache.listOfCharacters = new List<Entity>();
        if (Directory.Exists(rootDirectory))
        {
            foreach (FileInfo file in new DirectoryInfo(rootDirectory).GetFiles())
            {
                string directory = file.Directory.FullName + "\\";
                string fileName = file.Name.Replace(extension, "");
                DataCache.listOfCharacters.Add(JsonUtility.FromJson<Entity>(readData(directory, fileName, false)));
            }
        }
    }

    public static void deleteCharacter(string in_name)
    {
        string rootDirectory = dir + playerDir;
        if (Directory.Exists(rootDirectory))
        {
            File.Delete(rootDirectory + in_name + extension);
        }
    }
    #endregion

    #region Items
    public static void saveAreaItem(string in_name, ItemListWrapper in_wrapper)
    {
        string directory = dir + areaDir + in_name + "/";
        string fileName = itemJson;

        string getJson = JsonUtility.ToJson(in_wrapper);

        writeData(getJson, directory, fileName, false);
    }
    public static void loadAllAreaItem()
    {
        Dictionary<string, listOfAreaItems> listOfWrapper = new Dictionary<string, listOfAreaItems>();
        string rootDirectory = dir + areaDir;
        foreach (DirectoryInfo folder in new DirectoryInfo(rootDirectory).GetDirectories())
        {
            string directory = folder.FullName + "\\";
            string fileName = itemJson;

            string getJson = readData(directory, fileName, false);

            if (getJson != null)
                DataCache.inPlayAreaItem.Add(folder.Name, JsonUtility.FromJson<listOfAreaItems>(getJson).listOfItems);
        }
    }
    #endregion

    #region storages
    public static void loadAllStorages()
    {
        Dictionary<string, StorageWrapper> listOfWrapper = new Dictionary<string, StorageWrapper>();
        string rootDirectory = dir + areaDir;
        foreach (DirectoryInfo folder in new DirectoryInfo(rootDirectory).GetDirectories())
        {
            string directory = folder.FullName + "\\";
            string fileName = storageJson;
            string getJson = readData(directory, fileName, false);
            if (getJson != null)
                DataCache.inPlayStorages.Add(folder.Name, JsonUtility.FromJson<StorageWrapper>(getJson).listOfStorage);
        }
    }

    public static void saveAllStorage()
    {
        foreach (KeyValuePair<string, List<Storage>> it_list in DataCache.inPlayStorages)
        {
            string directory = dir + areaDir + it_list.Key + "/";
            string fileName = storageJson;
            StorageWrapper temp_wrapper = new StorageWrapper();
            temp_wrapper.listOfStorage = it_list.Value;

            string getJson = JsonUtility.ToJson(temp_wrapper);
            writeData(getJson, directory, fileName, false);
        }
    }
    #endregion

    #region Plants
    public static void loadAllEntityPlants()
    {
        Dictionary<string, PlantDAOWrapper> listOfWrapper = new Dictionary<string, PlantDAOWrapper>();
        string rootDirectory = dir + areaDir;
        foreach (DirectoryInfo folder in new DirectoryInfo(rootDirectory).GetDirectories())
        {
            string directory = folder.FullName;
            string fileName = itemJson;

            string getJson = readData(directory, fileName, false);
            if (getJson != null)
                DataCache.inPlayPlants.Add(folder.Name, JsonUtility.FromJson<PlantDAOWrapper>(getJson).listOfPlants);
        }
    }

    public static void saveAllEntityPlants()
    {
        foreach (KeyValuePair<string, List<Plant>> it_list in DataCache.inPlayPlants)
        {
            string directory = dir + areaDir + it_list.Key + "/";
            string fileName = plantJson;
            PlantDAOWrapper temp_wrapper = new PlantDAOWrapper();
            temp_wrapper.listOfPlants = it_list.Value;

            string getJson = JsonUtility.ToJson(temp_wrapper);
            writeData(getJson, directory, fileName, false);
        }
    }
    #endregion

    #region NPC
    public static void loadAllNPC()
    {

        Dictionary<string, EntityWrapper> listOfWrapper = new Dictionary<string, EntityWrapper>();
        string rootDirectory = dir + areaDir;
        foreach (DirectoryInfo folder in new DirectoryInfo(rootDirectory).GetDirectories())
        {
            string directory = folder.FullName + "\\";
            string fileName = npcJson;

            string getJson = readData(directory, fileName, false);

            if (getJson != null)
            {
                EntityWrapper temp_wrapper = JsonUtility.FromJson<EntityWrapper>(getJson);
                DataCache.inPlayNPC.Add(folder.Name, temp_wrapper.listOfNPC);
            }
        }

    }

    public static void saveAllNPC()
    {
        foreach (KeyValuePair<string, List<Entity>> it_list in DataCache.inPlayNPC)
        {
            string directory = dir + areaDir + it_list.Key + "/";
            string fileName = npcJson;


            EntityWrapper temp_wrapper = new EntityWrapper();
            foreach (Entity it_npc in it_list.Value)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                temp_wrapper.listOfNPC.Add(it_npc);
            }

            string getJson = JsonUtility.ToJson(temp_wrapper);
            writeData(getJson, directory, fileName, false);
        }
    }
    #endregion

    #region local
    public static void saveCalendar(localConfig in_calendar)
    {
        string directory = dir;
        string fileName = localJson;

        string getJson = JsonUtility.ToJson(in_calendar);
        writeData(getJson, directory, fileName, false);
    }

    public static localConfig loadCalendar()
    {
        string directory = dir;
        string fileName = localJson;


        string getJson = readData(directory, fileName, false);
        return JsonUtility.FromJson<localConfig>(getJson);

    }
    #endregion
}

// ReSharper disable MemberCanBePrivate.Global

public static class SimpleAESEncryption
{
    /// <summary>
    /// A class containing AES-encrypted text, plus the IV value required to decrypt it (with the correct password)
    /// </summary>
    public struct AESEncryptedText
    {
        public string IV;
        public string EncryptedText;
    }

    /// <summary>
    /// Encrypts a given text string with a password
    /// </summary>
    /// <param name="plainText">The text to encrypt</param>
    /// <param name="password">The password which will be required to decrypt it</param>
    /// <returns>An AESEncryptedText object containing the encrypted string and the IV value required to decrypt it.</returns>
    public static AESEncryptedText Encrypt(string plainText, string password)
    {
        using (var aes = Aes.Create())
        {
            aes.GenerateIV();
            aes.Key = ConvertToKeyBytes(aes, password);

            var textBytes = Encoding.UTF8.GetBytes(plainText);

            var aesEncryptor = aes.CreateEncryptor();
            var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);


            return new AESEncryptedText
            {

                IV = Convert.ToBase64String(aes.IV),
                EncryptedText = Convert.ToBase64String(encryptedBytes)
            };
        }
    }

    //This is poor practice to use the same IV key but oh well for simplicit sake
    public static AESEncryptedText Encrypt(string plainText, string IV, string password)
    {
        using (var aes = Aes.Create())
        {
            aes.IV = Convert.FromBase64String(IV);
            aes.Key = ConvertToKeyBytes(aes, password);

            var textBytes = Encoding.UTF8.GetBytes(plainText);

            var aesEncryptor = aes.CreateEncryptor();
            var encryptedBytes = aesEncryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);


            return new AESEncryptedText
            {

                IV = Convert.ToBase64String(aes.IV),
                EncryptedText = Convert.ToBase64String(encryptedBytes)
            };
        }
    }
    /// <summary>
    /// Decrypts an AESEncryptedText with a password
    /// </summary>
    /// <param name="encryptedText">The AESEncryptedText object to decrypt</param>
    /// <param name="password">The password to use when decrypting</param>
    /// <returns>The original plainText string.</returns>
    public static string Decrypt(AESEncryptedText encryptedText, string password)
    {
        return Decrypt(encryptedText.EncryptedText, encryptedText.IV, password);
    }

    /// <summary>
    /// Decrypts an encrypted string with an IV value password
    /// </summary>
    /// <param name="encryptedText">The encrypted string to be decrypted</param>
    /// <param name="iv">The IV value which was generated when the text was encrypted</param>
    /// <param name="password">The password to use when decrypting</param>
    /// <returns>The original plainText string.</returns>
    public static string Decrypt(string encryptedText, string iv, string password)
    {
        using (Aes aes = Aes.Create())
        {
            var ivBytes = Convert.FromBase64String(iv);
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);

            var decryptor = aes.CreateDecryptor(ConvertToKeyBytes(aes, password), ivBytes);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedTextBytes, 0, encryptedTextBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

    // Ensure the AES key byte-array is the right size - AES will reject it otherwise
    private static byte[] ConvertToKeyBytes(SymmetricAlgorithm algorithm, string password)
    {
        algorithm.GenerateKey();

        var keyBytes = Encoding.UTF8.GetBytes(password);
        var validKeySize = algorithm.Key.Length;

        if (keyBytes.Length != validKeySize)
        {
            var newKeyBytes = new byte[validKeySize];
            Array.Copy(keyBytes, newKeyBytes, Math.Min(keyBytes.Length, newKeyBytes.Length));
            keyBytes = newKeyBytes;
        }

        return keyBytes;
    }
}
