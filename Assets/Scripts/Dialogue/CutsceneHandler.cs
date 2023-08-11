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

    3. #cutplay:player@Wakeup@wait
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
    private Character character;
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

                    character = (valueArray[0].ToLower() == "player") ? PlayerMovement.instance.GetComponent<Character>() : NPCManager.instance.npc_dict[valueArray[0]].GetComponent<Character>();
                    character.Play(valueArray[1]);

                    dontWait = valueArray.Length < 3;
                    if (dontWait) break;

                    character.SetIsAnimating(true);
                    while (character.IsAnimating()) yield return null;
                    break;
                
                case MOVE:
                    if (TagCountBelow(4)) break;

                    character = (valueArray[0].ToLower() == "player") ? PlayerMovement.instance.GetComponent<Character>() : NPCManager.instance.npc_dict[valueArray[0]].GetComponent<Character>();

                    posStrings = valueArray[1].Split(',');
                    float moveSpeed = float.Parse(valueArray[2]);

                    bool isAnimate = (valueArray[3].ToLower() == "false") ? false : true;
                    dontWait = valueArray.Length < 5;

                    if (dontWait)
                        StartCoroutine(character.PlayMoveActions(posStrings, moveSpeed, isAnimate));
                    else
                        yield return character.PlayMoveActions(posStrings, moveSpeed, isAnimate);
                    break;
                
                case WAIT:
                    break;
                
                default:
                    break;
            }
        }
        yield break;
    }

    public void AfterAnimComplete(Object targetCharacter, float delay)
    {
        StartCoroutine(AfterAnimExecute((Character)targetCharacter, delay));
    }

    IEnumerator AfterAnimExecute(Character targetCharacter, float delay)
    {
        yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(delay));
        targetCharacter.SetIsAnimating(false);
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
