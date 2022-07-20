using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface RebindChangeUI
{
   void RebindUI_text(string text, string oldPath, string newPath);

   void RebindUI_sprite(Sprite sprite, string oldPath, string newPath);
}
