using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player entered the boss spawn area, spawn the boss
            SpawnBoss();
        }
    }

    public void SpawnBoss()
    {
        Instantiate(bossPrefab, transform.position, Quaternion.identity);
    }
}
