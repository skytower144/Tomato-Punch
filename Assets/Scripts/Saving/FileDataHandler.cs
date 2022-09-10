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

    // Constructor
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public StringProgressData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        StringProgressData loadedData = ProgressManager.instance.progress_dict;

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
                loadedData = JsonUtility.FromJson<StringProgressData>(dataToLoad);
            }

            catch (Exception exc) {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + exc);
            }
        }

        return loadedData;
    }

    public void Save(StringProgressData data)
    {
        // Use Path.Combine to account for different OS's having different path separators.
        string fullPath = Path.Combine(dataDirPath, dataFileName);

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
        }

        catch (Exception exc) {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + exc);
        }
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
}
