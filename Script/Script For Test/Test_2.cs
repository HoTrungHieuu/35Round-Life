using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Test_2 : MonoBehaviour
{
    public GameObject itemObject;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        gameObject.transform.localPosition = new Vector3(worldPosition.x * 43.75f - 80f,0,0);
        UpdateStat();
    }
    private void UpdateStat()
    {
        if (itemObject != null)
        {
            GameObject statText = transform.Find("Image").gameObject;
            Image imageObject = statText.GetComponent<Image>();
            SpriteRenderer spriteRenderer = itemObject.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer.sprite;
            imageObject.sprite = sprite;

            statText = transform.Find("NameText").gameObject;
            TextMeshProUGUI textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().Name;

            statText = transform.Find("ClassText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().Class;

            statText = transform.Find("MoneyText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().Money.ToString() + "G";

            statText = transform.Find("HpText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Hp:"+itemObject.GetComponent<Stat>().Hp.ToString();

            statText = transform.Find("AtkText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Atk:"+itemObject.GetComponent<Stat>().Att.ToString();

            statText = transform.Find("SpeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Spe:"+itemObject.GetComponent<Stat>().Speed.ToString();

            statText = transform.Find("CrText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Cr:"+(itemObject.GetComponent<Stat>().CritRate*100f).ToString()+"%";

            statText = transform.Find("CrdText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Crd:"+ (itemObject.GetComponent<Stat>().CritDamage * 100f).ToString() + "%";

            statText = transform.Find("AccText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Acc:"+ (itemObject.GetComponent<Stat>().Accuracy * 100f).ToString() + "%";

            statText = transform.Find("EvaText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Eva:"+ (itemObject.GetComponent<Stat>().Evasion * 100f).ToString() + "%";

        }
    }
}
