using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatTablePop : MonoBehaviour
{
    public GameObject tableObject;
    public int index;
    private void OnMouseOver()
    {
        if (GameObject.Find("ShopMenu").GetComponent<Shop>().isEquip || GameObject.Find("ShopMenu").GetComponent<Shop>().isForm) return;
        tableObject.GetComponent<ItemStatTableUpdate>().itemObject = gameObject;
        tableObject.SetActive(true);
    }
    private void OnMouseDown()
    {
        if (GameObject.Find("ShopMenu").GetComponent<Shop>().isEquip || GameObject.Find("ShopMenu").GetComponent<Shop>().isForm) return;
        GameObject shopObject = GameObject.Find("ShopMenu");
        shopObject.GetComponent<Shop>().BuyItem(index);
        tableObject.GetComponent<ItemStatTableUpdate>().itemObject = null;
        tableObject.SetActive(false);
    }
    private void OnMouseExit()
    {
        if (GameObject.Find("ShopMenu").GetComponent<Shop>().isEquip || GameObject.Find("ShopMenu").GetComponent<Shop>().isForm) return;
        tableObject.GetComponent<ItemStatTableUpdate>().itemObject = null;
        tableObject.SetActive(false);
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
