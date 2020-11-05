using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpaceMenuCrosshair : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorPos = new Vector3(mousePos.x, mousePos.y, 0f);
        transform.position = cursorPos;
    }
}
