using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class FishTank : MonoBehaviour
{
    [Header("Tank Settings")]
    public Vector3 tankSize = new Vector3(10f, 6f, 8f);

    [Header("Fish Prefabs")]
    public GameObject normalFishPrefab;
    public GameObject predatorFishPrefab;

    [Header("UI References")]
    public TMP_InputField normalFishInput;
    public TMP_InputField predatorFishInput;
    public Button updateButton;
    public TMP_Text fishCountText;

    [Header("Spawning Settings")]
    public int initialNormalFish = 5;
    public int initialPredatorFish = 2;

    private List<GameObject> normalFishes = new List<GameObject>();
    private List<GameObject> predatorFishes = new List<GameObject>();

    private void Start()
    {
        // Setup UI
        normalFishInput.text = initialNormalFish.ToString();
        predatorFishInput.text = initialPredatorFish.ToString();
        updateButton.onClick.AddListener(UpdateFishCount);

        // Initial spawn
        SpawnInitialFish();
        UpdateFishCountText();
    }

    private void SpawnInitialFish()
    {
        for (int i = 0; i < initialNormalFish; i++)
        {
            SpawnNormalFish();
        }

        for (int i = 0; i < initialPredatorFish; i++)
        {
            SpawnPredatorFish();
        }
    }

    private void UpdateFishCount()
    {
        int targetNormalCount = int.Parse(normalFishInput.text);
        int targetPredatorCount = int.Parse(predatorFishInput.text);

        // Update normal fish count
        while (normalFishes.Count < targetNormalCount)
        {
            SpawnNormalFish();
        }
        while (normalFishes.Count > targetNormalCount)
        {
            RemoveNormalFish();
        }

        // Update predator fish count
        while (predatorFishes.Count < targetPredatorCount)
        {
            SpawnPredatorFish();
        }
        while (predatorFishes.Count > targetPredatorCount)
        {
            RemovePredatorFish();
        }

        UpdateFishCountText();
    }

    private void SpawnNormalFish()
    {
        Vector3 spawnPos = GetRandomPositionInTank();
        GameObject fish = Instantiate(normalFishPrefab, spawnPos, Quaternion.identity);
        normalFishes.Add(fish);
    }

    private void SpawnPredatorFish()
    {
        Vector3 spawnPos = GetRandomPositionInTank();
        GameObject fish = Instantiate(predatorFishPrefab, spawnPos, Quaternion.identity);
        predatorFishes.Add(fish);
    }

    private void RemoveNormalFish()
    {
        if (normalFishes.Count > 0)
        {
            GameObject fish = normalFishes[normalFishes.Count - 1];
            normalFishes.RemoveAt(normalFishes.Count - 1);
            Destroy(fish);
        }
    }

    private void RemovePredatorFish()
    {
        if (predatorFishes.Count > 0)
        {
            GameObject fish = predatorFishes[predatorFishes.Count - 1];
            predatorFishes.RemoveAt(predatorFishes.Count - 1);
            Destroy(fish);
        }
    }

    private Vector3 GetRandomPositionInTank()
    {
        return new Vector3(
            Random.Range(-tankSize.x / 2, tankSize.x / 2),
            Random.Range(-tankSize.y / 2, tankSize.y / 2),
            Random.Range(-tankSize.z / 2, tankSize.z / 2)
        );
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, -tankSize.x / 2 + 0.5f, tankSize.x / 2 - 0.5f),
            Mathf.Clamp(position.y, -tankSize.y / 2 + 0.5f, tankSize.y / 2 - 0.5f),
            Mathf.Clamp(position.z, -tankSize.z / 2 + 0.5f, tankSize.z / 2 - 0.5f)
        );
    }

    public Vector3 GetTankSize()
    {
        return tankSize;
    }

    private void UpdateFishCountText()
    {
        fishCountText.text = $"Normal: {normalFishes.Count} | Predator: {predatorFishes.Count}";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, tankSize);
    }
}