using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameObject EquipmentUI = GameObject.Find("EquipmentUI");
        GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
        EquipmentDetail.gameObject.SetActive(true);
        EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject = gameObject;
        for (int i = 0; i < GameObject.Find("EnemiesMainForm").transform.childCount; i++)
        {
            GameObject.Find("EnemiesMainForm").transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
