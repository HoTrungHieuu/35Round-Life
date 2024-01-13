using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedBarUpdate : MonoBehaviour
{
    public Slider slider;
    public void UpdateSpeedbar(float currentValue, float maxValue)
    {
        if (currentValue > maxValue)
        {
            slider.value = 0f;
            return;
        }
        if (currentValue < 0)
        {
            slider.value = 1f;
            return;
        }
        slider.value = 1 - (currentValue / maxValue);
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
