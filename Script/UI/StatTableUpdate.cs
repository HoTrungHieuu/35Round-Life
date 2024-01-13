using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatTableUpdate : MonoBehaviour
{
    public GameObject itemObject;
    public Sprite GuardianIcon;
    public Sprite WarriorIcon;
    public Sprite AssassinIcon;
    public Sprite RangerIcon;
    public Sprite MageIcon;
    public Sprite SageIcon;
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
        gameObject.transform.localPosition = new Vector3(worldPosition.x * 43.75f - 80f, 0, 0);
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
            imageObject.color = GetLevelCharacter(itemObject);

            statText = transform.Find("NameText").gameObject;
            TextMeshProUGUI textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().Name;

            statText = transform.Find("ClassText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().Class;

            statText = transform.Find("ClassIcon").gameObject;
            imageObject = statText.GetComponent<Image>();
            imageObject.sprite = ChangeClassIcon(itemObject);

            statText = transform.Find("DamgeTypeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = itemObject.GetComponent<Stat>().DamageType;
            
            statText = transform.Find("HpText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Hp:" + itemObject.GetComponent<Stat>().Hp.ToString();

            statText = transform.Find("AtkText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Atk:" + itemObject.GetComponent<Stat>().Att.ToString();

            statText = transform.Find("DefText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Def:" + itemObject.GetComponent<Stat>().Def.ToString();

            statText = transform.Find("SpeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Spe:" + itemObject.GetComponent<Stat>().Speed.ToString();

            statText = transform.Find("CrText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Cr:" + (itemObject.GetComponent<Stat>().CritRate * 100f).ToString() + "%";

            statText = transform.Find("CrdText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Crd:" + (itemObject.GetComponent<Stat>().CritDamage * 100f).ToString() + "%";

            statText = transform.Find("AccText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Acc:" + (itemObject.GetComponent<Stat>().Accuracy * 100f).ToString() + "%";

            statText = transform.Find("EvaText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Eva:" + (itemObject.GetComponent<Stat>().Evasion * 100f).ToString() + "%";

            statText = transform.Find("SkillText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.SetText("");
            for (int i = 0;i< itemObject.GetComponent<Stat>().Skill.Count;i++)
            {
                textMeshProObject.text += itemObject.GetComponent<Stat>().Skill[i] + "\n";
            }
        }
    }

    private Color GetLevelCharacter(GameObject characterObjectt)
    {
        switch (characterObjectt.GetComponent<Stat>().LevelType)
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

    private Sprite ChangeClassIcon(GameObject characterObject)
    {
        switch (characterObject.GetComponent<Stat>().Class)
        {
            case "Guardian":
                return GuardianIcon;
            case "Warrior":
                return WarriorIcon;
            case "Assassin":
                return AssassinIcon;
            case "Ranger":
                return RangerIcon;
            case "Mage":
                return MageIcon;
            case "Sage":
                return SageIcon;
            default:
                return null;
        }
    }
}
