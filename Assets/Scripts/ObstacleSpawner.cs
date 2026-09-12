using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Tham Chieu Trong Scene")]
    public Transform groundTransform;   
    public Transform playerTransform;   
    public GameObject[] obstaclePrefabs;
    public GameObject coinPrefab;        

    [Header("Cau Hinh Lan Luot (Truc Z)")]
    public float[] zLanes = new float[] { -12f, -2f, 8f };

    [Header("Cau Hinh Phan Bo Tren Truc X")]
    public float minXGap = 12f;           
    public float maxXGap = 18f;           
    public float initialSafeDistance = 30f; 

    [Header("Cau Hinh Sinh Coin")]
    [Range(0f, 1f)]
    public float coinSpawnChance = 0.35f; 
    public float minCoinGap = 20f;        

    private float planeUnitSize = 10f;
    private float lastSpawnedEdgeX;       
    private float lastCoinSpawnedX = -9999f; 
    private int lastLane = -1;
    private int lastCoinLane = -1; // Luu vet lan cua Coin gan nhat
    private int lastPrefabIndex = -1; 
    private bool isInitialized = false;

    IEnumerator Start()
    {
        yield return null; 

        if (ValidateInputs())
        {
            InitializeSpawner();
        }
    }

    bool ValidateInputs()
    {
        if (groundTransform == null || playerTransform == null || coinPrefab == null) return false;
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return false;

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            if (obstaclePrefabs[i] == null) return false;
        }

        return true;
    }

    void InitializeSpawner()
    {
        lastSpawnedEdgeX = playerTransform.position.x + initialSafeDistance;
        isInitialized = true;

        SpawnObstaclesToFront();
    }

    void Update()
    {
        if (!isInitialized || groundTransform == null) return;
        SpawnObstaclesToFront();
    }

    void SpawnObstaclesToFront()
    {
        float groundFrontX = groundTransform.position.x + (groundTransform.localScale.x * planeUnitSize / 2f) - 10f;

        while (lastSpawnedEdgeX < groundFrontX)
        {
            bool canSpawnCoin = (lastSpawnedEdgeX - lastCoinSpawnedX >= minCoinGap);
            bool shouldSpawnCoin = canSpawnCoin && (Random.value < coinSpawnChance);

            float gap = Random.Range(minXGap, maxXGap);

            if (shouldSpawnCoin)
            {
                int coinLane = GetValidCoinLane();
                int prefabIndex = GetValidPrefabIndex();
                GameObject prefab = obstaclePrefabs[prefabIndex];

                float coinLen = GetPrefabLengthX(coinPrefab);
                float obsLen = GetPrefabLengthX(prefab);

                float spawnCenterX = lastSpawnedEdgeX + gap + (Mathf.Max(coinLen, obsLen) / 2f);

                // Sinh Coin
                float coinWorldY = groundTransform.position.y + coinPrefab.transform.position.y;
                Vector3 coinPos = new Vector3(spawnCenterX, coinWorldY, zLanes[coinLane]);
                Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, transform);

                // Sinh Obstacle o 1 trong 2 lan con lai
                List<int> remainingLanes = new List<int>();
                for (int i = 0; i < zLanes.Length; i++)
                {
                    if (i != coinLane) remainingLanes.Add(i);
                }
                int obstacleLane = remainingLanes[Random.Range(0, remainingLanes.Count)];

                float obsWorldY = groundTransform.position.y + prefab.transform.position.y + 2f;
                Vector3 obsPos = new Vector3(spawnCenterX, obsWorldY, zLanes[obstacleLane]);

                Quaternion spawnRotation = prefab.transform.rotation;
                if (Random.value > 0.5f) spawnRotation *= Quaternion.Euler(0, 180f, 0);

                Instantiate(prefab, obsPos, spawnRotation, transform);

                // Cap nhat vi tri va luu vet lan Coin CHINH XAC
                lastSpawnedEdgeX = spawnCenterX + (Mathf.Max(coinLen, obsLen) / 2f);
                lastCoinSpawnedX = spawnCenterX;
                lastCoinLane = coinLane; 
                lastLane = obstacleLane;
                lastPrefabIndex = prefabIndex;
            }
            else
            {
                int prefabIndex = GetValidPrefabIndex();
                GameObject prefab = obstaclePrefabs[prefabIndex];
                float obsLen = GetPrefabLengthX(prefab);

                float spawnCenterX = lastSpawnedEdgeX + gap + (obsLen / 2f);

                int currentLane = GetValidLane();
                float worldY = groundTransform.position.y + prefab.transform.position.y + 2f;

                Vector3 spawnPosition = new Vector3(spawnCenterX, worldY, zLanes[currentLane]);

                Quaternion spawnRotation = prefab.transform.rotation;
                if (Random.value > 0.5f) spawnRotation *= Quaternion.Euler(0, 180f, 0);

                Instantiate(prefab, spawnPosition, spawnRotation, transform);

                lastSpawnedEdgeX = spawnCenterX + (obsLen / 2f);
                lastLane = currentLane;
                lastPrefabIndex = prefabIndex;
            }
        }
    }

    float GetPrefabLengthX(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                b.Encapsulate(renderers[i].bounds);
            }
            return b.size.x;
        }

        Collider col = prefab.GetComponentInChildren<Collider>();
        if (col != null) return col.bounds.size.x;

        return 3f;
    }

    int GetValidPrefabIndex()
    {
        List<int> validIndices = new List<int>();

        for (int i = 0; i < obstaclePrefabs.Length; i++)
        {
            if (IsValidPrefab(i)) validIndices.Add(i);
        }

        if (validIndices.Count == 0) return Random.Range(0, obstaclePrefabs.Length);
        
        return validIndices[Random.Range(0, validIndices.Count)];
    }

    bool IsValidPrefab(int currentIndex)
    {
        if (lastPrefabIndex == -1) return true;
        if (currentIndex == lastPrefabIndex) return false;

        int currentID = currentIndex + 1;
        int lastID = lastPrefabIndex + 1;

        if (IsInGroup(currentID, 1, 8, 11) && IsInGroup(lastID, 1, 8, 11)) return false;
        if (IsInGroup(currentID, 2, 6) && IsInGroup(lastID, 2, 6)) return false;
        if (IsInGroup(currentID, 3, 7) && IsInGroup(lastID, 3, 7)) return false;

        return true;
    }

    bool IsInGroup(int id, params int[] group)
    {
        for (int i = 0; i < group.Length; i++)
        {
            if (id == group[i]) return true;
        }
        return false;
    }

    int GetValidLane()
    {
        List<int> availableLanes = new List<int>();
        for (int i = 0; i < zLanes.Length; i++)
        {
            if (i != lastLane) availableLanes.Add(i);
        }
        return availableLanes[Random.Range(0, availableLanes.Count)];
    }

    int GetValidCoinLane()
    {
        List<int> availableLanes = new List<int>();
        for (int i = 0; i < zLanes.Length; i++)
        {
            // Loai bo lan cua Coin truoc do
            if (i != lastCoinLane) availableLanes.Add(i);
        }

        if (availableLanes.Count == 0) return Random.Range(0, zLanes.Length);

        return availableLanes[Random.Range(0, availableLanes.Count)];
    }
}