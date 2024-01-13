using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackgroundPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int index = Generalspecifications.Round / 3;
        if (index > 10) index = 10;
        gameObject.GetComponent<Image>().sprite = Generalspecifications.imageList[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
