using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectEquipment : MonoBehaviour
{
    public int posisionItem;
    public int posisionCharater;
    public int positionInEquip;
    private void OnMouseDown()
    {

        GameObject ShopMenu = GameObject.Find("ShopMenu");
        ShopMenu.GetComponent<Shop>().positionItemInEquip = posisionItem;
        ShopMenu.GetComponent<Shop>().positionInEquipSelected = positionInEquip;
        ShopMenu.GetComponent<Shop>().SelectItemInEquip();
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
