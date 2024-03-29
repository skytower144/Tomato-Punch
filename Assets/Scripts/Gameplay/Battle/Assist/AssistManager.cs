using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistManager : MonoBehaviour
{
    public AssistType mato, ruby, lazer;
    [System.NonSerialized] public float assistDamage;
    public bool isBlast { get; private set; }

    public string DecideSkill(int characterType, int currentFeather)
    {
        AssistType assistCharacter =
            characterType == 0 ? mato :
            ((characterType == 1) && AssistAvailable(ruby)) ? ruby :
            ((characterType == 2) && AssistAvailable(lazer))  ? lazer :
            mato;
        
        // Calculate Feather Amount, Use max possible feathers
        for (int i = currentFeather; i >= 1; i--) {
            if (assistCharacter.featherUsageOrder[i - 1] != null) {
                GameManager.gm_instance.battle_system.featherPointManager.SubtractFeatherPoint(i);
                assistDamage = assistCharacter.featherUsageOrder[i - 1].skillDamage;

                if (!ProhibitBlast())
                    isBlast = i >= 3;
                
                return assistCharacter.featherUsageOrder[i - 1].animString;
            }
        }
        return "";
    }

    public bool AssistAvailable(AssistType assistCharacter)
    {
        // Check if player has enabled any assist for this character.
        for (int i = 0; i < 5; i++) {
            if (assistCharacter.featherUsageOrder[i] != null)
                return true;
        }
        return false;
    }

    public void SetIsBlast(bool state)
    {
        if (isBlast != state) isBlast = state;
    }
    
    private bool ProhibitBlast()
    {
        return GameManager.gm_instance.battle_system.IsGangfight;
    }
}

[System.Serializable]
public class AssistType
{
    public List<AssistSkill> featherUsageOrder;
}
