using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticUpdate : MonoBehaviour
{
    public GameObject damageDealBar;
    private List<Vector3> team1Position = new();
    private List<Vector3> team2Position = new();
    public void UpdateStatisticTable(List<StatForGameplay> listTeam1, List<StatForGameplay> listTeam2)
    {
        for (int i = 0; i< listTeam1.Count; i++)
        {
            listTeam1.Sort((y, x) => x.TotalDamageDeal.CompareTo(y.TotalDamageDeal));
            
            GameObject DamageDealTotal = transform.Find("Team1").Find("DamageDealTotal").gameObject;
            GameObject instance = Instantiate(damageDealBar, team1Position[i], Quaternion.identity);
            instance.transform.SetParent(DamageDealTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam1[i].TotalDamageDeal, listTeam1[0].TotalDamageDeal);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam1[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam1[i].TotalDamageDeal);
            instance.transform.localPosition = team1Position[i];
            instance.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;

            listTeam1.Sort((y, x) => x.TotalDamageTake.CompareTo(y.TotalDamageTake));
            GameObject DamageTakeTotal = transform.Find("Team1").Find("DamageTakeTotal").gameObject;
            instance = Instantiate(damageDealBar, team1Position[i], Quaternion.identity);
            instance.transform.SetParent(DamageTakeTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam1[i].TotalDamageTake, listTeam1[0].TotalDamageTake);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam1[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam1[i].TotalDamageTake);
            instance.transform.localPosition = team1Position[i];
            instance.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;

            listTeam1.Sort((y, x) => x.TotalHealing.CompareTo(y.TotalHealing));
            GameObject HealTotal = transform.Find("Team1").Find("HealTotal").gameObject;
            instance = Instantiate(damageDealBar, team1Position[i], Quaternion.identity);
            instance.transform.SetParent(HealTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam1[i].TotalHealing, listTeam1[0].TotalHealing);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam1[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam1[i].TotalHealing);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeColor(Color.green);
            instance.transform.localPosition = team1Position[i];
            instance.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;
        }

        for (int i = 0; i < listTeam2.Count; i++)
        {
            listTeam2.Sort((y, x) => x.TotalDamageDeal.CompareTo(y.TotalDamageDeal));
            GameObject DamageDealTotal = transform.Find("Team2").Find("DamageDealTotal").gameObject;
            GameObject instance = Instantiate(damageDealBar, team2Position[i], Quaternion.identity);
            instance.transform.SetParent(DamageDealTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam2[i].TotalDamageDeal, listTeam2[0].TotalDamageDeal);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam2[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam2[i].TotalDamageDeal);
            instance.transform.localPosition = team2Position[i];
            instance.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;
            instance.transform.GetChild(0).GetChild(1).transform.Rotate(Vector3.up, 180f);

            listTeam2.Sort((y, x) => x.TotalDamageTake.CompareTo(y.TotalDamageTake));
            GameObject DamageTakeTotal = transform.Find("Team2").Find("DamageTakeTotal").gameObject;
            instance = Instantiate(damageDealBar, team2Position[i], Quaternion.identity);
            instance.transform.SetParent(DamageTakeTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam2[i].TotalDamageTake, listTeam2[0].TotalDamageTake);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam2[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam2[i].TotalDamageTake);
            instance.transform.localPosition = team2Position[i];
            instance.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;
            instance.transform.GetChild(0).GetChild(1).transform.Rotate(Vector3.up, 180f);

            listTeam2.Sort((y, x) => x.TotalHealing.CompareTo(y.TotalHealing));
            GameObject HealTotal = transform.Find("Team2").Find("HealTotal").gameObject;
            instance = Instantiate(damageDealBar, team2Position[i], Quaternion.identity);
            instance.transform.SetParent(HealTotal.transform);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().UpdateDamagebar(listTeam2[i].TotalHealing, listTeam2[0].TotalHealing);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNameText(listTeam2[i].name);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeNumber(listTeam2[i].TotalHealing);
            instance.transform.GetChild(0).GetChild(1).GetComponent<DamgeBarUpdate>().ChangeColor(Color.green);
            instance.transform.localPosition = team2Position[i];
            instance.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().alignment = TMPro.TextAlignmentOptions.TopRight;
            instance.transform.GetChild(0).GetChild(1).transform.Rotate(Vector3.up, 180f);
        }
    }

    private void InitialTeam1Position()
    {
        for (int i = 80; i > -121; i -= 50)
        {
            team1Position.Add(new Vector3(-105, i, 0));
        }
    }

    private void InitialTeam2Position()
    {
        for (int i = 80; i > -121; i -= 50)
        {
            team2Position.Add(new Vector3(160, i, 0));
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        
        InitialTeam1Position();
        InitialTeam2Position();
        GameObject Team1 = transform.Find("Team1").gameObject;
        Team1.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team1.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team1.transform.Find("HealTotal").gameObject.SetActive(false);
        GameObject Team2 = transform.Find("Team2").gameObject;
        Team2.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team2.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team2.transform.Find("HealTotal").gameObject.SetActive(false);
        transform.Find("StatisticText").GetComponent<TextMeshProUGUI>().text = "Damage Deal";
        gameObject.SetActive(false);
        /*List<StatForGameplay> listTeam1 = new();
        List<StatForGameplay> listTeam2 = new();
        listTeam1.Add(new StatForGameplay()
        {
            name = "a",
            TotalDamageDeal = 5,
            TotalDamageTake = 3,
            TotalHealing =2,
        });
        listTeam1.Add(new StatForGameplay()
        {
            name = "b",
            TotalDamageDeal = 2,
            TotalDamageTake = 4,
            TotalHealing = 5,
        });
        listTeam1.Add(new StatForGameplay()
        {
            name = "c",
            TotalDamageDeal = 1,
            TotalDamageTake = 2,
            TotalHealing = 4,
        });
        listTeam2.Add(new StatForGameplay()
        {
            name = "a",
            TotalDamageDeal = 5,
            TotalDamageTake = 3,
            TotalHealing = 2,
        });
        listTeam2.Add(new StatForGameplay()
        {
            name = "b",
            TotalDamageDeal = 2,
            TotalDamageTake = 4,
            TotalHealing = 5,
        });
        listTeam2.Add(new StatForGameplay()
        {
            name = "c",
            TotalDamageDeal = 1,
            TotalDamageTake = 2,
            TotalHealing = 4,
        });
        UpdateStatisticTable(listTeam1, listTeam2);*/
    }


    public void DamageDealBtn()
    {
        GameObject Team1 = transform.Find("Team1").gameObject;
        Team1.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team1.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team1.transform.Find("HealTotal").gameObject.SetActive(false);
        GameObject Team2 = transform.Find("Team2").gameObject;
        Team2.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team2.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team2.transform.Find("HealTotal").gameObject.SetActive(false);
        transform.Find("StatisticText").GetComponent<TextMeshProUGUI>().text = "Damage Deal";
    }

    public void DamageTakeBtn()
    {
        GameObject Team1 = transform.Find("Team1").gameObject;
        Team1.transform.Find("DamageDealTotal").gameObject.SetActive(false);
        Team1.transform.Find("DamageTakeTotal").gameObject.SetActive(true);
        Team1.transform.Find("HealTotal").gameObject.SetActive(false);
        GameObject Team2 = transform.Find("Team2").gameObject;
        Team2.transform.Find("DamageDealTotal").gameObject.SetActive(false);
        Team2.transform.Find("DamageTakeTotal").gameObject.SetActive(true);
        Team2.transform.Find("HealTotal").gameObject.SetActive(false);
        transform.Find("StatisticText").GetComponent<TextMeshProUGUI>().text = "Damage Take";
    }

    public void HealingBtn()
    {
        GameObject Team1 = transform.Find("Team1").gameObject;
        Team1.transform.Find("DamageDealTotal").gameObject.SetActive(false);
        Team1.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team1.transform.Find("HealTotal").gameObject.SetActive(true);
        GameObject Team2 = transform.Find("Team2").gameObject;
        Team2.transform.Find("DamageDealTotal").gameObject.SetActive(false);
        Team2.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team2.transform.Find("HealTotal").gameObject.SetActive(true);
        transform.Find("StatisticText").GetComponent<TextMeshProUGUI>().text = "Healing";
    }

    public void ExitBtn()
    {
        GameObject Team1 = transform.Find("Team1").gameObject;
        Team1.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team1.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team1.transform.Find("HealTotal").gameObject.SetActive(false);
        GameObject Team2 = transform.Find("Team2").gameObject;
        Team2.transform.Find("DamageDealTotal").gameObject.SetActive(true);
        Team2.transform.Find("DamageTakeTotal").gameObject.SetActive(false);
        Team2.transform.Find("HealTotal").gameObject.SetActive(false);
        transform.Find("StatisticText").GetComponent<TextMeshProUGUI>().text = "Damage Deal";
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
