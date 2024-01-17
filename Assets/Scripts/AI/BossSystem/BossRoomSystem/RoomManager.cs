using Game.AI.BossSystem;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public DoorController doorController { get; private set; }
    public BossSpawner bossSpawner { get; private set; }
	public Boss boss { get; private set; }

	void Update()
    {
        if (boss.Health.IsDead)
        {
            doorController.OpenAllDoors();
        }
        else
        {
            doorController.CloseAllDoors();
        }
    }
}
