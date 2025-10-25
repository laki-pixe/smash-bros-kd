using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [Header("Destino del jugador al caer")]
    public Transform teleportTarget; // Asigna aquí tu Empty Object en el inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget.position;
        }
    }
}