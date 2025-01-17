using UnityEngine;

public class Spell : MonoBehaviour
{
    [SerializeField] private string Name;

    [SerializeField] private SpellType SpellType;
    
    [SerializeField] private CastType CastType;

    [SerializeField] private AttributesEnum CostType;

    [SerializeField] private SkillType SkillType;

    [SerializeField] private int Cost;

    [SerializeField] private GameObject SpellAffect;

    [SerializeField] private float CastsPerSecond;

    [SerializeField] protected float NextAttack;


    public int CalculateManaCost(int intelligence)
    {
        float tempCost = GetCost();

        tempCost *= 1 + ((intelligence * GlobalValues.SPDamIntInterval) * GlobalValues.SPDamPerInt);

        return (int)tempCost;
    }

    public virtual void SetStats(SpellStats stats)
    {
        Name = stats.Name;
        name = Name;

        SpellType = stats.SpellType;
        CastType = stats.CastType;
        CostType = stats.CostType;
        SkillType = stats.SkillType;

        SpellAffect = stats.SpellAffect;

        Cost = stats.ManaCost;

        CastsPerSecond = stats.CastRate;
    }

    public virtual void Cast(Hand hand)
    {

    }

    public virtual bool Equals(Spell spell)
    {
        if (SpellType == spell.SpellType && 
            CastType == spell.CastType &&
            CostType == spell.CostType &&
            Cost == spell.Cost &&
            SpellAffect == spell.SpellAffect &&
            CastsPerSecond == spell.CastsPerSecond &&
            SkillType == spell.SkillType)
        {
            return true;
        }

        return false;
    }


    public string GetName()
    {
        return Name;
    }

    public SpellType GetSpellType()
    {
        return SpellType;
    }

    public CastType GetCastType()
    {
        return CastType;
    }

    public AttributesEnum GetCostType()
    {
        return CostType;
    }

    public SkillType GetSkillType()
    {
        return SkillType;
    }

    public int GetCost()
    {
        return Cost;
    }

    public GameObject GetSpellAffect()
    {
        return SpellAffect;
    }

    public float GetCastRate()
    {
        return CastsPerSecond;
    }

    public float GetNextAttack()
    {
        return NextAttack;
    }


    public void SetNextAttack(float time)
    {
        NextAttack = time;
    }
}
