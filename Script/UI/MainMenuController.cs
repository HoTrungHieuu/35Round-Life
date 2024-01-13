using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Start").gameObject.SetActive(false);
    }

    public void ExitBtn()
    {
        Application.Quit();
    }

    public void StartBtn()
    {
        transform.Find("Start").gameObject.SetActive(true);
        transform.Find("Menu").gameObject.SetActive(false);
    }

    public void BackBtn()
    {
        transform.Find("Start").gameObject.SetActive(false);
        transform.Find("Menu").gameObject.SetActive(true);
    }

    public void LevelBtn(int index)
    {
        Generalspecifications.Round = 1;
        Generalspecifications.Money = 10;
        if (index == 0) Generalspecifications.Life = 999;
        else Generalspecifications.Life = 7 - (index - 1) * 2;
        SceneManager.LoadScene("MainShop");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
