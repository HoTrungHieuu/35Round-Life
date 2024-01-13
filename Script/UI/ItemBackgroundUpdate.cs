using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBackgroundUpdate : MonoBehaviour
{
    public GameObject ItemBackground;
    GameObject ShopMenu;
    // Start is called before the first frame update
    void Start()
    {
        ShopMenu = GameObject.Find("ShopMenu");
        for (int i = 0; i < ShopMenu.GetComponent<Shop>().ItemInventoryPositions.Count - 1; i++)
        {
            GameObject instance = Instantiate(ItemBackground, ShopMenu.GetComponent<Shop>().ItemInventoryPositions[i], Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = ShopMenu.GetComponent<Shop>().ItemInventoryPositions[i] + new Vector3(0, 0, 8);
            

        }
    }

    // Update is called once per frame
    void Update()
    {
        ShopMenu = GameObject.Find("ShopMenu");
        
        for (int i = 0; i < ShopMenu.GetComponent<Shop>().ItemInventoryPositions.Count - 1; i++)
        {
            

            if (ShopMenu.GetComponent<Shop>().listItemInventory[i] == null)
            {
                transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
            else
            {
                switch (ShopMenu.GetComponent<Shop>().listItemInventory[i].GetComponent<ItemStat>().LevelType)
                {
                    case 1:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
                        break;
                    case 2:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0.5f, 0);
                        break;
                    case 3:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0, 0, 1);
                        break;
                    case 4:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.5f, 0, 0.5f);
                        break;
                    case 5:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 0.647f, 0);
                        break;
                    case 6:
                        transform.GetChild(i).transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1f, 0, 0);
                        break;
                }
            }
        }
    }
}
