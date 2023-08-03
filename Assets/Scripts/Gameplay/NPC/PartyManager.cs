using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public PartyCandidateControl candidateControl;
    [SerializeField] private List<PartyMember> partyMembers;

    public void JoinParty(NPCController npc)
    {
        if (!CanJoinParty(npc.ReturnID()))
            return;

        PartyMember newMember = new PartyMember(npc);
        partyMembers.Add(newMember);
        newMember.follow.EnableFollow();
    }

    public void LeaveParty(string id)
    {
        foreach (PartyMember member in partyMembers) {
            if (member.id == id) {
                member.follow.DisableFollow();
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

    public List<PartyMember> ReturnPartyMembers()
    {
        return partyMembers;
    }

    public void RestorePartyMembers(List<PartyMember> party_members)
    {
        for (int i = partyMembers.Count - 1; i >= 0; i--)
            LeaveParty(partyMembers[i].id);

        partyMembers = party_members;

        foreach (PartyMember member in partyMembers) {
            member.follow = NPCManager.instance.npc_dict[member.id].gameObject.GetComponent<NPCFollow>();
            member.follow.EnableFollow();
        }
        candidateControl.progressAssistant.InitiateRestore();
    }
}

[System.Serializable]
public class PartyMember
{
    public string id;
    [System.NonSerialized] public NPCFollow follow;

    public PartyMember(NPCController npc)
    {
        id = npc.ReturnID();
        follow = npc.GetComponent<NPCFollow>();
    }
}
