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

    public static Texture2D ReturnSpriteTexture2D(Sprite sprite)
    {
        var croppedTexture = new Texture2D( (int)sprite.rect.width, (int)sprite.rect.height );

        var pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x, 
            (int)sprite.textureRect.y, 
            (int)sprite.textureRect.width, 
            (int)sprite.textureRect.height
        );
        croppedTexture.SetPixels( pixels );
        croppedTexture.Apply();

        return croppedTexture;
    }
}
