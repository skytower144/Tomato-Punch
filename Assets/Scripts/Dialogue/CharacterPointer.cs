using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPointer : MonoBehaviour
{
    [Header("[ Animator Same Hierarchy : NPCController, PlayerMovement ]")]
    [SerializeField]
    [ProgressInterface(typeof(Character))]
    public Object targetCharacter;
}
