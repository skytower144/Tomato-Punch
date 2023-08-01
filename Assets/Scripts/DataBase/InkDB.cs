using System.Collections.Generic;
using UnityEngine;

public class InkDB
{
    static Dictionary<string, TextAsset> InkCatalog = new Dictionary<string, TextAsset>();

    public static TextAsset ReturnTextAsset(string language, string sceneName, string npcName, string inkFileName)
    {
        string id = $"Dialogue/{language}/{sceneName}/{npcName}/{inkFileName}";
        if (InkCatalog.ContainsKey(id))
            return InkCatalog[id];
        
        //GameManager.DoDebug($"Added\n{id} to Ink Catalog");
        InkCatalog[id] = Resources.Load<TextAsset>($"{id}");
        return InkCatalog[id];
    }

    public static TextAsset ReturnTextAsset(string language, string sceneName, string questId)
    {
        string id = $"Dialogue/{language}/{sceneName}/LocationPortal/{questId}";

        if (InkCatalog.ContainsKey(id))
            return InkCatalog[id];
        
        //GameManager.DoDebug($"Added\n{id} to Ink Catalog");
        InkCatalog[id] = Resources.Load<TextAsset>($"{id}");
        return InkCatalog[id];
    }    
}
