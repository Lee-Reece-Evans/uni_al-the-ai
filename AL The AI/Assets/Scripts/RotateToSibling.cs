using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToSibling : MonoBehaviour
{
    private Sibling_Placement sp;
    void Start()
    {
        sp = GetComponent<Sibling_Placement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sp.siblingGO != null)
        {
            // keep y pos fixed
            Vector3 lookPos = new Vector3(sp.siblingGO.transform.position.x, transform.position.y, sp.siblingGO.transform.position.z);
            transform.LookAt(lookPos);
        }
    }
}
