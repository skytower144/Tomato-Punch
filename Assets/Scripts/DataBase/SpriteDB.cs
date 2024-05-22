using System.Collections.Generic;
using UnityEngine;

public class SpriteDB
{
    static Dictionary<string, Sprite> spriteCatalog = new Dictionary<string, Sprite>();

    public static Sprite ReturnPortrait(string fileName)
    {
        if (spriteCatalog.ContainsKey(fileName))
            return spriteCatalog[fileName];
        
        //GameManager.DoDebug($"Added\n{fileName} to Portrait Catalog");
        spriteCatalog[fileName] = Resources.Load<Sprite>($"Sprites/Portraits/{fileName}");
        return spriteCatalog[fileName];
    }
    public static Sprite ReturnCutsceneSprite(string fileName)
    {
        if (spriteCatalog.ContainsKey(fileName))
            return spriteCatalog[fileName];
        
        spriteCatalog[fileName] = Resources.Load<Sprite>($"Sprites/Cutscenes/{fileName}");
        return spriteCatalog[fileName];
    }
}
