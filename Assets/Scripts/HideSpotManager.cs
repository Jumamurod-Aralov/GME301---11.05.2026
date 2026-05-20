using UnityEngine;
using System.Collections.Generic;

public class HideSpotManager : MonoBehaviour
{
    public static HideSpotManager Instance { get; private set; }

    private HashSet<Transform> occupiedSpots = new HashSet<Transform>();
    private Transform[] allHideSpots;

    void Awake()
    {
        // CHANGED: Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // CHANGED: Cache all HideSpots by tag
        GameObject[] spots = GameObject.FindGameObjectsWithTag("HideSpot");
        allHideSpots = new Transform[spots.Length];
        for (int i = 0; i < spots.Length; i++)
        {
            allHideSpots[i] = spots[i].transform;
        }
        Debug.Log($"HideSpotManager found {allHideSpots.Length} hide spots");
    }

    // CHANGED: Find nearest empty spot and occupy it
    public Transform FindNearestEmptySpot(Vector3 position)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform spot in allHideSpots)
        {
            if (occupiedSpots.Contains(spot)) continue;

            float distance = Vector3.Distance(position, spot.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = spot;
            }
        }

        if (nearest != null)
            occupiedSpots.Add(nearest);

        return nearest;
    }

    // CHANGED: Release spot when enemy leaves
    public void ReleaseHideSpot(Transform spot)
    {
        occupiedSpots.Remove(spot);
    }

    public bool IsSpotOccupied(Transform spot) => occupiedSpots.Contains(spot);
}