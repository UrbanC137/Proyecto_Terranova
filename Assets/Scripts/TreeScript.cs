using System.Collections;  // Aseg�rate de tener esta directiva para usar IEnumerator
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public int hitsToFall = 5; // N�mero de golpes necesarios para que el �rbol caiga
    private int currentHits = 0; // Contador de golpes
    private Rigidbody rb; // Referencia al Rigidbody del �rbol
    public float destroyDelay = 5f; // Tiempo en segundos antes de que el �rbol desaparezca
    public GameObject trunkPrefab; // Referencia al prefab del tronco
    public float trunkSpawnRadius = 1f; // Radio alrededor del �rbol donde aparecer�n los troncos

    void Start()
    {
        // Obtener el Rigidbody y configurarlo al inicio
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Desactiva las f�sicas al principio
        }
        else
        {
            Debug.LogError("El �rbol necesita un Rigidbody para simular su ca�da.");
        }
    }

    // M�todo para detectar colisiones con el hacha
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Axe")) // Aseg�rate de que el hacha tenga la etiqueta "Axe"
        {
            currentHits++;
            Debug.Log("El �rbol ha sido golpeado: " + currentHits + " veces.");

            // Si se ha golpeado el n�mero de veces requerido, activar el Rigidbody y comenzar la cuenta regresiva para destruir el �rbol
            if (currentHits >= hitsToFall)
            {
                if (rb != null)
                {
                    rb.isKinematic = false; // Activa las f�sicas para que el �rbol caiga
                    Debug.Log("�El �rbol est� cayendo!");

                    // Iniciar la corrutina para destruir el �rbol despu�s de un tiempo
                    StartCoroutine(DestroyTreeAfterDelay());
                }
            }
        }
    }

    // Corrutina para destruir el �rbol despu�s de unos segundos
    private IEnumerator DestroyTreeAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay); // Espera unos segundos

        // Instanciar tres troncos en posiciones ligeramente diferentes alrededor del �rbol
        if (trunkPrefab != null)
        {
            for (int i = 0; i < 3; i++) // Generar 3 troncos
            {
                // Calcular una posici�n aleatoria alrededor del �rbol
                Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-trunkSpawnRadius, trunkSpawnRadius), 0, Random.Range(-trunkSpawnRadius, trunkSpawnRadius));

                Instantiate(trunkPrefab, spawnPosition, Quaternion.identity); // Crear el tronco
            }

            Debug.Log("3 troncos han aparecido.");
        }

        Destroy(gameObject); // Destruir el objeto del �rbol
    }
}
