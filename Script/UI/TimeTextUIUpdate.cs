using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeTextUIUpdate : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(Time.timeScale >0) gameObject.GetComponent<TextMeshProUGUI>().text = ((int)(time / 5)).ToString();
    }
}
