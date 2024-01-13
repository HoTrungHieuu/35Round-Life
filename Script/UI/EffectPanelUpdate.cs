using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EffectPanelUpdate : MonoBehaviour
{
    public GameObject effectObject;
    public GameObject icon;
    private List<Vector3> listEffectPosition = new();
    // Start is called before the first frame update
    public void EffectUpdate(List<string> listEffect)
    {
        if(transform.childCount > 0)
        {
            for (int i = 0;i < transform.childCount;i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        listEffectPositionInitial();
        for (int i = 0; i < listEffect.Count; i++)
        {
            GameObject instance = Instantiate(effectObject, listEffectPosition[i], Quaternion.identity);
            instance.transform.SetParent(gameObject.transform);
            instance.transform.localPosition = listEffectPosition[i];
            string effect = listEffect[i].Split("#")[0];
            List<GameObject> iconObjects = icon.GetComponent<ListEffects>().listEffects;
            if (effect.Contains("x")) effect = effect.Split("x")[0];
            GameObject iconObject = iconObjects.FirstOrDefault(i => i.name == effect);
            if (iconObject != null)
            {
                instance.GetComponent<Image>().sprite = iconObject.GetComponent<SpriteRenderer>().sprite;
            }

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void listEffectPositionInitial()
    {
        for (int i = -30; i < 31; i+=10)
        {
            for (int j = -70; j < 71; j+=15)
            {
                listEffectPosition.Add(new Vector3(j,i,0));
            }
        }
    }
}
