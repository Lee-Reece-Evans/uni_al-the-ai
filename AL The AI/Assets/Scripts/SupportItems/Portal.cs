using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour, IHasSibling, IPooledObject
{
    public string poolTag;
    public GameObject siblingGO;
    public GameObject player;
    CharacterController playerController;
    FirstPersonController firstPersonController;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<CharacterController>();
        firstPersonController = player.GetComponent<FirstPersonController>();
    }

    private void OnDisable()
    {
        siblingGO = null;
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    public void PassSiblingGO(GameObject sibling)
    {
        siblingGO = sibling;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SFXManager2D.instance.PlayTeleportSound();
            playerController.enabled = false;
            other.gameObject.transform.position = siblingGO.transform.position + Vector3.up + siblingGO.transform.forward * 2;
            other.gameObject.transform.rotation = siblingGO.transform.rotation;
            firstPersonController.InitMouseLook(); // reinitialise the rotation so that the fps controller doesn't snap back.
            playerController.enabled = true;
        }
    }
}
