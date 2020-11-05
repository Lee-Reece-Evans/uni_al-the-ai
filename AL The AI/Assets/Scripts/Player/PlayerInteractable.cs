using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private float structureRange = 5;
    [SerializeField] private float FriendlyAIRange = 50;

    private int interactableLayer;

    private void Start()
    {
        interactableLayer = (1 << LayerMask.NameToLayer("Interactable")) | (1 << LayerMask.NameToLayer("Buildable"));
    }

    private void TryInteraction(RaycastHit Hit)
    {
        IInteractable interactable = Hit.transform.GetComponentInParent<IInteractable>();

        if (interactable != null)
            interactable.DoInteraction();
    }

    private void Update()
    {
        if (GameManager.instance.gamePaused || GameManager.instance.gameOver) // don't interact if game paused or is gameover
            return;

        if (Input.GetKeyDown(KeyCode.F) && !IngameMenuManager.instance.shopMenuObj.activeSelf)
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit Hit, Mathf.Infinity, interactableLayer))
            {
                if (Hit.transform.root.gameObject.CompareTag("structure"))
                {
                    if (Hit.distance <= structureRange)
                        TryInteraction(Hit);
                }
                else // for AI interaction
                {
                    if (Hit.distance <= FriendlyAIRange)
                        TryInteraction(Hit);
                }
            }
        }
    }
}
