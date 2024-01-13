using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : MonoBehaviour
{
    public int posision;
    public int type;
    private void OnMouseDown()
    {
        if (GameObject.Find("ShopMenu").GetComponent<Shop>().isEquip || GameObject.Find("ShopMenu").GetComponent<Shop>().isForm) return;
        if(GameObject.Find("ShopMenu").GetComponent<Shop>().typeSelected == type && GameObject.Find("ShopMenu").GetComponent<Shop>().positionSelected == posision)
        {
            GameObject.Find("ShopMenu").GetComponent<Shop>().positionSelected = 0;
            GameObject.Find("ShopMenu").GetComponent<Shop>().typeSelected = 0;
        }
        else
        {
            GameObject.Find("ShopMenu").GetComponent<Shop>().positionSelected = posision;
            GameObject.Find("ShopMenu").GetComponent<Shop>().typeSelected = type;
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
