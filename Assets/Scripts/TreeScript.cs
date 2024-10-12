using System.Collections;  // Asegúrate de tener esta directiva para usar IEnumerator
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public int hitsToFall = 5; // Número de golpes necesarios para que el árbol caiga
    private int currentHits = 0; // Contador de golpes
    private Rigidbody rb; // Referencia al Rigidbody del árbol
    public float destroyDelay = 5f; // Tiempo en segundos antes de que el árbol desaparezca
    public GameObject trunkPrefab; // Referencia al prefab del tronco
    public float trunkSpawnRadius = 1f; // Radio alrededor del árbol donde aparecerán los troncos

    void Start()
    {
        // Obtener el Rigidbody y configurarlo al inicio
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Desactiva las físicas al principio
        }
        else
        {
            Debug.LogError("El árbol necesita un Rigidbody para simular su caída.");
        }
    }

    // Método para detectar colisiones con el hacha
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe")) // Asegúrate de que el hacha tenga la etiqueta "Axe"
        {
            currentHits++;
            Debug.Log("El árbol ha sido golpeado: " + currentHits + " veces.");

            // Si se ha golpeado el número de veces requerido, activar el Rigidbody y comenzar la cuenta regresiva para destruir el árbol
            if (currentHits >= hitsToFall)
            {
                if (rb != null)
                {
                    rb.isKinematic = false; // Activa las físicas para que el árbol caiga
                    Debug.Log("¡El árbol está cayendo!");

                    // Iniciar la corrutina para destruir el árbol después de un tiempo
                    StartCoroutine(DestroyTreeAfterDelay());
                }
            }
        }
    }

    // Corrutina para destruir el árbol después de unos segundos
    private IEnumerator DestroyTreeAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay); // Espera unos segundos

        // Instanciar tres troncos en posiciones ligeramente diferentes alrededor del árbol
        if (trunkPrefab != null)
        {
            for (int i = 0; i < 3; i++) // Generar 3 troncos
            {
                // Calcular una posición aleatoria alrededor del árbol
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-trunkSpawnRadius, trunkSpawnRadius), 0, Random.Range(-trunkSpawnRadius, trunkSpawnRadius));

                Instantiate(trunkPrefab, spawnPosition, Quaternion.identity); // Crear el tronco
            }

            Debug.Log("3 troncos han aparecido.");
        }

        Destroy(gameObject); // Destruir el objeto del árbol
    }
}
