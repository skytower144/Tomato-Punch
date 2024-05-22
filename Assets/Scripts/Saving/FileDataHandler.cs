using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "matoPunchOut";
    private readonly string backupExtension = ".bak";

    // Constructor
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public SaveData Load(string profileId, bool allowRollback = true)
    {
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        SaveData loadedData = ProgressManager.instance.save_data;

        if (File.Exists(fullPath))
        {
            try {
                // Load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // Read the file's text into dataToLoad
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // Optionally decrypt the data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize data from Json back into the C# object.
                loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
            }

            catch (Exception exc)
            {
                if (allowRollback) // Prevent infinite recursion when rollback succeeds, but data still fails to load for some reason.
                {
                    Debug.LogWarning($"Error occured when trying to load data from file: {fullPath} \nAttempting to rollback. \n{exc}");
                    bool rollBackSuccess = AttemptRollback(fullPath);

                    if (rollBackSuccess)
                    {
                        loadedData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError($"Error occured when trying to load file at path {fullPath} \nBackup data may be corrput. \n{exc}");
                }
            }
        }

        return loadedData;
    }

    public void Save(SaveData data, string profileId)
    {
        ProgressManager.instance.SavePlayerData(profileId == "Slot_New");

        // Use Path.Combine to account for different OS's having different path separators.
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        try {
            // Create directory which the file will be written to if it doesn't already exist.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            // Serialize the C# game data (StringProgressData) into Json.
            string dataToStore = JsonUtility.ToJson(data, true);

            // Optionally encrypt the data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }

            // Write the serialized data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            //// BACKUP ////
            SaveData verifiedSaveData = Load(profileId);

            // If data is not corrupted, proceed to backup
            if (verifiedSaveData != null)
            {
                File.Copy(fullPath, backupFilePath, true); // true -> overwrite if backupFilePath exists.
            }
            else
            {
                throw new Exception("Save data may be corrupted:\nFailed to create Backup file.");
            }
        }

        catch (Exception exc) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + exc);
        }
    }

    public void Delete(string profileId)
    {
        if (profileId == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

        try
        {
            if (File.Exists(fullPath))
                Directory.Delete(Path.GetDirectoryName(fullPath), true);

            else
                GameManager.DoDebug($"=== Data to delete was not found at path {fullPath} ===");
            
        }
        catch (Exception exc)
        {
            Debug.LogWarning($"Failed to delete profile data: {profileId} at path: {fullPath} \n{exc}");
        }
    }
    
    public Dictionary<string, SaveData> LoadAllProfiles()
    {
        Dictionary<string, SaveData> profileDictionary = new Dictionary<string, SaveData>();

        // Loop over all directory names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;

            // Check if the data file exists. If not, this folder is not a profile -> skip
            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if (!File.Exists(fullPath))
            {
                GameManager.DoDebug($"=== Skipping directory when loading all profiles because it does not contain data: {profileId} ===");
                continue;
            }

            // Load the save data for this profile and put it in the dictionary
            SaveData profileData = Load(profileId);

            // Ensure the profile data is not null
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError($"Tried to load profile but something went wrong. ProfileId: {profileId}");
            }
        }

        return profileDictionary;
    }

    private bool AttemptRollback(string fullPath)
    {
        bool rollBackSuccess = false;
        string backupFilePath = fullPath + backupExtension;

        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                rollBackSuccess = true;
                Debug.LogWarning($"Initiating rollback to backup file at: {backupFilePath}");
            }
            else
            {
                throw new Exception("Backup file was not found.");
            }
        }

        catch (Exception exc)
        {
            Debug.LogError($"Error occured when trying to rollback to backup file at: {backupFilePath} \n{exc}");
        }

        return rollBackSuccess;
    }

    // Implementation of XOR encryption
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }

    public bool CheckFileExists(string profileId)
    {
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
        return File.Exists(fullPath);
    }
}
