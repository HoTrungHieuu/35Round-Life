using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TextAdjustWithObject : MonoBehaviour
{
    public GameObject buttonObject;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(buttonObject.transform.localPosition.x, buttonObject.transform.localPosition.y - 80f, 0);
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
