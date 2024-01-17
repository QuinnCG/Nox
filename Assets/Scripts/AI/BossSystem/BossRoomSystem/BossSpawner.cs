using Game.Player;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject bossPrefab;
	public Transform SpawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerManager.Instance.PossessedCharacter)
        {
            // Player entered the boss spawn area, spawn the boss
            SpawnBoss();
        }
    }

    public void SpawnBoss()
    {
        Instantiate(bossPrefab, SpawnPoint.position, Quaternion.identity);
    }
}
