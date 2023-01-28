using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FormationLeaderSelectionStrategy
{
    RANDOM,
    THE_HEALTHY,
    THE_WEAK,
}

public class Formation : MonoBehaviour
{
    public bool keep { get; private set; }
    private FormationMember m_Leader;
    private FormationMember[] m_Members;
    private Transform[] m_Slots;
    [SerializeField] private FormationLeaderSelectionStrategy leaderSelectionStrategy;

    private bool initDone = false;
    
    void Start()
    {
        if (initDone)
            return;

        int childCount = transform.childCount;
        m_Slots = new Transform[childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            m_Slots[i] = transform.GetChild(i);
        }
        m_Members = new FormationMember[m_Slots.Length];
        keep = true;
        initDone = true;
        
        RegisterMember(GetComponentInParent<FormationMember>());
    }

    public void RegisterMember(FormationMember member)
    {
        if (!initDone)
            Start();

        if (MemberIndex(member) != -1)
        {
            return;
        }

        bool successful = false;
        for (int i = 0; i < m_Slots.Length; ++i)
        {
            if (m_Members[i] == null)
            {
                m_Members[i] = member;
                successful = true;
                member.OnRegister(this, m_Slots[i]);
                break;
            }
        }
        
        if (successful && member.isLeader)
            ChangeLeader(member);
    }

    public void RemoveMember(FormationMember member)
    {
        if (member == m_Leader)
            m_Leader = null;
        m_Members[MemberIndex(member)] = null;
        member.OnRemove();
    }

    private void ChangeLeader(FormationMember newLeader)
    {
        transform.parent = newLeader.transform;
        transform.localPosition = Vector3.zero;
        if (m_Leader != null)
        {
            m_Leader.isLeader = false;
        }

        m_Leader = newLeader;
        m_Leader.isLeader = true;
        
        SwapSlots(0, MemberIndex(m_Leader));

        m_Leader.OnRegister(this, m_Slots[0]);
        for (int i = 1; i < m_Slots.Length; ++i)
        {
            FormationMember member = m_Members[i];
            if (member == null)
                continue;
            member.OnRegister(this, m_Slots[i]);
        }
    }

    private void OnDestroy()
    {
        try
        {
            RemoveMember(m_Leader);
            FormationMember newLeader = SelectNewLeader();
            if (newLeader == null)
            {
                ClearFormation();
            }
            else
            {
                ChangeLeader(newLeader);
            }
        }
        catch (Exception e)
        {
        }
    }

    private void ClearFormation()
    {
        foreach (var member in m_Members)
        {
            member.OnRemove();
        }
    }

    private void SwapSlots(int i1, int i2)
    {
        if (i1 == i2)
            return;
        (m_Members[i1], m_Members[i2]) = (m_Members[i2], m_Members[i1]);
    }

    private int MemberIndex(FormationMember member)
    {
        for (int i = 0; i < m_Members.Length; ++i)
        {
            if (m_Members[i] == member)
            {
                return i;
            }
        }

        return -1;
    }

    private FormationMember SelectNewLeader()
    {
        switch (leaderSelectionStrategy)
        {
            case FormationLeaderSelectionStrategy.RANDOM:
                int i = Random.Range(0, m_Members.Length);
                FormationMember newLeader = m_Members[i];
                if (newLeader == m_Leader)
                    newLeader = m_Members[(i + 1) % m_Members.Length];
                return newLeader;
            case FormationLeaderSelectionStrategy.THE_HEALTHY:
                FormationMember healthyMember = null;
                foreach (var member in m_Members)
                {
                    if (member == m_Leader)
                        continue;
                    if (healthyMember == null)
                        healthyMember = member;
                    Func<FormationMember, int> getHealth =
                        x => x.thisShip.GetComponent<ShipDamageModel>().Health;
                    if (getHealth(member) > getHealth(healthyMember))
                        healthyMember = member;
                }

                return healthyMember;
            case FormationLeaderSelectionStrategy.THE_WEAK:
                FormationMember weakMember = null;
                foreach (var member in m_Members)
                {
                    if (member == m_Leader)
                        continue;
                    if (weakMember == null)
                        weakMember = member;
                    Func<FormationMember, int> getHealth =
                        x => x.thisShip.GetComponent<ShipDamageModel>().Health;
                    if (getHealth(member) < getHealth(weakMember))
                        weakMember = member;
                }

                return weakMember;
        }

        return null;
    }
}