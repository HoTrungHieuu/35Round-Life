using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public int LevelType;
    public string Class;
    public string Name;
    public string DamageType;
    public float Hp;
    public float Att;
    public float Def;
    public float Speed;
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
    public List<string> Skill;
    public List<int> PassiveActiveTime;
    public int Money;
    public bool isDiscount;
}
