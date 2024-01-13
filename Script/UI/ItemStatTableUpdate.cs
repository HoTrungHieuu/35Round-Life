using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatTableUpdate : MonoBehaviour
{
    public GameObject itemObject;
    public int indexDisplay = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(indexDisplay == 0)
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            gameObject.transform.localPosition = new Vector3(worldPosition.x * 73f - 80f, 0, 0);
        }
        else
        {
            gameObject.transform.localPosition = new Vector3(-6.3f * 73f, 0, 0);
        }
        
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

            statText = transform.Find("BackgrroundImage").gameObject;
            imageObject = statText.GetComponent<Image>();
            imageObject.color = GetLevelItem(itemObject);

            statText = transform.Find("NameText").gameObject;
            TextMeshProUGUI textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = AddSpacesBeforeUppercase(itemObject.GetComponent<ItemStat>().Name);

            statText = transform.Find("TypeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<ItemStat>().Type;

            

            bool isHp = false;
            bool isAtk = false;
            bool isDef = false;
            bool isCr = false;
            bool isCrd = false;
            bool isAcc = false;
            bool isEva = false;
            for (int i = 1; i < 8; i++)
            {
                if(itemObject.GetComponent<ItemStat>().Hp>0 && !isHp)
                {
                    statText = transform.Find("text_"+i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Hp:" + itemObject.GetComponent<ItemStat>().Hp.ToString();
                    isHp= true;
                }else if (itemObject.GetComponent<ItemStat>().Att > 0 && !isAtk)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Atk:" + itemObject.GetComponent<ItemStat>().Att.ToString();
                    isAtk = true;
                }
                else if (itemObject.GetComponent<ItemStat>().Def > 0 && !isDef)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Def:" + itemObject.GetComponent<ItemStat>().Def.ToString();
                    isDef = true;
                }
                else if (itemObject.GetComponent<ItemStat>().CritRate > 0 && !isCr)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Cr:" + (itemObject.GetComponent<ItemStat>().CritRate*100).ToString() + "%";
                    isCr = true;
                }
                else if (itemObject.GetComponent<ItemStat>().CritDamage > 0 && !isCrd)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Crd:" + (itemObject.GetComponent<ItemStat>().CritDamage * 100).ToString() + "%";
                    isCrd = true;
                }
                else if (itemObject.GetComponent<ItemStat>().Accuracy > 0 && !isAcc)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Acc:" + (itemObject.GetComponent<ItemStat>().Accuracy * 100).ToString() + "%";
                    isAcc = true;
                }
                else if (itemObject.GetComponent<ItemStat>().Evasion > 0 && !isEva)
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "Eva:" + (itemObject.GetComponent<ItemStat>().Evasion * 100).ToString() + "%";
                    isEva = true;
                }
                else
                {
                    statText = transform.Find("text_" + i).gameObject;
                    textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
                    textMeshProObject.text = "";
                }
            }
            statText = transform.Find("SkillText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.SetText("");
            for (int i = 0; i < itemObject.GetComponent<ItemStat>().passiveSkill.Count; i++)
            {
                textMeshProObject.text += itemObject.GetComponent<ItemStat>().passiveSkill[i] + "\n";
            }



        }
    }
    private Color GetLevelItem(GameObject itemObjectt)
    {
        switch (itemObjectt.GetComponent<ItemStat>().LevelType)
        {
            case 1:
                return new Color(0.5f, 0.5f, 0.5f);
            case 2:
                return new Color(0, 0.5f, 0);
            case 3:
                return new Color(0, 0, 1);
            case 4:
                return new Color(0.5f, 0, 0.5f);
            case 5:
                return new Color(1f, 0.647f, 0);
            case 6:
                return new Color(1f, 0, 0);
        }
        return Color.white;
    }
    private string AddSpacesBeforeUppercase(string input)
    {
        string pattern = @"(?<!^)(?=[A-Z])"; 
        string replacement = " "; 
        string result = Regex.Replace(input, pattern, replacement);
        return result;
    }
}
