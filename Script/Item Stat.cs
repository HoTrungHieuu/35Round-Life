using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStat : MonoBehaviour
{
    public int LevelType;
    public string Type;
    public string Name;
    public float Hp;
    public float Att;
    public float Def;
    public float CritRate;
    public float CritDamage;
    public float Accuracy;
    public float Evasion;
    public float PhysicalResistance;
    public float MagicalResistance;
    public float Penetration;
    public float LifeSteal;
    public float DamageReflect;
    public float HealingAddition;
    public float DamageAddition;
    public float DamageReducion;
    public float EnergyAddition;
    public List<string> passiveSkill;
    public List<int> PassiveActiveTime;
    public int Money;
    public bool isDiscount;
}
