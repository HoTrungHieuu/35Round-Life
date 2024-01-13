using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GamePlay : MonoBehaviour
{
    
    private float time;
    private float realTime;
    private float count = 0;
    private bool isComplete = false;
    private List<StatForGameplay> listClone;
    // Start is called before the first frame update
    void Start()
    {
        listClone = new();
        foreach (StatForGameplay list in ListObjectForGameplay.list)
        {
            List<int> passiveActiveTimeNew = new();
            for (int i=0; i < list.PassiveActiveTime.Count; i++)
            {
                passiveActiveTimeNew.Add(list.PassiveActiveTime[i]);
            }
            StatForGameplay _list = new()
            {
                team = list.team,
                position = list.position,
                name = list.name,
                classType = list.classType,
                damageType = list.damageType,
                HealPoint = list.HealPoint,
                ShieldPoint = list.ShieldPoint,
                EnergyPoint = 0,
                Attack = list.Attack,
                Deffend = list.Deffend,
                Speed = list.Speed,
                CritRate = list.CritRate,
                CritDamage = list.CritDamage,
                Accuracy = list.Accuracy,
                Evasion = list.Evasion,
                PhysicalResistance = list.PhysicalResistance,
                MagicalResistance = list.MagicalResistance,
                Penetration = list.Penetration,
                LifeSteal = list.LifeSteal,
                HealingAddition = list.HealingAddition,
                HealingReducion = list.HealingReducion,
                DamageAddition = list.DamageAddition,
                DamageReducion = list.DamageReducion,
                DamageReflect = list.DamageReflect,
                EnergyAddition = list.EnergyAddition,
                HealingOnTurn = list.HealingOnTurn,
                AttackCount = list.AttackCount,
                PassiveActiveTime = passiveActiveTimeNew,
                ListBuff = new(),
                ListDebuff = new(),
                TotalDamageDeal = 0,
                TotalDamageTake = 0,
                TotalHealing = 0,
            };
            listClone.Add(_list);
            
            
        }
        UpdateStat();
        UpdateHpBar();
        UpdateEnergyBar();
        UpdateShieldBar();
        UpdateEffectPanel();
        UpdateSpeedBar();
        Time.timeScale = 5.0f;
        UpdatePassiveSkillAtStart();
        UpdateEquipmentActiveAtStart();
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.deltaTime;
        foreach (StatForGameplay list in listClone)
        {
            if(list.HealPoint>0) list.Speed -= time;
        }
        if (listClone.FirstOrDefault(l => l.Speed <= 0) != null && time > 0)
        {
            foreach (StatForGameplay list in listClone.FindAll(l => l.Speed <= 0))
            {
                list.Speed = UpdateSpeed(ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position), list.ListBuff, list.ListDebuff);
                if (list.HealPoint <= 0) continue;    
                if (list.ListDebuff.FirstOrDefault(l => l.Contains("stun")) == null) AttackerPlay(list);

                 
                UpdateDamageFromBurn(list,list.ListBuff,list.ListDebuff);
                if (list.ListBuff.Count>0 || list.ListDebuff.Count>0)
                {
                    
                    RemoveBuffAndDebuff(list);
                    
                }
                
                    
            }
            
            UpdateHpBar();
            UpdateEnergyBar();
            UpdateShieldBar();
            UpdateEffectPanel();
            UpdateSpeedBar();
        }
        UpdateHpBar();
        UpdateEnergyBar();
        UpdateShieldBar();
        UpdateEffectPanel();
        UpdateSpeedBar();
        DealDamageFromTime();
        DeathUpdate();
        UpdatePassiveSkillAnyTimes();
        UpdateEquipmentActiveAnyTimes();
        if (listClone.FindAll(l => l.team == 1 && l.HealPoint > 0).Count == 0 && !isComplete)
        {

            GameObject.Find("DamageUI").transform.Find("Menu").GetComponent<MenuGameplay>().SetUpMainMenu(false);
            List<StatForGameplay> listTeam1 = listClone.FindAll(l => l.team == 1);
            List<StatForGameplay> listTeam2 = listClone.FindAll(l => l.team == 2);
            GameObject.Find("DamageUI").transform.Find("Menu").Find("Statistic UI").GetChild(0).GetComponent<StatisticUpdate>().UpdateStatisticTable(listTeam1, listTeam2);
            isComplete = true;
            Time.timeScale = 0.0f;
        }
        if (listClone.FindAll(l => l.team == 2 && l.HealPoint > 0).Count == 0 && !isComplete)
        {
            
            GameObject.Find("DamageUI").transform.Find("Menu").GetComponent<MenuGameplay>().SetUpMainMenu(true);
            List<StatForGameplay> listTeam1 = listClone.FindAll(l => l.team == 1);
            List<StatForGameplay> listTeam2 = listClone.FindAll(l => l.team == 2);
            GameObject.Find("DamageUI").transform.Find("Menu").Find("Statistic UI").GetChild(0).GetComponent<StatisticUpdate>().UpdateStatisticTable(listTeam1, listTeam2);
            isComplete = true;
            Time.timeScale = 0.0f;
        }
        
    }
    private void DeathUpdate()
    {
        foreach(StatForGameplay list in listClone)
        {
            if (list.HealPoint <= 0)
            {
                CleanseAllBuff(list);
                CleanseAllDebuff(list);
                list.EnergyPoint = 0;
                list.Speed = ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed;
            }
        }
    }
    private void AttackerPlay(StatForGameplay attacker)
    {
        switch (attacker.name)
        {
            case "Tin":
                if (attacker.EnergyPoint < 100)
                {
                    TinAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    TinSkill(attacker);
                    attacker.EnergyPoint = 0;
                }

                break;
            case "Hansel":
                if (attacker.EnergyPoint < 100)
                {
                    HanselAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    HanselSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Wolf":
                if (attacker.EnergyPoint < 100)
                {
                    WolfAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    WolfSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Belle":
                if (attacker.EnergyPoint < 100)
                {
                    BelleAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    BelleSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Huntsman":
                if (attacker.EnergyPoint < 100)
                {
                    HuntsmanAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    HuntsmanSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Rabbit":
                if (attacker.EnergyPoint < 100)
                {
                    RabbitAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    RabbitSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Otohime":
                if (attacker.EnergyPoint < 100)
                {
                    OtohimeAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    OtohimeSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Lion":
                if (attacker.EnergyPoint < 100)
                {
                    LionAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    LionSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Esme":
                if (attacker.EnergyPoint < 100)
                {
                    EsmeAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    EsmeSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Vita":
                if (attacker.EnergyPoint < 100)
                {
                    VitaAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    VitaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Limagi":
                if (attacker.EnergyPoint < 100)
                {
                    LimagiAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    LimagiSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Puss":
                if (attacker.EnergyPoint < 100)
                {
                    PussAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    PussSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Robin":
                if (attacker.EnergyPoint < 100)
                {
                    RobinAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    RobinSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Mermaid":
                if (attacker.EnergyPoint < 100)
                {
                    MermaidAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    MermaidSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Red":
                if (attacker.EnergyPoint < 100)
                {
                    RedAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    RedSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Cheshire":
                if (attacker.EnergyPoint < 100)
                {
                    CheshireAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    CheshireSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Goldilocks":
                if (attacker.EnergyPoint < 100)
                {
                    GoldilocksAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    GoldilocksSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Rapunzel":
                if (attacker.EnergyPoint < 100)
                {
                    RapunzelAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    RapunzelSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Dorothy":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 3 == 0)
                {
                    DorothySpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    DorothyAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    DorothySkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Kaguya":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    KaguyaSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    KaguyaAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    KaguyaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Tsuru":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    TsuruSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    TsuruAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    TsuruSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Quinter":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    QuinterSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    QuinterAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    QuinterSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Snite":
                if (attacker.EnergyPoint < 100)
                {
                    SniteAttack(attacker);
                    AddEnergy(attacker, 20f);
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    SniteSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Dracula":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    DraculaSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    DraculaAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    DraculaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Cinderella":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    CinderellaSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    CinderellaAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    CinderellaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Tamamo":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    TamamoSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    TamamoAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    TamamoSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Nutcracker":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    NutcrackerSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    NutcrackerAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    NutcrackerSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Alice":
                if (attacker.EnergyPoint < 100)
                {
                    AliceAttack(attacker);
                    AddEnergy(attacker, 25);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    AliceSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Aurora":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    AuroraSpecialAttack(attacker);
                    AddEnergy(attacker, 25);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    AuroraAttack(attacker);
                    AddEnergy(attacker, 25);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    AuroraSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Jack":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    JackSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    JackAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    JackSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Pigy":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    PigySpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    PigyAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    PigySkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Jinjur":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    JinjurSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    JinjurAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    JinjurSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Yuki":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    YukiSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    YukiAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    YukiSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Quite":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    QuiteSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    QuiteAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    QuiteSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Jeon":
                if (attacker.EnergyPoint < 100)
                {
                    JeonAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    JeonSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Skuld":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    SkuldSpeicalAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    SkuldAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    SkuldSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Mist":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    MistSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    MistAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    MistSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Eir":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    EirSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    EirAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    EirSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Brunhild":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    BrunhildSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    BrunhildAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    BrunhildSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Hildr":
                if (attacker.PassiveActiveTime[1] > 0)
                {
                    attacker.PassiveActiveTime[1]--;
                    break;
                }
                if (attacker.EnergyPoint < 100)
                {
                    HildrAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    HildrSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Helsing":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    HelsingSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    HelsingAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    HelsingSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Mircalla":
                if (attacker.EnergyPoint < 100)
                {
                    MircallaAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    MircallaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Chang'e":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    ChangeSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    ChangeAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    ChangeSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Befana":
                if (attacker.EnergyPoint < 100)
                {
                    
                    attacker.PassiveActiveTime[0] -= (100 - (int)attacker.EnergyPoint);
                    if (attacker.PassiveActiveTime[0] < 0)
                    {
                        attacker.HealPoint += attacker.PassiveActiveTime[0] * (0.8f * ListObjectForGameplay.list.FirstOrDefault(l => l.team == attacker.team && l.position == attacker.position).HealPoint);
                        attacker.PassiveActiveTime[0] = 0;
                    }
                    attacker.AttackCount++;
                }
                BefanaSkill(attacker);
                attacker.EnergyPoint = 0;

                break;
            case "Santas":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    SantasSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    SantasAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    SantasSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            case "Morgiana":
                if (attacker.EnergyPoint < 100 && attacker.AttackCount % 4 == 0)
                {
                    MorgianaSpecialAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint < 100)
                {
                    MorgianaAttack(attacker);
                    AddEnergy(attacker, 20f);
                    attacker.AttackCount++;
                }
                else if (attacker.EnergyPoint >= 100)
                {
                    MorgianaSkill(attacker);
                    attacker.EnergyPoint = 0;
                }
                break;
            default:
                break;
        }

    }
    private void TinAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void TinSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("stun#1");
        AttackNormalFormWithDebuff(attacker, 1.8f, listDebuffs, "Magical");
    }
    private void HanselAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void HanselSkill(StatForGameplay attacker)
    {
        AttackEnemyWithLowestHPPercentForm(attacker, 2.5f, "Physical");
    }
    private void WolfAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void WolfSkill(StatForGameplay reciever)
    {
        List<string> listBuffs = new();
        listBuffs.Add("attackUpx0.3#3");
        listBuffs.Add("defendUpx0.3#3");
        AddBuff(reciever, listBuffs);
    }
    private void BelleAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void BelleSkill(StatForGameplay healer)
    {
        HealAllyWithLowerHPPercentForm(healer, 2.5f);
    }
    private void HuntsmanAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void HuntsmanSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("attackDownx0.2#2");
        AttackNormalFormWithDebuff(attacker, 1.8f, listDebuffs, "Physical");
    }
    private void RabbitAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void RabbitSkill(StatForGameplay attack)
    {
        StatForGameplay ally = FindAllyHighestAttack(attack);
        AddEnergy(ally, 60);
        List<string> listBuffs = new();
        listBuffs.Add("attackUpx0.2#2");
        AddBuff(ally, listBuffs);
    }
    private void OtohimeAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void OtohimeSkill(StatForGameplay attack)
    {
        HealAllAllyForm(attack, 1.5f);
    }
    private void LionAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void LionSkill(StatForGameplay attack)
    {
        AttackNormalFormWithHealingForYourSelf(attack, 2f, 0.5f, "Physical");
    }
    private void EsmeAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f,"Magical");
    }
    private void EsmeSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("burn#3");
        AttackRandomFormWithManyAttacksAndDebuff(attacker, 3, 1.4f, listDebuffs, "Magical");

    }
    private void VitaAttack(StatForGameplay attacker)
    {
        AttackNormalFormWithHealing(attacker, 1f, 1f,"Magical");
    }
    private void VitaSkill(StatForGameplay attacker)
    {
        AttackNormalFormWithHealing(attacker, 2.5f , 1f,"Magical");
    }
    private void LimagiAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f,"Magical");
    }
    private void LimagiSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("damageReducionDownx0.3#3");
        AttackNormalFormWithDebuff(attacker, 2.1f, listDebuffs,"Magical");
    }
    private void PussAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void PussSkill(StatForGameplay attacker)
    {
        AttackNormalFormWithManyAttacks(attacker, 4, 0.9f, "Physical");
        List<string> listBuffs = new();
        listBuffs.Add("speedUpx0.3#3");
        AddBuff(attacker, listBuffs);
    }
    private void RobinAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void RobinSkill(StatForGameplay attacker)
    {
        AttackNormalAndBehindForm(attacker, 2f, "Physical");
    }
    private void MermaidAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void MermaidSkill(StatForGameplay attacker)
    {
        AddShieldByMaxHp(attacker,0.2f);
        List<string> listBuffs = new();
        listBuffs.Add("taunt#3");
        AddBuff(attacker , listBuffs);
    }
    private void RedAttack(StatForGameplay attacker)
    {
        AttackHorizontalForm(attacker, 0.8f, "Physical");
    }
    private void RedSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("attackDownx0.2#2");
        AttackHorizontalFormWithDebuff(attacker, 2f, listDebuffs, "Physical");
    }
    private void CheshireAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void CheshireSkill(StatForGameplay attacker)
    {
        AttackEnemyWithLowestHPPercentForm(attacker, 2f, "Physical");
        List<string> listBuffs = new();
        listBuffs.Add("speedUpx0.2#3");
        AddBuff(attacker, listBuffs);
    }
    private void GoldilocksAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void GoldilocksSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("physicalResistantDownx0.3#2");
        attacker.CritRate = 1f;
        AttackNormalFormWithDebuff(attacker, 2.5f, listDebuffs, "Physical");
    }
    private void RapunzelAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void RapunzelSkill(StatForGameplay attacker)
    {
        HealAllAllyFormAndCleanseDebuffAndHealingMaxHp(attacker, 1.8f, 2, 0.1f);
    }
    private void DorothyAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void DorothySpecialAttack(StatForGameplay attacker)
    {
        HealAllyWithLowerHPPercentForm(attacker, 2.5f);
    }
    private void DorothySkill(StatForGameplay attacker)
    {
        HealAllAllyForm(attacker, 2f);
        
    }
    private void KaguyaAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker , 1f, "Physical");
    }
    private void KaguyaSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("defendDownx0.3#3");
        AttackNormalFormWithDebuff(attacker, 2f, listDebuffs, "Physical");
    }
    private void KaguyaSkill(StatForGameplay attacker)
    {
        AttackBehindForm(attacker, 2.6f, "True");
    }
    private void TsuruAttack(StatForGameplay attacker)
    {
        HealAllyWithLowerHPPercentForm(attacker, 1f);
    }
    private void TsuruSpecialAttack(StatForGameplay attacker)
    {
        HealAllAllyForm(attacker, 1f);
    }
    private void TsuruSkill(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("attackUpx0.3#4");
        listBuffs.Add("speedUpx0.3#4");
        AddBuff(attacker, listBuffs);
    }

    private void QuinterAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("speedDownx0.2#2");
        AttackNormalFormWithDebuff(attacker, 1.0f, listDebuffs, "Magical");
    }
    private void QuinterSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("speedDownx0.2#2");
        AttackRandomFormWithManyAttacksAndDebuff(attacker , 4 , 1.2f, listDebuffs, "Magical");
    }
    private void QuinterSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        attacker.PassiveActiveTime[0] = 1;
        AttackAllEnemiesForm(attacker, 1.5f, "Magical");
        
    }
    private void SniteAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void SniteSkill(StatForGameplay attacker)
    {
        AttackAllEnemiesFormAndAddShieldOnDamage(attacker, 1.5f, "Magical");
        
    }
    private void DraculaAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void DraculaSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("damageReducionDownx0.2#2");
        AttackNormalFormWithDebuff(attacker, 1.8f, listDebuffs, "Physical");
    }
    private void DraculaSkill(StatForGameplay attacker)
    {
        AttackNormalAndBehindForm(attacker, 2f, "Physical");
    }
    private void CinderellaAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void CinderellaSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("stun#2");
        AttackNormalFormWithDebuff(attacker, 1.6f, listDebuffs, "Magical");
    }
    private void CinderellaSkill(StatForGameplay attacker)
    {
        AttackRandomFormWithManyAttacks(attacker, 12, 0.4f, "Magical");
    }
    private void TamamoAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void TamamoSpecialAttack(StatForGameplay attacker)
    {
        AttackRandomFormWithManyAttacks(attacker, 3 , 1.3f, "Magical");
    }
    private void TamamoSkill(StatForGameplay attacker)
    {
        AttackAllEnemiesForm(attacker, 2f, "Magical");
    }
    private void NutcrackerAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "True");
    }
    private void NutcrackerSpecialAttack(StatForGameplay attacker)
    {
        AttackEnemyWithHighestAtkForm(attacker, 1.6f, "True");
    }
    private void NutcrackerSkill(StatForGameplay attacker)
    {
        AttackEnemyWithHighestAtkForm(attacker, 2.3f, "True");
    }
    private void AliceAttack(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[1] = 1;
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void AliceSkill(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackNormalFormWithManyAttacks(attacker, 3, 1.2f, "True");
    }
    private void AuroraAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void AuroraSpecialAttack(StatForGameplay attacker)
    {
        AttackNormalFormAndCleanseAllBuff(attacker, 1.8f, "Magical");
    }
    private void AuroraSkill(StatForGameplay attacker)
    {
        AttackRandomFormAndCleanseAllBuffWithManyAttacks(attacker, 3, 1.5f, "Magical");
    }
    private void JackAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void JackSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("healingReducionDownx0.5#2");
        AttackEnemyWithHighestAtkFormWithDebuff(attacker, 1.8f, listDebuffs, "Physical");
    }
    private void JackSkill(StatForGameplay attacker)
    {
        AttackEnemyWithHighestAtkForm(attacker, 2.8f, "Physical");
    }
    private void PigyAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");
    }
    private void PigySpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("attackDownx0.5#2");
        AttackEnemyWithHighestAtkFormWithDebuff(attacker, 1.5f, listDebuffs, "Magical");
    }
    private void PigySkill(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("damageReducionUpx0.28#3");
        AddShieldBehindFormWithBuff(attacker, 0.28f, listBuffs);
        listBuffs.Add("taunt#3");
        AddShieldByMaxHp(attacker, 0.28f);
        AddBuff(attacker, listBuffs);
    }
    private void JinjurAttack(StatForGameplay attacker)
    {
        AttackNormalAndBehindForm(attacker, 0.7f, "Physical");
    }
    private void JinjurSpecialAttack(StatForGameplay attacker)
    {
        AttackRandomFormWithManyAttacks(attacker, 4, 1.2f, "True");
    }
    private void JinjurSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("evasionDownx0.2#3");
        listDebuffs.Add("defendDownx0.2#3");
        listDebuffs.Add("damageReducionDownx0.2#3");
        AttackNormalFormWithDebuff(attacker, 3f, listDebuffs, "Physical");
    }
    private void YukiAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void YukiSpecialAttack(StatForGameplay attacker)
    {
        
        AttackNormalFormWithHealingForYourSelf(attacker, 1.5f,1f,"Physical" );
        List<string> listBuffs = new();
        listBuffs.Add("evasionUpx0.3#3");
        listBuffs.Add("defendUpx0.3#3");
        AddBuff(attacker, listBuffs);
    }
    private void YukiSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("accuracyDownx0.5#2");
        
        AttackHorizontalBehindFormWithDebuff(attacker, 2.1f, listDebuffs, "Physical");
    }
    private void QuiteAttack(StatForGameplay attacker)
    {
        AttackNormalFormAndCleaseBuff(attacker, 1f,1, "Magical");
        

    }
    private void QuiteSpecialAttack(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("defendUpx0.3#3");
        HealAllyWithLowerHPPercentFormAndBuff(attacker, 2.5f,listBuffs);
    }
    private void QuiteSkill(StatForGameplay attacker)
    {
        HealAllAllyFormAndCleanseDebuff(attacker, 3f, 2);
    }
    private void JeonAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void JeonSkill(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackNormalForm(attacker, 3f, "True");
    }
    private void SkuldAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void SkuldSpeicalAttack(StatForGameplay attacker)
    {
        AttackRandomFormWithManyAttacks(attacker,4, 1f, "Physical");
    }
    private void SkuldSkill(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackNormalAndBehindForm(attacker, 2f, "Physical");
        
    }
    private void MistAttack(StatForGameplay attacker)
    {
        AttackEnemyWithLowestHPPercentForm(attacker, 1f, "Physical");
    }
    private void MistSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("speedDownx0.2#3");
        AttackEnemyWithHighestAtkFormWithDebuff(attacker, 1.8f, listDebuffs, "Physical");
        List<string> listBuffs = new();
        listBuffs.Add("speedUpx0.2#4");
        AddBuff(attacker,listBuffs);
    }
    private void MistSkill(StatForGameplay attacker)
    {
        AttackEnemyWithHighestAtkForm(attacker, 3f, "True");
        List<string> listBuffs = new();
        listBuffs.Add("damageAdditionUpx0.3#3");
        AddBuff(attacker, listBuffs);
    }
    private void EirAttack(StatForGameplay attacker)
    {
        HealAllyWithLowerHPPercentForm(attacker, 1f);
    }
    private void EirSpecialAttack(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("attackUpx0.3#3");
        listBuffs.Add("defendUpx0.3#3");
        HealAllyWithLowerHPPercentFormAndBuffAndCleanseDebuff(attacker, 1.3f, 2, listBuffs);
    }
    private void EirSkill(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("speedUpx0.7#4");
        AddBuff(attacker, listBuffs);
    }
    private void BrunhildAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void BrunhildSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("attackDownx0.3#3");
        AttackNormalAndBehindFormWithDebuff(attacker, 2.5f, listDebuffs, "Physical");
        List<string> listBuffs = new();
        listBuffs.Add("taunt#3");
        AddBuff(attacker, listBuffs);
    }
    private void BrunhildSkill(StatForGameplay attacker)
    {
        List<string> listBuffs = new();
        listBuffs.Add("lifeStealUpx0.5#4");
        listBuffs.Add("damageReflectUpx0.5#4");
        listBuffs.Add("taunt#3");
        AddBuff(attacker, listBuffs);
        
    }
    private void HildrAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void HildrSkill(StatForGameplay attacker)
    {
        AddShieldByMaxHp(attacker, 0.6f);
        attacker.PassiveActiveTime[1] = 2;
    }
    private void HelsingAttack(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackNormalForm(attacker, 1f, "Physical");
        
    }
    private void HelsingSpecialAttack(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("defendDownx0.3#2");
        listDebuffs.Add("damageReducionDownx0.3#2");
        AttackEnemyWithLowestHPPercentFormWithDebuff(attacker, 2f, listDebuffs, "Physical");
        
    }
    private void HelsingSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("stun#1");
        AttackAllEnemiesFormWithDebuff(attacker, 1.8f, listDebuffs, "Physical");

    }
    private void MircallaAttack(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackNormalForm(attacker, 1f, "Magical");
        

    }
    private void MircallaSkill(StatForGameplay attacker)
    {
        List<string> listDebuffs = new();
        listDebuffs.Add("attackDownx0.1#3");
        listDebuffs.Add("defendDownx0.1#3");
        AttackEnemyWithHighestAtkFormWithDebuffManyAttacks(attacker, 6, 1.5f, listDebuffs, "Magical");

    }
    private void ChangeAttack(StatForGameplay attacker)
    {
        AttackNormalForm(attacker, 1f, "Magical");

    }
    private void ChangeSpecialAttack(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        HealAllyWithLowerHPPercentFormAndCleanseAllDebuff(attacker, 3f);

    }
    private void ChangeSkill(StatForGameplay attacker)
    {
        StatForGameplay yourSelf = ListObjectForGameplay.list.FirstOrDefault(l => l.team == attacker.team && l.position == attacker.position);
        yourSelf.Accuracy += 10f;
        attacker.Accuracy = yourSelf.Accuracy;
        AttackNormalAndBehindFormCleanseAllBuffAndHealing(attacker, 3f, 0.8f, "Magical");
        yourSelf.Accuracy -= 10f;

    }
    private void BefanaSkill(StatForGameplay attacker)
    {

        float damagePercent = 3f + (0.5f * attacker.PassiveActiveTime[0]/100);
        AttackAllEnemiesForm(attacker, damagePercent * 0.4f, "Physical");
        AttackAllEnemiesForm(attacker, damagePercent * 0.4f, "Magical");
        AttackAllEnemiesForm(attacker, damagePercent * 0.4f, "True");
    }
    private void SantasAttack(StatForGameplay attacker)
    {

        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void SantasSpecialAttack(StatForGameplay attacker)
    {

        AttackRandomForm(attacker, 2.5f, "Physical");
        HealRandomForm(attacker, 3f);
    }
    private void SantasSkill(StatForGameplay attacker)
    {


        AttackAllEnemiesForm(attacker, 2.2f, "Physical");
        HealAllAllyForm(attacker, 2.5f);
    }
    private void MorgianaAttack(StatForGameplay attacker)
    {

        AttackNormalForm(attacker, 1f, "Physical");
    }
    private void MorgianaSpecialAttack(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[1] = 1;
        AttackEnemyWithLowestHPPercentForm(attacker, 2.3f, "Physical");
    }
    private void MorgianaSkill(StatForGameplay attacker)
    {
        attacker.PassiveActiveTime[0] = 1;
        AttackEnemyWithLowestHPPercentForm(attacker, 3.5f, "Physical");
    }
    private int GenerateRandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    private Vector3 getPosition(StatForGameplay attacker)
    {
        switch (attacker.team)
        {
            case 1:
                switch (attacker.position)
                {
                    case 0:
                        return new Vector3(-3, 2, 0);
                    case 1:
                        return new Vector3(-3, -2, 0);
                    case 2:
                        return new Vector3(-6, 3, 0);
                    case 3:
                        return new Vector3(-6, 0, 0);
                    case 4:
                        return new Vector3(-6, -3, 0);

                }
                break;
            case 2:
                switch (attacker.position)
                {
                    case 0:
                        return new Vector3(3, 2, 0);
                    case 1:
                        return new Vector3(3, -2, 0);
                    case 2:
                        return new Vector3(6, 3, 0);
                    case 3:
                        return new Vector3(6, 0, 0);
                    case 4:
                        return new Vector3(6, -4, 0);
                }
                break;
        }
        return new Vector3(0, 0, 0);
    }
    private float DamageAfterClassType(StatForGameplay attacker, StatForGameplay reciever, float damage)
    {
        switch (attacker.classType)
        {
            case "Warrior":
                switch (reciever.classType)
                {
                    case "Sage":
                        return damage * 1.25f;
                }
                break;
            case "Ranger":
                switch (reciever.classType)
                {
                    case "Assassin":
                        return damage * 1.25f;
                    case "Guardian":
                        return damage * 0.75f;
                }
                break;
            case "Mage":
                switch (reciever.classType)
                {
                    case "Warrior":
                        return damage * 1.25f;
                    case "Guardian":
                        return damage * 1.25f;
                    case "Sage":
                        return damage * 0.75f;
                }
                break;
            case "Assassin":
                switch (reciever.classType)
                {
                    case "Mage":
                        return damage * 1.25f;
                    case "Ranger":
                        return damage * 1.25f;
                    case "Guardian":
                        return damage * 0.75f;
                    case "Warrior":
                        return damage * 0.75f;
                }
                break;
        }
        return damage;
    }
    private float DealDamage(StatForGameplay attacker, StatForGameplay reciever, float percentDamgage,string damageType)
    {
        float damage;
        bool isCrit = false;
        bool isEva = false;
        if (damageType == "True") damage = attacker.Attack * percentDamgage;
        else
        {
            float defend = reciever.Deffend - reciever.Deffend * attacker.Penetration;
            if (defend<0)defend = 0;
            damage = (attacker.Attack - defend) * percentDamgage;
        }
            
        if (damageType == "Physical") damage -= (damage * reciever.PhysicalResistance);
        if (damageType == "Magical") damage -= (damage * reciever.MagicalResistance);
        
        if (GenerateRandomNumber(1, 100) <= attacker.CritRate * 100f)
        {
            damage *= (attacker.CritDamage + 1.5f);
            isCrit = true;
        }
        damage += damage * (attacker.DamageAddition - reciever.DamageReducion);
        
        if(damage<0)damage = 0;
        if(GenerateRandomNumber(1, 100) <= (reciever.Evasion - attacker.Accuracy)*100f)
        {
            damage = 0;
            isEva = true;
        }
        damage = DamageAfterClassType(attacker, reciever, damage);
        damage = UpdateDamageOfPassiveSkill(attacker, reciever, damage, isCrit, isEva);
        damage = UpdateDamageOfEquipmentActive(attacker, reciever, damage, isCrit, isEva);

        reciever.ShieldPoint -= damage;
        if(reciever.ShieldPoint < 0)
        {
            reciever.HealPoint += reciever.ShieldPoint;
            reciever.ShieldPoint = 0f;
        }
        attacker.TotalDamageDeal += damage;
        reciever.TotalDamageTake += damage;
        if(reciever.DamageReflect>0) DealDamageFromReflect(attacker,reciever ,damage * reciever.DamageReflect);
        if(attacker.LifeSteal>0) HealFromLifeSteal(attacker, damage * attacker.LifeSteal);
        UpdatePassiveSkillAfterAttack(attacker);
        UpdateStat();
        
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().DamageTextExport(damage, damageType, getPosition(reciever), isCrit,isEva);
        return damage;
    }
    private float DealTrueDamageFromOtherResource(StatForGameplay attacker, StatForGameplay reciever, float damage)
    {
        float damg = damage;
        bool isCrit = false;
        bool isEva = false;
        if (GenerateRandomNumber(1, 100) <= attacker.CritRate * 100f)
        {
            damg *= attacker.CritDamage;
            isCrit = true;
        }
        damg += damage * (attacker.DamageAddition - reciever.DamageReducion);
        if (damg < 0) damg = 0;
        if (GenerateRandomNumber(1, 100) <= reciever.Evasion - attacker.Accuracy)
        {
            damg = 0;
            isEva = true;
        }
        damg = DamageAfterClassType(attacker, reciever, damg);
        reciever.ShieldPoint -= damg;
        if (reciever.ShieldPoint < 0)
        {
            reciever.HealPoint += reciever.ShieldPoint;
            reciever.ShieldPoint = 0f;
        }
        attacker.TotalDamageDeal += damg;
        reciever.TotalDamageTake += damg;
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().DamageTextExport(damg, "True", getPosition(reciever), isCrit,isEva);
        return damg;
    }
    private float DealTrueDamageByMaxHp(StatForGameplay reciever, float percentHp)
    {
        float damg = ListObjectForGameplay.list.FirstOrDefault(l=>l.team == reciever.team && l.position==reciever.position).HealPoint * percentHp;
        bool isCrit = false;
        bool isEva = false;

        damg -= damg * reciever.DamageReducion;
        if (damg < 0) damg = 0;
        reciever.ShieldPoint -= damg;
        if (reciever.ShieldPoint < 0)
        {
            reciever.HealPoint += reciever.ShieldPoint;
            reciever.ShieldPoint = 0f;
        }
        reciever.TotalDamageTake += damg;
        if(damg>0) GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().DamageTextExport(damg, "True", getPosition(reciever), isCrit, isEva);
        return damg;
    }
    private void DealDamageFromReflect(StatForGameplay attacker, StatForGameplay reciever, float damage)
    {
        float dmg = damage;
        dmg += dmg * (attacker.DamageAddition - reciever.DamageReducion);
        if (dmg < 0) dmg = 0;
        dmg = DamageAfterClassType(attacker, reciever, dmg);
        attacker.ShieldPoint -= dmg;
        if (attacker.ShieldPoint < 0)
        {
            attacker.HealPoint += attacker.ShieldPoint;
            attacker.ShieldPoint = 0f;
        }
        reciever.TotalDamageDeal += dmg;
        attacker.TotalDamageTake += dmg;
        UpdateDamageReflectOfEquipmentActive(attacker, reciever, dmg);
        if (dmg >0) GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().DamageTextExport(dmg, "True", getPosition(attacker), false,false);
    }
    private void HealFromLifeSteal(StatForGameplay attacker, float damage)
    {
        float healing = damage * attacker.HealingAddition * attacker.HealingReducion;
        healing = UpdateHealingOfEquipmentActive(attacker,attacker,healing);
        attacker.HealPoint += healing;
        if (attacker.HealPoint >= ListObjectForGameplay.list.FirstOrDefault(l => l.team == attacker.team && l.position == attacker.position).HealPoint)
        {
            attacker.HealPoint = ListObjectForGameplay.list.FirstOrDefault(l => l.team == attacker.team && l.position == attacker.position).HealPoint;
        }
        attacker.TotalHealing += healing;
        if (healing >0) GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().HealingTextExport(healing, getPosition(attacker));
    }
    private void RegenHeal(StatForGameplay healer, StatForGameplay reciever, float percentHeal)
    {
        float healing = (healer.Attack * percentHeal) * healer.HealingAddition * reciever.HealingReducion;
        healing = UpdateHeailngOfPassiveSkill(healer, reciever, healing);
        healing = UpdateHealingOfEquipmentActive(healer, reciever, healing);
        reciever.HealPoint += healing;

        if (reciever.name!= "Befana" && reciever.HealPoint >= ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint)
        {
            reciever.HealPoint = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint;
        }
        healer.TotalHealing += healing;
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().HealingTextExport(healing, getPosition(reciever));

    }
    private void HealByMaxHp(StatForGameplay healer, StatForGameplay reciever, float percentHp)
    {
        float healing = ListObjectForGameplay.list.FirstOrDefault(l=>l.team == reciever.team && l.position == reciever.position).HealPoint * percentHp * healer.HealingAddition * reciever.HealingReducion;
        healing = UpdateHeailngOfPassiveSkill(healer, reciever, healing);
        healing = UpdateHealingOfEquipmentActive(healer, reciever, healing);
        reciever.HealPoint += healing;

        if (reciever.name != "Befana" && reciever.HealPoint >= ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint)
        {
            reciever.HealPoint = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint;
        }
        healer.TotalHealing += healing;
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().HealingTextExport(healing, getPosition(reciever));
    }
    private void HealByMaxHpOfHealer(StatForGameplay healer, StatForGameplay reciever, float percentHp)
    {
        float healing = ListObjectForGameplay.list.FirstOrDefault(l => l.team == healer.team && l.position == healer.position).HealPoint * percentHp * healer.HealingAddition * reciever.HealingReducion;
        healing = UpdateHeailngOfPassiveSkill(healer, reciever, healing);
        healing = UpdateHealingOfEquipmentActive(healer, reciever, healing);
        reciever.HealPoint += healing;

        if (reciever.name != "Befana" && reciever.HealPoint >= ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint)
        {
            reciever.HealPoint = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint;
        }
        healer.TotalHealing += healing;
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().HealingTextExport(healing, getPosition(reciever));
    }
    private void HealingByDamage(StatForGameplay healer ,StatForGameplay reciever, float damgageDeal, float percentHeal)
    {
        float healing = (damgageDeal * percentHeal) * healer.HealingAddition * healer.HealingReducion;
        healing = UpdateHeailngOfPassiveSkill(healer, reciever, healing);
        healing = UpdateHealingOfEquipmentActive(healer, reciever, healing);
        reciever.HealPoint += healing ;
        if (reciever.name != "Befana" &&    reciever.HealPoint >= ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint)
        {
            reciever.HealPoint = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint;
        }
        healer.TotalHealing += healing;
        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().HealingTextExport(healing, getPosition(reciever));
    }
    private void AddShieldByMaxHp(StatForGameplay reciever, float percentMaxHp)
    {
        float shield = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).HealPoint * percentMaxHp;
        reciever.ShieldPoint += shield;
        
    }
    private void AddShieldByMaxHpOfGiver(StatForGameplay giver, StatForGameplay reciever, float percentMaxHp)
    {
        float shield = ListObjectForGameplay.list.FirstOrDefault(l => l.team == giver.team && l.position == giver.position).HealPoint * percentMaxHp;
        reciever.ShieldPoint += shield;
    }
    private void AddShieldByDamage(StatForGameplay reciever, float damage , float shieldPercent)
    {
        float shield = damage * shieldPercent;
        reciever.ShieldPoint += shield;
    }
    private void AddEnergy(StatForGameplay reciever, float energyPoint)
    {
        reciever.EnergyPoint += energyPoint * (1+reciever.EnergyAddition);
    }
    private void CleanseBuff(StatForGameplay reciever, int quantity)
    {
        if (reciever.ListBuff.Count < quantity)
        {
            CleanseAllBuff(reciever);
            return;
        }
        for (int i = 0; i < quantity; i++)
        {
            GenerateRandomNumber(1, reciever.ListBuff.Count);
            reciever.ListBuff.RemoveAt(GenerateRandomNumber(1, reciever.ListBuff.Count) - 1);
        }
    }
    private void CleanseAllBuff(StatForGameplay reciever)
    {
        reciever.ListBuff = new();
    }
    private int CleanseDebuff(StatForGameplay reciever, int quantity)
    {
        if (reciever.ListDebuff.Count < quantity)
        {
            CleanseAllDebuff(reciever);
            return reciever.ListDebuff.Count;
        }
        for(int i = 0; i < quantity; i++)
        {
            GenerateRandomNumber(1, reciever.ListBuff.Count);
            reciever.ListDebuff.RemoveAt(GenerateRandomNumber(1, reciever.ListDebuff.Count) - 1);
        }
        return quantity;
    }
    private void CleanseAllDebuff(StatForGameplay reciever)
    {
        reciever.ListDebuff = new();
    }
    private void AddBuff(StatForGameplay reciever, List<string> listBuffs)
    {
        foreach (string item in listBuffs)
        {
            reciever.ListBuff.Add(item);
        }
    }
    private void AddDebuff(StatForGameplay reciever, List<string> listDebuffs)
    {
        foreach (string item in listDebuffs)
        {
            reciever.ListDebuff.Add(item);
        }
    }
    private void AttackNormalForm(StatForGameplay attacker, float damagePercent,string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;
        
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent,damgType);
    }
    private void AttackNormalFormWithManyAttacks(StatForGameplay attacker, int numberAttack, float damagePercent, string damgType)
    {
        for (int i = 0; i < numberAttack; i++)
        {
            AttackNormalForm(attacker, damagePercent, damgType);
        }
    }
    private void AttackNormalFormAndCleaseBuff(StatForGameplay attacker, float damagePercent, int buffQuntity, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent, damgType);
        CleanseBuff(reciever, buffQuntity);
    }
    private void AttackNormalFormAndCleanseAllBuff(StatForGameplay attacker, float damagePercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent, damgType);
        CleanseAllBuff(reciever);
    }
    private void AttackNormalFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent,damgType);
        AddDebuff(reciever, listDebuffs);
    }
    private void AttackNormalFormWithHealing(StatForGameplay attacker, float damagePercent, float healingPercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        float damageDeal = DealDamage(attacker, reciever, damagePercent,damgType);
        HealAllyWithLowerHPPercentFormByDamage(attacker, damageDeal, healingPercent);
    }
    private void AttackNormalFormWithHealingForYourSelf(StatForGameplay attacker, float damagePercent, float healingPercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        float damageDeal = DealDamage(attacker, reciever, damagePercent, damgType);
        HealingByDamage(attacker, attacker, damageDeal, healingPercent);
    }
    private void AttackNormalFormFromOtherResource(StatForGameplay attacker, float damageDeal, float damagePercent)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                break;
            default:
                break;
        }
        DealTrueDamageFromOtherResource(attacker, reciever, damageDeal * damagePercent);
        
    }
    private void AttackBehindForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent,damgType);
    }
    private void AttackBehindFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        switch (attacker.position)
        {
            case 0:
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 1:
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                break;
            case 2:
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 3:
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                break;
            case 4:
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 4);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 3);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 2);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 1);
                }
                else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    reciever = listEnemies.FirstOrDefault(l => l.position == 0);
                }
                break;
            default:
                break;
        }
        DealDamage(attacker, reciever, damagePercent,damgType);
        AddDebuff(reciever, listDebuffs);
    }
    private void AttackHorizontalForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay>  listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        if(listEnemiesHaveTaunt.Count > 0)
        {
            if (listEnemiesHaveTaunt.FirstOrDefault(l=> l.position == 0) != null || listEnemiesHaveTaunt.FirstOrDefault(l=> l.position == 1) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l =>l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l=> l.position == 1));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
        }
        else
        {
            if (listEnemies.FirstOrDefault(l => l.position == 0) != null || listEnemies.FirstOrDefault(l => l.position == 1) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
        }
        foreach (StatForGameplay reciever in recieveres) DealDamage(attacker, reciever, damagePercent,damgType);

        
    }
    private void AttackHorizontalFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        if (listEnemiesHaveTaunt.Count > 0)
        {
            if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null || listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
        }
        else
        {
            if (listEnemies.FirstOrDefault(l => l.position == 0) != null || listEnemies.FirstOrDefault(l => l.position == 1) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
        }
        foreach (StatForGameplay reciever in recieveres)
        {
            DealDamage(attacker, reciever, damagePercent,damgType);
            AddDebuff(reciever, listDebuffs);
        }
    }
    private void AttackHorizontalBehindForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        if (listEnemiesHaveTaunt.Count > 0)
        {
            if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null || listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null || listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
            {
                

                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
        }
        else
        {
            if (listEnemies.FirstOrDefault(l => l.position == 2) != null || listEnemies.FirstOrDefault(l => l.position == 3) != null || listEnemies.FirstOrDefault(l => l.position == 4) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
        }
        foreach (StatForGameplay reciever in recieveres) DealDamage(attacker, reciever, damagePercent,damgType);


    }
    private void AttackHorizontalBehindFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        if (listEnemiesHaveTaunt.Count > 0)
        {
            if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null || listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null || listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
            {


                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listClone.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listClone.FirstOrDefault(l => l.position == 1));
                }
            }
        }
        else
        {
            if (listEnemies.FirstOrDefault(l => l.position == 2) != null || listEnemies.FirstOrDefault(l => l.position == 3) != null || listEnemies.FirstOrDefault(l => l.position == 4) != null)
            {
                if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                }
            }
            else
            {
                if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                }
                if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                {
                    recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                }
            }
        }
        foreach (StatForGameplay reciever in recieveres)
        {
            DealDamage(attacker, reciever, damagePercent,damgType);
            AddDebuff(reciever, listDebuffs);
        }
    }
    private void AttackNormalAndBehindForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        if(listEnemiesHaveTaunt.Count > 0)
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        foreach (StatForGameplay reciever in recieveres) DealDamage(attacker, reciever, damagePercent,damgType);
        
    }
    private void AttackNormalAndBehindFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        if (listEnemiesHaveTaunt.Count > 0)
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        foreach (StatForGameplay reciever in recieveres)
        {
            DealDamage(attacker, reciever, damagePercent,damgType);
            AddDebuff(reciever, listDebuffs);
        } 
            

    }
    private void AttackNormalAndBehindFormCleanseAllBuffAndHealing(StatForGameplay attacker, float damagePercent,float HealingPercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemiesHaveTaunt = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        if (listEnemiesHaveTaunt.Count > 0)
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemiesHaveTaunt.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (attacker.position)
            {
                case 0:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }

                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 1:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                case 2:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 3:
                    if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    break;
                case 4:
                    if (listEnemies.FirstOrDefault(l => l.position == 1) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 1));
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 0) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 0));
                        if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                        }
                        if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                        {
                            recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                        }
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 4) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 4));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 3) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 3));
                    }
                    else if (listEnemies.FirstOrDefault(l => l.position == 2) != null)
                    {
                        recieveres.Add(listEnemies.FirstOrDefault(l => l.position == 2));
                    }
                    break;
                default:
                    break;
            }
        }
        float damageTotal = 0;
        foreach (StatForGameplay reciever in recieveres)
        {
            damageTotal = DealDamage(attacker, reciever, damagePercent, damgType);
            CleanseAllBuff(reciever);
        }
        HealAllAllyFormByDamage(attacker, damageTotal, HealingPercent);

    }
    private void AttackEnemyWithLowestHPPercentForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        float minPercentHp = 1f;
        foreach (StatForGameplay enemy in listEnemies.FindAll(l => l.team == enemyTeam))
        {
            float PercentHp = enemy.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == enemy.team && l.position == enemy.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                recieveres = new();
                minPercentHp = PercentHp;
                recieveres.Add(enemy);
            }
            else if (PercentHp == minPercentHp)
            {
                recieveres.Add(enemy);
            }
        }
        if (recieveres.Count == 0) return;
        recieveres.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        DealDamage(attacker, recieveres[0], damagePercent,damgType);

        
        
    }
    private void AttackEnemyWithLowestHPPercentFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        float minPercentHp = 1f;
        foreach (StatForGameplay enemy in listEnemies.FindAll(l => l.team == enemyTeam))
        {
            float PercentHp = enemy.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == enemy.team && l.position == enemy.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                recieveres = new();
                minPercentHp = PercentHp;
                recieveres.Add(enemy);
            }
            else if (PercentHp == minPercentHp)
            {
                recieveres.Add(enemy);
            }
        }
        if (recieveres.Count == 0) return;
        recieveres.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        DealDamage(attacker, recieveres[0], damagePercent, damgType);
        AddDebuff(recieveres[0], listDebuffs);


    }
    private void AttackEnemyWithHighestAtkForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);

        float maxAttack = 0f;
        foreach (StatForGameplay list in listEnemies)
        {

            if (maxAttack < list.Attack)
            {
                recieveres = new();
                maxAttack = list.Attack;
                recieveres.Add(list);
            }
            else if (maxAttack == list.Attack)
            {
                recieveres.Add(list);
            }
        }
        if (recieveres.Count == 0) return;

        DealDamage(attacker, recieveres[0], damagePercent,damgType);

        
    }
    private void AttackEnemyWithHighestAtkFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        List<StatForGameplay> recieveres = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        float maxAttack = 0f;
        foreach (StatForGameplay list in listEnemies)
        {

            if (maxAttack < list.Attack)
            {
                recieveres = new();
                maxAttack = list.Attack;
                recieveres.Add(list);
            }
            else if (maxAttack == list.Attack)
            {
                recieveres.Add(list);
            }
        }
        if (recieveres.Count == 0) return;

        DealDamage(attacker, recieveres[0], damagePercent,damgType);
        AddDebuff(recieveres[0], listDebuffs);
    }
    private void AttackEnemyWithHighestAtkFormWithDebuffManyAttacks(StatForGameplay attacker,int quantity, float damagePercent, List<string> listDebuffs, string damgType)
    {
        for (int i = 0; i < quantity; i++)
        {
            AttackEnemyWithHighestAtkFormWithDebuff(attacker, damagePercent, listDebuffs, damgType);
        }
    }
    private void AttackRandomForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        int numberRandom;
        do
        {
            numberRandom = GenerateRandomNumber(0, 4);
            reciever = listEnemies.FirstOrDefault(l => l.position == numberRandom);
            if (listEnemies.Count == 0) return;
        } while (reciever == null);
        DealDamage(attacker, reciever, damagePercent,damgType);

        
    }
    private void AttackRandomFormAndCleanseAllBuff(StatForGameplay attacker, float damagePercent, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        int numberRandom;
        do
        {
            numberRandom = GenerateRandomNumber(0, 4);
            reciever = listEnemies.FirstOrDefault(l => l.position == numberRandom);
            if (listEnemies.Count == 0) return;
        } while (reciever == null);
        DealDamage(attacker, reciever, damagePercent, damgType);
        CleanseAllBuff(reciever);


    }
    private void AttackRandomFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        StatForGameplay reciever = new();
        int enemyTeam = 1;
        if (attacker.team == 1) enemyTeam = 2;
        else if (attacker.team == 2) enemyTeam = 1;

        List<StatForGameplay> listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0 && l.ListBuff.FirstOrDefault(l => l.Contains("taunt")) != null);
        if (listEnemies.Count == 0) listEnemies = listClone.FindAll(l => l.team == enemyTeam && l.HealPoint > 0);
        int numberRandom;
        do
        {
            numberRandom = GenerateRandomNumber(0, 4);
            reciever = listEnemies.FirstOrDefault(l => l.position == numberRandom);
            if (listEnemies.Count == 0) return;
        } while (reciever == null);
        DealDamage(attacker, reciever, damagePercent,damgType);
        AddDebuff(reciever, listDebuffs);
    }
    private void AttackRandomFormWithManyAttacks(StatForGameplay attacker, int numberAttack, float damagePercent, string damgType)
    {
        for (int i = 0; i < numberAttack; i++)
        {
            AttackRandomForm(attacker, damagePercent, damgType);
            
        }
    }
    private void AttackRandomFormAndCleanseAllBuffWithManyAttacks(StatForGameplay attacker, int numberAttack, float damagePercent, string damgType)
    {
        for (int i = 0; i < numberAttack; i++)
        {
            AttackRandomFormAndCleanseAllBuff(attacker, damagePercent, damgType);

        }
    }
    private void AttackRandomFormWithManyAttacksAndDebuff(StatForGameplay attacker, int numberAttack, float damagePercent, List<string> listDebuffs, string damgType)
    {
        for (int i = 0; i < numberAttack; i++)
        {
            AttackRandomFormWithDebuff(attacker, damagePercent,listDebuffs,damgType);
        }
    }
    private int AttackAllEnemiesForm(StatForGameplay attacker, float damagePercent, string damgType)
    {
        int countTime = 0;
        if (attacker.team == 1 && listClone.FindAll(l => l.team == 2 && l.HealPoint > 0).Count > 0)
        {
            
            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 2 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    countTime++;
                    DealDamage(attacker, reciever, damagePercent,damgType);
                }
            }
            
        }
        else if (attacker.team == 2 && listClone.FindAll(l => l.team == 1 && l.HealPoint > 0).Count > 0)
        {
            
            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 1 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    countTime++;
                    DealDamage(attacker, reciever, damagePercent,damgType);
                }
            }
            
        }
        return countTime;
    }
    private int AttackAllEnemiesFormAndAddShieldOnDamage(StatForGameplay attacker, float damagePercent, string damgType)
    {
        int countTime = 0;
        if (attacker.team == 1 && listClone.FindAll(l => l.team == 2 && l.HealPoint > 0).Count > 0)
        {

            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 2 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    countTime++;
                    float damage = DealDamage(attacker, reciever, damagePercent,damgType);
                    AddShieldByDamage(attacker, damage , 1f);
                }
            }

        }
        else if (attacker.team == 2 && listClone.FindAll(l => l.team == 1 && l.HealPoint > 0).Count > 0)
        {

            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 1 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    countTime++;
                    float damage = DealDamage(attacker, reciever, damagePercent,damgType);
                    AddShieldByDamage(attacker, damage, 1f);
                }
            }

        }
        return countTime;
    }
    private void AttackAllEnemiesFormWithDebuff(StatForGameplay attacker, float damagePercent, List<string> listDebuffs, string damgType)
    {
        if (attacker.team == 1 && listClone.FindAll(l => l.team == 2 && l.HealPoint > 0).Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 2 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    DealDamage(attacker, reciever, damagePercent,damgType);
                    AddDebuff(reciever, listDebuffs);
                }
            }
        }
        else if (attacker.team == 2 && listClone.FindAll(l => l.team == 1 && l.HealPoint > 0).Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                StatForGameplay reciever = listClone.FirstOrDefault(l => l.team == 1 && l.position == i && l.HealPoint > 0);
                if (reciever != null)
                {
                    DealDamage(attacker, reciever, damagePercent,damgType);
                    AddDebuff(reciever, listDebuffs);
                }
            }
        }
    }
    private void HealAllyWithLowerHPPercentForm(StatForGameplay healer, float healPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));

        RegenHeal(healer, allies[0], healPercent);
        
    }
    private void HealAllyWithLowerHPPercentFormByUserMaxHp(StatForGameplay healer, float hpPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));

        HealByMaxHpOfHealer(healer, allies[0], hpPercent);
        

    }
    private void HealAllyWithLowerHPPercentFormAndBuff(StatForGameplay healer, float healPercent, List<string> listBuffs)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));

        RegenHeal(healer, allies[0], healPercent);
        AddBuff(allies[0], listBuffs);
    }
    private void HealAllyWithLowerHPPercentFormAndBuffAndCleanseDebuff(StatForGameplay healer, float healPercent, int DebuffsQuantity, List<string> listBuffs)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));

        RegenHeal(healer, allies[0], healPercent);
        CleanseDebuff(allies[0], DebuffsQuantity);
        AddBuff(allies[0], listBuffs);
    }
    private void HealAllyWithLowerHPPercentFormAndCleanseAllDebuff(StatForGameplay healer, float healPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));

        RegenHeal(healer, allies[0], healPercent);
        CleanseAllDebuff(allies[0]);

    }
    private void HealAllyWithLowerHPPercentFormByDamage(StatForGameplay healer,float damgageDeal, float healPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        HealingByDamage(healer, allies[0], damgageDeal, healPercent);
        
    }
    private void HealRandomForm(StatForGameplay healer, float healPercent)
    {
        StatForGameplay reciever = new();

        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        int numberRandom;
        do
        {
            numberRandom = GenerateRandomNumber(0, 4);
            reciever = listAllies.FirstOrDefault(l => l.position == numberRandom);
            if (listAllies.Count == 0) return;
        } while (reciever == null);
        RegenHeal(healer, reciever, healPercent);


    }

    private void HealAllAllyForm(StatForGameplay healer, float healPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l =>l.position == i);
            if (ally != null)
            {
                RegenHeal(healer, ally, healPercent);
            }
        }
        
    }
    private void HealAllAllyFormByDamage(StatForGameplay healer,float damage, float healPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                HealingByDamage(healer, ally, damage, healPercent);
            }
        }

    }
    private void HealAllAllyFormAndCleanseDebuff(StatForGameplay healer, float healPercent, int DebuffsQuantity)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                RegenHeal(healer, ally, healPercent);
                CleanseDebuff(ally, DebuffsQuantity);
            }
        }

    }
    private void HealAllAllyFormAndCleanseDebuffAndHealingMaxHp(StatForGameplay healer, float healPercent, int DebuffsQuantity, float percentHp)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                RegenHeal(healer, ally, healPercent);
                int quantityDebuff = CleanseDebuff(ally, DebuffsQuantity);
                HealByMaxHp(healer, ally, percentHp * quantityDebuff);
            }
        }

    }

    private void HealAllAllyFormAndBuffAndCleanseDebuff(StatForGameplay healer, float healPercent, int DebuffsQuantity, List<string> listBuffs)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                RegenHeal(healer, ally, healPercent);
                AddBuff(ally, listBuffs);
                CleanseDebuff(ally, DebuffsQuantity);
            }
        }

    }
    private void HealAndAddShiledByMaxHpAllAllyForm(StatForGameplay healer, float healPercent, float percentMaxHp)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == healer.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                RegenHeal(healer, ally, healPercent);
                AddShieldByMaxHp(ally, percentMaxHp);
            }
        }
        
    }
    private void AddShiledByMaxHpOfGiverAllAllyForm(StatForGameplay giver, float percentMaxHp)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        for (int i = 0; i < 5; i++)
        {
            StatForGameplay ally = listAllies.FirstOrDefault(l => l.position == i);
            if (ally != null)
            {
                AddShieldByMaxHpOfGiver(giver, ally, percentMaxHp);
            }
        }

    }
    private void AddShieldBehindForm(StatForGameplay giver, float hpPercent)
    {
        List<StatForGameplay> recieveres = new();
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        switch (giver.position)
        {
            case 0:
                if(listAllies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 2));
                }
                if (listAllies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 3));
                }
                break;
            case 1:
                if (listAllies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 3));
                }
                if (listAllies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 4));
                }
                break;
            default:
                break;
        }
        foreach (StatForGameplay reciever in recieveres) AddShieldByMaxHpOfGiver(giver,reciever, hpPercent);
    }
    private void AddShieldBehindFormWithBuff(StatForGameplay giver, float hpPercent, List<string> listBuffs)
    {
        List<StatForGameplay> recieveres = new();
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        switch (giver.position)
        {
            case 0:
                if (listAllies.FirstOrDefault(l => l.position == 2) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 2));
                }
                if (listAllies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 3));
                }
                break;
            case 1:
                if (listAllies.FirstOrDefault(l => l.position == 3) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 3));
                }
                if (listAllies.FirstOrDefault(l => l.position == 4) != null)
                {
                    recieveres.Add(listAllies.FirstOrDefault(l => l.position == 4));
                }
                break;
            default:
                break;
        }
        foreach (StatForGameplay reciever in recieveres)
        {
            AddShieldByMaxHpOfGiver(giver,reciever, hpPercent);
            AddBuff(reciever, listBuffs);
        }
            
    }
    private void AddShiledByMaxHpOfGiverAllyWithLowerHPPercentForm(StatForGameplay giver, float maxHpPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        float minPercentHp = 1f;
        foreach (StatForGameplay ally in listAllies)
        {

            float PercentHp = ally.HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).HealPoint;
            if (PercentHp < minPercentHp)
            {
                allies = new();
                minPercentHp = PercentHp;
                allies.Add(ally);
            }
            else if (PercentHp == minPercentHp)
            {
                allies.Add(ally);
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        AddShieldByMaxHpOfGiver(giver, allies[0], maxHpPercent);

    }
    private void AddEnergyRandomForm(StatForGameplay giver, float energyPoint)
    {
        StatForGameplay reciever = new();

        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        int numberRandom;
        do
        {
            numberRandom = GenerateRandomNumber(0, 4);
            reciever = listAllies.FirstOrDefault(l => l.position == numberRandom);
        } while (reciever == null);
        AddEnergy(reciever,energyPoint);


    }
    private bool CheckAllyHaveLowerHp(StatForGameplay giver, float hpLowerPercent)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint >0);
        if (listAllies.Count == 0) return false;

        for (int i = 0; i < listAllies.Count; i++)
        {
           
            if (listAllies[i].HealPoint / ListObjectForGameplay.list.FirstOrDefault(l => l.team == listAllies[i].team && l.position == listAllies[i].position).HealPoint <= hpLowerPercent)
            {
                return true;
            }
        }
        return false;
    }
    private StatForGameplay FindAllyHighestMaxHp(StatForGameplay giver)
    {
        List<StatForGameplay> listAllies = ListObjectForGameplay.list.FindAll(l => l.team == giver.team);
        List<StatForGameplay> allies = new();

        float maxHp = 0f;
        foreach (StatForGameplay ally in listAllies)
        {

            
            if (maxHp < ally.HealPoint)
            {
                allies = new();
                maxHp = ally.HealPoint;
                allies.Add(listClone.FirstOrDefault(l=>l.team == ally.team && l.position == ally.position));
            }
            else if (maxHp == ally.HealPoint)
            {
                allies.Add(listClone.FirstOrDefault(l => l.team == ally.team && l.position == ally.position));
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        return allies[0];
    }
    private StatForGameplay FindAllyLowestMaxHp(StatForGameplay giver)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team);
        List<StatForGameplay> allies = new();

        float maxHp = 999999f;
        foreach (StatForGameplay ally in listAllies)
        {

            
            if (maxHp > ally.HealPoint)
            {
                allies = new();
                maxHp = ally.HealPoint;
                allies.Add(listClone.FirstOrDefault(l => l.team == ally.team && l.position == ally.position));
            }
            else if (maxHp == ally.HealPoint)
            {
                allies.Add(listClone.FirstOrDefault(l => l.team == ally.team && l.position == ally.position));
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        return allies[0];
    }
    private StatForGameplay FindAllyHighestAttack(StatForGameplay giver)
    {
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team);
        List<StatForGameplay> allies = new();

        float highestAttack = 0f;
        foreach (StatForGameplay ally in listAllies)
        {


            if (highestAttack < ally.Attack)
            {
                allies = new();
                highestAttack = ally.Attack;
                allies.Add(ally);
            }
            else if (highestAttack == ally.HealPoint)
            {
                allies.Add(ally);
            }
        }
        
        return allies[0];
    }
    private StatForGameplay FindAllyHighestMaxHpExceptYourself(StatForGameplay giver)
    {
        List<StatForGameplay> listAllies = ListObjectForGameplay.list.FindAll(l => l.team == giver.team && l.position!= giver.position);
        List<StatForGameplay> allies = new();

        float maxHp = 0f;
        foreach (StatForGameplay ally in listAllies)
        {


            if (maxHp < ally.HealPoint)
            {
                allies = new();
                maxHp = ally.HealPoint;
                allies.Add(listClone.FirstOrDefault(l => l.team == ally.team && l.position == ally.position));
            }
            else if (maxHp == ally.HealPoint)
            {
                allies.Add(listClone.FirstOrDefault(l => l.team == ally.team && l.position == ally.position));
            }
        }
        allies.Sort((x, y) => x.HealPoint.CompareTo(y.HealPoint));
        if (allies.Count == 0) return null;
        return allies[0];
    }
    private List<StatForGameplay> FindAlliesFront(StatForGameplay giver)
    {
        if (giver == null) return new();
        List<StatForGameplay> listAllies = listClone.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        if(giver.position == 2)
        {
            if (listAllies.FirstOrDefault(l => l.position == 0) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 0));
        }
        else if (giver.position == 3)
        {
            if(listAllies.FirstOrDefault(l => l.position == 0) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 0));

            if (listAllies.FirstOrDefault(l => l.position == 1) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 1));
        }
        else if (giver.position == 4)
        {
            if (listAllies.FirstOrDefault(l => l.position == 1) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 1));
        }
        return allies;
    }
    private List<StatForGameplay> FindAlliesBehind(StatForGameplay giver)
    {
        List<StatForGameplay> listAllies = ListObjectForGameplay.list.FindAll(l => l.team == giver.team && l.HealPoint > 0);
        List<StatForGameplay> allies = new();

        if (giver.position == 0)
        {
            if (listAllies.FirstOrDefault(l => l.position == 2) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 2));
            if (listAllies.FirstOrDefault(l => l.position == 3) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 3));
        }
        else if (giver.position == 1)
        {
            if (listAllies.FirstOrDefault(l => l.position == 3) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 3));
            if (listAllies.FirstOrDefault(l => l.position == 4) != null) allies.Add(listAllies.FirstOrDefault(l => l.position == 4));
        }
        return allies;
    }
    private void UpdateHpBar()
    {
        foreach (StatForGameplay list in listClone)
        {
            if (list.team == 1)
            {
                if (list.position == 0)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P1 = Team1.transform.Find("P1").gameObject;
                    GameObject HealBarUI = P1.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 1)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P2 = Team1.transform.Find("P2").gameObject;
                    GameObject HealBarUI = P2.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 2)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P3 = Team1.transform.Find("P3").gameObject;
                    GameObject HealBarUI = P3.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 3)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P4 = Team1.transform.Find("P4").gameObject;
                    GameObject HealBarUI = P4.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 4)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P5 = Team1.transform.Find("P5").gameObject;
                    GameObject HealBarUI = P5.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
            }
            else if (list.team == 2)
            {
                if (list.position == 0)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P1 = Team2.transform.Find("P1").gameObject;
                    GameObject HealBarUI = P1.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 1)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P2 = Team2.transform.Find("P2").gameObject;
                    GameObject HealBarUI = P2.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 2)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P3 = Team2.transform.Find("P3").gameObject;
                    GameObject HealBarUI = P3.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 3)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P4 = Team2.transform.Find("P4").gameObject;
                    GameObject HealBarUI = P4.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 4)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P5 = Team2.transform.Find("P5").gameObject;
                    GameObject HealBarUI = P5.transform.Find("HealBar UI(Clone)").gameObject;
                    GameObject HealBar = HealBarUI.transform.Find("HealBar").gameObject;
                    HealBar.GetComponent<HealbarUpdate>().UpdateHealbar(list.HealPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
            }


        }
    }
    private void UpdateEnergyBar()
    {
        foreach (StatForGameplay list in listClone)
        {
            if (list.team == 1)
            {
                if (list.position == 0)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P1 = Team1.transform.Find("P1").gameObject;
                    GameObject EnergyBarUI = P1.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 1)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P2 = Team1.transform.Find("P2").gameObject;
                    GameObject EnergyBarUI = P2.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 2)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P3 = Team1.transform.Find("P3").gameObject;
                    GameObject EnergyBarUI = P3.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 3)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P4 = Team1.transform.Find("P4").gameObject;
                    GameObject EnergyBarUI = P4.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 4)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P5 = Team1.transform.Find("P5").gameObject;
                    GameObject EnergyBarUI = P5.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
            }
            else if (list.team == 2)
            {
                if (list.position == 0)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P1 = Team2.transform.Find("P1").gameObject;
                    GameObject EnergyBarUI = P1.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 1)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P2 = Team2.transform.Find("P2").gameObject;
                    GameObject EnergyBarUI = P2.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 2)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P3 = Team2.transform.Find("P3").gameObject;
                    GameObject EnergyBarUI = P3.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 3)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P4 = Team2.transform.Find("P4").gameObject;
                    GameObject EnergyBarUI = P4.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
                else if (list.position == 4)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P5 = Team2.transform.Find("P5").gameObject;
                    GameObject EnergyBarUI = P5.transform.Find("EnergyBar UI(Clone)").gameObject;
                    GameObject EnergyBar = EnergyBarUI.transform.Find("EnergyBar").gameObject;
                    EnergyBar.GetComponent<EnergyBarUpdate>().UpdateEnergybar(list.EnergyPoint, 100);
                }
            }


        }
    }
    private void UpdateShieldBar()
    {
        foreach (StatForGameplay list in listClone)
        {
            if (list.team == 1)
            {
                if (list.position == 0)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P1 = Team1.transform.Find("P1").gameObject;
                    GameObject ShieldBarUI = P1.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 1)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P2 = Team1.transform.Find("P2").gameObject;
                    GameObject ShieldBarUI = P2.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 2)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P3 = Team1.transform.Find("P3").gameObject;
                    GameObject ShieldBarUI = P3.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 3)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P4 = Team1.transform.Find("P4").gameObject;
                    GameObject ShieldBarUI = P4.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 4)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P5 = Team1.transform.Find("P5").gameObject;
                    GameObject ShieldBarUI = P5.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
            }
            else if (list.team == 2)
            {
                if (list.position == 0)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P1 = Team2.transform.Find("P1").gameObject;
                    GameObject ShieldBarUI = P1.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 1)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P2 = Team2.transform.Find("P2").gameObject;
                    GameObject ShieldBarUI = P2.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 2)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P3 = Team2.transform.Find("P3").gameObject;
                    GameObject ShieldBarUI = P3.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 3)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P4 = Team2.transform.Find("P4").gameObject;
                    GameObject ShieldBarUI = P4.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
                else if (list.position == 4)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P5 = Team2.transform.Find("P5").gameObject;
                    GameObject ShieldBarUI = P5.transform.Find("ShieldBar UI(Clone)").gameObject;
                    GameObject ShieldBar = ShieldBarUI.transform.Find("ShieldBar").gameObject;
                    ShieldBar.GetComponent<ShieldBarUpdate>().UpdateShieldbar(list.ShieldPoint, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).HealPoint);
                }
            }


        }
    }
    private void UpdateEffectPanel()
    {
        foreach (StatForGameplay list in listClone)
        {
            if (list.team == 1)
            {
                if (list.position == 0)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P1 = Team1.transform.Find("P1").gameObject;
                    GameObject EffectUI = P1.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 1)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P2 = Team1.transform.Find("P2").gameObject;
                    GameObject EffectUI = P2.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 2)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P3 = Team1.transform.Find("P3").gameObject;
                    GameObject EffectUI = P3.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 3)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P4 = Team1.transform.Find("P4").gameObject;
                    GameObject EffectUI = P4.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 4)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P5 = Team1.transform.Find("P5").gameObject;
                    GameObject EffectUI = P5.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
            }
            else if (list.team == 2)
            {
                if (list.position == 0)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P1 = Team2.transform.Find("P1").gameObject;
                    GameObject EffectUI = P1.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 1)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P2 = Team2.transform.Find("P2").gameObject;
                    GameObject EffectUI = P2.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 2)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P3 = Team2.transform.Find("P3").gameObject;
                    GameObject EffectUI = P3.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 3)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P4 = Team2.transform.Find("P4").gameObject;
                    GameObject EffectUI = P4.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
                else if (list.position == 4)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P5 = Team2.transform.Find("P5").gameObject;
                    GameObject EffectUI = P5.transform.Find("EffectUI(Clone)").gameObject;
                    GameObject ListEffect = EffectUI.transform.Find("ListEffect").gameObject;
                    List<string> listEf = new();
                    listEf.AddRange(list.ListBuff);
                    listEf.AddRange(list.ListDebuff);
                    ListEffect.GetComponent<EffectPanelUpdate>().EffectUpdate(listEf);
                }
            }


        }
    }
    private void UpdateSpeedBar()
    {
        foreach (StatForGameplay list in listClone)
        {
            if (list.team == 1)
            {
                if (list.position == 0)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P1 = Team1.transform.Find("P1").gameObject;
                    GameObject SpeedBarUI = P1.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    //Debug.Log(list.Speed + " " + ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 1)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P2 = Team1.transform.Find("P2").gameObject;
                    GameObject SpeedBarUI = P2.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 2)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P3 = Team1.transform.Find("P3").gameObject;
                    GameObject SpeedBarUI = P3.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 3)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P4 = Team1.transform.Find("P4").gameObject;
                    GameObject SpeedBarUI = P4.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 4)
                {
                    GameObject Team1 = transform.Find("Team1").gameObject;
                    GameObject P5 = Team1.transform.Find("P5").gameObject;
                    GameObject SpeedBarUI = P5.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
            }
            else if (list.team == 2)
            {
                if (list.position == 0)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P1 = Team2.transform.Find("P1").gameObject;
                    GameObject SpeedBarUI = P1.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 1)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P2 = Team2.transform.Find("P2").gameObject;
                    GameObject SpeedBarUI = P2.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 2)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P3 = Team2.transform.Find("P3").gameObject;
                    GameObject SpeedBarUI = P3.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 3)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P4 = Team2.transform.Find("P4").gameObject;
                    GameObject SpeedBarUI = P4.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
                else if (list.position == 4)
                {
                    GameObject Team2 = transform.Find("Team2").gameObject;
                    GameObject P5 = Team2.transform.Find("P5").gameObject;
                    GameObject SpeedBarUI = P5.transform.Find("SpeedBarUI(Clone)").gameObject;
                    GameObject SpeedBar = SpeedBarUI.transform.Find("SpeedBar").gameObject;
                    SpeedBar.GetComponent<SpeedBarUpdate>().UpdateSpeedbar(list.Speed, ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position).Speed);
                }
            }


        }
    }
    private void RemoveBuffAndDebuff(StatForGameplay attacker)
    {
        List<string> listEffect = new();
        foreach (string item in attacker.ListBuff)
        {
            for (int i = 1; i < 5; i++)
            {
                if (item.Contains("#" + i.ToString()))
                {
                    listEffect.Add(item.Replace("#" + i.ToString(), "#" + (i - 1).ToString()));
                    break;
                }
            }
        }
        for (int i = 0; i < listEffect.Count; i++)
        {
            attacker.ListBuff[i] = listEffect[i];
        }
        attacker.ListBuff.RemoveAll(l => l.Contains("#0"));
        listEffect = new();
        foreach (string item in attacker.ListDebuff)
        {
            for (int i = 1; i < 5; i++)
            {
                if (item.Contains("#" + i.ToString()))
                {
                    listEffect.Add(item.Replace("#" + i.ToString(), "#" + (i - 1).ToString()));
                    break;
                }
            }
        }
        for (int i = 0; i < listEffect.Count; i++)
        {
            attacker.ListDebuff[i] = listEffect[i];
        }
        attacker.ListDebuff.RemoveAll(l => l.Contains("#0"));
    }
    private void UpdateStat()
    {
        foreach (StatForGameplay list in listClone)
        {
            float attackPercent = 1f;
            float defendPercent = 1f;
            float critPercent = 0f;
            float critDamgagePercent = 0f;
            float accuaracyPercent = 0f;
            float evasionPercent = 0f;
            float physicalResistantPercent = 0f;
            float magicalResistantPercent = 0f;
            float damageAddition = 0f;
            float damageReducion = 0f;
            float penetration = 0f;
            float lifeSteal = 0f;
            float damageReflect = 0f;
            float healingAddition = 0f;
            float healingReducion = 0f;
            float energyAddtion = 0f;
            if (list.ListBuff.Count > 0)
            {
                if (list.ListBuff.FirstOrDefault(l => l.Contains("attackUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("attackUp"));
                    foreach (string item in ListBuffs)
                    {
                        attackPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("defendUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("defendUp"));
                    foreach (string item in ListBuffs)
                    {
                        defendPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("accuracyUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("accuracyUp"));
                    foreach (string item in ListBuffs)
                    {
                        accuaracyPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("evasionUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("evasionUp"));
                    foreach (string item in ListBuffs)
                    {
                        evasionPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("physicalResistantUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("physicalResistantUp"));
                    foreach (string item in ListBuffs)
                    {
                        physicalResistantPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("magicalResistantUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("magicalResistantUp"));
                    foreach (string item in ListBuffs)
                    {
                        magicalResistantPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("damageAdditionUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("damageAdditionUp"));
                    foreach (string item in ListBuffs)
                    {
                        damageAddition += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("damageReducionUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("damageReducionUp"));
                    foreach (string item in ListBuffs)
                    {
                        damageReducion += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("lifeStealUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("lifeStealUp"));
                    foreach (string item in ListBuffs)
                    {
                        lifeSteal += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("damageReflectUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("damageReflectUp"));
                    foreach (string item in ListBuffs)
                    {
                        damageReflect += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("healingAdditionUp")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("healingAdditionUp"));
                    foreach (string item in ListBuffs)
                    {
                        healingAddition += float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
            }
            if (list.ListDebuff.Count > 0)
            {
                if (list.ListDebuff.FirstOrDefault(l => l.Contains("attackDown")) != null)
                {
                    List<string> ListDebuffs = list.ListDebuff.FindAll(l => l.Contains("attackDown"));
                    foreach (string item in ListDebuffs)
                    {
                        attackPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListDebuff.FirstOrDefault(l => l.Contains("defendDown")) != null)
                {
                    List<string> ListDebuffs = list.ListDebuff.FindAll(l => l.Contains("defendUp"));
                    foreach (string item in ListDebuffs)
                    {
                        defendPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("accuracyDown")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("accuracyDown"));
                    foreach (string item in ListBuffs)
                    {
                        accuaracyPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListBuff.FirstOrDefault(l => l.Contains("evasionDown")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("evasionDown"));
                    foreach (string item in ListBuffs)
                    {
                        evasionPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("physicalResistantDown")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("physicalResistantDown"));
                    foreach (string item in ListBuffs)
                    {
                        physicalResistantPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
                if (list.ListBuff.FirstOrDefault(l => l.Contains("magicalResistantDown")) != null)
                {
                    List<string> ListBuffs = list.ListBuff.FindAll(l => l.Contains("magicalResistantDown"));
                    foreach (string item in ListBuffs)
                    {
                        magicalResistantPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }


                if (list.ListDebuff.FirstOrDefault(l => l.Contains("damageAdditionDown")) != null)
                {
                    List<string> ListDebuffs = list.ListDebuff.FindAll(l => l.Contains("damageAdditionDown"));
                    foreach (string item in ListDebuffs)
                    {
                        damageAddition -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListDebuff.FirstOrDefault(l => l.Contains("damageReducionDown")) != null)
                {
                    List<string> ListDebuffs = list.ListDebuff.FindAll(l => l.Contains("damageReducionDown"));
                    foreach (string item in ListDebuffs)
                    {
                        damageReducion -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }

                if (list.ListDebuff.FirstOrDefault(l => l.Contains("healingReducionDown")) != null)
                {
                    List<string> ListDebuffs = list.ListDebuff.FindAll(l => l.Contains("healingReducionDown"));
                    foreach (string item in ListDebuffs)
                    {
                        healingReducion -= float.Parse(item.Split("#")[0].Split("x")[1]);
                    }
                }
            }

            if (attackPercent < 0f) attackPercent = 0f;
            if (defendPercent < 0f) defendPercent = 0f;

            StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == list.team && l.position == list.position);
            list.Attack = attacker.Attack * attackPercent;
            list.Deffend = attacker.Deffend * defendPercent;
            list.CritRate = attacker.CritRate + critPercent;
            list.CritDamage = attacker.CritDamage + critDamgagePercent;
            list.Accuracy = attacker.Accuracy + accuaracyPercent;
            list.Evasion = attacker.Evasion + accuaracyPercent;
            list.PhysicalResistance = attacker.PhysicalResistance + physicalResistantPercent;
            list.MagicalResistance = attacker.MagicalResistance + magicalResistantPercent;
            list.DamageAddition = attacker.DamageAddition + damageAddition;
            list.DamageReducion = attacker.DamageReducion + damageReducion;
            list.Penetration = attacker.Penetration + penetration;
            list.LifeSteal = attacker.LifeSteal + lifeSteal;
            list.DamageReflect = attacker.DamageReflect + damageReflect;
            list.HealingAddition = attacker.HealingAddition + healingAddition;
            list.HealingReducion = attacker.HealingReducion + healingReducion;
            list.EnergyAddition = attacker.EnergyAddition + energyAddtion;
            
        }
    }
    private void UpdatePassiveSkillAtStart()
    {
        for (int i = 0; i < listClone.Count; i++)
        {
            StatForGameplay carrier = listClone[i];
            StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
            StatForGameplay ally = new();
            List<string> listDebuffs = new();
            switch (carrier.name)
            {
                case "Cheshire":
                    listDebuffs = new();
                    listDebuffs.Add("stun#1");
                    AttackEnemyWithLowestHPPercentFormWithDebuff(carrier, 1.3f, listDebuffs, "Physical");
                    break;
                case "Kaguya":
                    attacker.Attack += attacker.Attack * 0.1f;
                    break;
                case "Snite":
                    AddShiledByMaxHpOfGiverAllAllyForm(carrier, 0.05f);
                    attacker.Attack += attacker.HealPoint * 0.03f;
                    break;
                case "Aurora":
                    List<StatForGameplay> allies = new();
                    allies = FindAlliesFront(carrier);
                    foreach(StatForGameplay aly in allies)
                    {
                        ListObjectForGameplay.list.FirstOrDefault(l => l.team == aly.team && l.position == aly.position).MagicalResistance += 0.2f;
                    }
                    break;
                case "Jack":
                    listDebuffs = new();
                    listDebuffs.Add("stun#2");
                    AttackEnemyWithHighestAtkFormWithDebuff(carrier, 1.5f, listDebuffs, "True");
                    break;
                case "Quite":
                    attacker.HealPoint += 0.1f * attacker.HealPoint;
                    carrier.HealPoint = attacker.HealPoint;
                    break;
                case "Brunhild":
                    attacker.DamageAddition += attacker.DamageReducion * 1.5f;
                    attacker.DamageReducion = 0;
                    break;
                case "Hildr":
                    attacker.Attack += attacker.Deffend * 2f;
                    attacker.HealPoint += attacker.Deffend * 50f;
                    attacker.Deffend = 0;
                    attacker.HealPoint += attacker.HealPoint * 0.1f;
                    carrier.HealPoint = attacker.HealPoint;
                    break;
                case "Helsing":
                    if (attacker.Accuracy > 1f)
                    {
                        attacker.Attack *= attacker.Accuracy;
                        attacker.Accuracy = 1f;
                    }
                    break;
                case "Mircalla":
                    carrier.HealPoint -= attacker.HealPoint * 0.5f;
                    attacker.Attack += attacker.Attack * 0.3f;
                    attacker.LifeSteal += 0.3f;
                    carrier.ShieldPoint = attacker.Attack * 3f;
                    carrier.PassiveActiveTime[1] = (int)attacker.Attack;
                    carrier.PassiveActiveTime[2] = (int)attacker.Speed;
                    break;
                case "Chang'e":
                    ally = new();
                    ally = FindAllyHighestMaxHp(carrier);
                    ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).Attack += attacker.Attack;
                    ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).LifeSteal += 0.15f;
                    ally = FindAllyLowestMaxHp(carrier);
                    ally.ShieldPoint = attacker.HealPoint * 0.3f;
                    ListObjectForGameplay.list.FirstOrDefault(l => l.team == ally.team && l.position == ally.position).DamageReducion += 0.15f;
                    break;
                case "Befana":
                    carrier.PassiveActiveTime[0] = (int)(attacker.CritRate * 100 * 2.5f + attacker.CritDamage * 100 * 1.5f);
                    attacker.CritRate = 0f;
                    attacker.CritDamage = 0f;
                    break;
                case "Santas":
                    attacker.HealPoint += attacker.HealPoint * 0.2f;
                    attacker.Attack += attacker.Attack * 0.2f;
                    attacker.Deffend += attacker.Deffend * 0.2f;
                    carrier.HealPoint = attacker.HealPoint;
                    break;
                case "Morgiana":
                    if (attacker.Penetration > 1f) attacker.DamageAddition += (attacker.Penetration - 1f);
                    ally = FindAllyHighestMaxHpExceptYourself(carrier);
                    if (ally == null) break;
                    carrier.PassiveActiveTime[2] = ally.position;
                    attacker.Attack += ally.Attack * 0.3f;
                    attacker.Deffend += ally.Attack * 0.3f;
                    carrier.ShieldPoint += ally.HealPoint * 0.3f;
                    break;
                default:
                    break;
            }
        }
        

    }
    private void UpdatePassiveSkillAnyTimes()
    {
        for (int i = 0; i < listClone.Count; i++)
        {
            StatForGameplay carrier = listClone[i];
            StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
            StatForGameplay ally = new();
            
            switch (carrier.name)
            {
                case "Mermaid":
                    if (carrier.ShieldPoint > 0 && carrier.PassiveActiveTime[0] == 0)
                    {
                        attacker.DamageReducion += 0.2f;
                        carrier.PassiveActiveTime[0] = 1;
                    }
                    else if (carrier.ShieldPoint <= 0 && carrier.PassiveActiveTime[0] == 1)
                    {
                        attacker.DamageReducion -= 0.2f;
                        carrier.PassiveActiveTime[0] = 0;
                    }
                    break;
                case "Pigy":
                    if (carrier.PassiveActiveTime[0] == 1)
                    {
                        if (CheckAllyHaveLowerHp(carrier, 0.5f))
                        {
                            carrier.HealPoint -= carrier.HealPoint * 0.25f;
                            if (carrier.HealPoint <= 0) carrier.HealPoint = 1;
                            AddShiledByMaxHpOfGiverAllyWithLowerHPPercentForm(carrier, 0.5f);
                            carrier.PassiveActiveTime[0] = 0;
                        }

                    }
                    break;
                case "Jeon":
                    if (carrier.HealPoint<=attacker.HealPoint*0.5f && carrier.PassiveActiveTime[2] == 0)
                    {
                        attacker.Evasion += 0.4f;
                        carrier.PassiveActiveTime[2] = 1;

                    }
                    else if (carrier.HealPoint > attacker.HealPoint * 0.5f && carrier.PassiveActiveTime[2] == 1)
                    {
                        attacker.Evasion -= 0.4f;
                        carrier.PassiveActiveTime[2] = 0;

                    }
                    break;
                case "Brunhild":
                    attacker.DamageReducion = 1 - carrier.HealPoint / attacker.HealPoint;
                    if (attacker.DamageReducion > 0.6f) attacker.DamageReducion = 0.6f;
                    break;
                case "Hildr":
                    if (carrier.HealPoint <= 0 && carrier.PassiveActiveTime[2] == 1)
                    {
                        carrier.HealPoint = attacker.HealPoint * 0.5f;
                        AddEnergy(carrier, 50);
                        carrier.PassiveActiveTime[2] = 0;
                    }
                    break;
                case "Mircalla":
                    attacker.Attack = carrier.PassiveActiveTime[1] * (((2 - carrier.HealPoint / attacker.HealPoint) > 1.5f)?1.5f: (2 - carrier.HealPoint / attacker.HealPoint));
                    attacker.Speed = carrier.PassiveActiveTime[2] * (((0.5f + carrier.HealPoint / (attacker.HealPoint*2) ) < 0.75f) ? 0.75f : (0.5f + carrier.HealPoint / (attacker.HealPoint * 2)));
                    break;
                case "Befana":

                    attacker.LifeSteal = 0f;
                    
                    if (carrier.ShieldPoint > 0)
                    {
                        HealingByDamage(carrier, carrier, carrier.ShieldPoint, 1f);
                        carrier.ShieldPoint = 0;
                    }
                    break;
                case "Morgiana":
                    
                    ally = listClone.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.PassiveActiveTime[2] && l.HealPoint>0);
                    if (ally == null) break;
                    if (carrier.HealPoint <= 0)
                    {
                        carrier.HealPoint = 1;
                        carrier.ShieldPoint = ally.HealPoint;
                        ally.HealPoint = 0;
                    }
                    break;
                default:
                    
                    break;
            }
            

        }
        

    }

    private void UpdatePassiveSkillAfterAttack(StatForGameplay carrier)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        switch (carrier.name)
        {
            case "Robin":
                if (carrier.PassiveActiveTime[0] <10)
                {
                    List<string> listBuffs = new();
                    listBuffs.Add("attackUpx0.02");
                    AddBuff(carrier, listBuffs);
                    carrier.PassiveActiveTime[0]++;
                }
                break;
            case "Cinderella":
                if (GenerateRandomNumber(1, 5) == 1)
                {
                    AddEnergyRandomForm(carrier, 20f);
                }
                break;
            
            case "Yuki":
                CleanseBuff(carrier, 2);
                break;
            case "Helsing":
                if (carrier.PassiveActiveTime[1] < 21 && carrier.PassiveActiveTime[0]==1)
                {
                    
                    attacker.Speed -= attacker.Speed * 0.02f;
                    carrier.PassiveActiveTime[1]++;
                    carrier.PassiveActiveTime[0] = 0;
                }
                break;
            default:
                break;
        }

    }

    private float UpdateDamageOfPassiveSkill(StatForGameplay carrier, StatForGameplay reciever , float damage,bool isCrit,bool isEvasion)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        float AdditionDamage;
        List<string> listDebuffs = new();
        List<StatForGameplay> listAllies = new();
        switch (carrier.name)
        {
            case "Esme":
                if (reciever.ListDebuff.FirstOrDefault(l=>l.Contains("burn")) != null)
                {
                    damage = damage + damage * 0.1f;
                }
                break;
            case "Tamamo":
                listDebuffs.Add("magicalResistantDownx0.1#3");
                AddDebuff(reciever, listDebuffs);
                break;
            case "Alice":
                StatForGameplay Enemy = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position);
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if(reciever.HealPoint - damage < Enemy.HealPoint * 0.2f)
                    {
                        reciever.HealPoint = 0;
                        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().AnotherTextExport("EXCUTE",getPosition(reciever), Color.white);
                    }
                    
                    carrier.PassiveActiveTime[0] = 0;
                }
                else if (carrier.PassiveActiveTime[1] == 1)
                {
                    if (reciever.HealPoint - damage < Enemy.HealPoint * 0.05f)
                    {
                        reciever.HealPoint = 0;
                        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().AnotherTextExport("EXCUTE", getPosition(reciever), Color.white);
                    }

                    carrier.PassiveActiveTime[1] = 0;
                }
                break;
            case "Quinter":
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if(reciever.ListDebuff.FindAll(l=>l.Contains("speedDown")).Count >= 3)
                    {
                        listDebuffs.Add("stun#2");
                        AddDebuff(reciever, listDebuffs);
                    }
                    else
                    {
                        listDebuffs.Add("speedDownx0.3#2");
                        AddDebuff(reciever, listDebuffs);
                    }
                    carrier.PassiveActiveTime[0] = 0;
                }
                listDebuffs.Add("magicalResistantDownx0.1#3");
                AddDebuff(reciever, listDebuffs);
                break;
            case "Jeon":
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if (reciever.ShieldPoint > 0)
                    {
                        reciever.ShieldPoint = 0;
                        GameObject.Find("DamageUI").transform.Find("TextControler").gameObject.GetComponent<DamageTextUpdate>().AnotherTextExport("SHIELD REMOVED", getPosition(reciever), Color.white);
                    }
                    
                    carrier.PassiveActiveTime[0] = 0;
                }
                break;
            case "Mist":
                AddShieldByDamage(carrier, damage, 0.2f);


                break;
            case "Skuld":
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if (reciever.ListDebuff.FindAll(l=>l.Contains("burn")).Count > 0)
                    {
                        damage += damage * 0.1f * reciever.ListDebuff.FindAll(l => l.Contains("burn")).Count;
                    }
                    
                    
                    carrier.PassiveActiveTime[0] = 0;
                }
                listDebuffs.Add("burn#3");
                AddDebuff(reciever, listDebuffs);
                break;
            case "Hildr":
                AdditionDamage = 0;
                if (carrier.PassiveActiveTime[0] > 0)
                {
                    
                    AdditionDamage = carrier.PassiveActiveTime[0] + carrier.ShieldPoint * 0.5f;
                    AdditionDamage -= reciever.Deffend;
                    AdditionDamage -= AdditionDamage * reciever.PhysicalResistance;
                    AdditionDamage += AdditionDamage * (carrier.DamageAddition - reciever.DamageReducion);
                    if (AdditionDamage < 0) AdditionDamage = 0;
                    carrier.ShieldPoint = 0;
                    carrier.PassiveActiveTime[0] = 0;
                    
                }
                
                if (GenerateRandomNumber(1, 10) < 5)
                {
                    
                    listDebuffs.Add("evasionx0.5#3");
                    AddDebuff(reciever, listDebuffs);
                    damage = (damage + AdditionDamage) * 1.3f;
                }
                else damage = damage + AdditionDamage;
                break;
            case "Helsing":
                if(reciever.ListDebuff.FindAll(l => l.Contains("healingReducion")).Count == 0)
                {
                    listDebuffs.Add("healingReducionDownx0.5#2");
                    AddDebuff(reciever, listDebuffs);
                }
                break;
            case "Mircalla":
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if(carrier.HealPoint > attacker.HealPoint * 0.05f)
                    {
                        DealTrueDamageFromOtherResource(carrier, reciever, attacker.HealPoint * 0.1f);
                        carrier.HealPoint -= attacker.HealPoint * 0.05f;
                    }
                    carrier.PassiveActiveTime[0] = 0;


                }
                break;
            case "Santas":
                int number = GenerateRandomNumber(1, 3);
                if(number == 1)
                {
                    listDebuffs.Add("attackDownx0.2#1");
                    AddDebuff(reciever, listDebuffs);
                }
                else if (number == 2)
                {
                    listDebuffs.Add("defendDownx0.2#1");
                    AddDebuff(reciever, listDebuffs);
                }else if (number == 3)
                {
                    reciever.ShieldPoint = 0;
                }

                break;
            case "Morgiana":
                AdditionDamage = 0;
                StatForGameplay ally = listClone.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.PassiveActiveTime[2]);
                if (ally == null)
                {
                    carrier.PassiveActiveTime[0] = 0;
                    carrier.PassiveActiveTime[1] = 0;
                    break;
                }
                if (carrier.PassiveActiveTime[0] == 1)
                {
                    if (ally.HealPoint > 0)
                    {
                        AdditionDamage += ally.HealPoint * 0.2f;
                        ally.HealPoint -= AdditionDamage;
                        if (ally.HealPoint <= 0) ally.HealPoint = 1;
                        DealTrueDamageFromOtherResource(carrier, reciever, AdditionDamage);
                    }
                    carrier.PassiveActiveTime[0] = 0;
                }
                else if (carrier.PassiveActiveTime[1] == 1)
                {
                    if (ally.HealPoint > 0)
                    {
                        HealingByDamage(carrier, ally,damage, 1f);
                    }
                    carrier.PassiveActiveTime[1] = 0;
                }

                break;
            default:
                break;
        }
        listDebuffs = new();
        switch (reciever.name)
        {
            case "Quinter":
                listDebuffs.Add("speedDownx0.3#2");
                AddDebuff(carrier, listDebuffs);
                break;
            case "Jeon":
                if(isEvasion == true && reciever.PassiveActiveTime[1] <= 10)
                {
                    HealByMaxHp(reciever, reciever, 0.12f);
                    ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position).Attack *= 1.05f;
                    reciever.PassiveActiveTime[1]++;
                }
                break;
            case "Hildr":
                if (reciever.PassiveActiveTime[1] > 0)
                {
                    reciever.PassiveActiveTime[0] += (int)damage;
                }
                break;
            case "Chang'e":
                CleanseAllDebuff(reciever);
                break;
            case "Santas":
                listAllies = FindAlliesBehind(reciever);
                foreach(StatForGameplay ally in listAllies)
                {
                    HealingByDamage(reciever, ally, damage, 0.2f);
                }
                listAllies = new();
                break;
        }
        if(FindAlliesFront(reciever).FindAll(l=>l.name == "Santas").Count > 0)
        {
            
            listAllies = FindAlliesFront(reciever).FindAll(l => l.name == "Santas");
            foreach(StatForGameplay ally in listAllies)
            {
                ally.HealPoint -= damage * 0.2f;
                damage -= damage * 0.2f;
            }
        }
        return damage;

    }
    
    private float UpdateHeailngOfPassiveSkill(StatForGameplay carrier, StatForGameplay reciever, float healing)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        List<string> listBuffs = new();
        switch (carrier.name)
        {
            case "Dorothy":
                AddShieldByMaxHp(reciever, 0.1f);
                break;
            case "Eir":
                AttackNormalFormFromOtherResource(carrier, healing, 0.3f);
                break;
            case "Chang'e":
                if (carrier.PassiveActiveTime[0] == 1 && carrier.PassiveActiveTime[1] <= 5)
                {
                    attacker.HealingAddition += 0.1f;
                    carrier.PassiveActiveTime[1]++;
                    carrier.PassiveActiveTime[0] = 0;
                }
                break;
            case "Santas":
                int number = GenerateRandomNumber(1, 3);
                if (number == 1)
                {
                    listBuffs.Add("attackUpx0.1#1");
                    AddBuff(reciever, listBuffs);
                }
                else if (number == 2)
                {
                    listBuffs.Add("defendUpx0.1#1");
                    AddBuff(reciever, listBuffs);
                }
                else if (number == 3)
                {
                    AddShieldByMaxHp(reciever, 0.1f);
                    
                }

                break;
            default:
                break;
        }
        return healing;

    }
    private void UpdateEquipmentActiveAtStart()
    {
        for (int i = 0; i < listClone.Count; i++)
        {
            StatForGameplay carrier = listClone[i];
            StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
            List<EquipmentForGameplay> listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == carrier.team && l.position == carrier.position);
            StatForGameplay statAddition = new();
            foreach (EquipmentForGameplay item in listEquip)
            {
                switch (item.Name)
                {
                    case "CrusaderSword":
                        if (attacker.Attack >= 230)
                        {
                            statAddition.DamageAddition += 0.13f;
                        }
                        break;
                    case "LightforgeSword":
                        if (attacker.Attack >= 230)
                        {
                            statAddition.Attack += attacker.Attack * 0.13f;
                        }
                        break;
                    case "HighlordSpear":
                        if (attacker.Attack >= 150 && attacker.HealPoint >= 1300 && attacker.Deffend >= 70)
                        {
                            statAddition.Attack += attacker.Attack * 0.07f;
                            statAddition.HealPoint += attacker.HealPoint * 0.07f;
                            statAddition.Deffend += attacker.HealPoint * 0.07f;
                        }
                        break;
                    case "MarksmanBow":
                        if (attacker.CritRate >= 0.8)
                        {
                            statAddition.CritRate += 0.10f;
                            statAddition.CritDamage += 0.50f;
                        }
                        break;
                    case "MasterAxe":

                        statAddition.Attack += attacker.Attack * 0.05f;
                        statAddition.HealPoint += attacker.HealPoint * 0.05f;

                        break;
                    case "RabbitChain":
                        statAddition.HealPoint += attacker.HealPoint * 0.1f;
                        break;
                    case "FalconeyeCrossbow":
                        if (attacker.CritRate >= 0.75f)
                        {
                            attacker.Penetration += 0.25f;
                        }
                        break;
                    case "Masamune":
                        attacker.Speed *= 0.75f;
                        break;
                    case "NightswornArmor":
                        statAddition.Evasion += 0.8f;
                        break;
                    case "GoldenDress":
                        statAddition.DamageAddition += 0.30f;
                        break;
                    case "Thunderblade":
                        attacker.Speed *= 0.65f;
                        break;
                    case "CyberhexSweeper":
                        if(attacker.Penetration >= 1f)
                        {
                            item.PassiveActiveTime[0] = 1;
                        }
                        break;
                    case "JeonWooChiPole":
                        statAddition.DamageReducion += 0.3f;


                        break;
                    default:
                        break;
                }
            }
            attacker.HealPoint += statAddition.HealPoint;
            attacker.Attack += statAddition.Attack;
            attacker.Deffend += statAddition.Deffend;
            attacker.CritRate += statAddition.CritRate;
            attacker.CritDamage += statAddition.CritDamage;
            attacker.Evasion += statAddition.Evasion;
            attacker.Accuracy += statAddition.Accuracy;
            attacker.PhysicalResistance += statAddition.PhysicalResistance;
            attacker.DamageAddition += statAddition.DamageAddition;
            attacker.DamageReducion += statAddition.DamageReducion;
            attacker.LifeSteal += statAddition.LifeSteal;
            attacker.HealingAddition += statAddition.HealingAddition;
            attacker.EnergyAddition += statAddition.EnergyAddition;
        }

        
        

    }
    private void UpdateEquipmentActiveAnyTimes()
    {

        for (int i = 0; i < listClone.Count; i++)
        {
            StatForGameplay carrier = listClone[i];
            StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
            List<EquipmentForGameplay> listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == carrier.team && l.position == carrier.position);
            StatForGameplay statAddition = new();
            List<string> listBuffs = new();
            foreach (EquipmentForGameplay item in listEquip)
            {
                switch (item.Name)
                {
                    case "ValorArmor":
                        if (carrier.HealPoint <= attacker.HealPoint*0.3f && item.PassiveActiveTime[0]==1)
                        {
                            carrier.HealPoint += attacker.HealPoint * 0.3f;
                            AddEnergy(carrier, 60);
                            item.PassiveActiveTime[0] = 0;
                        }
                        break;
                    case "Muramasa":
                        if (carrier.HealPoint <= attacker.HealPoint * 0.4f && item.PassiveActiveTime[0] == 0)
                        {
                            attacker.LifeSteal += 0.3f;
                            item.PassiveActiveTime[0] = 1;
                        }
                        else if (carrier.HealPoint > attacker.HealPoint * 0.4f && item.PassiveActiveTime[0] == 1)
                        {
                            attacker.LifeSteal -= 0.3f;
                            item.PassiveActiveTime[0] = 0;
                        }
                        break;
                    case "JusticarRobes":
                        if(CheckAllyHaveLowerHp(carrier,0.3f) && item.PassiveActiveTime[0] == 1)
                        {
                            HealAllyWithLowerHPPercentFormByUserMaxHp(carrier, 0.3f);
                            listBuffs.Add("taunt#4");
                            AddBuff(carrier, listBuffs);
                            listBuffs = new();
                            AddShieldByMaxHp(carrier, 0.3f);
                            item.PassiveActiveTime[0] = 0;
                        }
                        break;
                    case "GoldenDress":
                        if (carrier.HealPoint > attacker.HealPoint * 0.5f && item.PassiveActiveTime[0] == 0)
                        {
                            statAddition.DamageReducion += 0.30f;
                            statAddition.DamageAddition -= 0.30f;
                            item.PassiveActiveTime[0] = 1;
                        }
                        else if(carrier.HealPoint <= attacker.HealPoint * 0.5f && item.PassiveActiveTime[0] == 1)
                        {
                            statAddition.DamageAddition += 0.30f;
                            statAddition.DamageReducion -= 0.30f;
                            item.PassiveActiveTime[0] = 0;
                        }
                        break;
                    case "JeonWooChiPole":
                        if(carrier.ShieldPoint<=0 && item.PassiveActiveTime[0] == 0)
                        {
                            statAddition.DamageReducion -= 0.3f;
                            item.PassiveActiveTime[0] = 1;
                        }
                        else if(carrier.ShieldPoint > 0 && item.PassiveActiveTime[0] == 1)
                        {
                            statAddition.DamageReducion += 0.3f;
                            item.PassiveActiveTime[0] = 0;
                        }
                        
                        
                        break;
                    case "AngelicRegalia":
                        if(carrier.HealPoint<= 0)
                        {
                            if(GenerateRandomNumber(1,2)==1 && item.PassiveActiveTime[0] == 0)
                            {
                                carrier.HealPoint = attacker.HealPoint * 0.7f;
                                AddEnergy(carrier, 50);
                            }
                            else
                            {
                                item.PassiveActiveTime[0] = 1;
                            }
                        }
                        else if (carrier.HealPoint > 0)
                        {
                            item.PassiveActiveTime[0] = 0;
                        }


                        break;
                    default:
                        break;
                }
            }
            attacker.HealPoint += statAddition.HealPoint;
            attacker.Attack += statAddition.Attack;
            attacker.Deffend += statAddition.Deffend;
            attacker.CritRate += statAddition.CritRate;
            attacker.CritDamage += statAddition.CritDamage;
            attacker.Evasion += statAddition.Evasion;
            attacker.Accuracy += statAddition.Accuracy;
            attacker.PhysicalResistance += statAddition.PhysicalResistance;
            attacker.DamageAddition += statAddition.DamageAddition;
            attacker.DamageReducion += statAddition.DamageReducion;
            attacker.LifeSteal += statAddition.LifeSteal;
            attacker.HealingAddition += statAddition.HealingAddition;
            attacker.EnergyAddition += statAddition.EnergyAddition;
        }
    }
    private float UpdateDamageOfEquipmentActive(StatForGameplay carrier, StatForGameplay reciever, float damage, bool isCrit, bool isEvasion)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        List<EquipmentForGameplay> listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == carrier.team && l.position == carrier.position);
        float damageAddition;
        StatForGameplay enemy;
        List<string> listDebuffs = new();
        List<string> listBuffs = new();
        List<StatForGameplay> listAllies = new();
        StatForGameplay statAddition = new();
        foreach (EquipmentForGameplay item in listEquip)
        {
            switch (item.Name)
            {
                case "RabbitChain":
                    if(GenerateRandomNumber(1,2)==1 && item.PassiveActiveTime[0] <= 10)
                    {
                        attacker.Deffend *= 1.08f;
                        item.PassiveActiveTime[0]++;
                    }
                    break;
                case "Masamune":
                    if (GenerateRandomNumber(1, 2) == 1)
                    {
                        DealTrueDamageFromOtherResource(carrier, reciever, reciever.HealPoint * 0.1f);
                    }
                    break;
                case "StaffofOz":
                    if (GenerateRandomNumber(1, 2) ==1)
                    {
                        AddEnergy(carrier, 20);
                    }

                    break;
                case "NightblossomKimono":
                    CleanseDebuff(carrier, 3);

                    break;
                case "Souldrinker":
                    enemy = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position);
                    damageAddition = enemy.HealPoint * 0.08f;
                    DealTrueDamageFromOtherResource(carrier, reciever, damageAddition);
                    HealingByDamage(carrier, carrier, damageAddition, 1f);
                    damageAddition = 0;
                    enemy = new();
                    break;
                case "Thunderblade":
                    if(GenerateRandomNumber(1,10) < 4)
                    {
                        carrier.Speed = 0;
                    }
                    break;
                case "CyberhexSweeper":
                    if (item.PassiveActiveTime[0] == 1)
                    {
                        DealTrueDamageFromOtherResource(carrier, reciever, damage * 0.35f);
                    }
                    break;
                case "ShadowboundCleaver":
                    listDebuffs.Add("damageReducionDownx0.15#3");
                    AddDebuff(reciever, listDebuffs);
                    listDebuffs = new();
                    break;
                case "HalberdofConquest":
                    enemy = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position);
                    float damagePercent = (1-(reciever.HealPoint / enemy.HealPoint));
                    if (damagePercent < 0) damagePercent = 0;
                    damage *= (1 + damagePercent);
                    break;
                case "JeonWooChiPole":
                    float chance = 10;
                    chance += attacker.HealPoint / 200;
                    if (GenerateRandomNumber(1, 100) < chance + 1)
                    {
                        AddShieldByMaxHp(carrier, 0.25f);
                    }
                    break;
                default:
                    break;
            }
        }
        if(attacker!= null)
        {
            attacker.HealPoint += statAddition.HealPoint;
            attacker.Attack += statAddition.Attack;
            attacker.Deffend += statAddition.Deffend;
            attacker.CritRate += statAddition.CritRate;
            attacker.CritDamage += statAddition.CritDamage;
            attacker.Evasion += statAddition.Evasion;
            attacker.Accuracy += statAddition.Accuracy;
            attacker.PhysicalResistance += statAddition.PhysicalResistance;
            attacker.DamageAddition += statAddition.DamageAddition;
            attacker.DamageReducion += statAddition.DamageReducion;
            attacker.LifeSteal += statAddition.LifeSteal;
            attacker.HealingAddition += statAddition.HealingAddition;
            attacker.EnergyAddition += statAddition.EnergyAddition;
        }
        

        StatForGameplay taker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position);
        listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == reciever.team && l.position == reciever.position);
        statAddition = new();
        listDebuffs = new();
        listBuffs = new();
        foreach (EquipmentForGameplay item in listEquip)
        {
            switch (item.Name)
            {
                case "ShadowboundPlate":
                    listDebuffs.Add("attackDownx0.1#2");
                    AddDebuff(carrier, listDebuffs);
                    listDebuffs = new();
                    break;
                case "SylvanCape":
                    if (isEvasion)
                    {
                        HealByMaxHp(reciever, reciever, 0.08f);
                    }
                    break;
                case "NightswornArmor":
                    if (isEvasion && item.PassiveActiveTime[0]<10)
                    {
                        statAddition.Evasion -= 0.06f;
                        item.PassiveActiveTime[0]++;
                    }
                    break;
                case "HellswornArmor":
                   
                    if (damage>=(reciever.HealPoint+reciever.ShieldPoint) && item.PassiveActiveTime[0] ==1)
                    {
                        
                        damage = reciever.HealPoint + reciever.ShieldPoint;
                        reciever.ShieldPoint += reciever.HealPoint + taker.HealPoint * 1f;
                        reciever.HealPoint = 1;
                        listBuffs.Add("damageAdditionUpx1#4");
                        listBuffs.Add("lifeStealUpUpx1#4");
                        AddBuff(reciever, listBuffs);
                        listBuffs = new();
                        item.PassiveActiveTime[0] = 0;

                    }
                    break;
                case "EternalArmor":
                    if (damage >= taker.HealPoint * 0.15f)
                    {
                        damage = taker.HealPoint * 0.15f;

                    }
                    break;
                case "AdamantiumArmor":
                    if (GenerateRandomNumber(1,2)==1 && item.PassiveActiveTime[0]>0)
                    {
                        HealByMaxHp(reciever, reciever, 0.35f);
                        AddEnergy(reciever, 60);
                        item.PassiveActiveTime[0]--;
                    }
                    break;
                case "DragonBoneArmor":
                    if (GenerateRandomNumber(1, 2) == 1 && item.PassiveActiveTime[0] < 8)
                    {
                        taker.Attack *= 1.1f;
                        taker.Deffend *= 1.1f;
                        item.PassiveActiveTime[0]++;
                    }
                    break;
                default:
                    break;
            }
        }
        if(taker!= null)
        {
            taker.HealPoint += statAddition.HealPoint;
            taker.Attack += statAddition.Attack;
            taker.Deffend += statAddition.Deffend;
            taker.CritRate += statAddition.CritRate;
            taker.CritDamage += statAddition.CritDamage;
            taker.Evasion += statAddition.Evasion;
            taker.Accuracy += statAddition.Accuracy;
            taker.PhysicalResistance += statAddition.PhysicalResistance;
            taker.DamageAddition += statAddition.DamageAddition;
            taker.DamageReducion += statAddition.DamageReducion;
            taker.LifeSteal += statAddition.LifeSteal;
            taker.HealingAddition += statAddition.HealingAddition;
            taker.EnergyAddition += statAddition.EnergyAddition;
        }
        
        return damage;

    }

    private float UpdateDamageReflectOfEquipmentActive(StatForGameplay carrier, StatForGameplay reciever, float damage)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        List<EquipmentForGameplay> listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == carrier.team && l.position == carrier.position);
        float AdditionDamage;
        List<string> listDebuffs = new();
        List<StatForGameplay> listAllies = new();
        StatForGameplay statAddition = new();
        foreach (EquipmentForGameplay item in listEquip)
        {
            switch (item.Name)
            {
                
                default:
                    break;
            }
        }
        attacker.HealPoint += statAddition.HealPoint;
        attacker.Attack += statAddition.Attack;
        attacker.Deffend += statAddition.Deffend;
        attacker.CritRate += statAddition.CritRate;
        attacker.CritDamage += statAddition.CritDamage;
        attacker.Evasion += statAddition.Evasion;
        attacker.Accuracy += statAddition.Accuracy;
        attacker.PhysicalResistance += statAddition.PhysicalResistance;
        attacker.DamageAddition += statAddition.DamageAddition;
        attacker.DamageReducion += statAddition.DamageReducion;
        attacker.LifeSteal += statAddition.LifeSteal;
        attacker.HealingAddition += statAddition.HealingAddition;
        attacker.EnergyAddition += statAddition.EnergyAddition;

        StatForGameplay taker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == reciever.team && l.position == reciever.position);
        listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == reciever.team && l.position == reciever.position);
        statAddition = new();
        listDebuffs = new();
        foreach (EquipmentForGameplay item in listEquip)
        {
            switch (item.Name)
            {
                case "ImmortalArmor":
                    HealingByDamage(reciever, reciever, damage, 1f);
                    break;
                
                default:
                    break;
            }
        }
        taker.HealPoint += statAddition.HealPoint;
        taker.Attack += statAddition.Attack;
        taker.Deffend += statAddition.Deffend;
        taker.CritRate += statAddition.CritRate;
        taker.CritDamage += statAddition.CritDamage;
        taker.Evasion += statAddition.Evasion;
        taker.Accuracy += statAddition.Accuracy;
        taker.PhysicalResistance += statAddition.PhysicalResistance;
        taker.DamageAddition += statAddition.DamageAddition;
        taker.DamageReducion += statAddition.DamageReducion;
        taker.LifeSteal += statAddition.LifeSteal;
        taker.HealingAddition += statAddition.HealingAddition;
        taker.EnergyAddition += statAddition.EnergyAddition;
        return damage;

    }
    private float UpdateHealingOfEquipmentActive(StatForGameplay carrier, StatForGameplay reciever, float healing)
    {
        StatForGameplay attacker = ListObjectForGameplay.list.FirstOrDefault(l => l.team == carrier.team && l.position == carrier.position);
        List<EquipmentForGameplay> listEquip = ListObjectForGameplay.listequipment.FindAll(l => l.team == carrier.team && l.position == carrier.position);

        List<string> listDebuffs = new();
        List<StatForGameplay> listAllies = new();
        StatForGameplay statAddition = new();
        foreach (EquipmentForGameplay item in listEquip)
        {
            switch (item.Name)
            {
                case "HeartbreakStaff":
                    if (carrier.team == reciever.team && carrier.position == reciever.position) break;
                    if (GenerateRandomNumber(1,2)==1)
                    {
                        AddEnergy(reciever, 60);
                    }
                    break;
                default:
                    break;
            }
        }
        attacker.HealPoint += statAddition.HealPoint;
        attacker.Attack += statAddition.Attack;
        attacker.Deffend += statAddition.Deffend;
        attacker.CritRate += statAddition.CritRate;
        attacker.CritDamage += statAddition.CritDamage;
        attacker.Evasion += statAddition.Evasion;
        attacker.Accuracy += statAddition.Accuracy;
        attacker.PhysicalResistance += statAddition.PhysicalResistance;
        attacker.DamageAddition += statAddition.DamageAddition;
        attacker.DamageReducion += statAddition.DamageReducion;
        attacker.LifeSteal += statAddition.LifeSteal;
        attacker.HealingAddition += statAddition.HealingAddition;
        attacker.EnergyAddition += statAddition.EnergyAddition;

        
        return healing;

    }
    private float UpdateSpeed(StatForGameplay attacker, List<string> listBuffs, List<string> listDebuffs)
    {
        float speedPercent = 0f;
        if (listBuffs.FirstOrDefault(l => l.Contains("speedUp")) != null)
        {
            List<string> ListBuffss = listBuffs.FindAll(l => l.Contains("speedUp"));
            foreach (string item in ListBuffss)
            {
                speedPercent += float.Parse(item.Split("#")[0].Split("x")[1]);
            }
        }
        if (listDebuffs.FirstOrDefault(l => l.Contains("speedDown")) != null)
        {
            List<string> ListDebuffss = listDebuffs.FindAll(l => l.Contains("speedDown"));
            foreach (string item in ListDebuffss)
            {
                speedPercent -= float.Parse(item.Split("#")[0].Split("x")[1]);
            }
        }
        if (speedPercent > 0.9f) speedPercent = 0.9f;
        return attacker.Speed - attacker.Speed * speedPercent;
    }
    private void UpdateDamageFromBurn(StatForGameplay attacker, List<string> listBuffs, List<string> listDebuffs)
    {
        int burnCount = listDebuffs.FindAll(l => l.Contains("burn")).Count;

        float damage = DealTrueDamageByMaxHp(attacker, 0.03f * burnCount);
        List<StatForGameplay> listSkuld = listClone.FindAll(l => l.team != attacker.team && l.name == "Skuld");
        foreach(StatForGameplay Skuld in listSkuld)
        {
            HealingByDamage(Skuld, Skuld, damage, 0.1f);
        }
    }
    private void UpdateDamageFromTime(StatForGameplay reciever, float percentHp)
    {
        float damage = DealTrueDamageByMaxHp(reciever,percentHp);
    }

    private void DealDamageFromTime()
    {
        realTime += Time.deltaTime;
        
        float percentHpLost = 0;
        if (count > 20 && count <= 40) percentHpLost = 0.02f;
        else if (count > 40 && count <= 60) percentHpLost = 0.05f;
        else if (count > 60 && count <= 80) percentHpLost = 0.11f;
        else if (count > 80 && count <= 100) percentHpLost = 0.14f;
        else if (count > 100 && count <= 120) percentHpLost = 0.2f;
        else if (count > 120 && count <= 130) percentHpLost = 0.25f;
        else if (count > 130 && count <= 140) percentHpLost = 0.3f;
        else if (count > 140 && count <= 150) percentHpLost = 0.35f;
        else if (count > 150 && count <= 160) percentHpLost = 0.4f;
        else if (count > 160) percentHpLost = 0.5f;
        if ((int)(realTime / 5f) > count)
        {
            count++;
            if (percentHpLost > 0)
            {
                foreach (StatForGameplay list in listClone)
                {
                    if(list.HealPoint > 0)
                    {
                        UpdateDamageFromTime(list, percentHpLost);
                    }
                    
                }
            }
        }
            
        
    }
}
