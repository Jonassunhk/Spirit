using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basketballCage : MonoBehaviour
{
    public bool positionFixed = false;
    public soundManager soundManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.x >= -91.6f && !positionFixed)
        {
            positionFixed = true;
            soundManager.PlaySound("boxHit");
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -252.72f);
        }
    }
}
