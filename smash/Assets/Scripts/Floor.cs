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
            // Teletransportar al jugador
            other.transform.position = teleportTarget.position;

            // Quitar 50 de vida
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(30);
                Debug.Log($"{other.name} cayó al piso y perdió 50 de vida. Vida restante: {player.health}");
            }
        }
    }
}
