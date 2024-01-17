using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject[] doors;

    public void OpenAllDoors()
    {
        foreach (var door in doors)
        {
            door.SetActive(false);
        }
    }

    public void CloseAllDoors()
    {
        foreach (var door in doors)
        {
            door.SetActive(true);
        }
    }
}
