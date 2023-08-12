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
    2. #cutmove:target:@x-y@speed@animate

    3. #cutmove:Babycat@2-4@13@false
    4. #cutmove:Babycat@2-4@13@true@wait
    5. #cutmove:Babycat@2-3,4-3,5-5@13@true@wait
    6. #cutmove:player@10-22,11-24,8-21@5@true@wait
    */

    private const string TURN = "cutturn";
    /*
    1. #cutturn:target@DIR-duration@wait
    2. #cutturn:target@DIR-duration
    
    3. #cutturn:player@UP-0.5,LEFT-1.2
    4. #cutturn:StartingPoint_Gob@RIGHT-0.5,DOWN-0.5,LEFT-0.5,UP-0.5@wait
    */

    private const string WAIT = "cutwait";
    /*
    1. #cutwait:duration
    
    2. #cutwait:0.5
    */
    
    private Character character;
    private string[] splitTag, valueArray;
    string currentTag;
    float duration;
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

                    character = GetTargetCharacter(valueArray[0]);
                    character.Play(valueArray[1]);

                    dontWait = valueArray.Length < 3;
                    if (dontWait) break;

                    character.SetIsAnimating(true);

                    if (character.UsesDefaultAnimator()) {
                        AnimationClip playingClip = ReturnAnimationClip(character.UsesDefaultAnimator(), valueArray[1]);
                        Invoke("SetIsAnimatingFalse", playingClip.length);
                    }
                    while (character.IsAnimating()) yield return null;
                    break;
                
                case MOVE:
                    if (TagCountBelow(4)) break;

                    character = GetTargetCharacter(valueArray[0]);
                    string[] posStrings = valueArray[1].Split(',');
                    float moveSpeed = float.Parse(valueArray[2]);

                    bool isAnimate = (valueArray[3].ToLower() == "false") ? false : true;
                    dontWait = valueArray.Length < 5;

                    if (dontWait)
                        StartCoroutine(character.PlayMoveActions(posStrings, moveSpeed, isAnimate));
                    else
                        yield return character.PlayMoveActions(posStrings, moveSpeed, isAnimate);
                    break;
                
                case TURN:
                    if (TagCountBelow(2)) break;

                    character = GetTargetCharacter(valueArray[0]);
                    string[] dirStrings = valueArray[1].Split(',');
                    dontWait = valueArray.Length < 3;

                    if (dontWait)
                        StartCoroutine(PlayTurnActions(character, dirStrings));
                    else
                        yield return PlayTurnActions(character, dirStrings);
                    break;
                
                case WAIT:
                    duration = float.Parse(valueArray[0]);
                    yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(duration));
                    break;
                
                default:
                    break;
            }
        }
        yield break;
    }

    private Character GetTargetCharacter(string name)
    {
        return (name.ToLower() == "player") ? PlayerMovement.instance.GetComponent<Character>() : NPCManager.instance.npc_dict[name].GetComponent<Character>();
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

    private AnimationClip ReturnAnimationClip(Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == clipName)
                return clip;
        }
        Debug.LogError($"Animation clip not found : {clipName}");
        return null;
    }

    private void SetIsAnimatingFalse() // Invoke
    {
        character.SetIsAnimating(false);
    }

    public static void FaceAdjustment(Animator anim, string facing_direction)
    {
        float face_x = 0f;
        float face_y = 0f;

        switch (facing_direction) {
            case "UP":
                face_y = 1f;
                break;
            
            case "RU":
                (face_x, face_y) = (1f, 1f);
                break;
            
            case "RIGHT":
                face_x = 1f;
                break;
            
            case "RD":
                (face_x, face_y) = (1f, -1f);
                break;
            
            case "DOWN":
                face_y = -1f;
                break;
            
            case "LD":
                (face_x, face_y) = (-1f, -1f);
                break;
            
            case "LEFT":
                face_x = -1f;
                break;
            
            case "LU":
                (face_x, face_y) = (-1f, 1f);
                break;
            
            default:
                Debug.LogError($"{facing_direction} : Wrong direction string.");
                break;
        }
        anim.SetFloat("moveX", face_x);
        anim.SetFloat("moveY", face_y);
    }

    IEnumerator PlayTurnActions(Character character, string[] dirStrings)
    {
        string[] dirString;
        
        foreach (string dir_dur in dirStrings) {
            dirString = dir_dur.Split('-');
            character.Turn(dirString[0].ToUpper());

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(float.Parse(dirString[1])));
        }
    }
}
