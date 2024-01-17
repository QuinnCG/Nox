using Game.AI.BossSystem;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public DoorController doorController;
    public BossSpawner bossSpawner;
    public Boss isdead;

    void Update()
    {
        if (Boss.Health.IsDead())
        {
            doorController.OpenAllDoors();
        }
        else
        {
            doorController.CloseAllDoors();
        }
    }
}
