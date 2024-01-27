using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{

    public float movementSpeed = 1.0f;
    public float jumpForce = 10.0f;

    private string horizontalNme = "Horizontal", verticalName = "Vertical", jumpName = "space";
    private float horizontalValue, verticalValue;
    private Rigidbody body;
    private bool isGrounded;
    private int dashCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.01f);
        horizontalValue = Input.GetAxis(horizontalNme);
        verticalValue = Input.GetAxis(verticalName);

        if (isGrounded)
        {
            dashCount = 1;
        }

        if(isGrounded &&  (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            Debug.Log("Jump");
            Jump();
        }
        else if(!isGrounded && dashCount > 0 && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            Dash();
            dashCount--;
        }
    }

    private void FixedUpdate()
    {
        body.AddForce(new Vector3(horizontalValue*movementSpeed, 0, 0), ForceMode.Force);
    }

    private void Jump()
    {
        body.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }

    private void Dash()
    {
        body.AddForce(new Vector3(0, jumpForce/1.2f, 0), ForceMode.Impulse);
    }
}
