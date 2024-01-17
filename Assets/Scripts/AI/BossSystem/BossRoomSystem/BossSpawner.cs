using Game.Player;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject boss;
    public Transform SpawnPoint;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerManager.Instance.PossessedCharacter)
        {
            // Player entered the boss spawn area, spawn the boss
            SpawnBoss();
        }
    }

    public void SpawnBoss()
    {
        Instantiate(boss, SpawnPoint.position, Quaternion.identity);
    }
}
