using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Set_Invisibility : MonoBehaviour
{
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private Material[] originalMats;
    [SerializeField] private Material invisibleMat;

    public bool invisible = true;
    public int insideDetectorTrigger = 0;

    private void Start()
    {
        originalMats = new Material[meshRenderers.Length];

        // take a copy of original materials
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMats[i] = meshRenderers[i].material;
        }
        UnSeen(); // set invisible
    }

    private void OnDisable()
    {
        if (!invisible) // reset back to invisible by default
        {
            insideDetectorTrigger = 0;
            invisible = true;
            UnSeen();
        }
    }

    public void Seen()
    {
        // put back original materials
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = originalMats[i];
        }
    }

    public void UnSeen()
    {
        // change materials to invisible
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = invisibleMat;
        }
    }
}
