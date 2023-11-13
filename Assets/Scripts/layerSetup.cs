using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layerSetup : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        
        int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        gameObject.layer = LayerIgnoreRaycast;

        var children = gameObject.GetComponentsInChildren<Transform>(includeInactive: true) ;
        foreach (var child in children)
        {
            Debug.Log(child);
            child.gameObject.layer = LayerIgnoreRaycast;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
