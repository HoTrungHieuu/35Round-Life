using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatForGameplay
{
    public int team { get; set; }
    public int position { get; set; }
    public string name { get; set; }
    public string classType { get; set; }
    public string damageType { get; set; }
    public float HealPoint { get; set; }
    public float ShieldPoint { get; set; }
    public float EnergyPoint { get; set; }
    public float Attack { get; set; }
    public float Deffend { get; set; }
    public float Speed { get; set; }
    public float CritRate { get; set; }
    public float CritDamage { get; set; }
    public float Accuracy { get;set; }
    public float Evasion { get; set; }
    public float PhysicalResistance { get; set; }   
    public float MagicalResistance { get; set; }
    public float Penetration { get; set; }
    public float LifeSteal { get; set; }
    public float HealingAddition { get; set; }
    public float HealingReducion { get; set; }
    public float DamageAddition { get; set; }
    public float DamageReducion { get; set; }
    public float DamageReflect { get;set; }
    public float EnergyAddition { get; set; }
    public float HealingOnTurn { get; set; }
    public float AttackCount { get; set; }
    public List<int> PassiveActiveTime { get; set; }
    public List<string> ListBuff { get; set; }
    public List<string> ListDebuff { get; set; }
    public float TotalDamageDeal { get; set; }
    public float TotalDamageTake { get; set; }
    public float TotalHealing { get; set; }
}
