using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    private const string PLAY = "cutplay";
    /*
    0. If using wait, animation should NOT be LOOP

    1. #cutplay:target@animstring@wait (once)
    2. #cutplay:target@animstring      (once/loop)

    3. #cutplay:BabyCat@ztest2@wait
    4. #cutplay:BabyCat@escort_cat
    */

    private const string MOVE = "cutmove";
    /*
    0. REQUIRES UNITY ANIMATOR BLEND TREE

    1. #cutmove:target:@x-y@speed@animate@wait

    2. #cutmove:Babycat@2-4@13@false
    3. #cutmove:Babycat@2-4@13@true@wait
    4. #cutmove:Babycat@2-3,4-3,5-5@13@true@wait
    5. #cutmove:player@10-22,11-24,8-21@5@true@wait
    */

    private const string WAIT = "cutwait";
    /*
    1. #cutwait:duration
    */
    
    [System.NonSerialized] public NPCController targetNpc;
    private string[] splitTag, valueArray, posStrings;
    private Vector2[] posArray;
    string currentTag;
    bool dontWait;
    
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

                    dontWait = valueArray.Length < 3;
                    if (dontWait) break;

                    targetNpc.SetIsAnimating(true);
                    while (targetNpc.IsAnimating) yield return null;
                    break;
                
                case MOVE:
                    if (TagCountBelow(4)) break;

                    bool isPlayer = false;

                    if ((valueArray[0].ToLower() == "player"))
                        isPlayer = true;
                    else
                        targetNpc = NPCManager.instance.npc_dict[valueArray[0]];

                    posStrings = valueArray[1].Split(',');
                    float moveSpeed = float.Parse(valueArray[2]);

                    bool isAnimate = (valueArray[3].ToLower() == "false") ? false : true;
                    dontWait = valueArray.Length < 5;
                    
                    if (isPlayer)
                    {
                        if (dontWait)
                            StartCoroutine(PlayerMovement.instance.PlayMoveActions(posStrings, moveSpeed, isAnimate));
                        else
                            yield return PlayerMovement.instance.PlayMoveActions(posStrings, moveSpeed, isAnimate);
                    }
                    else
                    {
                        if (dontWait)
                            StartCoroutine(targetNpc.npcMove.PlayMoveActions(targetNpc, posStrings, moveSpeed, isAnimate));
                        else
                            yield return targetNpc.npcMove.PlayMoveActions(targetNpc, posStrings, moveSpeed, isAnimate);
                    }
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
