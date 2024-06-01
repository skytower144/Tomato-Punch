using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Animations;

/* FORMAT
    #tag:value
    #tag:value
    /cut
*/
public class CutsceneHandler : MonoBehaviour
{
    private const string PLAY = "cutplay";
    /*
        # If using wait, animation should NOT be LOOP
        # If target is a background object, insert '!' before target's name 

        1. #cutplay:target@animstring@wait (once)
        2. #cutplay:target@animstring      (once/loop)

        3. #cutplay:player@Wakeup@wait
        4. #cutplay:BabyCat@escort_cat

        5. #cutplay:!television@animstring@wait
        6. #cutplay:!television@flicker
    */
    private const string MOVE = "cutmove";
    /*
        # REQUIRES UNITY ANIMATOR BLEND TREE
        # MUST be IDLE before moving npc/player

        1. #cutmove:target@x~y@speed@animate@wait
        2. #cutmove:target@x~y@speed@animate
        3. #cutmove:target@_~y@speed@animate
        4. #cutmove:target@x~_@speed@animate

        5. #cutmove:Babycat@2~4@13@false
        6. #cutmove:Babycat@2~4@13@true@wait
        7. #cutmove:Babycat@2~3,4~3,5~5@13@true@wait
        8. #cutmove:player@10~22,11~24,8~21@5@true@wait
    */
    private const string TURN = "cutturn";
    /*
        1. #cutturn:target@DIR-duration@wait
        2. #cutturn:target@DIR-duration
        
        3. #cutturn:player@UP
        4. #cutturn:player@UP@wait
        4. #cutturn:player@UP-0.5,LEFT-1.2
        5. #cutturn:StartingPoint_Gob@RIGHT-0.5,DOWN-0.5,LEFT-0.5,UP-0.5@wait
    */
    private const string IMAGE = "cutimage";
    /*
        1. #cutimage:index@true
        2. #cutimage:index@false
    */
    private const string FADEIN = "cutfadein";
    private const string FADEOUT = "cutfadeout";
    /*
        1. #cutfadein:duration@delay@fps@wait
        2. #cutfadein:duration@delay@fps
        3. #cutfadein:duration@delay
        4. #cutfadein:duration
    */
    private const string SETCAMERA = "cutsetcamera";
    /*
        1. #cutsetcamera:x@y
        2. #cutsetcamera:_
        2. #cutsetcamera:playerX@playerY
        3. #cutsetcamera:reset
    */
    private const string MOVECAMERA = "cutmovecamera";
    /*
        1. #cutmovecamera:x@y@duration
        2. #cutmovecamera:_@_@duration
        3. #cutmovecamera:x@y@duration@easetype
        4. #cutmovecamera:x@y@duration@easetype@wait
        5. #cutmovecamera:x@y@duration@_@wait
    */

    private const string SPAWN = "cutspawn";
    private const string DESTROY = "cutdestroy";
    /*
        1. #cutspawn:index
        2. #cutdestroy:index
    */
    private const string WAIT = "cutwait";
    /*
        1. #cutwait:duration
        2. #cutwait:0.5
    */
    public FadeInOut FadeControl;
    public RectTransform UiCanvasTransform;
    public SpriteRenderer CutSceneHolder;
    private List<Sprite> imageList;
    private List<GameObject> spawnList;
    private List<GameObject> currentSpawns;
    private PlayerMovement playerMov;
    private PlayerCamera playerCamera;
    private Character character;
    private AnimationClip playingClip;
    private string[] splitTag, valueArray;
    private string currentTag, easeType;
    private float duration, delay, posX, posY;
    private int fps, index;
    private bool dontWait;
    private bool dialogueBoxFlag = false;

    void Start()
    {
        playerMov = PlayerMovement.instance;
        playerCamera = playerMov.cameraControl;
    }
    
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

                    dontWait = valueArray.Length < 3;
                    bool isObject = valueArray[0][0] == '!';
                    
                    if (isObject) {
                        Animator objectAnim = null;
                        string objectName = valueArray[0].Remove(0, 1);
                        GameObject[] bgObjects = GameObject.FindGameObjectsWithTag("BGObject");

                        foreach (GameObject bgObject in bgObjects) {
                            if (bgObject.name == objectName) {
                                objectAnim = bgObject.GetComponent<Animator>();
                                objectAnim.Play(valueArray[1]);
                                break;
                            }
                        }
                        if (objectAnim == null) Debug.LogError($"Couldn't find object named : {valueArray[0]}");
                        if (dontWait) {
                            dialogueBoxFlag = true;
                            break;
                        }
                        playingClip = ReturnAnimationClip(objectAnim, valueArray[1]);
                        yield return new WaitForSecondsRealtime(playingClip.length);
                    }
                    else {
                        character = GetTargetCharacter(valueArray[0]);
                        character.Play(valueArray[1]);
                        if (dontWait) {
                            dialogueBoxFlag = true;
                            break;
                        }
                        character.SetIsAnimating(true);

                        if (character.UsesDefaultAnimator()) {
                            playingClip = ReturnAnimationClip(character.UsesDefaultAnimator(), valueArray[1]);
                            Invoke("SetIsAnimatingFalse", playingClip.length);
                        }
                        while (character.IsAnimating()) yield return null;
                    }
                    break;
                
                case MOVE:
                    if (TagCountBelow(4)) break;

                    character = GetTargetCharacter(valueArray[0]);
                    string[] posStrings = valueArray[1].Split(',');
                    float moveSpeed = float.Parse(valueArray[2]);

