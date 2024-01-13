using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockUIUpdate : MonoBehaviour
{
    public int position;
    private bool isLock;
    public GameObject textObject;
    private bool isExist;
    public Sprite unlockImage;
    public Sprite lockImage;
    // Start is called before the first frame update
    void Start()
    {
        if(ListObjectForInsert.listLock[position] == null)
        {
            isLock = false;
            gameObject.GetComponent<Image>().sprite = unlockImage;
        }
        else
        {
            isLock = true;
            gameObject.GetComponent<Image>().sprite = lockImage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(textObject.GetComponent<TextMeshProUGUI>().text == "")
        {
            gameObject.GetComponent<Image>().sprite = unlockImage;
            ListObjectForInsert.listLock[position] = null;
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            isExist = false;
        }
        else
        {
            
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            isExist = true;
        }
        
    }

    public void LockBtn()
    {
        if (!isExist) return;
        if (isLock)
        {
            isLock = false;
            ListObjectForInsert.listLock[position] = null;
            gameObject.GetComponent<Image>().sprite = unlockImage;
        }
        else if (!isLock)
        {
            isLock = true;
            ListObjectForInsert.listLock[position] = GameObject.Find("ShopMenu").GetComponent<Shop>().listObjectsRandom[position];
            gameObject.GetComponent<Image>().sprite = lockImage;
        }
    }
    
}
