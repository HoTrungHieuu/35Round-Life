using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAdjustWithObject : MonoBehaviour
{
    public GameObject buttonObject;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(buttonObject.transform.localPosition.x + 120f, buttonObject.transform.localPosition.y - 50f, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
