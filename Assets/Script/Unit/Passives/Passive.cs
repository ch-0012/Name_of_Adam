using System.Collections;
using UnityEngine;
using System;

[Serializable]
public class Passive : MonoBehaviour
{
    [SerializeField] protected PassiveType type;    
    protected bool isUsed = false;

    public PassiveType GetPassiveType()
    {
        return type;
    }

    public virtual void Use(BattleUnit caster, BattleUnit receiver)
    {
        return;
    }
}