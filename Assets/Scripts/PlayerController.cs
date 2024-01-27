using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    //public Transform spawn;
    public LayerMask ground;
    public GameObject pickUpHint;
    public GameObject pickupMessage;
    public TextMeshProUGUI pickupText;

    // Nomi degli assi di input per il movimento
    private string horizontalName = "Horizontal";

    // Valori dell'input orizzontale e verticale
    private float horizontalValue;

    // Riferimento al componente Rigidbody del giocatore
    private Rigidbody body;
    private SpriteRenderer sprite;
    private Animator animator;
    private GameObject cam;
    private AudioSource source;

    // Flag che indica se il giocatore ? a contatto con il terreno
    private bool isGrounded;

    // Contatore per il numero di doppi salti disponibili
    private int dashCount = 1;

    private GameObject pickable;
    private bool keyPicked;
    private bool canMove;

    // Start ? chiamato prima del primo frame
    void Start()
    {
        // Ottieni il riferimento al componente Rigidbody
        body = GetComponent<Rigidbody>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        isGrounded = false;
        canMove = true;
    }

    // Update ? chiamato una volta per ogni frame
    void Update()
    {
        // Verifica se il giocatore ? a contatto con il terreno
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.88f, ground);

        // Ottieni i valori dell'input orizzontale e verticale
        horizontalValue = Input.GetAxis(horizontalName);

        //transform.Translate(Vector2.right * horizontalValue * movementSpeed * Time.deltaTime);

        //Imposta il parametro sull'animator
        animator.SetFloat(horizontalName, horizontalValue);
        Debug.Log(animator.GetFloat("Horizontal"));

        // Ruota lo sprite nella direzione di movimento
        if(horizontalValue < 0)
        {
            sprite.flipX = true;
        } else if(horizontalValue > 0)
        {
            sprite.flipX = false;
        }
        else
        {

        }

        if(Mathf.Abs(horizontalValue)>0 && isGrounded)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
        else
        {
            source.Stop();
        }
        /*
         * if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Run");
            horizontalValue *= 5;
        }
        */

        // Resetta il contatore dei dash se il giocatore ? a terra
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
        /*else if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Spawn");
            body.Move(spawn.position, Quaternion.identity);
            body.velocity = Vector3.zero;
        }
        else*/ if (Input.GetKeyDown(KeyCode.E))
        {
            if (canMove)
            {
                if (pickable)
                {
                    //pickable.SetActive(false);
                    pickable.transform.SetParent(Camera.main.transform);
                    pickable.transform.localPosition = new Vector3(0, 2f, 5f);



                    if (pickable.name == "Key")
                    {
                        Debug.Log("Key Picked!");
                        keyPicked = true;
                        pickable.transform.SetParent(Camera.main.transform);
                        pickable.transform.localPosition = new Vector3(0f, 1f, 4f);
                        pickable.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        pickable.GetComponent<ParticleSystem>().emissionRate = 0f; ;
                        pickable.GetComponent<AudioSource>().Play();
                        pickable = null;
                    }
                    else
                    {
                        pickupText.text = pickable.GetComponent<Pickup>().description;
                        pickupMessage.SetActive(true);
                        canMove = false;
                        pickable = null;
                    }
                }
            }
            else
            {
                pickupMessage.SetActive(false);
                canMove = true;
            }
        }

        //body.velocity = new Vector3(horizontalValue, body.velocity.y, body.velocity.z);
    }

    // FixedUpdate ? chiamato ad intervalli fissi e viene utilizzato per la fisica
    private void FixedUpdate()
    {
        // Applica la forza per il movimento laterale
        //body.AddForce(new Vector3(horizontalValue * movementSpeed, 0, 0), ForceMode.Force);
        if (canMove)
        {
            body.Move(body.position + new Vector3(horizontalValue * movementSpeed * Time.fixedDeltaTime, 0f, 0f), Quaternion.identity);
            if (Mathf.Abs(body.velocity.magnitude) > maxMovementSpeed)
            {
                body.velocity = body.velocity.normalized * maxMovementSpeed;
            }
        }
        //Debug.Log(body.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Pickup"))
        {
            pickable = other.gameObject;
            pickUpHint.SetActive(true);
        }
        else if (other.CompareTag("Reset"))
        {   
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
        }
        else if(other.CompareTag("Door") && keyPicked)
        {
            other.GetComponent<Animation>().Play();
            other.GetComponent<AudioSource>().Play();
        }
        else if(other.CompareTag("Next Level"))
        {
            // Carica la scena successiva
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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