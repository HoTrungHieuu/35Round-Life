using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBarUpdate : MonoBehaviour
{
    public Slider slider;
    public void UpdateShieldbar(float currentValue, float maxValue)
    {
        if(currentValue > maxValue)
        {
            slider.value = 1f;
        }
        else
        {
            slider.value = currentValue / maxValue;
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
