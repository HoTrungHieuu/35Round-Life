

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    public GameObject listSumary;
    public int positionSelected;
    public int typeSelected;
    public int positionItemInEquip;
    public int positionInEquipSelected;
    public int positionCharacterInTeam;
    public bool isEquip = false;
    public bool isForm = false;
    private List<GameObject> listCharacters;
    private List<GameObject> listItems;
    public List<GameObject> listObjectsRandom = new();
    private GameObject[] buyObjects = new GameObject[6];
    public List<Vector3> ItemRandPositions = new();
    public List<Vector3> ItemInventoryPositions = new();
    public List<Vector3> CharacterInventoryPositions = new();
    public List<Vector3> ItemClassifyPositions = new();
    public List<Vector3> CharacterInFormationPositions = new();
    public List<Vector3> CharacterInTeamPositions = new();
    private List<GameObject> listCharacterBought = new();
    private List<GameObject> listItemBought = new();
    public GameObject[] listItemInventory = new GameObject[15];
    public GameObject[] listCharacterInventory = new GameObject[9];
    public GameObject[] listItemClassify = new GameObject[15];
    public GameObject[] listCharacterFormation = new GameObject[9];

    public GameObject Border;
    // Start is called before the first frame update
    void Awake()
    {
        
        
        ScreenInitial();
        listCharacters = listSumary.GetComponent<ListSumary>().getListCharacters();
        listItems = listSumary.GetComponent<ListSumary>().getListItems();
        ItemRandPositions.Add(new Vector3(2, 3, 0));
        ItemRandPositions.Add(new Vector3(6, 3, 0));
        ItemRandPositions.Add(new Vector3(2, 0, 0));
        ItemRandPositions.Add(new Vector3(6, 0, 0));
        ItemRandPositions.Add(new Vector3(2, -3, 0));
        ItemRandPositions.Add(new Vector3(6, -3, 0));
        ItemInventoryInitial();
        CharacterInventoryInitial();
        ItemClasssifyInitial();
        CharaterFormationInitial();
        CharaterTeamInitial();
        RollRandom();
        InitialAllSystem();
    }

    // Update is called once per frame
    void Update()
    {
        
        UpdateBorder();
        UpdateBackGround();
        UpdateMoneySell();
    }
    private void UpdateBackGround()
    {
        GameObject BackGoundMain = GameObject.Find("BackGoundMain");
        GameObject Inventory = transform.Find("Inventory").gameObject;
       
        if (Inventory.transform.Find("Item").gameObject.activeSelf)
        {
            BackGoundMain.transform.Find("ItemBackground").gameObject.SetActive(true);
            BackGoundMain.transform.Find("CharacterBackground").gameObject.SetActive(false);

        }
        else if (Inventory.transform.Find("Character").gameObject.activeSelf)
        {
            BackGoundMain.transform.Find("ItemBackground").gameObject.SetActive(false);
            BackGoundMain.transform.Find("CharacterBackground").gameObject.SetActive(true);
        }
        else
        {
            BackGoundMain.transform.Find("ItemBackground").gameObject.SetActive(false);
            BackGoundMain.transform.Find("CharacterBackground").gameObject.SetActive(false);
        }
    }
    private void UpdateBorder()
    {
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory.transform.Find("Item").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
        Destroy(GameObject.Find("Border(Clone)"));
        
        if (typeSelected == 1 && Item.activeSelf)
        {
            GameObject instance = Instantiate(Border, ItemInventoryPositions[positionSelected], Quaternion.identity);
            instance.transform.position = ItemInventoryPositions[positionSelected];

        }
        if (typeSelected == 2 && Character.activeSelf)
        {
            GameObject instance = Instantiate(Border, CharacterInventoryPositions[positionSelected], Quaternion.identity);
            instance.transform.position = CharacterInventoryPositions[positionSelected];
        }
        if (positionCharacterInTeam == 6 && CharacterInForm.activeSelf)
        {
            int count = 0;
            for (int i = 0; i < positionSelected; i++)
            {
                if(listCharacterInventory[i] == null) count++; 
            }
            
            GameObject instance = Instantiate(Border, CharacterInFormationPositions[positionSelected - count], Quaternion.identity);
            instance.transform.position = CharacterInFormationPositions[positionSelected - count];
            instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        if (positionCharacterInTeam>0 && positionCharacterInTeam<6 && CharacterInForm.activeSelf)
        {
            
            GameObject instance = Instantiate(Border, CharacterInTeamPositions[positionCharacterInTeam-1], Quaternion.identity);
            instance.transform.position = CharacterInTeamPositions[positionCharacterInTeam-1];
            instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }
    private void UpdateMoneySell()
    {
        TextMeshProUGUI text = GameObject.Find("Main UI").transform.Find("All Button On Screen").Find("Sell Button").GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (typeSelected == 1 && listItemInventory[positionSelected] != null)
        {
            text.text = "(" + moneyCountSell(listItemInventory[positionSelected]).ToString() + "G)";
        }
        else if (typeSelected == 2 && listCharacterInventory[positionSelected] != null)
        {
            text.text = "(" + moneyCountSell(listCharacterInventory[positionSelected]).ToString() + "G)";
        }
        else
        {
            text.text = "(0G)";   
        }
    }
    private int GenerateRandomNumber(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
    public void RollRandomBtn()
    {
        if (Generalspecifications.Money < 1) return;
        for(int i = 0; i < ListObjectForInsert.listLock.Count(); i++)
        {
            if (ListObjectForInsert.listLock[i]==null) break;
            if (i == 5) return;
        }
        Generalspecifications.Money -= 1;
        RollRandom();
    }
    public void RollRandom()
    {
        
        GameObject shopRandom = this.transform.Find("ShopRandom").gameObject;
        if (shopRandom.transform.childCount > 0)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject itemInShop = shopRandom.transform.GetChild(i).gameObject;
                Destroy(itemInShop);
            }
            listObjectsRandom = new();
        }


        for (int i = 0; i < 6; i++)
        {
            buyObjects[i] = new GameObject("B" + (i + 1));
            buyObjects[i].transform.SetParent(shopRandom.transform);
        }

        for (int i = 0; i < 6; i++)
        {

            if (ListObjectForInsert.listLock[i] != null)
            {
                listObjectsRandom.Add(ListObjectForInsert.listLock[i]);
            }
            else if(ListObjectForInsert.listRandom[i] == null)
            {
                RandomItem();
            }
            else if(ListObjectForInsert.listRandom[i].name == "New")
            {
                listObjectsRandom.Add(null);
                ListObjectForInsert.listRandom[i] = null;
                
            }
            else
            {
                listObjectsRandom.Add(ListObjectForInsert.listRandom[i]);

                ListObjectForInsert.listRandom[i] = null;
            }
            
        }

        for (int i =0;i<6;i++)
        {
            if (listObjectsRandom[i] == null) continue;
            GameObject instance = Instantiate(listObjectsRandom[i], ItemRandPositions[i], Quaternion.identity);
            instance.transform.SetParent(buyObjects[i].transform);
            instance.transform.position = ItemRandPositions[i];
            instance.SetActive(true);
            if(instance.GetComponent<Stat>() != null)
            {
                instance.AddComponent<BoxCollider2D>();
                StatTabePop instance_1 = instance.AddComponent<StatTabePop>();
                GameObject AllStatUI = GameObject.Find("AllStatTableUI");
                GameObject StatUI = AllStatUI.transform.Find("StatUI").gameObject;
                GameObject StatTable = StatUI.transform.Find("StatTable").gameObject;
                instance_1.tableObject = StatTable;
                instance_1.index = i;
            }else if (instance.GetComponent<ItemStat>() != null)
            {
                instance.AddComponent<BoxCollider2D>();
                ItemStatTablePop instance_1 = instance.AddComponent<ItemStatTablePop>();
                GameObject AllStatUI = GameObject.Find("AllStatTableUI");
                GameObject ItemStatUI = AllStatUI.transform.Find("ItemStatUI").gameObject;
                GameObject ItemStatTable = ItemStatUI.transform.Find("ItemStatTable").gameObject;
                instance_1.tableObject = ItemStatTable;
                instance_1.index = i;
            }
            

        }
    }
    private void RandomItem()
    {
        
        
        if (GenerateRandomNumber(1, 10) < 6)
        {
            List<GameObject> itemList = listGameObjectItem(listItems);
            
            GameObject objectRandom = Instantiate(itemList[GenerateRandomNumber(0, itemList.Count - 1)], new Vector3(0, 0, 0), Quaternion.identity);
            if (GenerateRandomNumber(1, 100) <= 15)
            {
                objectRandom.GetComponent<ItemStat>().isDiscount = true;
            }
            objectRandom.SetActive(false);
            listObjectsRandom.Add(objectRandom);
        }
        else
        {
            List<GameObject> characterList = listGameObjectCharacter(listCharacters);
            GameObject objectRandom = Instantiate(characterList[GenerateRandomNumber(0, characterList.Count - 1)], new Vector3(0, 0, 0), Quaternion.identity);
            if (GenerateRandomNumber(1, 100) <= 15)
            {
                objectRandom.GetComponent<Stat>().isDiscount = true;
            }
            objectRandom.SetActive(false);
            listObjectsRandom.Add(objectRandom);
        }
    }
    private List<GameObject> listGameObjectCharacter(List<GameObject> characterList)
    {
        int[] rateCheck = CheckRate(Generalspecifications.Round);
        int randomNumber = GenerateRandomNumber(1, 100);
        if (randomNumber <= rateCheck[0])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 1);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 2);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 3);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 4);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3] + rateCheck[4])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 5);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3] + rateCheck[4] + rateCheck[5])
        {
            return characterList.FindAll(l => l.GetComponent<Stat>().LevelType == 6);
        }
        return null;
    }
    private List<GameObject> listGameObjectItem(List<GameObject> itemList)
    {
        int[] rateCheck = CheckRate(Generalspecifications.Round);
        int randomNumber = GenerateRandomNumber(1, 100);
        if (randomNumber <= rateCheck[0])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 1);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 2);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 3);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 4);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3] + rateCheck[4])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 5);
        }
        else if (randomNumber <= rateCheck[0] + rateCheck[1] + rateCheck[2] + rateCheck[3] + rateCheck[4] + rateCheck[5])
        {
            return itemList.FindAll(l => l.GetComponent<ItemStat>().LevelType == 6);
        }
        
        return null;
    }
    public int[] CheckRate(int round)
    {
        int[] rateRandom = new int[6];
        switch (round)
        {
            case 1:
                rateRandom = new int[] { 100, 0, 0, 0, 0, 0 };
                return rateRandom;
            case 2:
                rateRandom = new int[] { 100, 0, 0, 0, 0, 0 };
                return rateRandom;
            case 3:
                rateRandom = new int[] { 100, 0, 0, 0, 0, 0 };
                return rateRandom;
            case 4:
                rateRandom = new int[] { 95, 5, 0, 0, 0, 0 };
                return rateRandom;
            case 5:
                rateRandom = new int[] { 90, 10, 0, 0, 0, 0 };
                return rateRandom;
            case 6:
                rateRandom = new int[] { 80, 20, 0, 0, 0, 0 };
                return rateRandom;
            case 7:
                rateRandom = new int[] { 70, 30, 5, 0, 0, 0 };
                return rateRandom;
            case 8:
                rateRandom = new int[] { 60, 40, 10, 0, 0, 0 };
                return rateRandom;
            case 9:
                rateRandom = new int[] { 50, 45, 5, 0, 0, 0 };
                return rateRandom;
            case 10:
                rateRandom = new int[] { 40, 50, 10, 0, 0, 0 };
                return rateRandom;
            case 11:
                rateRandom = new int[] { 30, 55, 15, 0, 0, 0 };
                return rateRandom;
            case 12:
                rateRandom = new int[] { 20, 60, 20, 5, 0, 0 };
                return rateRandom;
            case 13:
                rateRandom = new int[] { 15, 60, 25, 10, 0, 0 };
                return rateRandom;
            case 14:
                rateRandom = new int[] { 15, 50, 30, 5, 0, 0 };
                return rateRandom;
            case 15:
                rateRandom = new int[] { 10, 45, 35, 10, 0, 0 };
                return rateRandom;
            case 16:
                rateRandom = new int[] { 5, 40, 40, 15, 0, 0 };
                return rateRandom;
            case 17:
                rateRandom = new int[] { 5, 30, 45, 20, 0, 0 };
                return rateRandom;
            case 18:
                rateRandom = new int[] { 5, 20, 50, 25, 0, 0 };
                return rateRandom;
            case 19:
                rateRandom = new int[] { 2, 15, 50, 30, 3, 0 };
                return rateRandom;
            case 20:
                rateRandom = new int[] { 2, 13, 40, 40, 5, 0 };
                return rateRandom;
            case 21:
                rateRandom = new int[] { 0, 10, 30, 50, 10, 0 };
                return rateRandom;
            case 22:
                rateRandom = new int[] { 0, 5, 25, 50, 20, 0 };
                return rateRandom;
            case 23:
                rateRandom = new int[] { 0, 5, 25, 45, 25, 0 };
                return rateRandom;
            case 24:
                rateRandom = new int[] { 0, 5, 25, 40, 30, 0 };
                return rateRandom;
            case 25:
                rateRandom = new int[] { 0, 5, 25, 35, 35, 0 };
                return rateRandom;
            case 26:
                rateRandom = new int[] { 0, 0, 25, 35, 40, 5 };
                return rateRandom;
            case 27:
                rateRandom = new int[] { 0, 0, 22, 30, 45, 8 };
                return rateRandom;
            case 28:
                rateRandom = new int[] { 0, 0, 17, 25, 50, 13 };
                return rateRandom;
            case 29:
                rateRandom = new int[] { 0, 0, 12, 20, 50, 23 };
                return rateRandom;
            case 30:
                rateRandom = new int[] { 0, 0, 7, 15, 50, 28 };
                return rateRandom;
            case 31:
                rateRandom = new int[] { 0, 0, 2, 10, 50, 38 };
                return rateRandom;
            case 32:
                rateRandom = new int[] { 0, 0, 2, 5, 50, 45 };
                return rateRandom;
            case 33:
                rateRandom = new int[] { 0, 0, 2, 3, 50, 45 };
                return rateRandom;
            case 34:
                rateRandom = new int[] { 0, 0, 2, 3, 50, 45 };
                return rateRandom;
            case 35:
                rateRandom = new int[] { 0, 0, 2, 3, 50, 45 };
                return rateRandom;
            default:
                break;
        }
        return null;
    }
    public void ItemBtn()
    {
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory.transform.Find("Item").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        Item.SetActive(true);
        Character.SetActive(false);
        ItemAfterClassify.SetActive(false);
        CharacterInTeam.SetActive(false);
    }
    public void CharacterBtn()
    {
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory.transform.Find("Item").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        Item.SetActive(false);
        Character.SetActive(true);
        ItemAfterClassify.SetActive(false);
        CharacterInTeam.SetActive(false);
    }
    public int moneyCountSell(GameObject itemObject)
    {
        int money = 0;
        if (itemObject.GetComponent<Stat>() != null)
        {
            money += itemObject.GetComponent<Stat>().Money;
            if (money % 2 == 1)
            {
                money /= 2;
                money++;
            }
            else money /= 2;
            for (int i = 0; i < itemObject.transform.childCount; i++)
            {
                int mon = itemObject.transform.GetChild(i).GetComponent<ItemStat>().Money;
                if (mon % 2 == 1)
                {
                    mon /= 2;
                    mon++;
                }
                else mon /= 2;
                money += mon;
            }



        }
        else if (itemObject.GetComponent<ItemStat>() != null)
        {
            money += itemObject.GetComponent<ItemStat>().Money;
            if (money % 2 == 1)
            {
                money /= 2;
                money++;
            }
            else money /= 2;

        }
        return money;
    }
    public void SellButton()
    {
        if(typeSelected == 1)
        {
            if (listItemInventory[positionSelected] != null)
            {
                listItemInventory[positionSelected] = null;
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject Item = Inventory.transform.Find("Item").gameObject;
                for (int i =0; i< Item.transform.childCount; i++)
                {
                    if(Item.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionSelected)
                    {
                        Generalspecifications.Money += moneyCountSell(Item.transform.GetChild(i).gameObject);
                        Destroy(Item.transform.GetChild(i).gameObject);
                        break;
                    }
                }
            }
        }else if(typeSelected == 2)
        {
            if (listCharacterInventory[positionSelected] != null)
            {
                listCharacterInventory[positionSelected] = null;
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject Character = Inventory.transform.Find("Character").gameObject;
                for (int i = 0; i < Character.transform.childCount; i++)
                {
                    if (Character.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionSelected)
                    {
                        Generalspecifications.Money += moneyCountSell(Character.transform.GetChild(i).gameObject);
                        Destroy(Character.transform.GetChild(i).gameObject);
                        break;
                    }
                }
            }
        }
        typeSelected = 0;
        positionSelected = 0;
    }
    
    public void EquipBtn()
    {
        if (typeSelected == 2)
        {
            int i;
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Character = Inventory.transform.Find("Character").gameObject;
            for (i = 0; i < Character.transform.childCount; i++)
            {
                if (Character.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionSelected)
                {
                    break;
                }
            }
            GameObject EquipmentUI = GameObject.Find("EquipmentUI");
            GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
            EquipmentDetail.SetActive(true);
            EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject = Character.transform.GetChild(i).gameObject;
            GameObject MainUI = GameObject.Find("Main UI");
            MainUI.transform.Find("All Button On Screen").gameObject.SetActive(false);
            MainUI.transform.Find("BuyUIMain").gameObject.SetActive(false);

            isEquip = true;

            GameObject Inventory_1 = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory_1.transform.Find("Item").gameObject;
            GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
            GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
            GameObject CharacterInTeam = Inventory_1.transform.Find("CharacterInTeam").gameObject;
            GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
            Item.SetActive(false);
            Character_1.SetActive(false);
            ItemAfterClassify.SetActive(true);
            CharacterInTeam.SetActive(false);
            CharacterInForm.SetActive(false);
            listItemClassifyClear();
        }
        
    }
    public void EquipBtnOnForm()
    {
        if(positionCharacterInTeam == 6)
        {
            transform.Find("Inventory").Find("CharacterInTeam").gameObject.SetActive(false);
            transform.Find("Inventory").Find("CharacterInForm").gameObject.SetActive(false);
            int i;
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Character = Inventory.transform.Find("Character").gameObject;
            for (i = 0; i < Character.transform.childCount; i++)
            {
                if (Character.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionSelected)
                {
                    break;
                }
            }
            GameObject EquipmentUI = GameObject.Find("EquipmentUI");
            GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
            EquipmentDetail.SetActive(true);
            EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject = Character.transform.GetChild(i).gameObject;
            GameObject MainUI = GameObject.Find("Main UI");
            MainUI.transform.Find("All Button On Screen").gameObject.SetActive(false);
            MainUI.transform.Find("BuyUIMain").gameObject.SetActive(false);

            isEquip = true;

            GameObject Inventory_1 = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory_1.transform.Find("Item").gameObject;
            GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
            GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
            /*GameObject CharacterInTeam = Inventory_1.transform.Find("CharacterInTeam").gameObject;
            GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;*/
            Item.SetActive(false);
            Character_1.SetActive(false);
            ItemAfterClassify.SetActive(true);
            /*CharacterInTeam.SetActive(false);
            CharacterInForm.SetActive(false);*/
            listItemClassifyClear();
        }
        if (positionCharacterInTeam < 6 && positionCharacterInTeam > 0)
        {
            transform.Find("Inventory").Find("CharacterInTeam").gameObject.SetActive(false);
            transform.Find("Inventory").Find("CharacterInForm").gameObject.SetActive(false);
            int i;
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
            GameObject P = new();
            for (i = 0; i < 5; i++)
            {
                P = CharacterInTeam.transform.Find("P" + i).gameObject;
                if (P.transform.childCount == 0) continue;
                if (P.transform.GetChild(0).GetComponent<CharacterSelectFormation>().positionInTeam == positionCharacterInTeam)
                {
                    break;
                }
            }

            GameObject EquipmentUI = GameObject.Find("EquipmentUI");
            GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
            EquipmentDetail.SetActive(true);
            EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject = P.transform.GetChild(0).gameObject;
            GameObject MainUI = GameObject.Find("Main UI");
            MainUI.transform.Find("All Button On Screen").gameObject.SetActive(false);
            MainUI.transform.Find("BuyUIMain").gameObject.SetActive(false);

            isEquip = true;

            GameObject Inventory_1 = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory_1.transform.Find("Item").gameObject;
            GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
            GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
            /*GameObject CharacterInTeam_1 = Inventory_1.transform.Find("CharacterInTeam").gameObject;
            GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;*/
            Item.SetActive(false);
            Character_1.SetActive(false);
            ItemAfterClassify.SetActive(true);
            /*CharacterInTeam_1.SetActive(false);
            CharacterInForm.SetActive(false);*/
            listItemClassifyClear();

        }
    }
    public void FormButton()
    {
        GameObject FormationUI = GameObject.Find("FormationUI");
        GameObject Formate = FormationUI.transform.Find("Formate").gameObject;
        Formate.SetActive(true);
        GameObject MainUI = GameObject.Find("Main UI");
        MainUI.transform.Find("All Button On Screen").gameObject.SetActive(false);
        MainUI.transform.Find("BuyUIMain").gameObject.SetActive(false);

        isForm = true;

        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory.transform.Find("Item").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
        Item.SetActive(false);
        Character.SetActive(false);
        ItemAfterClassify.SetActive(false);
        CharacterInTeam.SetActive(true);
        CharacterInForm.SetActive(true);
        ExportCharaterIntoForm();
    }
    private bool isStart()
    {
        GameObject CharacterInTeam = transform.Find("Inventory").Find("CharacterInTeam").gameObject;
        for (int i = 0; i < CharacterInTeam.transform.childCount; i++)
        {
            if (CharacterInTeam.transform.GetChild(i).childCount != 0) return true;
        }
        return false;
    }
    public void StartButton()
    {

        if (!isStart()) return;
        Destroy(GameObject.Find("Bag"));
        GameObject Bag = new GameObject("Bag");
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject instance = Instantiate(Inventory.transform.Find("CharacterInTeam").gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        ListObjectForInsert.TeamPrepare = instance;
        instance.transform.SetParent(Bag.transform);
        for (int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) continue;
            GameObject instance1 = Instantiate(listItemInventory[i], new Vector3(0, 0, 0), Quaternion.identity);
            instance1.SetActive(false);
            ListObjectForInsert.ListItem[i] = instance1;
            instance1.transform.SetParent(Bag.transform);
        }
        for (int i = 0; i < 9; i++)
        {
            if (listCharacterInventory[i] == null) continue;
            GameObject instance1 = Instantiate(listCharacterInventory[i], new Vector3(0, 0, 0), Quaternion.identity);
            instance1.SetActive(false);
            ListObjectForInsert.listCharacter[i] = instance1;
            instance1.transform.SetParent(Bag.transform);
        }

        GameObject objectRandom = new("ObjectRandom");
        objectRandom.transform.SetParent(Bag.transform);
        for (int i = 0; i < 6; i++)
        {
            GameObject instance1 = new();
            if (listObjectsRandom[i] == null)
            {
                instance1 = new GameObject("New");
                instance1.SetActive(false);
                ListObjectForInsert.listRandom[i] = instance1;
            }
            else
            {
                instance1 = Instantiate(listObjectsRandom[i], new Vector3(0, 0, 0), Quaternion.identity);
                instance1.SetActive(false);
                ListObjectForInsert.listRandom[i] = instance1;
            }
            
            
            instance1.transform.SetParent(objectRandom.transform);
        }

        GameObject objectLock = new("ObjectLock");
        objectLock.transform.SetParent(Bag.transform);
        for (int i = 0; i < 6; i++)
        {
            if (ListObjectForInsert.listLock[i] == null) continue;
            GameObject instance1 = Instantiate(listObjectsRandom[i], new Vector3(0, 0, 0), Quaternion.identity);
            instance1.SetActive(false);
            ListObjectForInsert.listLock[i] = instance1;
            instance1.transform.SetParent(objectLock.transform);
        }

        DontDestroyOnLoad(Bag);
        SceneManager.LoadScene("Enemy");
    }
    public void PutCharacterIntoTeam(int index)
    {
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        GameObject P = CharacterInTeam.transform.Find("P"+index.ToString()).gameObject;
        GameObject instance;
        if (positionCharacterInTeam < 6 && positionCharacterInTeam> 0)
        {
            GameObject P_1 = CharacterInTeam.transform.Find("P" + (positionCharacterInTeam-1).ToString()).gameObject;
            instance = Instantiate(P_1.transform.GetChild(0).gameObject, CharacterInTeamPositions[index], Quaternion.identity);
            instance.transform.SetParent(P.transform);
            instance.transform.position = CharacterInTeamPositions[index];
            instance.transform.localScale = new Vector3(1f, 1f, 1f);
            instance.GetComponent<CharacterSelectFormation>().positionCharacter = 0;
            instance.GetComponent<CharacterSelectFormation>().positionInTeam = index + 1;
            Destroy(P_1.transform.GetChild(0).gameObject);
            ExportCharaterIntoForm();
            return;
        }
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        if (Character.transform.childCount == 0) return;
        int i;
        for(i = 0;i<Character.transform.childCount;i++)
        {
            if(Character.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionSelected)
            {
                
                break;
            }
        }
        instance = Instantiate(Character.transform.GetChild(i).gameObject, CharacterInTeamPositions[index], Quaternion.identity);
        instance.transform.SetParent(P.transform);
        instance.transform.position = CharacterInTeamPositions[index];
        instance.transform.localScale = new Vector3(1f, 1f, 1f);
        instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
        DestroyImmediate(instance.GetComponent<ItemSelect>());
        instance.AddComponent<CharacterSelectFormation>();
        instance.GetComponent<CharacterSelectFormation>().positionCharacter = 0;
        instance.GetComponent<CharacterSelectFormation>().positionInTeam = index+1;
        listCharacterInventory[positionSelected] = null;
        Destroy(Character.transform.GetChild(i).gameObject);
        ExportCharaterIntoForm();
    }

    public void RemoveBtn()
    {
        if (positionCharacterInTeam == 6 || positionCharacterInTeam == 0) return;
        if(CheckCharacterIsFull()) return;
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        GameObject P = CharacterInTeam.transform.Find("P" + (positionCharacterInTeam-1).ToString()).gameObject;
        DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>());
        DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<CharacterSelectFormation>());
        AddCharacterInInventory(P.transform.GetChild(0).gameObject);
        Destroy(P.transform.GetChild(0).gameObject);
        ExportCharaterIntoForm();
    }

    public void SwapCharacterPositionInteam(int _positionCharacterInTeam)
    {
        if(positionCharacterInTeam <6 && positionCharacterInTeam>0 && _positionCharacterInTeam<6 && _positionCharacterInTeam > 0)
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
            GameObject P = CharacterInTeam.transform.Find("P" + (positionCharacterInTeam - 1).ToString()).gameObject;
            GameObject _P = CharacterInTeam.transform.Find("P" + (_positionCharacterInTeam - 1).ToString()).gameObject;


            GameObject instance = Instantiate(P.transform.GetChild(0).gameObject, CharacterInTeamPositions[_positionCharacterInTeam - 1], Quaternion.identity);
            instance.transform.SetParent(_P.transform);
            instance.transform.position = CharacterInTeamPositions[_positionCharacterInTeam - 1];
            instance.transform.localScale = new Vector3(1f, 1f, 1f);
            instance.GetComponent<CharacterSelectFormation>().positionInTeam = _positionCharacterInTeam;

            GameObject _instance = Instantiate(_P.transform.GetChild(0).gameObject, CharacterInTeamPositions[positionCharacterInTeam - 1], Quaternion.identity);
            _instance.transform.SetParent(P.transform);
            _instance.transform.position = CharacterInTeamPositions[positionCharacterInTeam - 1];
            _instance.transform.localScale = new Vector3(1f, 1f, 1f);
            _instance.GetComponent<CharacterSelectFormation>().positionInTeam = positionCharacterInTeam;

            Destroy(P.transform.GetChild(0).gameObject);
            Destroy(_P.transform.GetChild(0).gameObject);

            ExportCharaterIntoForm();
        }
        else if (positionCharacterInTeam < 6 && positionCharacterInTeam > 0 && _positionCharacterInTeam == 6 && !CheckCharacterIsFull())
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
            GameObject P = CharacterInTeam.transform.Find("P" + (positionCharacterInTeam - 1).ToString()).gameObject;
            DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>());
            DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<CharacterSelectFormation>());
            AddCharacterInInventory(P.transform.GetChild(0).gameObject);
            Destroy(P.transform.GetChild(0).gameObject);

            _positionCharacterInTeam = positionCharacterInTeam;
            positionCharacterInTeam = 6;
            PutCharacterIntoTeam(_positionCharacterInTeam-1);

            ExportCharaterIntoForm();
        }else if (positionCharacterInTeam == 6 && _positionCharacterInTeam <6 && _positionCharacterInTeam>0 && !CheckCharacterIsFull())
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
            GameObject P = CharacterInTeam.transform.Find("P" + (_positionCharacterInTeam - 1).ToString()).gameObject;
            DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>());
            DestroyImmediate(P.transform.GetChild(0).gameObject.GetComponent<CharacterSelectFormation>());
            AddCharacterInInventory(P.transform.GetChild(0).gameObject);
            Destroy(P.transform.GetChild(0).gameObject);

            PutCharacterIntoTeam(_positionCharacterInTeam - 1);

            ExportCharaterIntoForm();
        }
        
    }

    private void ExportCharaterIntoForm()
    {
        ClearCharacterOfForm();
        int j = 0;
        for (int i = 0; i< 9; i++)
        {
            if (listCharacterInventory[i] == null) continue;
            listCharacterFormation[j] = listCharacterInventory[i];
            GameObject instance = Instantiate(listCharacterFormation[j], CharacterInFormationPositions[j], Quaternion.identity);
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
            instance.transform.SetParent(CharacterInForm.transform);
            instance.transform.position = CharacterInFormationPositions[j];
            instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
            instance.transform.localScale = new Vector3(1f, 1f, 1f);
            DestroyImmediate(instance.GetComponent<ItemSelect>());
            instance.AddComponent<CharacterSelectFormation>();
            instance.GetComponent<CharacterSelectFormation>().positionCharacter = i;
            instance.GetComponent<CharacterSelectFormation>().positionInTeam = 6;
            j++;
        }
    }
    private void ClearCharacterOfForm()
    {
        positionSelected = 0;
        positionCharacterInTeam = 0;
        for (int i = 0; i < 9; i++)
        {
            listCharacterFormation[i] = null;
        }
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
        for (int i = 0; i < CharacterInForm.transform.childCount; i++)
        {
            Destroy(CharacterInForm.transform.GetChild(i).gameObject);
        }
        if (GameObject.Find("New Game Object") != null) Destroy(GameObject.Find("New Game Object"));
    }

    public void BackButtonForEquipment()
    {
        if (isForm)
        {
            transform.Find("Inventory").Find("CharacterInTeam").gameObject.SetActive(true);
            transform.Find("Inventory").Find("CharacterInForm").gameObject.SetActive(true);
        }
        listItemClassifyClear();
        
        //ClearCharacterOfForm();
        GameObject EquipmentUI = GameObject.Find("EquipmentUI");
        GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
        EquipmentDetail.SetActive(false);
        isEquip = false;

        GameObject FormationUI = GameObject.Find("FormationUI");
        GameObject Formate = FormationUI.transform.Find("Formate").gameObject;
        /*Formate.SetActive(false);
        isForm = false;*/

        if (!Formate.activeSelf)
        {
            GameObject MainUI = GameObject.Find("Main UI");
            MainUI.transform.Find("All Button On Screen").gameObject.SetActive(true);
            MainUI.transform.Find("BuyUIMain").gameObject.SetActive(true);

            GameObject Inventory_1 = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory_1.transform.Find("Item").gameObject;
            GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
            GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
            GameObject CharacterInTeam = Inventory_1.transform.Find("CharacterInTeam").gameObject;
            GameObject CharacterInForm = Inventory_1.transform.Find("CharacterInForm").gameObject;
            Item.SetActive(false);
            Character_1.SetActive(true);
            ItemAfterClassify.SetActive(false);
        }
        GameObject ItemStatTable = GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().itemObject = null;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay = 0;
        ItemStatTable.SetActive(false);
        /*else if (Formate.activeSelf)
        {

        }

        GameObject MainUI = GameObject.Find("Main UI");
        MainUI.transform.Find("All Button On Screen").gameObject.SetActive(true);
        MainUI.transform.Find("BuyUIMain").gameObject.SetActive(true);

        GameObject Inventory_1 = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory_1.transform.Find("Item").gameObject;
        GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory_1.transform.Find("CharacterInTeam").gameObject;
        GameObject CharacterInForm = Inventory_1.transform.Find("CharacterInForm").gameObject;
        Item.SetActive(false);
        Character_1.SetActive(true);
        ItemAfterClassify.SetActive(false);
        CharacterInTeam.SetActive(false);
        CharacterInForm.SetActive(false);
        listItemClassifyClear();*/
    }

    public void BackButton()
    {
        listItemClassifyClear();
        ClearCharacterOfForm();
        GameObject EquipmentUI = GameObject.Find("EquipmentUI");
        GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
        EquipmentDetail.SetActive(false);
        isEquip = false;

        GameObject FormationUI = GameObject.Find("FormationUI");
        GameObject Formate = FormationUI.transform.Find("Formate").gameObject;
        Formate.SetActive(false);
        isForm = false;

        GameObject MainUI = GameObject.Find("Main UI");
        MainUI.transform.Find("All Button On Screen").gameObject.SetActive(true);
        MainUI.transform.Find("BuyUIMain").gameObject.SetActive(true);

        GameObject Inventory_1 = transform.Find("Inventory").gameObject;
        GameObject Item = Inventory_1.transform.Find("Item").gameObject;
        GameObject Character_1 = Inventory_1.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory_1.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory_1.transform.Find("CharacterInTeam").gameObject;
        GameObject CharacterInForm = Inventory_1.transform.Find("CharacterInForm").gameObject;
        Item.SetActive(false);
        Character_1.SetActive(true);
        ItemAfterClassify.SetActive(false);
        CharacterInTeam.SetActive(false);
        CharacterInForm.SetActive(false);
        listItemClassifyClear();
    }
    public void WeaponBtn()
    {
        listItemClassifyClear();
        int j = 0;
        for(int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) continue;

            if (listItemInventory[i].GetComponent<ItemStat>().Type == "Weapon")
            {
                listItemClassify[j] = listItemInventory[i];
                GameObject instance = Instantiate(listItemClassify[j], ItemClassifyPositions[j], Quaternion.identity);
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
                instance.transform.SetParent(ItemAfterClassify.transform);
                instance.transform.position = ItemClassifyPositions[j];
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 4;
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelectEquipment>();
                instance.GetComponent<ItemSelectEquipment>().posisionItem = i;
                instance.GetComponent<ItemSelectEquipment>().posisionCharater = positionSelected;
                instance.GetComponent<ItemSelectEquipment>().positionInEquip = 1;
                j++;
            }
        }
        positionInEquipSelected = 1;
    }
    public void AmorBtn()
    {
        listItemClassifyClear();
        int j = 0;
        for (int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) continue;

            if (listItemInventory[i].GetComponent<ItemStat>().Type == "Amor")
            {
                listItemClassify[j] = listItemInventory[i];
                GameObject instance = Instantiate(listItemClassify[j], ItemClassifyPositions[j], Quaternion.identity);
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
                instance.transform.SetParent(ItemAfterClassify.transform);
                instance.transform.position = ItemClassifyPositions[j];
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 4;
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelectEquipment>();
                instance.GetComponent<ItemSelectEquipment>().posisionItem = i;
                instance.GetComponent<ItemSelectEquipment>().posisionCharater = positionSelected;
                instance.GetComponent<ItemSelectEquipment>().positionInEquip = 2;
                j++;
            }
        }
        positionInEquipSelected = 2;
    }
    public void Accessory_1Btn()
    {
        listItemClassifyClear();
        int j = 0;
        for (int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) continue;

            if (listItemInventory[i].GetComponent<ItemStat>().Type == "Accessory")
            {
                listItemClassify[j] = listItemInventory[i];
                GameObject instance = Instantiate(listItemClassify[j], ItemClassifyPositions[j], Quaternion.identity);
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
                instance.transform.SetParent(ItemAfterClassify.transform);
                instance.transform.position = ItemClassifyPositions[j];
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 4;
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelectEquipment>();
                instance.GetComponent<ItemSelectEquipment>().posisionItem = i;
                instance.GetComponent<ItemSelectEquipment>().posisionCharater = positionSelected;
                instance.GetComponent<ItemSelectEquipment>().positionInEquip = 3;
                j++;
            }
        }
        positionInEquipSelected = 3;
    }
    public void Accessory_2Btn()
    {
        listItemClassifyClear();
        int j = 0;
        for (int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) continue;

            if (listItemInventory[i].GetComponent<ItemStat>().Type == "Accessory")
            {
                listItemClassify[j] = listItemInventory[i];
                GameObject instance = Instantiate(listItemClassify[j], ItemClassifyPositions[j], Quaternion.identity);
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
                instance.transform.SetParent(ItemAfterClassify.transform);
                instance.transform.position = ItemClassifyPositions[j];
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 4;
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelectEquipment>();
                instance.GetComponent<ItemSelectEquipment>().posisionItem = i;
                instance.GetComponent<ItemSelectEquipment>().posisionCharater = positionSelected;
                instance.GetComponent<ItemSelectEquipment>().positionInEquip = 4;
                j++;
            }
        }
        positionInEquipSelected = 4;
    }

    public void UnequipBtn()
    {
        if (CheckItemIsFull()) return; 
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        /*int j;
        for (j = 0; j < Character.transform.childCount; j++)
        {
            if (Character.transform.GetChild(j).GetComponent<ItemSelect>().posision == positionSelected)
            {
                break;
            }
        }*/
        GameObject EquipmentUI = GameObject.Find("EquipmentUI");
        GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
        GameObject Character_postion = EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject;
        //GameObject Character_postion = Character.transform.GetChild(j).gameObject;
        GameObject item_position = new();
        bool isItem = false;
        int k;
        for (k = 0; k < Character_postion.transform.childCount; k++)
        {
            if (Character_postion.transform.GetChild(k).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == positionInEquipSelected)
            {
                item_position = Character_postion.transform.GetChild(k).gameObject;

                isItem = true;
                break;
            }
        }
        if (!isItem)
        {
            return;
        }
        else if (isItem)
        {
            if (CheckItemIsFull()) return;
            GameObject instance1 = Instantiate(item_position, ItemInventoryPositions[positionItemInEquip], Quaternion.identity);
            DestroyImmediate(instance1.GetComponent<ItemPosisionOfCharacter>());
            DestroyImmediate(instance1.GetComponent<BoxCollider2D>());

            instance1.SetActive(true);
            AddItemInInventory(instance1);
            Destroy(instance1);
            Destroy(item_position);
            positionInEquipSelected = 0;
        }

        listItemClassifyClear();
    }
    public void SelectItemInEquip()
    {
        if (positionInEquipSelected == 1)
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory.transform.Find("Item").gameObject;
            for (int i = 0; i < Item.transform.childCount; i++)
            {
                if (Item.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionItemInEquip)
                {
                    /*GameObject Character = Inventory.transform.Find("Character").gameObject;
                    int j;
                    for (j = 0; j<Character.transform.childCount; j++)
                    {
                        if (Character.transform.GetChild(j).GetComponent<ItemSelect>().posision == positionSelected)
                        {
                            break;
                        }
                    }*/
                    GameObject instance = Instantiate(Item.transform.GetChild(i).gameObject, new Vector3(0,0,0), Quaternion.identity);
                    GameObject EquipmentUI = GameObject.Find("EquipmentUI");
                    GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
                    GameObject Character_postion = EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject;
                    //GameObject Character_postion = Character.transform.GetChild(j).gameObject;
                    GameObject Weapon_position = new();
                    bool isWeapon = false;
                    int k;
                    for (k = 0; k < Character_postion.transform.childCount; k++)
                    {
                        if (Character_postion.transform.GetChild(k).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 1)
                        {
                            Weapon_position = Character_postion.transform.GetChild(k).gameObject;
                            
                            isWeapon = true;
                            break;
                        }
                    }
                    if (!isWeapon)
                    {
                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        listItemInventory[positionItemInEquip] = null;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }else if (isWeapon)
                    {
                        GameObject instance1 = Instantiate(Weapon_position, ItemInventoryPositions[positionItemInEquip], Quaternion.identity);
                        instance1.transform.SetParent(Item.transform);
                        instance1.AddComponent<ItemSelect>();
                        instance1.GetComponent<ItemSelect>().posision = positionItemInEquip;
                        instance1.GetComponent<ItemSelect>().type = 1;
                        instance1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        instance1.transform.position = ItemInventoryPositions[positionItemInEquip];
                        DestroyImmediate(instance1.GetComponent<ItemPosisionOfCharacter>());
                        DestroyImmediate(Weapon_position.GetComponent<ItemSelectEquipment>());
                        DestroyImmediate(Weapon_position.GetComponent<BoxCollider2D>());


                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        instance1.SetActive(true);
                        listItemInventory[positionItemInEquip] = instance1;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        Destroy(Weapon_position);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }
                    
                }
            }
        }
        else if (positionInEquipSelected == 2)
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory.transform.Find("Item").gameObject;
            for (int i = 0; i < Item.transform.childCount; i++)
            {
                if (Item.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionItemInEquip)
                {
                    GameObject Character = Inventory.transform.Find("Character").gameObject;
                    /*int j;
                    for (j = 0; j < Character.transform.childCount; j++)
                    {
                        if (Character.transform.GetChild(j).GetComponent<ItemSelect>().posision == positionSelected)
                        {
                            break;
                        }
                    }*/
                    GameObject instance = Instantiate(Item.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                    GameObject EquipmentUI = GameObject.Find("EquipmentUI");
                    GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
                    GameObject Character_postion = EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject;
                    //GameObject Character_postion = Character.transform.GetChild(j).gameObject;
                    GameObject Amor_position = new();
                    bool isAmor = false;
                    int k;
                    for (k = 0; k < Character_postion.transform.childCount; k++)
                    {
                        if (Character_postion.transform.GetChild(k).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 2)
                        {
                            Amor_position = Character_postion.transform.GetChild(k).gameObject;

                            isAmor = true;
                            break;
                        }
                    }
                    if (!isAmor)
                    {
                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        listItemInventory[positionItemInEquip] = null;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }
                    else if (isAmor)
                    {
                        GameObject instance1 = Instantiate(Amor_position, ItemInventoryPositions[positionItemInEquip], Quaternion.identity);
                        instance1.transform.SetParent(Item.transform);
                        instance1.AddComponent<ItemSelect>();
                        instance1.GetComponent<ItemSelect>().posision = positionItemInEquip;
                        instance1.GetComponent<ItemSelect>().type = 1;
                        instance1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        instance1.transform.position = ItemInventoryPositions[positionItemInEquip];
                        DestroyImmediate(instance1.GetComponent<ItemPosisionOfCharacter>());
                        DestroyImmediate(Amor_position.GetComponent<ItemSelectEquipment>());
                        DestroyImmediate(Amor_position.GetComponent<BoxCollider2D>());


                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        instance1.SetActive(true);
                        listItemInventory[positionItemInEquip] = instance1;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        Destroy(Amor_position);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }

                }
            }
        }
        else if (positionInEquipSelected == 3)
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory.transform.Find("Item").gameObject;
            for (int i = 0; i < Item.transform.childCount; i++)
            {
                if (Item.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionItemInEquip)
                {
                    GameObject Character = Inventory.transform.Find("Character").gameObject;
                    /*int j;
                    for (j = 0; j < Character.transform.childCount; j++)
                    {
                        if (Character.transform.GetChild(j).GetComponent<ItemSelect>().posision == positionSelected)
                        {
                            break;
                        }
                    }*/
                    GameObject instance = Instantiate(Item.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                    GameObject EquipmentUI = GameObject.Find("EquipmentUI");
                    GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
                    GameObject Character_postion = EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject;
                    //GameObject Character_postion = Character.transform.GetChild(j).gameObject;
                    GameObject Accessory_position = new();
                    bool isAccessory = false;
                    int k;
                    for (k = 0; k < Character_postion.transform.childCount; k++)
                    {
                        if (Character_postion.transform.GetChild(k).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 3)
                        {
                            Accessory_position = Character_postion.transform.GetChild(k).gameObject;

                            isAccessory = true;
                            break;
                        }
                    }
                    if (!isAccessory)
                    {
                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        listItemInventory[positionItemInEquip] = null;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }
                    else if (isAccessory)
                    {
                        GameObject instance1 = Instantiate(Accessory_position, ItemInventoryPositions[positionItemInEquip], Quaternion.identity);
                        instance1.transform.SetParent(Item.transform);
                        instance1.AddComponent<ItemSelect>();
                        instance1.GetComponent<ItemSelect>().posision = positionItemInEquip;
                        instance1.GetComponent<ItemSelect>().type = 1;
                        instance1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        instance1.transform.position = ItemInventoryPositions[positionItemInEquip];
                        DestroyImmediate(instance1.GetComponent<ItemPosisionOfCharacter>());
                        DestroyImmediate(Accessory_position.GetComponent<ItemSelectEquipment>());
                        DestroyImmediate(Accessory_position.GetComponent<BoxCollider2D>());


                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        instance1.SetActive(true);
                        listItemInventory[positionItemInEquip] = instance1;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        Destroy(Accessory_position);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }

                }
            }
        }
        else if (positionInEquipSelected == 4)
        {
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory.transform.Find("Item").gameObject;
            for (int i = 0; i < Item.transform.childCount; i++)
            {
                if (Item.transform.GetChild(i).GetComponent<ItemSelect>().posision == positionItemInEquip)
                {
                    GameObject Character = Inventory.transform.Find("Character").gameObject;
                    /*int j;
                    for (j = 0; j < Character.transform.childCount; j++)
                    {
                        if (Character.transform.GetChild(j).GetComponent<ItemSelect>().posision == positionSelected)
                        {
                            break;
                        }
                    }*/
                    GameObject instance = Instantiate(Item.transform.GetChild(i).gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                    GameObject EquipmentUI = GameObject.Find("EquipmentUI");
                    GameObject EquipmentDetail = EquipmentUI.transform.Find("EquipmentDetail").gameObject;
                    GameObject Character_postion = EquipmentDetail.GetComponent<EquipmentDetailUpdate>().characterObject;
                    //GameObject Character_postion = Character.transform.GetChild(j).gameObject;
                    GameObject Accessory_position = new();
                    bool isAccessory = false;
                    int k;
                    for (k = 0; k < Character_postion.transform.childCount; k++)
                    {
                        if (Character_postion.transform.GetChild(k).GetComponent<ItemPosisionOfCharacter>().EquipmentPosition == 4)
                        {
                            Accessory_position = Character_postion.transform.GetChild(k).gameObject;

                            isAccessory = true;
                            break;
                        }
                    }
                    if (!isAccessory)
                    {
                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                       

                        instance.SetActive(false);
                        listItemInventory[positionItemInEquip] = null;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }
                    else if (isAccessory)
                    {
                        GameObject instance1 = Instantiate(Accessory_position, ItemInventoryPositions[positionItemInEquip], Quaternion.identity);
                        instance1.transform.SetParent(Item.transform);
                        instance1.AddComponent<ItemSelect>();
                        instance1.GetComponent<ItemSelect>().posision = positionItemInEquip;
                        instance1.GetComponent<ItemSelect>().type = 1;
                        instance1.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        instance1.transform.position = ItemInventoryPositions[positionItemInEquip];
                        DestroyImmediate(instance1.GetComponent<ItemPosisionOfCharacter>());
                        DestroyImmediate(Accessory_position.GetComponent<ItemSelectEquipment>());
                        DestroyImmediate(Accessory_position.GetComponent<BoxCollider2D>());


                        instance.transform.SetParent(Character_postion.transform);
                        DestroyImmediate(instance.GetComponent<ItemSelect>());
                        instance.AddComponent<ItemPosisionOfCharacter>();
                        instance.GetComponent<ItemPosisionOfCharacter>().EquipmentPosition = positionInEquipSelected;

                        

                        instance.SetActive(false);
                        instance1.SetActive(true);
                        listItemInventory[positionItemInEquip] = instance1;
                        Destroy(Item.transform.GetChild(i).gameObject);
                        Destroy(Accessory_position);
                        positionItemInEquip = 0;
                        positionInEquipSelected = 0;
                        break;
                    }

                }
            }
        }
        listItemClassifyClear();
        GameObject ItemStatTable = GameObject.Find("AllStatTableUI").transform.Find("ItemStatUI").Find("ItemStatTable").gameObject;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().itemObject = null;
        ItemStatTable.GetComponent<ItemStatTableUpdate>().indexDisplay = 0;
        ItemStatTable.SetActive(false);
    }


    private void listItemClassifyClear()
    {
        for (int i = 0; i < 15; i++)
        {
            listItemClassify[i] = null;
        }
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
        for(int i = 0; i < ItemAfterClassify.transform.childCount; i++)
        {
            Destroy(ItemAfterClassify.transform.GetChild(i).gameObject);
        }
        if(GameObject.Find("New Game Object")!=null) Destroy(GameObject.Find("New Game Object"));
        
    }
    public int moneyCount(GameObject itemObject)
    {
        int money = 0;
        if (itemObject.GetComponent<Stat>() != null)
        {
            money = itemObject.GetComponent<Stat>().Money;
            if (itemObject.GetComponent<Stat>().isDiscount)
            {
                if (money % 2 == 1)
                {
                    money /= 2;
                    money++;
                }
                else money /= 2;
            }
        }
        else if (itemObject.GetComponent<ItemStat>() != null)
        {
            money = itemObject.GetComponent<ItemStat>().Money;
            if (itemObject.GetComponent<ItemStat>().isDiscount)
            {
                
                if (money % 2 == 1)
                {
                    money /= 2;
                    money++;
                }else money /= 2;

            }
        }
        return money;
    }
    public void BuyItem(int itemIndex)
    {
        
        if (listObjectsRandom[itemIndex] == null) return;

        if(listObjectsRandom[itemIndex].GetComponent<Stat>() != null)
        {
            if (CheckCharacterIsFull())
            {
                return;
            }
            if (Generalspecifications.Money < moneyCount(listObjectsRandom[itemIndex]))
            {
                return;
            }
            Generalspecifications.Money -= moneyCount(listObjectsRandom[itemIndex]);
            listObjectsRandom[itemIndex].GetComponent<Stat>().isDiscount = false;
            AddCharacterInInventory(listObjectsRandom[itemIndex]);
            ListObjectForInsert.listLock[itemIndex] = null;
        }else if(listObjectsRandom[itemIndex].GetComponent<ItemStat>() != null)
        {
            if (CheckItemIsFull())
            {
                return;
            }
            if (Generalspecifications.Money < moneyCount(listObjectsRandom[itemIndex]))
            {
                return;
            }
            Generalspecifications.Money -= moneyCount(listObjectsRandom[itemIndex]);
            listObjectsRandom[itemIndex].GetComponent<ItemStat>().isDiscount = false;
            AddItemInInventory(listObjectsRandom[itemIndex]);
            ListObjectForInsert.listLock[itemIndex] = null;
        }
        listObjectsRandom[itemIndex] = null;
        buyObjects[itemIndex].SetActive(false);
    }

    private void AddItemInInventory(GameObject item)
    {
        for (int i=0; i < 15; i++)
        {
            if (listItemInventory[i] == null)
            {
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject Item = Inventory.transform.Find("Item").gameObject;
                GameObject instance = Instantiate(item, ItemInventoryPositions[i], Quaternion.identity);
                instance.transform.SetParent(Item.transform);
                listItemInventory[i] = instance;
                instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                instance.SetActive(true);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
                instance.transform.position = ItemInventoryPositions[i];
                DestroyImmediate(instance.GetComponent<BoxCollider2D>());
                DestroyImmediate(instance.GetComponent<ItemSelect>());
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelect>();
                instance.GetComponent<ItemSelect>().posision = i;
                instance.GetComponent<ItemSelect>().type = 1;
                break;
            }
        }
    }
    private bool CheckItemIsFull()
    {
        for (int i = 0; i < 15; i++)
        {
            if (listItemInventory[i] == null) return false;
        }
        return true;
    }

    private void AddCharacterInInventory(GameObject character)
    {
        for (int i = 0; i < 9; i++)
        {
            if (listCharacterInventory[i] == null)
            {
                GameObject Inventory = transform.Find("Inventory").gameObject;
                GameObject Character = Inventory.transform.Find("Character").gameObject;
                GameObject instance = Instantiate(character, CharacterInventoryPositions[i], Quaternion.identity);
                instance.transform.SetParent(Character.transform);
                listCharacterInventory[i] = instance;
                instance.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                instance.SetActive(true);
                instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
                instance.transform.position = CharacterInventoryPositions[i];
                DestroyImmediate(instance.GetComponent<BoxCollider2D>());
                DestroyImmediate(instance.GetComponent<ItemSelect>());
                instance.AddComponent<BoxCollider2D>();
                instance.AddComponent<ItemSelect>();
                instance.GetComponent<ItemSelect>().posision = i;
                instance.GetComponent<ItemSelect>().type = 2;
                break;
            }
        }
    }
    private bool CheckCharacterIsFull()
    {
        for (int i = 0; i < 9; i++)
        {
            if (listCharacterInventory[i] == null) return false;
        }
        return true;
    }

    private void ScreenInitial()
    {
        GameObject Inventory = transform.Find("Inventory").gameObject;
        GameObject Character = Inventory.transform.Find("Character").gameObject;
        GameObject ItemAfterClassify = Inventory.transform.Find("ItemAfterClassify").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        GameObject CharacterInForm = Inventory.transform.Find("CharacterInForm").gameObject;
        ItemAfterClassify.SetActive(false);
        Character.SetActive(false);
        CharacterInTeam.SetActive(false);
        CharacterInForm.SetActive(false);
    }

    private void CharacterInventoryInitial()
    {
        for(int i = 2; i > -5; i = i - 3)
        {
            for (int j = -6; j < -1; j = j+2)
            {
                CharacterInventoryPositions.Add(new Vector3(j, i, 0));
            }
        }

    }

    private void ItemInventoryInitial()
    {
        for(int i = 2; i > -5; i=i-2)
        {
            for (int j = -7; j < 0; j=j+2)
            {
                ItemInventoryPositions.Add(new Vector3(j,i, 0));
            }
        }
    }

    private void ItemClasssifyInitial()
    {
        for (int i = 4; i > -5; i = i - 2)
        {
            for (int j = 3; j < 8; j = j + 2)
            {
                ItemClassifyPositions.Add(new Vector3(j, i, 0));
            }
        }
    }

    private void CharaterFormationInitial()
    {
        for (int i = 3; i > -4; i = i - 3)
        {
            for (int j = 1; j < 8; j = j + 3)
            {
                CharacterInFormationPositions.Add(new Vector3(j, i, 0));
            }
        }
    }

    private void CharaterTeamInitial()
    {

        CharacterInTeamPositions.Add(new Vector3(-3, 2, 0));
        CharacterInTeamPositions.Add(new Vector3(-3, -2, 0));
        CharacterInTeamPositions.Add(new Vector3(-6, 3, 0));
        CharacterInTeamPositions.Add(new Vector3(-6, 0, 0));
        CharacterInTeamPositions.Add(new Vector3(-6, -3, 0));

    }

    private void InitialAllSystem()
    {
        for (int i = 0; i < 15; i++)
        {
            if (ListObjectForInsert.ListItem[i] == null) continue;
            //listItemInventory[i] = ListObjectForInsert.ListItem[i];
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Item = Inventory.transform.Find("Item").gameObject;
            GameObject instance = Instantiate(ListObjectForInsert.ListItem[i], ItemInventoryPositions[i], Quaternion.identity);
            instance.transform.SetParent(Item.transform);
            listItemInventory[i] = instance;
            instance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            instance.SetActive(true);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
            instance.transform.position = ItemInventoryPositions[i];
            DestroyImmediate(instance.GetComponent<BoxCollider2D>());
            DestroyImmediate(instance.GetComponent<ItemSelect>());
            instance.AddComponent<BoxCollider2D>();
            instance.AddComponent<ItemSelect>();
            instance.GetComponent<ItemSelect>().posision = i;
            instance.GetComponent<ItemSelect>().type = 1;
        }
        for (int i = 0; i < 9; i++)
        {
            if (ListObjectForInsert.listCharacter[i] == null) continue;
            //listCharacterInventory[i] = ListObjectForInsert.listCharacter[i];
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject Character = Inventory.transform.Find("Character").gameObject;
            GameObject instance = Instantiate(ListObjectForInsert.listCharacter[i], CharacterInventoryPositions[i], Quaternion.identity);
            instance.transform.SetParent(Character.transform);
            listCharacterInventory[i] = instance;
            instance.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            instance.SetActive(true);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 0;
            instance.transform.position = CharacterInventoryPositions[i];
            DestroyImmediate(instance.GetComponent<BoxCollider2D>());
            DestroyImmediate(instance.GetComponent<ItemSelect>());
            instance.AddComponent<BoxCollider2D>();
            instance.AddComponent<ItemSelect>();
            instance.GetComponent<ItemSelect>().posision = i;
            instance.GetComponent<ItemSelect>().type = 2;
        }
        if (ListObjectForInsert.TeamPrepare == null) return;
        for (int i = 0; i < 5; i++)
        {
            GameObject P = ListObjectForInsert.TeamPrepare.transform.Find("P" + i.ToString()).gameObject;
            if(P.transform.childCount == 0) continue;
            GameObject Inventory = transform.Find("Inventory").gameObject;
            GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
            GameObject _P = CharacterInTeam.transform.Find("P" + i.ToString()).gameObject;
            GameObject instance;
            instance = Instantiate(P.transform.GetChild(0).gameObject, CharacterInTeamPositions[i], Quaternion.identity);
            instance.transform.SetParent(_P.transform);
            instance.transform.position = CharacterInTeamPositions[i];
            instance.transform.localScale = new Vector3(1f, 1f, 1f);
            instance.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
    }
}
