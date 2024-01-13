using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectFormation : MonoBehaviour
{
    public int positionCharacter;
    public int positionInTeam;

    private void OnMouseDown()
    {
        GameObject ShopMenu = GameObject.Find("ShopMenu");
        if (ShopMenu.GetComponent<Shop>().positionCharacterInTeam == positionInTeam && ShopMenu.GetComponent<Shop>().positionSelected == positionCharacter)
        {
            ShopMenu.GetComponent<Shop>().positionCharacterInTeam = 0;
            ShopMenu.GetComponent<Shop>().positionSelected = 0;
            return;
        }
        if(ShopMenu.GetComponent<Shop>().positionCharacterInTeam >0)
        {
            if (ShopMenu.GetComponent<Shop>().positionCharacterInTeam < 6)
            {
                ShopMenu.GetComponent<Shop>().positionSelected = positionCharacter;
            }
            
            ShopMenu.GetComponent<Shop>().SwapCharacterPositionInteam(positionInTeam);
            return;
        }
        //if(ShopMenu.GetComponent<Shop>().positionCharacterInTeam == positionInTeam && ShopMenu.GetComponent<Shop>().positionSelected)
        ShopMenu.GetComponent<Shop>().positionSelected = positionCharacter;
        ShopMenu.GetComponent<Shop>().positionCharacterInTeam = positionInTeam;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
