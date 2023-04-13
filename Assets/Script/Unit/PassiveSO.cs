using UnityEditor;
using UnityEngine;

public enum FigureType
{
    Percent,
    Integer,
}

[CreateAssetMenu(fileName = "Passive", menuName = "Scriptable Object/Passive")]
public class PassiveSO : ScriptableObject
{
    //public PassiveType PassiveType = PassiveType.AFTERATTACK;
    //public bool IsPercent = false;
    //public bool IsNegative = false;
    //public bool IsTargetSelf = false;
    //// Memo : 자기자신, 피격된 적, 랜덤 등으로 세분화가 필요해보임
    //public Stat TargetStat;
    //[TextArea] public string Description;

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