using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAdjustRelyOnItem : MonoBehaviour
{
    public GameObject gameObject;
    public int Index;
    public TextMeshProUGUI textMeshProObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> listItem = gameObject.GetComponent<Shop>().listObjectsRandom;
        if (listItem.Count < Index) return;
        if (listItem.Count > 0)
        {
            if(listItem[Index] == null)
            {
                textMeshProObject.text = "";
            }
            else
            {
                if(listItem[Index].GetComponent<Stat>() != null)
                {
                    textMeshProObject.text = GameObject.Find("ShopMenu").GetComponent<Shop>().moneyCount(listItem[Index]).ToString() + "G";
                    if (listItem[Index].GetComponent<Stat>().isDiscount)
                    {
                        textMeshProObject.text += "\n Discount";
                    }

                }
                else if(listItem[Index].GetComponent<ItemStat>() != null)
                {
                    
                    textMeshProObject.text = GameObject.Find("ShopMenu").GetComponent<Shop>().moneyCount(listItem[Index]).ToString() + "G";
                    if (listItem[Index].GetComponent<ItemStat>().isDiscount)
                    {
                        textMeshProObject.text += "\n Discount";
                    }
                }
                
            }
            
        }
    }
}
