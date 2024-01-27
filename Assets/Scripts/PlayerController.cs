using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    // Variabili per la gestione del movimento e del salto
    public float movementSpeed = 1.0f;
    public float jumpForce = 10.0f;
    public float runningSpeedMultiplier = 3.0f;

    // Nomi degli assi di input per il movimento
    private string horizontalName = "Horizontal", verticalName = "Vertical";

    // Valori dell'input orizzontale e verticale
    private float horizontalValue, verticalValue;

    // Riferimento al componente Rigidbody del giocatore
    private Rigidbody body;

    // Flag che indica se il giocatore è a contatto con il terreno
    private bool isGrounded;

    // Contatore per il numero di dash disponibili
    private int dashCount = 1;

    // Start è chiamato prima del primo frame
    void Start()
    {
        // Ottieni il riferimento al componente Rigidbody
        body = GetComponent<Rigidbody>();
    }

    // Update è chiamato una volta per ogni frame
    void Update()
    {
        // Verifica se il giocatore è a contatto con il terreno
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.01f);

        // Ottieni i valori dell'input orizzontale e verticale
        horizontalValue = Input.GetAxis(horizontalName);
        verticalValue = Input.GetAxis(verticalName);

        if(Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Run");
            horizontalValue *= 4;
        }

        // Resetta il contatore dei dash se il giocatore è a terra
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
        // Gestisci il dash in aria
        else if (!isGrounded && dashCount > 0 && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            Dash();
            dashCount--;
        }
    }

    // FixedUpdate è chiamato ad intervalli fissi e viene utilizzato per la fisica
    private void FixedUpdate()
    {
        // Applica la forza per il movimento laterale
        body.AddForce(new Vector3(horizontalValue * movementSpeed, 0, 0), ForceMode.Force);
    }

    // Funzione per gestire il salto
    private void Jump()
    {
        body.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }

    // Funzione per gestire il dash in aria
    private void Dash()
    {
        body.AddForce(new Vector3(0, jumpForce / 1.5f, 0), ForceMode.Impulse);
    }
}