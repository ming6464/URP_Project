using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public bool Run;

    void Update()
    {  
       

        if (Run)
        {
            Quaternion rotation = Quaternion.FromToRotation(a.forward, b.forward);
            a.rotation = rotation;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(a.position,a.position + a.forward * 5);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(b.position,b.position + b.forward * 5);
    }


}