                    bool isAnimate = (valueArray[3].ToLower() == "false") ? false : true;
                    dontWait = valueArray.Length < 5;

                    if (dontWait) {
                        dialogueBoxFlag = true;
                        StartCoroutine(character.PlayMoveActions(posStrings, moveSpeed, isAnimate));
                    }
                    else
                        yield return character.PlayMoveActions(posStrings, moveSpeed, isAnimate);
                    break;
                
                case TURN:
                    if (TagCountBelow(2)) break;

                    character = GetTargetCharacter(valueArray[0]);
                    string[] dirStrings = valueArray[1].Split(',');
                    dontWait = valueArray.Length < 3;

                    if (dontWait) {
                        dialogueBoxFlag = true;
                        StartCoroutine(PlayTurnActions(character, dirStrings));
                    }
                    else
                        yield return PlayTurnActions(character, dirStrings);
                    break;
                
                case IMAGE:
                    index = int.Parse(valueArray[0]);
                    if (index < imageList.Count) {
                        CutSceneHolder.gameObject.SetActive(valueArray[1] == "true");
                        CutSceneHolder.sprite = imageList[index];
                    }
                    break;

                case FADEIN:
                case FADEOUT:
                    duration = float.Parse(valueArray[0]);
                    delay = valueArray.Length >= 2 ? float.Parse(valueArray[1]) : 0f;
                    fps = valueArray.Length >= 3 ? int.Parse(valueArray[2]) : 60;
                    dontWait = valueArray.Length < 4;

                    if (dontWait)
                        StartCoroutine(FadeControl.Fade(duration, delay, fps, tag_key == FADEIN));
                    else
                        yield return StartCoroutine(FadeControl.Fade(duration, delay, fps, tag_key == FADEIN));
                    break;
                
                case SETCAMERA:
                    if (valueArray[0] == "reset") {
                        playerCamera.ResetPlayerCamera();
                        break;
                    }
                    playerCamera.DetachPlayerCamera();
                    if (valueArray[0] == "_")
                        break;
                    
                    posX = valueArray[0] == "playerX" ? playerMov.GetPlayerPos().x : float.Parse(valueArray[0]);
                    posY = valueArray[1] == "playerY" ? playerMov.GetPlayerPos().y : float.Parse(valueArray[1]);
                    playerCamera.SetCameraPosition(posX, posY);
                    break;
                
                case MOVECAMERA:
                    posX = valueArray[0] == "_" ? playerCamera.transform.position.x : float.Parse(valueArray[0]);
                    posY = valueArray[1] == "_" ? playerCamera.transform.position.y : float.Parse(valueArray[1]);
                    duration = float.Parse(valueArray[2]);
                    easeType = valueArray.Length >= 4 ? valueArray[3] : "_";
                    dontWait = valueArray.Length < 5;

                    playerCamera.MoveCamera(posX, posY, duration, easeType);
                    if (!dontWait) yield return WaitForCache.GetWaitForSecondReal(duration);
                    break;

                case SPAWN:
                    index = int.Parse(valueArray[0]);
                    if (index < spawnList.Count)
                        currentSpawns[index] = Instantiate(spawnList[index], transform);
                    break;
                
                case DESTROY:
                    index = int.Parse(valueArray[0]);
                    if (index < spawnList.Count && currentSpawns[index] != null) {
                        Destroy(currentSpawns[index]);
                        currentSpawns[index] = null;
                    }
                    break;

                case WAIT:
                    duration = float.Parse(valueArray[0]);
                    if (duration == 0) break;
                    yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(duration));
                    break;

                default:
                    break;
            }
        }
        if (dialogueBoxFlag) {
            dialogueBoxFlag = false;
            DialogueManager.instance.SetDialogueBox(true);
        }
        yield break;
    }

    private Character GetTargetCharacter(string name)
    {
        return (name.ToLower() == "player") ? playerMov.GetComponent<Character>() : NPCManager.instance.npc_dict[name].GetComponent<Character>();
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
    private void SetIsAnimatingFalse() // Invoke
    {
        character.SetIsAnimating(false);
    }

    public void FaceAdjustment(Animator anim, string facing_direction)
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
                Debug.LogError($"{anim.gameObject.name} - {facing_direction} : Wrong direction string.");
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
            character.Turn(dirString[0]);

            if (dirString.Length == 1)
                continue;
            
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime(float.Parse(dirString[1])));
        }
    }
    public void SetCutscenePosition()
    {
        transform.position = UiCanvasTransform.position;
    }
    public void InitSpawnList(List<GameObject> spawns)
    {
        spawnList = spawns;
        currentSpawns = new List<GameObject>();

        for (int i = 0; i < spawns.Count; i++)
            currentSpawns.Add(null);
    }
    public void InitImageList(List<Sprite> images)
    {
        imageList = images;
    }
    public static AnimationClip ReturnAnimationClip(Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
            if (clip.name == clipName)
                return clip;
        }
        Debug.LogError($"Animation clip not found : {clipName}");
        return null;
    }
    public static bool DoesAnimationExist(Animator animator, string animName)
    {
        int layerCount = animator.layerCount;

        for (int i = 0; i < layerCount; i++)
        {
            if (animator.HasState(i, Animator.StringToHash(animName)))
                return true;
        }
        return false;
    }
    public static string GetBaseLayerEntryAnimationTag(Animator anim)
    {
        AnimatorController controller = anim.runtimeAnimatorController as AnimatorController;
        return controller.layers[0].stateMachine.defaultState.name;
    }
}
