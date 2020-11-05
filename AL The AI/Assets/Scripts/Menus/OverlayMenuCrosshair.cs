using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayMenuCrosshair : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 cursorPos = new Vector3(mousePos.x, mousePos.y, 0f);
        transform.position = cursorPos;
    }
}
