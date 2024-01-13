using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealbarUpdate : MonoBehaviour
{
    public Slider slider;
    public void UpdateHealbar(float currentValue, float maxValue)
    {
        if(currentValue < 0)
        {
            slider.value = 0;
            return;
        }
        if(currentValue > maxValue)
        {
            slider.value = 1f;
            return;
        }
        slider.value = currentValue/maxValue;
    }

    // Update is called once per frame 
    void Update()
    {
        
    }
}
