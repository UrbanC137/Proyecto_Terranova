using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : MonoBehaviour
{
    private Animator animator;
    private string currentState;

    // Parámetros de animación existentes
    private string sleepAnimationParam = "Sleep";
    private string walkAnimationParam = "WalkForward";
    private string runAnimationParam = "Run Forward"; // Usar este parámetro para correr
    private string attackAnimationParam = "Attack1"; // Usar este parámetro para atacar
    private string sitAnimationParam = "Sit";

    // Referencia al jugador
    public Transform player;
    public float detectionRange = 20f; // Rango en el que el oso detecta al jugador
    public float attackRange = 5f; // Rango para iniciar el ataque
    public float maxChaseRange = 100f; // Rango máximo para dejar de perseguir al jugador
    public float walkSpeed = 2f;
    public float runSpeed = 2.5f;

    private float stateChangeTimer;
    private float stateDuration;

    // Daño que el oso inflige
    public float damage = 10f;
    private bool isAttacking = false; // Bandera para controlar si el oso está atacando

    // Intervalo entre ataques (para no aplicar daño continuamente)
    public float attackCooldown = 2f;
    private float lastAttackTime;

    // **NUEVO** Referencia al Box Collider en la mano del oso (asignar en el Inspector)
    public Collider attackCollider;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró el componente Animator en el oso.");
        }

        // Desactivar el collider de ataque al inicio
        attackCollider.enabled = false;

        // Iniciar en el estado de dormir
        ChangeState("Sleep");
    }

    void Update()
    {
        stateChangeTimer += Time.deltaTime;

        // Detectar la distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Verificar si el jugador está fuera del rango máximo de persecución
        if (distanceToPlayer > maxChaseRange)
        {
            // Salir del estado de persecución y volver a un estado aleatorio
            SelectRandomState(); // Corregido: Añadido método SelectRandomState
            return; // Salir del método Update para evitar realizar más acciones en este frame
        }

        // Si el jugador está dentro del rango de detección
        if (distanceToPlayer <= detectionRange)
        {
            // Si el jugador está en rango de ataque
            if (distanceToPlayer <= attackRange)
            {
                // Solo atacar si ha pasado suficiente tiempo desde el último ataque
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    ChangeState("Attack");
                    lastAttackTime = Time.time; // Actualizar el tiempo del último ataque
                }
            }
            else
            {
                // Perseguir al jugador
                ChangeState("Chase");
            }
        }
        else
        {
            // Cambiar de estado cuando se cumple el tiempo
            if (stateChangeTimer >= stateDuration)
            {
                SelectRandomState(); // Corregido: Añadido método SelectRandomState
            }
        }

        // Ejecutar comportamiento según el estado actual
        if (currentState == "Walk")
        {
            Walk();
        }
        else if (currentState == "Chase")
        {
            ChasePlayer();
        }
        else if (currentState == "Attack")
        {
            AttackPlayer(); // Ejecutar el ataque
        }
    }

    // Cambiar a un nuevo estado
    private void ChangeState(string newState)
    {
        // Reiniciar todos los parámetros de animación
        animator.SetBool(sleepAnimationParam, false);
        animator.SetBool(walkAnimationParam, false);
        animator.SetBool(runAnimationParam, false);
        animator.SetBool(attackAnimationParam, false);
        animator.SetBool(sitAnimationParam, false);

        // Configurar el nuevo estado
        currentState = newState;
        stateChangeTimer = 0f;

        switch (newState)
        {
            case "Sleep":
                animator.SetBool(sleepAnimationParam, true);
                stateDuration = Random.Range(5f, 10f); // Dormir por un tiempo aleatorio
                break;

            case "Walk":
                RotateRandomDirection();
                animator.SetBool(walkAnimationParam, true);
                stateDuration = Random.Range(25f, 35f); // Caminar por un tiempo aleatorio
                break;

            case "Sit":
                animator.SetBool(sitAnimationParam, true);
                stateDuration = Random.Range(3f, 7f); // Sentarse por un tiempo aleatorio
                break;

            case "Chase":
                animator.SetBool(runAnimationParam, true); // Usar animación de correr
                break;

            case "Attack":
                animator.SetBool(attackAnimationParam, true); // Usar animación de ataque
                isAttacking = true;
                attackCollider.enabled = true; // Activar el collider de ataque
                Invoke("ResetAttack", attackCooldown);
                break;
        }
    }

    // **NUEVO** Seleccionar aleatoriamente el próximo estado
    private void SelectRandomState()
    {
        int randomIndex = Random.Range(0, 3); // Selecciona un número entre 0 y 2

        switch (randomIndex)
        {
            case 0:
                ChangeState("Sleep");
                break;
            case 1:
                ChangeState("Walk");
                break;
            case 2:
                ChangeState("Sit");
                break;
        }
    }

    // Método para atacar al jugador
    private void AttackPlayer()
    {
        // Verificar si el jugador está en rango de ataque y si el ataque está ocurriendo
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && isAttacking)
        {
            // El daño se aplica cuando se detecta la colisión (ver el método OnTriggerEnter)
        }
    }

    // Desactivar el ataque después de un tiempo
    private void ResetAttack()
    {
        isAttacking = false;
        attackCollider.enabled = false; // Desactivar el collider de ataque después del ataque
    }

    // Detectar colisiones con el jugador
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Player"))
        {
            // Log para ver si la mano del oso ha colisionado con el jugador
            Debug.Log("El oso ha colisionado con el jugador.");

            // Aplicar daño al jugador si se detecta una colisión con el collider
            LogicPlayer playerHealth = other.GetComponent<LogicPlayer>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage); // Aplicar daño al jugador
                Debug.Log("El oso ha causado " + damage + " de daño al jugador. Salud restante: " + playerHealth.currentHealth);
            }
        }
    }

    // Método para mover al oso hacia adelante
    private void Walk()
    {
        transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
    }

    // Método para rotar al oso en una dirección aleatoria
    private void RotateRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f); // Selecciona un ángulo aleatorio
        transform.Rotate(0, randomAngle, 0);
    }

    // Método para perseguir al jugador
    private void ChasePlayer()
    {
        // Girar hacia el jugador
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Moverse hacia el jugador
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }
}