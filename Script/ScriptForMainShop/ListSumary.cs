using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListSumary : MonoBehaviour
{
    public List<GameObject> CharacterObjects;
    public List<GameObject> ItemObjects;
    public List<Sprite> imageList;
    public List<GameObject> getListCharacters()
    {
        return CharacterObjects;
    }
    public List<GameObject> getListItems()
    {
        return ItemObjects;
    }
    void Awake()
    {
        Generalspecifications.imageList = new();
        for (int i =0;i<imageList.Count;i++)
        {
            Generalspecifications.imageList.Add(imageList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
