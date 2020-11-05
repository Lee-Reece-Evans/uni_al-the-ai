using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    public int baseCrystalAmount = 4;

    [Header("Components")]
    public GameObject resource;
    public GameObject[] resources;
    public List<Transform> spawnPoints;
    public ResourcePoint[] resourcePoints;
    public Vector3[] resourcePosition;

    private float initialResources;
    private float resourceRemaining;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int numOfResources = baseCrystalAmount + DifficultyManager.instance.difficulty_ResourceModifier[DifficultyManager.instance.difficulty]; // work out how many resurces will be used.

        initialResources = numOfResources * 100f; // starting resources
        resourceRemaining = initialResources; // initalise resources remaining

        // initialise arrays
        resources = new GameObject[numOfResources];
        resourcePosition = new Vector3[numOfResources];
        resourcePoints = new ResourcePoint[numOfResources];

        // for loop here to instantialte number of resources based on difficulty?
        for (int i = 0; i < numOfResources; i++)
        {
            GameObject newResource = Instantiate(resource, this.gameObject.transform);
            resources[i] = newResource;
        }

        for (int i = 0; i < numOfResources; i++)
        {
            // set a random position for the resouce, for list of predifined locations, then remove that position from the list.
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            resources[i].transform.position = spawnPoints[spawnIndex].position;
            spawnPoints.Remove(spawnPoints[spawnIndex]);

            resourcePosition[i] = resources[i].transform.position;
            resourcePoints[i] = resources[i].GetComponentInChildren<ResourcePoint>();
        }
    }

    public void ResourceTaken(int amount)
    {
        if (resourceRemaining > 0)
        {
            resourceRemaining -= amount;

            if (resourceRemaining < 0)
                resourceRemaining = 0;

            float resourcePercentage = (resourceRemaining / initialResources) * 100f;
            SetResourceBar(resourcePercentage);
        }
        if (resourceRemaining == 0)
            GameManager.instance.GameLost();
    }

    private void SetResourceBar(float amount)
    {
        OnScreenUI_Manager.Instance.SetResourceValue(amount);
    }
}
