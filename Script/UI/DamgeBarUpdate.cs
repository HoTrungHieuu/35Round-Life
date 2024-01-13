using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamgeBarUpdate : MonoBehaviour
{
    public Slider slider;
    public void UpdateDamagebar(float currentValue, float maxValue)
    {
        if (currentValue < 0)
        {
            slider.value = 0;
            return;
        }
        if (currentValue > maxValue)
        {
            slider.value = 1f;
            return;
        }
        
        slider.value = currentValue / maxValue;
    }
    public void ChangeNameText(string name)
    {
        transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
    }
    public void ChangeColor(Color color)
    {
        transform.GetChild(1).GetChild(0).GetComponent<Image>().color = color;
    }
    public void ChangeNumber(float number)
    {
        transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().text = ((int)number).ToString();
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
