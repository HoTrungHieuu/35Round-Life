using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InsertObjectTeam2 : MonoBehaviour
{
    public GameObject[] objects = new GameObject[5];
    public GameObject[] ItemObjects = new GameObject[20];
    private List<Vector3> positions = new();
    public GameObject healBar;
    public GameObject energyBar;
    public GameObject shieldBar;
    public GameObject EffectPanel;
    public GameObject speedBar;
    // Start is called before the first frame update
    void Awake()
    {
        objectsInitial();
        GameObject[] gameObjectArray = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            gameObjectArray[i] = new GameObject("P" + (i + 1));
            gameObjectArray[i].transform.SetParent(this.transform);
        }
        positions.Add(new Vector3(3, 2, 0));
        positions.Add(new Vector3(3, -2, 0));
        positions.Add(new Vector3(6, 3, 0));
        positions.Add(new Vector3(6, 0, 0));
        positions.Add(new Vector3(6, -3, 0));
        for (int i = 0; i < 5; i++)
        {
            if (objects[i] == null) continue;
            GameObject instance = Instantiate(objects[i], positions[i], Quaternion.identity);
            instance.transform.SetParent(gameObjectArray[i].transform);
            instance.transform.localScale = new Vector3(-1, 1, 0);
            instance.transform.position = positions[i];
            GameObject ItemObjectss = new GameObject("Equipment");
            ItemObjectss.transform.SetParent(gameObjectArray[i].transform);
            StatFromEquipment statFromEquipment = new()
            {
                HealPoint = 0,
                Attack = 0,
                Deffend = 0,
                CritRate = 0,
                CritDamage = 0,
                Accuracy = 0,
                Evasion = 0,
            };
            for (int j = 4 * i; j < (4 * (i + 1) - 1); j++)
            {
                if (ItemObjects[j] == null) continue;
                GameObject instanceItem = Instantiate(ItemObjects[j], positions[i], Quaternion.identity);
                instanceItem.transform.SetParent(ItemObjectss.transform);
                instanceItem.SetActive(false);
                List<int> listPassiveActive = new();
                for (int z = 0; z < ItemObjects[j].GetComponent<ItemStat>().PassiveActiveTime.Count; z++)
                {
                    
                    listPassiveActive.Add(ItemObjects[j].GetComponent<ItemStat>().PassiveActiveTime[z]);


                }
                ListObjectForGameplay.listequipment.Add(new EquipmentForGameplay
                {
                    Name = ItemObjects[j].GetComponent<ItemStat>().Name,
                    team = 2,
                    position = i,
                    PassiveActiveTime = listPassiveActive
                });
                statFromEquipment.HealPoint += ItemObjects[j].GetComponent<ItemStat>().Hp;
                statFromEquipment.Attack += ItemObjects[j].GetComponent<ItemStat>().Att;
                statFromEquipment.Deffend += ItemObjects[j].GetComponent<ItemStat>().Def;
                statFromEquipment.CritRate += ItemObjects[j].GetComponent<ItemStat>().CritRate;
                statFromEquipment.CritDamage += ItemObjects[j].GetComponent<ItemStat>().CritDamage;
                statFromEquipment.Accuracy += ItemObjects[j].GetComponent<ItemStat>().Accuracy;
                statFromEquipment.Evasion += ItemObjects[j].GetComponent<ItemStat>().Evasion;
                statFromEquipment.PhysicalResistance += ItemObjects[j].GetComponent<ItemStat>().PhysicalResistance;
                statFromEquipment.MagicalResistance += ItemObjects[j].GetComponent<ItemStat>().MagicalResistance;
                statFromEquipment.Penetration += ItemObjects[j].GetComponent<ItemStat>().Penetration;
                statFromEquipment.LifeSteal += ItemObjects[j].GetComponent<ItemStat>().LifeSteal;
                statFromEquipment.DamageReflect += ItemObjects[j].GetComponent<ItemStat>().DamageReflect;
                statFromEquipment.HealingAddition += ItemObjects[j].GetComponent<ItemStat>().HealingAddition;
                statFromEquipment.DamageAddition += ItemObjects[j].GetComponent<ItemStat>().DamageAddition;
                statFromEquipment.DamageReducion += ItemObjects[j].GetComponent<ItemStat>().DamageReducion;
                statFromEquipment.EnergyAddition += ItemObjects[j].GetComponent<ItemStat>().EnergyAddition;
            }

            GameObject instance1 = Instantiate(healBar, positions[i], Quaternion.identity);
            instance1.transform.SetParent(gameObjectArray[i].transform);
            GameObject healBarObject = instance1.transform.Find("HealBar").gameObject;
            healBarObject.transform.localPosition = new Vector3(positions[i].x * 73f, positions[i].y * 73f + 100, 0);

            GameObject instance2 = Instantiate(energyBar, positions[i], Quaternion.identity);
            instance2.transform.SetParent(gameObjectArray[i].transform);
            GameObject energyBarObject = instance2.transform.Find("EnergyBar").gameObject;
            energyBarObject.transform.localPosition = new Vector3(positions[i].x * 73f, positions[i].y * 73f + 80, 0);

            GameObject instance3 = Instantiate(shieldBar, positions[i], Quaternion.identity);
            instance3.transform.SetParent(gameObjectArray[i].transform);
            GameObject shieldBarObject = instance3.transform.Find("ShieldBar").gameObject;
            shieldBarObject.transform.localPosition = new Vector3(positions[i].x * 73f, positions[i].y * 73f + 100, 0);

            GameObject instance4 = Instantiate(EffectPanel, positions[i], Quaternion.identity);
            instance4.transform.SetParent(gameObjectArray[i].transform);
            GameObject effectPanelObject = instance4.transform.Find("ListEffect").gameObject;
            effectPanelObject.transform.localPosition = new Vector3(positions[i].x * 73f, positions[i].y * 73f + 150, 0);
            effectPanelObject.GetComponent<EffectPanelUpdate>().icon = GameObject.Find("ListEffects");

            GameObject instance5 = Instantiate(speedBar, positions[i], Quaternion.identity);
            instance5.transform.SetParent(gameObjectArray[i].transform);
            GameObject speedBarObject = instance5.transform.Find("SpeedBar").gameObject;
            speedBarObject.transform.localPosition = new Vector3(positions[i].x * 73f + 100, positions[i].y * 73f, 0);

            ListObjectForGameplay.list.Add(new StatForGameplay
            {
                position = i,
                team = 2,
                name = objects[i].GetComponent<Stat>().Name,
                classType = objects[i].GetComponent<Stat>().Class,
                damageType = objects[i].GetComponent<Stat>().DamageType,
                HealPoint = objects[i].GetComponent<Stat>().Hp + statFromEquipment.HealPoint,
                ShieldPoint = 0,
                EnergyPoint = 0,
                Attack = objects[i].GetComponent<Stat>().Att + statFromEquipment.Attack,
                Deffend = objects[i].GetComponent<Stat>().Def + statFromEquipment.Deffend,
                Speed = objects[i].GetComponent<Stat>().Speed,
                CritRate = objects[i].GetComponent<Stat>().CritRate + statFromEquipment.CritRate,
                CritDamage = objects[i].GetComponent<Stat>().CritDamage + statFromEquipment.CritDamage,
                Accuracy = objects[i].GetComponent<Stat>().Accuracy + statFromEquipment.Accuracy,
                Evasion = objects[i].GetComponent<Stat>().Evasion + statFromEquipment.Evasion,
                PhysicalResistance = objects[i].GetComponent<Stat>().PhysicalResistance + statFromEquipment.PhysicalResistance,
                MagicalResistance = objects[i].GetComponent<Stat>().MagicalResistance + statFromEquipment.MagicalResistance,
                Penetration = objects[i].GetComponent<Stat>().Penetration + statFromEquipment.Penetration,
                LifeSteal = objects[i].GetComponent<Stat>().LifeSteal + statFromEquipment.LifeSteal,
                HealingAddition = 1f + objects[i].GetComponent<Stat>().HealingAddition + statFromEquipment.HealingAddition,
                HealingReducion = 1f,
                DamageAddition = objects[i].GetComponent<Stat>().DamageAddition + statFromEquipment.DamageAddition,
                DamageReducion = objects[i].GetComponent<Stat>().DamageReducion + statFromEquipment.DamageReducion,
                DamageReflect = objects[i].GetComponent<Stat>().DamageReflect + statFromEquipment.DamageReflect,
                EnergyAddition = objects[i].GetComponent<Stat>().EnergyAddition + statFromEquipment.EnergyAddition,
                HealingOnTurn = 0f,
                AttackCount = 1,
                PassiveActiveTime = objects[i].GetComponent<Stat>().PassiveActiveTime.ToList(),
                ListBuff = new(),
                ListDebuff = new(),
                TotalDamageDeal = 0,
                TotalDamageTake = 0,
                TotalHealing = 0,
            }); 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void objectsInitial()
    {

        for (int i = 0; i < 5; i++)
        {
            GameObject P = ListObjectForInsert.EnemyTeam.transform.Find("P" + i.ToString()).gameObject;
            if (P.transform.childCount == 0) continue;
            objects[i] = P.transform.GetChild(0).gameObject;
            for (int j = 0; j < P.transform.GetChild(0).childCount; j++)
            {
                if (P.transform.GetChild(0).GetChild(j).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 1)
                {
                    ItemObjects[i * 4] = P.transform.GetChild(0).GetChild(j).gameObject;
                }
                else if (P.transform.GetChild(0).GetChild(j).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 2)
                {
                    ItemObjects[i * 4 + 1] = P.transform.GetChild(0).GetChild(j).gameObject;
                }
                else if (P.transform.GetChild(0).GetChild(j).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 3)
                {
                    ItemObjects[i * 4 + 2] = P.transform.GetChild(0).GetChild(j).gameObject;
                }
                else if (P.transform.GetChild(0).GetChild(j).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 4)
                {
                    ItemObjects[i * 4 + 3] = P.transform.GetChild(0).GetChild(j).gameObject;
                }

            }
        }

    }
}
