using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RateTableUpdate : MonoBehaviour
{
    bool isOn = false;
    public void OnOrOff()
    {
        if(isOn)
        {
            isOn = false;
            gameObject.SetActive(false);
            transform.parent.Find("BackGround").gameObject.SetActive(false);
        }
        else if(!isOn)
        {
            isOn = true;
            gameObject.SetActive(true);
            transform.parent.Find("BackGround").gameObject.SetActive(true);
        }
    }
    private void rateTableDetail(int[] num)
    {
        for(int i =0; i<gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = num[i].ToString()+"%";

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        transform.parent.Find("BackGround").gameObject.SetActive(false);
        rateTableDetail(GameObject.Find("ShopMenu").GetComponent<Shop>().CheckRate(Generalspecifications.Round));
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
