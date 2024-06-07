using System.Collections.Generic;
using UnityEngine;

public class InkDB
{
    static Dictionary<string, TextAsset> InkCatalog = new Dictionary<string, TextAsset>();

    public static TextAsset ReturnTextAsset(string sceneName, string npcName, string inkFileName, bool isUniqueID)
    {
        if (string.IsNullOrEmpty(inkFileName))
            return null;
        
        string id;

        if (inkFileName.Contains("/"))
            id = $"Dialogue/eng/" + inkFileName;
        else {
            if (isUniqueID)
                sceneName = "aMobile";

            if (sceneName == "Cutscene")
                id = $"Dialogue/eng/Cutscene/{inkFileName}";
            else
                id = $"Dialogue/eng/{sceneName}/{npcName}/{inkFileName}";
        }
        if (InkCatalog.ContainsKey(id))
            return InkCatalog[id];
        
        //GameManager.DoDebug($"Added\n{id} to Ink Catalog");
        InkCatalog[id] = Resources.Load<TextAsset>($"{id}");
        return InkCatalog[id];
    }

    public static TextAsset ReturnTextAsset(string sceneName, string questId)
    {
        string id = $"Dialogue/eng/{sceneName}/LocationPortal/{questId}";

        if (InkCatalog.ContainsKey(id))
            return InkCatalog[id];
        
        //GameManager.DoDebug($"Added\n{id} to Ink Catalog");
        InkCatalog[id] = Resources.Load<TextAsset>($"{id}");
        return InkCatalog[id];
    }    
}
