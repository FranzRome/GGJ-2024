using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    // Variabili per la gestione del movimento e del salto
    public float movementSpeed = 1.0f;
    public float maxMovementSpeed = 5.0f;
    public float jumpForce = 10.0f;
    //public float runningSpeedMultiplier = 3.0f;
    public Transform spawn;
    public GameObject pickUpHint;

    // Nomi degli assi di input per il movimento
    private string horizontalName = "Horizontal", verticalName = "Vertical";

    // Valori dell'input orizzontale e verticale
    private float horizontalValue, verticalValue;

    // Riferimento al componente Rigidbody del giocatore
    private Rigidbody body;

    // Flag che indica se il giocatore � a contatto con il terreno
    private bool isGrounded;

    // Contatore per il numero di doppi salti disponibili
    private int dashCount = 1;

    private GameObject pickable;
    private bool keyPicked;

    // Start � chiamato prima del primo frame
    void Start()
    {
        // Ottieni il riferimento al componente Rigidbody
        body = GetComponent<Rigidbody>();
    }

    // Update � chiamato una volta per ogni frame
    void Update()
    {
        // Verifica se il giocatore � a contatto con il terreno
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.51f);

        // Ottieni i valori dell'input orizzontale e verticale
        horizontalValue = Input.GetAxis(horizontalName);

        verticalValue = Input.GetAxis(verticalName);

        /*
         * if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Run");
            horizontalValue *= 5;
        }
        */

        // Resetta il contatore dei dash se il giocatore � a terra
        if (isGrounded)
        {
            dashCount = 1;
        }

        // Gestisci il salto
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            Debug.Log("Jump");
            Jump();
        }
        // Gestisci il doppio salto
        else if (!isGrounded && dashCount > 0 && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            Dash();
            dashCount--;
        }
        // Riporta il Player allo Spawn
        else if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Spawn");
            body.Move(spawn.position, Quaternion.identity);
            body.velocity = Vector3.zero;
        }
        else if (Input.GetKeyDown(KeyCode.E) && pickable)
        {
            pickable.SetActive(false);
            pickUpHint.SetActive(false);
            if (pickable.name == "Key")
            {
                keyPicked = true;
            }
        }

        //body.velocity = new Vector3(horizontalValue, body.velocity.y, body.velocity.z);
    }

    // FixedUpdate � chiamato ad intervalli fissi e viene utilizzato per la fisica
    private void FixedUpdate()
    {
        // Applica la forza per il movimento laterale
        body.AddForce(new Vector3(horizontalValue * movementSpeed, 0, 0), ForceMode.Force);
        //body.velocity.x = Mathf.Clamp(body.velocity.x, -maxMovementSpeed, maxMovementSpeed);
        if(Mathf.Abs(body.velocity.magnitude) > maxMovementSpeed)
        {
            body.velocity = body.velocity.normalized * maxMovementSpeed;
        }
        Debug.Log(body.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pickup"))
        {
            pickable = other.gameObject;
            pickUpHint.SetActive(true);
        }
        else
        {
            if(other.CompareTag("Reset"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            pickable = null;
            pickUpHint.SetActive(false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalValue = context.ReadValue<Vector2>().x;
    }

    // Funzione per gestire il salto
    private void Jump()
    {
        body.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }

    // Funzione per gestire il dash in aria
    private void Dash()
    {
        body.velocity = new Vector3(body.velocity.x, 0f, 0f);
        body.AddForce(new Vector3(0, jumpForce / 1.5f, 0), ForceMode.Impulse);
    }

}