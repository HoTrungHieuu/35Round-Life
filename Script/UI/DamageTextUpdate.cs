using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageTextUpdate : MonoBehaviour
{
    public GameObject DamageText;
    public void DamageTextExport(float damage, string damageType, Vector3 position, bool isCrit, bool isEva)
    {
        GameObject instance = Instantiate(DamageText, position, Quaternion.identity);
        instance.transform.SetParent(transform);
        instance.transform.localPosition = (position * 73f) + new Vector3(0, 73f, 0);
        if(isCrit)
        {
            instance.GetComponent<TextMeshProUGUI>().text = "Critical\n" + ((int)damage).ToString();
            instance.GetComponent<TextMeshProUGUI>().fontSize = 70;
        }
            
        else instance.GetComponent<TextMeshProUGUI>().text = ((int)damage).ToString();

        if(isEva)
        {
            instance.GetComponent<TextMeshProUGUI>().text = "Miss";
            instance.GetComponent<TextMeshProUGUI>().fontSize = 70;
        }
        if (damageType == "Physical") instance.GetComponent<TextMeshProUGUI>().color = Color.red;
        if (damageType == "Magical") instance.GetComponent<TextMeshProUGUI>().color = Color.blue;
    }
    public void HealingTextExport(float healing,Vector3 position)
    {
        GameObject instance = Instantiate(DamageText, position, Quaternion.identity);
        instance.transform.SetParent(transform);
        instance.transform.localPosition = (position * 73f) + new Vector3(0, 73f, 0);

        instance.GetComponent<TextMeshProUGUI>().text = ((int)healing).ToString();
        instance.GetComponent<TextMeshProUGUI>().color = Color.green;
    }
    public void AnotherTextExport(string text, Vector3 position, Color color)
    {
        GameObject instance = Instantiate(DamageText, position, Quaternion.identity);
        instance.transform.SetParent(transform);
        instance.transform.localPosition = (position * 73f) + new Vector3(0, 73f, 0);

        instance.GetComponent<TextMeshProUGUI>().text = text.ToString();
        instance.GetComponent<TextMeshProUGUI>().color = color;
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
