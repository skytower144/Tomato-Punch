using System.Collections.Generic;
using UnityEngine;

public class SpriteDB
{
    static Dictionary<string, Sprite> PortraitCatalog = new Dictionary<string, Sprite>();
    static Dictionary<string, Sprite> BgCatalog = new Dictionary<string, Sprite>();

    public static Sprite ReturnPortrait(string fileName)
    {
        if (PortraitCatalog.ContainsKey(fileName))
            return PortraitCatalog[fileName];
        
        //GameManager.DoDebug($"Added\n{fileName} to Portrait Catalog");
        PortraitCatalog[fileName] = Resources.Load<Sprite>($"Portraits/{fileName}");
        return PortraitCatalog[fileName];
    }

    public static Sprite ReturnBg(string fileName)
    {
        if (BgCatalog.ContainsKey(fileName))
            return BgCatalog[fileName];
        
        //GameManager.DoDebug($"Added\n{fileName} to Background Catalog");
        BgCatalog[fileName] = Resources.Load<Sprite>($"BattleBackground/{fileName}");
        return BgCatalog[fileName];
    }
}
