using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAutoMoveAndDestroy : MonoBehaviour
{
    private float time;
    private float count;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * 2;
        if(count == 10 || Time.timeScale == 0)
        {
            Destroy(gameObject);
        }
        if (time > 1f)
        {
            Move();
            time = 0;
            count++;
        }
        
            
    }
    private void Move()
    {
        transform.localPosition += new Vector3(0, 43.75f/10f, 0);
    }
    
}
