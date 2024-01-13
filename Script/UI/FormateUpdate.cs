using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormateUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        FormUpdate();
    }

    private void FormUpdate()
    {
        GameObject ShopMenu = GameObject.Find("ShopMenu");
        GameObject Inventory = ShopMenu.transform.Find("Inventory").gameObject;
        GameObject CharacterInTeam = Inventory.transform.Find("CharacterInTeam").gameObject;
        GameObject P0 = CharacterInTeam.transform.Find("P0").gameObject;
        GameObject P1 = CharacterInTeam.transform.Find("P1").gameObject;
        GameObject P2 = CharacterInTeam.transform.Find("P2").gameObject;
        GameObject P3 = CharacterInTeam.transform.Find("P3").gameObject;
        GameObject P4 = CharacterInTeam.transform.Find("P4").gameObject;

        if (P0.transform.childCount != 0) transform.Find("P0Button").gameObject.SetActive(false);
        else transform.Find("P0Button").gameObject.SetActive(true);
        if (P1.transform.childCount != 0) transform.Find("P1Button").gameObject.SetActive(false);
        else transform.Find("P1Button").gameObject.SetActive(true);
        if (P2.transform.childCount != 0) transform.Find("P2Button").gameObject.SetActive(false);
        else transform.Find("P2Button").gameObject.SetActive(true);
        if (P3.transform.childCount != 0) transform.Find("P3Button").gameObject.SetActive(false);
        else transform.Find("P3Button").gameObject.SetActive(true);
        if (P4.transform.childCount != 0) transform.Find("P4Button").gameObject.SetActive(false);
        else transform.Find("P4Button").gameObject.SetActive(true);
    }
}
