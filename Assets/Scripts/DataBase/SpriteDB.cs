using System.Collections.Generic;
using UnityEngine;

public class SpriteDB
{
    static Dictionary<string, Sprite> PortraitCatalog = new Dictionary<string, Sprite>();

    public static Sprite ReturnPortrait(string fileName)
    {
        if (PortraitCatalog.ContainsKey(fileName))
            return PortraitCatalog[fileName];
        
        //GameManager.DoDebug($"Added\n{fileName} to Portrait Catalog");
        PortraitCatalog[fileName] = Resources.Load<Sprite>($"Portraits/{fileName}");
        return PortraitCatalog[fileName];
    }
}
