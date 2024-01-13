using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGameplay : MonoBehaviour
{
    private bool WinOrLose;
    public Sprite VictoryImage;
    public Sprite DefeatImage;
    private bool isX2 = false;
    public Sprite x1Image;
    public Sprite x2Image;
    // Start is called before the first frame update
    void Awake()
    {
        transform.gameObject.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetUpMainMenu(bool isWin)
    {
        WinOrLose = isWin;
        transform.gameObject.SetActive(true);
        if (WinOrLose)
        {

            transform.Find("TextImage").gameObject.GetComponent<Image>().sprite = VictoryImage;
            if(Generalspecifications.Round < 35) transform.Find("NextButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = "Next";
            else transform.Find("NextButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = "End";
            transform.Find("LifeText").GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("LifeText").GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            transform.Find("MiddleText").GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("Life-1Text").GetComponent<TextMeshProUGUI>().text = "";
            transform.Find("Life-1Text").GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 0);
            Generalspecifications.Round += 1 ;
            Generalspecifications.Money += 10 + Generalspecifications.Round;
        }
        else
        {

            transform.Find("TextImage").gameObject.GetComponent<Image>().sprite = DefeatImage;
            if (Generalspecifications.Life > 1) transform.Find("NextButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = "Retry";
            else transform.Find("NextButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = "Exit";
            transform.Find("LifeText").GetComponent<TextMeshProUGUI>().text = Generalspecifications.Life.ToString();
            transform.Find("LifeText").GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            transform.Find("MiddleText").GetComponent<TextMeshProUGUI>().text = "-->";
            transform.Find("Life-1Text").GetComponent<TextMeshProUGUI>().text = (Generalspecifications.Life-1).ToString();
            transform.Find("Life-1Text").GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
            Generalspecifications.Life -= 1;
            Generalspecifications.Money += 10 + Generalspecifications.Round;
        }
    }
    public void NextButton()
    {
        if(Generalspecifications.Life <= 0 || Generalspecifications.Round>35)
        {
            Application.Quit();
        }
        ListObjectForGameplay.list = new();
        ListObjectForGameplay.listequipment = new();
        GameObject Bag = GameObject.Find("Bag");
        DontDestroyOnLoad(Bag);
        Destroy(GameObject.Find("EnemyTeam"));
        SceneManager.LoadScene("MainShop");
    }

    public void ViewDeatilBtn()
    {
        transform.Find("Statistic UI").GetChild(0).gameObject.SetActive(true);
    }

    

    public void XBtn()
    {
        if (Time.timeScale == 0f) return;
        if (isX2)
        {
            isX2 = false;
            Time.timeScale /= 2;
            transform.parent.Find("AccelerateTime").Find("xButton").GetComponent<Image>().sprite = x1Image;
        }
        else if (!isX2)
        {
            isX2 = true;
            Time.timeScale *= 2;
            transform.parent.Find("AccelerateTime").Find("xButton").GetComponent<Image>().sprite = x2Image;
        }
        
    }
}
