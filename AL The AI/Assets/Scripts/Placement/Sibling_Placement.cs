using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sibling_Placement : Structure_Placement, IHasSibling // for placing structures with sibling dependants
{
    [Header("GameObjects")]
    public string siblingPrefabPoolTag;
    public GameObject siblingPrefab;
    public GameObject siblingGO;

    [Header("Local Variables")]
    public bool hasDisRestriction = false;
    public float minDist;
    public float maxDist;

    protected override void OnDisable()
    {
        base.OnDisable();

        siblingPrefab = null;
        siblingGO = null;
    }

    public void PassSiblingGO(GameObject sibling)
    {
        siblingGO = sibling;
    }

    protected override void BuildComplete()
    {
        GameObject sibling = ObjectPool.Instance.SpawnFromPool(structurePoolTag);

        if (sibling != null)
        {
            sibling.transform.position = transform.position;
            sibling.transform.rotation = transform.rotation;

            IHasSibling thisSibling = sibling.GetComponent<IHasSibling>();

            if (thisSibling != null)
                thisSibling.PassSiblingGO(siblingGO); // pass my sibling gameobject reference to my new structure.

            IHasSibling actualSibling = siblingGO.GetComponent<IHasSibling>();

            if (actualSibling != null)
                actualSibling.PassSiblingGO(sibling); // pass my new structure to be my sibling's sibling gameobject reference.

            sibling.SetActive(true);
        }

        ObjectPool.Instance.ReturnToPool(poolTag, gameObject); // put me back into the pool
    }

    protected override void PlaceStructure()
    {
        if (siblingGO == null) // sibling has not been instatiated
        {
            isPlaced = true;
            transform.parent = null;

            //siblingPrefab = Instantiate(siblingPrefab, transform.position, transform.rotation); // create fake sibling
            siblingPrefab = ObjectPool.Instance.SpawnFromPool(siblingPrefabPoolTag);

            if (siblingPrefab != null)
            {
                siblingPrefab.transform.position = transform.position;
                siblingPrefab.transform.rotation = transform.rotation;
                siblingPrefab.GetComponent<Sibling_Placement>().siblingGO = gameObject; // reference to this GO in sibling
                siblingGO = siblingPrefab;  // reference to sibling in this GO
                siblingPrefab.SetActive(true);
            }
        }
        else if (siblingGO != null) // sibling exsits
        {
            if (hasDisRestriction) // has a distance restriction to sibling
            {
                float distance = Vector3.Distance(siblingGO.transform.position, transform.position);

                if (distance >= minDist && distance <= maxDist) // sibling GO is or isnt in range for placement
                    base.PlaceStructure(); // can be placed
            }
            else
                base.PlaceStructure();
        }
    }

    public override void Refund()
    {
        string siblingPoolTag = siblingGO.GetComponent<Structure_Placement>().poolTag;
        ObjectPool.Instance.ReturnToPool(siblingPoolTag, siblingGO);
        ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
    }
}
