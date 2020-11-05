using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze_Tower : MonoBehaviour, IPooledObject
{
    public string poolTag;

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    private void OnTriggerEnter(Collider other)
    {
        Slowable slowable = other.GetComponent<Slowable>();

        if (slowable != null)
            slowable.Slow();
    }

    private void OnTriggerExit(Collider other)
    {
        Slowable slowable = other.GetComponent<Slowable>();

        if (slowable != null)
            slowable.UndoSlow();
    }
}
