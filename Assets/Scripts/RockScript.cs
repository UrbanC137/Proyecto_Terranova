using System.Collections;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    public int hitsToDestroy = 5; // Número de golpes necesarios para destruir la roca
    private int currentHits = 0;   // Contador de golpes
    public GameObject stonePrefab; // Referencia al prefab de la piedra que aparecerá
    public float stoneSpawnRadius = 1f; // Radio alrededor de la roca donde aparecerán las piedras
    public float stoneSpawnHeight = 1f; // Altura extra para que las piedras aparezcan más arriba
    public int stoneAmount = 1; // Cantidad de piedra que este objeto representará (puedes cambiarlo si quieres diferentes cantidades de piedras)
    private LogicPlayer playerLogic; // Referencia al script del jugador que controla el inventario de piedras


    // Método para detectar colisiones con el pico
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisionó tiene la etiqueta "Pickaxe" (pico)
        if (other.CompareTag("Pickaxe"))
        {
            currentHits++; // Incrementa el contador de golpes
            Debug.Log("La roca ha sido golpeada: " + currentHits + " veces.");

            // Si se ha alcanzado el número de golpes necesarios
            if (currentHits >= hitsToDestroy)
            {
                Debug.Log("¡La roca ha sido destruida!");

                // Llama al método para destruir la roca y generar piedras
                StartCoroutine(DestroyRock());
            }
        }
    }
    void Start()
    {
        // Encontrar al jugador en la escena (cambiar "Player" si tu jugador tiene otra etiqueta)
        playerLogic = GameObject.FindGameObjectWithTag("Player").GetComponent<LogicPlayer>();

        if (playerLogic == null)
        {
            Debug.LogError("No se encontró el script LogicPlayer en el jugador.");
        }
    }
    // Corrutina para destruir la roca y generar piedras
    private IEnumerator DestroyRock()
    {
        yield return new WaitForSeconds(0); // No hay delay, pero puedes añadir si lo prefieres

        // Instanciar tres piedras en posiciones ligeramente diferentes alrededor de la roca
        if (stonePrefab != null)
        {
            for (int i = 0; i < 3; i++) // Generar 3 piedras
            {
                // Calcular una posición aleatoria alrededor de la roca y aumentar la posición en Y
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(-stoneSpawnRadius, stoneSpawnRadius),
                    stoneSpawnHeight,  // Ajustamos la altura aquí
                    Random.Range(-stoneSpawnRadius, stoneSpawnRadius)
                );
                Instantiate(stonePrefab, spawnPosition, Quaternion.identity); // Crear la piedra
            }

            Debug.Log("3 piedras han aparecido.");
        }

        Destroy(gameObject); // Destruir la roca original
    }
}
