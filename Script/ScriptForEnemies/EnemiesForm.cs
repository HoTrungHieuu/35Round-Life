using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemiesForm : MonoBehaviour
{
    private List<Vector3> EnemiesInteamPosition = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        GameObject FormationUI = GameObject.Find("FormationUI");
        GameObject Formate = FormationUI.transform.Find("Formate").gameObject;
        Formate.SetActive(true);
        CharaterTeamInitial();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        PutEnemiesInteam(Generalspecifications.Round);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PutEnemiesInteam(int index)
    {
        
        GameObject AllEnemiesTeam = GameObject.Find("AllEnemiesTeam");
        GameObject EnemiesTeamC = AllEnemiesTeam.transform.Find("EnemiesTeamC" + index.ToString()).gameObject;
        for (int i = 0; i < 5; i++)
        {
            GameObject P = EnemiesTeamC.transform.Find("P"+i.ToString()).gameObject;
            if (P.transform.childCount == 0) continue;
            GameObject instance = Instantiate(P.transform.GetChild(0).gameObject, EnemiesInteamPosition[i], Quaternion.identity);   
            GameObject EnemiesMainForm = GameObject.Find("EnemiesMainForm");
            GameObject _P = EnemiesMainForm.transform.Find("P" + i.ToString()).gameObject;
            instance.transform.SetParent(_P.transform);
            instance.transform.position = EnemiesInteamPosition[i];
            instance.AddComponent<BoxCollider2D>();
            instance.AddComponent<EnemiesOnClick>();
            instance.SetActive(true);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
            instance.transform.localScale = new Vector3(-1f, 1f, 1f);
            bool isAccessory = false;
            for (int j =0;j< P.transform.GetChild(0).childCount; j++)
            {

                GameObject item = _P.transform.GetChild(0).GetChild(j).gameObject;
                item.SetActive(false);
                item.AddComponent<BoxCollider2D>();
                item.AddComponent<ItemPosisionOfCharacter>();
                if(item.GetComponent<ItemStat>().Type == "Weapon")
                {
                    item.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = 1;
                }
                else if (item.GetComponent<ItemStat>().Type == "Amor")
                {
                    item.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = 2;
                }
                else if (item.GetComponent<ItemStat>().Type == "Accessory" && isAccessory == false)
                {
                    item.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = 3;
                    isAccessory = true;
                }
                else if (item.GetComponent<ItemStat>().Type == "Accessory" && isAccessory == true)
                {
                    item.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = 4;
                }

            }
        }
    }
    public void BackBtnInEquip()
    {
        GameObject EquipmentUI = GameObject.Find("EquipmentUI");
        GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
        EquipmentDetail.gameObject.SetActive(false);
        for (int i =0;i< GameObject.Find("EnemiesMainForm").transform.childCount; i++)
        {
            GameObject.Find("EnemiesMainForm").transform.GetChild(i).gameObject.SetActive(true);
        }
        GameObject ItemStatTable = GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().itemObject = null;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay = 0;
        ItemStatTable.SetActive(false);
    }
    public void StartBtn()
    {
        GameObject Bag = GameObject.Find("Bag");
        GameObject ObjectRandom = Bag.transform.Find("ObjectRandom").gameObject;
        for (int i = 0; i < ObjectRandom.transform.childCount; i++)
        {
            ListObjectForInsert.listRandom[i] = null;
            Destroy(ObjectRandom.transform.GetChild(i).gameObject);
        }
        DontDestroyOnLoad(Bag);
        GameObject EnemyTeam = new GameObject("EnemyTeam");
        GameObject instance = Instantiate(GameObject.Find("EnemiesMainForm").gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        ListObjectForInsert.EnemyTeam = instance;
        instance.transform.SetParent(EnemyTeam.transform);
        instance.SetActive(false);
        DontDestroyOnLoad(EnemyTeam);
        SceneManager.LoadScene("SampleScene");
    }
    public void BackBtn()
    {
        GameObject Bag = GameObject.Find("Bag");
        DontDestroyOnLoad(Bag);
        SceneManager.LoadScene("MainShop");
    }
    private void CharaterTeamInitial()
    {

        EnemiesInteamPosition.Add(new Vector3(3, 2, 0));
        EnemiesInteamPosition.Add(new Vector3(3, -2, 0));
        EnemiesInteamPosition.Add(new Vector3(6, 3, 0));
        EnemiesInteamPosition.Add(new Vector3(6, 0, 0));
        EnemiesInteamPosition.Add(new Vector3(6, -3, 0));

    }
}
