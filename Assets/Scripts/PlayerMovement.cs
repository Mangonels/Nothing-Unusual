using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController cControllerRef; //Character controller reference
    Vector3 fullMovement; //Movement to be applied to character controller
    public float speed = 8f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    Vector3 velocity;

    public Transform groundCkeck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    private CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    Vector3 camdir; //Camera direction/forward vector
    Vector3 camright; //Camera right vector

    void Start()
    {
        cVCRef = transform.Find("FirstPersonCamera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCkeck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }

        //Update forward and right camera vectors for referencing
        camdir = cVCRef.State.CorrectedOrientation * Vector3.forward;
        camright = cVCRef.State.CorrectedOrientation * Vector3.right;
        //Debug.Log("Camera direction vector:" + camdir);
        //Debug.Log("Camera right vector:" + camright);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camdir, Color.red);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camright, Color.blue);

        //Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Calculate and apply movement to character controller:
        //The part of the movement influenced by the player's horizontal move input is relative to the camera vectors
        Vector2 horizontalInputMove = new Vector2((camright * x + camdir * z).x, (camright * x + camdir * z).z);
        //Full movement is sumed up here
        fullMovement = new Vector3(horizontalInputMove.x, 0.0f, horizontalInputMove.y);

        cControllerRef.Move(fullMovement * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        cControllerRef.Move(velocity * Time.deltaTime);


    }
}