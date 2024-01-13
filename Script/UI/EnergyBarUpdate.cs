using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUpdate : MonoBehaviour
{
    public Slider slider;
    public void UpdateEnergybar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
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
