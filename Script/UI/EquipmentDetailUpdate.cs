using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailUpdate : MonoBehaviour
{
    public GameObject characterObject;
    public Sprite weaponImage;
    public Sprite armorImage;
    public Sprite accessoryImage;
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
        UpdateStat();
    }
    private void UpdateStat()
    {
        float attack = 0;
        float hp = 0;
        float def = 0;
        float cr = 0;
        float crd = 0;
        float acc = 0;
        float eva = 0;
        if (characterObject != null)
        {
            if(characterObject.transform.childCount > 0)
            {
                for (int i = 0; i < characterObject.transform.childCount; i++)
                {
                    attack += characterObject.transform.GetChild(i).GetComponent<ItemStat>().Att;
                    hp += characterObject.transform.GetChild(i).GetComponent<ItemStat>().Hp;
                    def += characterObject.transform.GetChild(i).GetComponent<ItemStat>().Def;
                    cr += characterObject.transform.GetChild(i).GetComponent<ItemStat>().CritRate;
                    crd += characterObject.transform.GetChild(i).GetComponent<ItemStat>().CritDamage;
                    acc += characterObject.transform.GetChild(i).GetComponent<ItemStat>().Accuracy;
                    eva += characterObject.transform.GetChild(i).GetComponent<ItemStat>().Evasion;
                }
            }
            GameObject statText = transform.Find("Image").gameObject;
            Image imageObject = statText.GetComponent<Image>();
            SpriteRenderer spriteRenderer = characterObject.GetComponent<SpriteRenderer>();
            Sprite sprite = spriteRenderer.sprite;
            imageObject.sprite = sprite;

            bool isWeapon = false;
            bool isAmor = false;
            bool isAccessory_1 = false;
            bool isAccessory_2 =false;
            for (int i =0; i< characterObject.transform.childCount; i++)
            {
                if(characterObject.transform.GetChild(i).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 1)
                {
                    statText = transform.Find("WeponButton").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    spriteRenderer = characterObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                    sprite = spriteRenderer.sprite;
                    imageObject.sprite = sprite;
                    isWeapon = true;

                    statText = transform.Find("WeaponBackground").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    imageObject.color = GetLevelItem(characterObject.transform.GetChild(i).gameObject);
                }
                else if (characterObject.transform.GetChild(i).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 2)
                {
                    statText = transform.Find("AmorButton").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    spriteRenderer = characterObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                    sprite = spriteRenderer.sprite;
                    imageObject.sprite = sprite;
                    isAmor = true;

                    statText = transform.Find("AmorBackground").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    imageObject.color = GetLevelItem(characterObject.transform.GetChild(i).gameObject);
                }
                else if (characterObject.transform.GetChild(i).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 3)
                {
                    statText = transform.Find("Accessory_1Button").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    spriteRenderer = characterObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                    sprite = spriteRenderer.sprite;
                    imageObject.sprite = sprite;
                    isAccessory_1 = true;

                    statText = transform.Find("Accessory_1ground").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    imageObject.color = GetLevelItem(characterObject.transform.GetChild(i).gameObject);
                }
                else if (characterObject.transform.GetChild(i).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 4)
                {
                    statText = transform.Find("Accessory_2Button").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    spriteRenderer = characterObject.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();
                    sprite = spriteRenderer.sprite;
                    imageObject.sprite = sprite;
                    isAccessory_2 = true;

                    statText = transform.Find("Accessory_2ground").gameObject;
                    imageObject = statText.GetComponent<Image>();
                    imageObject.color = GetLevelItem(characterObject.transform.GetChild(i).gameObject);
                }
            }

            if (!isWeapon)
            {
                statText = transform.Find("WeponButton").gameObject;
                imageObject = statText.GetComponent<Image>();
                imageObject.sprite = weaponImage;
            }
            if (!isAmor)
            {
                statText = transform.Find("AmorButton").gameObject;
                imageObject = statText.GetComponent<Image>();
                imageObject.sprite = armorImage;
            }
            if (!isAccessory_1)
            {
                statText = transform.Find("Accessory_1Button").gameObject;
                imageObject = statText.GetComponent<Image>();
                imageObject.sprite = accessoryImage;
            }
            if (!isAccessory_2)
            {
                statText = transform.Find("Accessory_2Button").gameObject;
                imageObject = statText.GetComponent<Image>();
                imageObject.sprite = accessoryImage;
            }
            statText = transform.Find("NameText").gameObject;
            TextMeshProUGUI textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = characterObject.GetComponent<Stat>().Name;

            statText = transform.Find("ClassText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = characterObject.GetComponent<Stat>().Class;

            statText = transform.Find("ClassIcon").gameObject;
            imageObject = statText.GetComponent<Image>();
            imageObject.sprite = ChangeClassIcon(characterObject);

            statText = transform.Find("DamageTypeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = characterObject.GetComponent<Stat>().DamageType;

            statText = transform.Find("HpText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Hp:" + (characterObject.GetComponent<Stat>().Hp+hp).ToString();

            statText = transform.Find("AtkText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Atk:" + (characterObject.GetComponent<Stat>().Att+attack).ToString();

            statText = transform.Find("DefText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Def:" + (characterObject.GetComponent<Stat>().Def+def).ToString();

            statText = transform.Find("SpeText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Spe:" + characterObject.GetComponent<Stat>().Speed.ToString();

            statText = transform.Find("CrText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Cr:" + ((characterObject.GetComponent<Stat>().CritRate+cr) * 100f).ToString() + "%";

            statText = transform.Find("CrdText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Crd:" + ((characterObject.GetComponent<Stat>().CritDamage+crd) * 100f).ToString() + "%";

            statText = transform.Find("AccText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Acc:" + ((characterObject.GetComponent<Stat>().Accuracy+acc) * 100f).ToString() + "%";

            statText = transform.Find("EvaText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "Eva:" + ((characterObject.GetComponent<Stat>().Evasion+eva) * 100f).ToString() + "%";

            statText = transform.Find("SkillText").gameObject;
            textMeshProObject = statText.GetComponent<TextMeshProUGUI>();
            textMeshProObject.text = "";
            for (int i = 0; i < characterObject.GetComponent<Stat>().Skill.Count; i++)
            {
                textMeshProObject.text += characterObject.GetComponent<Stat>().Skill[i] + "\n";
            }
            GameObject ShopMenu = GameObject.Find("ShopMenu");
            if (ShopMenu == null) return;
            if (ShopMenu.transform.Find("Inventory").Find("ItemAfterClassify").childCount == 0)
            {
                statText.SetActive(true);
            }
            else
            {
                statText.SetActive(false);
            }
        }
    }
    public void DisplayItemDetail(int index)
    {
        
        if (characterObject.transform.childCount == 0)
        {
            GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject.SetActive(false);
            return;
        }
            
        GameObject item = null;
        for (int i =0;i< characterObject.transform.childCount; i++)
        {
            if(characterObject.transform.GetChild(i).gameObject.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == index)
            {
                item = characterObject.transform.GetChild(i).gameObject;
                break;
            }
        }
        if(item == null)
        {
            GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject.SetActive(false);
            return;
        }
        GameObject ItemStatTable = GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject;
        if(ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay == index)
        {
            ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay = 0;
            ItemStatTable.SetActive(false);
            return;
        }
        ItemStatTable.GetComponent<ItemStatTableUpdate>().itemObject = item;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay = index;
        ItemStatTable.SetActive(true);
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
