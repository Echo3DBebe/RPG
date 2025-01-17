﻿using System.Collections.Generic;
using UnityEngine;

public class ArmourHolder : Item
{
    [SerializeField] private int Armour;
    [SerializeField] private int CurrentDurability;
    [SerializeField] private int MaxDurability;

    [SerializeField] private List<ResistenceType> Resistences = new List<ResistenceType>();

    [SerializeField] private ArmourType ArmourType;

    [SerializeField] private List<Power> Enchantments;

    [SerializeField] private SkillType SkillType;

    public override void SpawnItem()
    {
        base.SpawnItem();
    }

    public override void StoreItem()
    {
        if (transform.childCount == 0)
        {
            return;
        }

        Destroy(transform.GetChild(0).gameObject);
    }

    public override void SetStats(ItemStats stats)
    {
        ArmourStats armStats = stats as ArmourStats;

        ArmourType = armStats.ArmourType;
        Armour = armStats.Armour;
        CurrentDurability = armStats.CurrentDurability;
        MaxDurability = armStats.MaxDurability;

        for (int i = 0; i < armStats.Resistences.Count; i++)
        {
            Resistences.Add(armStats.Resistences[i]);
        }

        Enchantments = new List<Power>();

        for (int i = 0; i < armStats.Enchantments.Length; i++)
        {
            Enchantments.Add(armStats.Enchantments[i]);
        }

        SkillType = armStats.SkillType;

        SetEquiped(armStats.IsEquiped);

        base.SetStats(stats);
    }

    public override bool Equals(Item Item)
    {
        ArmourHolder armourH = Item as ArmourHolder;

        if (armourH == null)
        {
            return false;
        }

        if (armourH.GetName() == GetName())
        {
            if (armourH.Armour == Armour)
            {
                if (armourH.Enchantments.Count == Enchantments.Count)
                {
                    for (int i = 0; i < Enchantments.Count; i++)
                    {
                        if (Enchantments[i].PowerType != armourH.Enchantments[i].PowerType || Enchantments[i].PowerID != armourH.Enchantments[i].PowerID)
                        {
                            return false;
                        }
                    }

                    if (armourH.SkillType == SkillType)
                    {
                        if (armourH.GetWeight() == GetWeight())
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public int GetArmour()
    {
        return Armour;
    }

    public int GetCurrentDurability()
    {
        return CurrentDurability;
    }

    public int GetMaxDurability()
    {
        return MaxDurability;
    }

    public int GetResistenceCount()
    {
        return Resistences.Count;
    }

    public DamageTypeEnum GetResistenceType(int id)
    {
        return Resistences[id].Type;
    }

    public byte GetResistence(int typeId)
    {
        return Resistences[typeId].Resistence;
    }

    public ArmourType GetArmourType()
    {
        return ArmourType;
    }

    public int GetEnchantCount()
    {
        return Enchantments.Count;
    }

    public Power GetEnchantment(int id)
    {
        return Enchantments[id];
    }

    public SkillType GetSkillType()
    {
        return SkillType;
    }

    public static ArmourHolder operator--(ArmourHolder armour)
    {
        armour.CurrentDurability--;

        return armour;
    }
}
