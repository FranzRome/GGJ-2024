using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Variabili per la gestione del movimento e del salto
    public float movementSpeed = 1.0f;
    public float maxMovementSpeed = 5.0f;
    public float jumpForce = 10.0f;
    //public float runningSpeedMultiplier = 3.0f;
    //public Transform spawn;
    public LayerMask ground;
    public GameObject startMessage;
    public GameObject pickUpHint;
    public GameObject pickupMessage;
    public TextMeshProUGUI pickupText;

    public GameObject music;

    // Nomi degli assi di input per il movimento
    private string horizontalName = "Horizontal";

    // Valori dell'input orizzontale e verticale
    private float horizontalValue;

    // Riferimento al componente Rigidbody del giocatore
    private Rigidbody body;
    private SpriteRenderer sprite;
    private Animator animator;
    private GameObject cam;
    private CameraController camControl;
    private AudioSource source;

    // Flag che indica se il giocatore ? a contatto con il terreno
    private bool isGrounded;

    // Contatore per il numero di doppi salti disponibili
    private int dashCount = 1;

    private GameObject pickable;
    private GameObject pickableClone;
    private bool keyPicked;
    //private bool canMove;

    // Start ? chiamato prima del primo frame
    void Start()
    {
        // Ottieni il riferimento al componente Rigidbody
        body = GetComponent<Rigidbody>();
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        cam = Camera.main.gameObject;
        camControl = cam.GetComponent<CameraController>();
        source = GetComponent<AudioSource>();
        music = GameObject.Find("Music");

        isGrounded = false;
        //canMove = true;
    }

    // Update ? chiamato una volta per ogni frame
    void Update()
    {
        // Verifica se il giocatore ? a contatto con il terreno
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.3f, ground);

        // Ottieni i valori dell'input orizzontale e verticale
        horizontalValue = Input.GetAxis(horizontalName);

        if(isGrounded)
        {
            animator.SetTrigger("Idle");
        }

        if (!pickupMessage.activeSelf && !startMessage.activeSelf)
        {

            //Imposta il parametro sull'animator
            animator.SetFloat(horizontalName, horizontalValue);
            //Debug.Log(animator.GetFloat("Horizontal"));

            // Ruota lo sprite nella direzione di movimento
            if (horizontalValue < 0)
            {
                sprite.flipX = true;
                camControl.offest.x = -1.5f;
            }
            else if (horizontalValue > 0)
            {
                sprite.flipX = false;
                camControl.offest.x = 1.5f;
            }
            else
            {

            }
            // Riproduce l'audio dei passi quando cammina a terra
            if (Mathf.Abs(horizontalValue) > 0 && isGrounded)
            {
                if (!source.isPlaying)
                {
                    source.pitch = Random.Range(-1f, 1.25f);
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
                //Debug.Log("Jump");
                Jump();
                animator.SetTrigger("Jump");            }
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
            else*/
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (pickable)
                {
                    //pickable.SetActive(false);
                    pickable.GetComponent<AudioSource>().Play();

                    if (pickable.name == "Key")
                    {
                        //Debug.Log("Key Picked!");
                        pickable.transform.SetParent(cam.transform);
                        keyPicked = true;
                        pickable.transform.localPosition = new Vector3(0f, 0.9f, 3f);
                        pickable.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        pickable.GetComponent<ParticleSystem>().emissionRate = 0f;
                        pickable = null;
                    }
                    else
                    {
                        pickableClone = Instantiate(pickable);
                        pickableClone.transform.SetParent(cam.transform);
                        pickableClone.transform.localPosition = new Vector3(0f, 0.1f, 3f);
                        pickupText.text = pickable.GetComponent<Pickup>().description;
                        pickUpHint.SetActive(false);
                        pickupMessage.SetActive(true);
                        //canMove = false;
                        //Destroy(pickable, 3);
                        //pickable = null;
                    }
                }
                
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Jump"))
        {
            pickupMessage.SetActive(false);
            //canMove = true;
            //Debug.Log(pickable.name);
            if (pickableClone != null)
            {
                Destroy(pickableClone);
                Destroy(pickable);
                pickable = null;
            }
            else
            {
                startMessage.SetActive(false);
            }
        }

        //body.velocity = new Vector3(horizontalValue, body.velocity.y, body.velocity.z);
    }

    // FixedUpdate ? chiamato ad intervalli fissi e viene utilizzato per la fisica
    private void FixedUpdate()
    {
        // Applica la forza per il movimento laterale
        
        if (!pickupMessage.activeSelf && !startMessage.activeSelf)
        {
            body.MovePosition(body.position + new Vector3(horizontalValue * movementSpeed * Time.fixedDeltaTime, 0f, 0f));
            //body.velocity = Vector3.zero;
            //body.AddForce(new Vector3(horizontalValue * movementSpeed, 0, 0), ForceMode.Force);
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
            Debug.Log(pickable.name);
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
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
               Destroy(music);
            }

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