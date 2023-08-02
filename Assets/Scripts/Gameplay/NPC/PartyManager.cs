using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] List<PartyMember> partyMembers;

    public void JoinParty(NPCController member)
    {
        if (!CanJoinParty(member.ReturnID()))
            return;

        PartyMember newMember = new PartyMember(member.ReturnID(), member.gameObject.GetComponent<NPCFollow>());
        member.transform.SetParent(transform);
        partyMembers.Add(newMember);
        newMember.member.EnableFollow();
    }

    public void LeaveParty(string id)
    {
        foreach (PartyMember member in partyMembers) {
            if (member.id == id) {
                member.member.DisableFollow();
                partyMembers.Remove(member);
                return;
            }
        }
    }

    private bool CanJoinParty(string id)
    {
        foreach (PartyMember member in partyMembers) {
            if (member.id == id) {
                Debug.LogError($"Detected trying to add an exisiting party member : {member.id}");
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public class PartyMember
{
    public string id;
    public NPCFollow member;
    public SceneName currentScene;
    private Vector2 position;

    public PartyMember(string id, NPCFollow member)
    {
        this.id = id;
        this.member = member;
        this.position = member.transform.position;
        currentScene = SceneControl.instance.GetSceneNameByPos(this.position);
    }
}
