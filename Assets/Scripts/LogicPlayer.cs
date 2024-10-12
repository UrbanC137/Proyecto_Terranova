using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importante para trabajar con la UI

public class LogicPlayer : MonoBehaviour
{
    public float speedMovement = 5.0f;   // Velocidad de movimiento hacia adelante/atr�s
    public float speedStrafe = 5.0f;     // Velocidad de movimiento lateral
    public float speedRotation = 200.0f; // Velocidad de rotaci�n
    public float jumpForce = 5.0f;       // Fuerza del salto
    public float runMultiplier = 2.0f;   // Multiplicador de velocidad al correr
    public int trunkCount = 0; // Contador de troncos recogidos
    public int stonCount = 0; // Contador de piedras recogidos

    private Animator anim;
    public float x, y;
    private bool isGrounded;             // Para verificar si el personaje est� en el suelo
    private Rigidbody rb;
    private bool isAttacking;            // Verificar si el personaje est� atacando
    private bool isAttackCoroutineRunning; // Para controlar la corrutina de ataque
    private bool isDead = false;         // **NUEVO**: Verificar si el jugador est� muerto

    // Referencia al hacha (se debe asignar en el Inspector)
    public GameObject axe;

    // **NUEVO** Variable de salud del jugador
    public int maxHealth = 100;  // Salud m�xima del jugador
    public int currentHealth;   // Salud actual del jugador

    // **NUEVO**: Referencia a la barra de vida (Slider)
    public Slider healthBar; // Asignar este slider desde el Inspector

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();  // A�adimos un Rigidbody para aplicar la fuerza del salto

        // Inicializar la salud del jugador
        currentHealth = maxHealth;

        // **NUEVO**: Inicializar la barra de salud
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Desactivar el collider del hacha al inicio
        axe.GetComponent<Collider>().enabled = false;
    }

    void Update()
    {
        // **NUEVO**: Si el jugador est� muerto, no hacer nada
        if (isDead)
        {
            return;
        }

        // Si el personaje est� atacando, no se permite mover o rotar
        if (!isAttacking)
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");

            float currentSpeedMovement = speedMovement;
            float currentSpeedStrafe = speedStrafe;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentSpeedMovement *= runMultiplier;
                currentSpeedStrafe *= runMultiplier;
            }

            // Movimiento
            transform.Translate(x * Time.deltaTime * currentSpeedStrafe, 0, y * Time.deltaTime * currentSpeedMovement);

            if (x != 0)
            {
                float rotationAngle = x * Time.deltaTime * speedRotation;
                transform.Rotate(0, rotationAngle, 0);
            }

            // Salto
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                anim.SetTrigger("Jump");
                isGrounded = false;
            }
        }

        // Ataque continuo mientras se mantiene presionada la tecla "E"
        if (Input.GetKey(KeyCode.E))
        {
            if (!isAttackCoroutineRunning)
            {
                StartCoroutine(ContinuousAttack());
            }
        }
        else
        {
            if (isAttacking)
            {
                anim.SetBool("isAttacking", false);
                isAttacking = false;
            }
        }

        if (!isAttacking)
        {
            anim.SetFloat("speedX", x);
            anim.SetFloat("speedY", y);
        }
    }

    // M�todo para recibir da�o del enemigo (como el oso)
    public void TakeDamage(int damage)
    {
        // **NUEVO**: Si el jugador ya est� muerto, no puede recibir m�s da�o
        if (isDead) return;

        currentHealth -= damage; // Reducir salud
        if (currentHealth < 0) currentHealth = 0; // Asegurarse de que no sea menor que 0
        Debug.Log("El jugador ha recibido " + damage + " de da�o. Salud restante: " + currentHealth);

        // **NUEVO**: Actualizar la barra de salud
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die(); // Llamar a la funci�n de muerte si la salud llega a 0
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto con el que colisiona tiene el tag "Trunk"
        if (other.CompareTag("Trunk"))
        {
            trunkCount++; // Incrementar el n�mero de troncos recogidos
            Debug.Log("Has recogido " + trunkCount + " madera(s).");

            // Destruir el objeto Trunk
            Destroy(other.gameObject);
        }

        // Verificar si el objeto con el que colisiona tiene el tag "Ston"
        if (other.CompareTag("Ston"))
        {
            stonCount++; // Incrementar el n�mero de piedras recogidas
            Debug.Log("Has recogido " + stonCount + " piedra(s).");

            // Destruir el objeto Ston
            Destroy(other.gameObject);
        }
    }


    // M�todo que se llama cuando el jugador muere
    private void Die()
    {
        isDead = true; // **NUEVO**: Marcar al jugador como muerto para evitar que siga interactuando
        Debug.Log("�Jugador ha muerto!");

        // **NUEVO**: Activar la animaci�n de muerte
        anim.SetTrigger("DieTrigger");

        // **NUEVO**: Opcionalmente, desactivar el movimiento y otros controles
        rb.isKinematic = true; // Desactivar f�sica para evitar que el personaje se mueva despu�s de morir
        // Tambi�n podr�as desactivar colisiones u otros componentes si es necesario.
    }

    // Corrutina para realizar el ataque continuo
    private IEnumerator ContinuousAttack()
    {
        isAttackCoroutineRunning = true;

        while (Input.GetKey(KeyCode.E))
        {
            anim.SetBool("isAttacking", true);
            isAttacking = true;

            // Activar el collider del hacha durante un corto periodo
            axe.GetComponent<Collider>().enabled = true;

            yield return new WaitForSeconds(0.1f); // Permitir la colisi�n por un breve tiempo

            // Desactivar el collider del hacha para evitar que cuente m�s golpes cuando no est� atacando
            axe.GetComponent<Collider>().enabled = false;

            yield return new WaitForSeconds(0.5f); // Intervalo entre ataques
        }

        isAttackCoroutineRunning = false;
        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
