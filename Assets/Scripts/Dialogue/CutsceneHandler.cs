using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    private const string PLAY = "cutplay";
    /*
    1. #cutplay:target@anim@wait (once)
    2. #cutplay:target@anim      (once/loop)
    3. If using wait, animation should NOT be loop
    */

    private const string MOVE = "cutmove";
    /*
    1. #cutmove:target:@x-y@speed@wait
    2. #cutmove:Babycat@(2-3,4-3,5-5)@13@wait
    */

    private const string WAIT = "cutwait";
    /*
    1. #cutwait:duration
    */
    [System.NonSerialized] public NPCController targetNpc;
    private string[] splitTag, valueArray;
    string currentTag;
    
    public IEnumerator HandleCutsceneTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            currentTag = tag;
            splitTag = tag.Split(':');
            
            if (TagCountBelow(2, true)) continue;
            
            string tag_key = splitTag[0].Trim();
            valueArray = splitTag[1].Trim().Split('@');
            
            switch (tag_key)
            {
                case PLAY:
                    if (TagCountBelow(2)) break;

                    targetNpc = NPCManager.instance.npc_dict[valueArray[0]];
                    targetNpc.Play(valueArray[1]);

                    if (valueArray.Length < 3) break;

                    targetNpc.SetIsAnimating(true);
                    while (targetNpc.IsAnimating) yield return null;
                    break;
                
                case MOVE: // #cutmove:Babycat@2-4@13@wait
                    if (TagCountBelow(2)) break;
                    
                    targetNpc = NPCManager.instance.npc_dict[valueArray[0]];
                    
                    break;
                
                case WAIT:
                    break;
                
                default:
                    break;
            }
        }
        yield break;
    }

    public void AfterAnimComplete(NPCController npc, float delay)
    {
        StartCoroutine(AfterAnimExecute(npc, delay));
    }

    IEnumerator AfterAnimExecute(NPCController npc, float delay)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));
        npc.SetIsAnimating(false);
    }

    private bool TagCountBelow(int minTagCount, bool isSplitTag = false)
    {
        string[] targetArray = isSplitTag ? splitTag : valueArray;

        if (targetArray.Length < minTagCount) {
            Debug.LogError($"Error occured parsing : {currentTag}");
            return true;
        }
        return false;
    }
}
