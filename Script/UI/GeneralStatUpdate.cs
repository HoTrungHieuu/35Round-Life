using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneralStatUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("Life Text").GetComponent<TextMeshProUGUI>().text = "Life: " + Generalspecifications.Life.ToString();
        transform.Find("Money Text").GetComponent<TextMeshProUGUI>().text = "Money: " + Generalspecifications.Money.ToString();
        transform.Find("Round Text").GetComponent<TextMeshProUGUI>().text = "Round: " + Generalspecifications.Round.ToString();
        
    }
}
