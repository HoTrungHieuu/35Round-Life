using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test_1 : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GameObject.Find("Bag").name);
    }
    public void Buttona()
    {
        GameObject Bag = GameObject.Find("Bag");
        DontDestroyOnLoad(Bag);
        SceneManager.LoadScene("MainShop");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
