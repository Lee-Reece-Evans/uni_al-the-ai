using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure_Placement : MonoBehaviour, IInteractable, IRepairable, IPooledObject  // base placement methods
{
    [Header("Tags")]
    public string structurePoolTag;
    public string poolTag;

    [Header("Components")]
    public Material placeMat;
    public Material blockedMat;
    public GameObject rangeIndicator;
    private Material[] realMats;
    private Transform player;
    private MeshRenderer[] childrenRen;

    [Header("Local Variables")]
    public bool isPlaced = false;
    public float spawnDistance = 4f;
    public float heightFromGround = 0f;
    public float rotationOffset = -90f;
    public float completion = 0f;

    private float buildSection = 100f;
    private int currentSection = 0;
    private List<GameObject> objInTrig = new List<GameObject>();
    private int groundLayer;

    public bool initialised = false;

    private void Start()
    {
        player = GameObject.Find("FPSController").transform;

        childrenRen = GetComponentsInChildren<MeshRenderer>(false); // only include active children

        realMats = new Material[childrenRen.Length];

        for (int i = 0; i < childrenRen.Length; i++)
        {
            realMats[i] = childrenRen[i].material;
            childrenRen[i].material = placeMat;
        }

        groundLayer = (1 << LayerMask.NameToLayer("Ground"));

        if (!initialised)
            AttachToPlayer();

        initialised = true;
    }

    private void OnEnable()
    {
        if (!initialised)
            return;

        if (!isPlaced) // Would be already placed if spawned by a destroyed turret instead of from shop, therefore not attaching is wanted.
            AttachToPlayer();
    }

    protected virtual void OnDisable()
    {
        if (!initialised)
            return;

        isPlaced = false;
        completion = 0f;
        buildSection = 100f;
        currentSection = 0;
        objInTrig.Clear();

        for (int i = 0; i < childrenRen.Length; i++)
            childrenRen[i].material = placeMat;

        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }

    public void SetPoolDetails(string tag)
    {
        poolTag = tag;
    }

    public void DoInteraction()
    {
        if (isPlaced && !IngameMenuManager.instance.shopUI.handsFull) // only be able to sell if it has been placed.
        {
            IngameMenuManager.instance.placementUI.SetStructure(this);
        }
    }

    private void AttachToPlayer()
    {
        transform.rotation = player.rotation;
        transform.Rotate(0, rotationOffset, 0);
        transform.parent = player;

        if (rangeIndicator != null)
            rangeIndicator.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
       if (!objInTrig.Contains(other.gameObject))
            objInTrig.Add(other.gameObject);

        if (objInTrig.Count.Equals(1))
            ApplyBlockedMaterial();
    }

    private void OnTriggerExit(Collider other)
    {
        if (objInTrig.Contains(other.gameObject))
            objInTrig.Remove(other.gameObject);

        if (objInTrig.Count.Equals(0))
            ApplyPlacementMaterial();
    }

    private void ApplyBlockedMaterial()
    {
        foreach (Renderer ren in childrenRen)
        {
            if (ren.material.name.Contains(placeMat.name))
                ren.material = blockedMat;
        }
    }

    private void ApplyPlacementMaterial()
    {
        foreach (Renderer ren in childrenRen)
        {
            if (ren.material.name.Contains(blockedMat.name))
                ren.material = placeMat;
        }
    }

    public void Repair(int amount)
    {
        if (objInTrig.Count.Equals(0) && isPlaced && completion < 100)
        {
            completion += amount;

            float sectiontoBuild = buildSection / childrenRen.Length;

            if (completion > sectiontoBuild)
            {
                childrenRen[currentSection].material = realMats[currentSection];
                buildSection += 100;
                currentSection++;
            }

            if (completion >= 100)
                BuildComplete();
        }
    }

    protected virtual void BuildComplete()
    {
        GameObject structure = ObjectPool.Instance.SpawnFromPool(structurePoolTag);

        if (structure != null)
        {
            structure.transform.position = transform.position;
            structure.transform.rotation = transform.rotation; ;
            structure.SetActive(true);
        }

        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    protected virtual void PlaceStructure()
    {
        isPlaced = true;
        transform.parent = null;
        IngameMenuManager.instance.shopUI.handsFull = false;

        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }

    public virtual void Refund()
    {
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }

    private void StucturePosition()
    {
        Vector3 structurePosition = player.position + player.forward * spawnDistance;

        RaycastHit groundHit;

        if (Physics.Raycast(structurePosition, Vector3.down, out groundHit, Mathf.Infinity, groundLayer))
        {
            transform.position = groundHit.point + Vector3.up * heightFromGround;
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
        }
    }

    private void RemoveDisabledFromList() // temp solution
    {
        // remove disabled objects from list if they were destroyed after entering and did not exit trigger
        objInTrig.RemoveAll(GO => GO.activeSelf == false || GO.transform.parent != null && GO.transform.parent.gameObject.activeSelf == false);

        if (objInTrig.Count.Equals(0))
            ApplyPlacementMaterial();
    }

    private void Update()
    {
        if (GameManager.instance.gamePaused || GameManager.instance.gameOver)
            return;

        if (!objInTrig.Count.Equals(0))
            RemoveDisabledFromList();

        if (!isPlaced)
        {
            StucturePosition();

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)) && !IngameMenuManager.instance.cancelMenuObj.activeSelf)
            {
                if (objInTrig.Count.Equals(0))
                {
                    SFXManager2D.instance.PlayPlacementSound();
                    PlaceStructure();
                }
                else
                    SFXManager2D.instance.PlayErrorSound();
            }
            else if (Input.GetMouseButtonDown(1) && !IngameMenuManager.instance.cancelMenuObj.activeSelf)
            {
                IngameMenuManager.instance.OpenCancelMenu();
            }
        }
    }
}
