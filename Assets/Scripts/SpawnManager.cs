using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] private SpawnPoint [] spawns;

    private void Awake()
    {
        Instance = this;
        spawns = GetComponentsInChildren<SpawnPoint>();
    }

    public Transform GetSpawnPoint()
    {
        return spawns[Random.Range(0, spawns.Length)].transform;
    }
}
