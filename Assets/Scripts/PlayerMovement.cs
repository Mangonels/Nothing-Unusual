using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController cControllerRef; //Character controller reference
    [SerializeField]
    private Vector3 playerMovement;
    public float speed = 8f;
    public float jumpHeight = 2.5f;
    public float gravity = -18f;
    [SerializeField]
    private Vector3 velocity;

    private CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    Vector3 camdir; //Camera direction/forward vector
    Vector3 camright; //Camera right vector

    void Start()
    {
        cVCRef = transform.Find("FirstPersonCamera").GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        //Update forward and right camera vectors for references
        camdir = cVCRef.State.CorrectedOrientation * Vector3.forward;
        camright = cVCRef.State.CorrectedOrientation * Vector3.right;
        //Debug.Log("Camera direction vector:" + camdir);
        //Debug.Log("Camera right vector:" + camright);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camdir, Color.red);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camright, Color.blue);

        //Get input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //-----------------------------------------------------
        //Calculate and apply movement to character controller:
        //-----------------------------------------------------

        //The part of the movement influenced by the player's horizontal move input is relative to the camera vectors
        Vector2 horizontalInputMove = new Vector2((camright * x + camdir * z).x, (camright * x + camdir * z).z);
        
        //Player movement calculation
        playerMovement = new Vector3(horizontalInputMove.x, 0.0f, horizontalInputMove.y);

        if (Input.GetButtonDown("Jump") && cControllerRef.isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Gravity velocity calculation
        if (cControllerRef.isGrounded && velocity.y < 0) //Being grounded stops y velocity
        {
            velocity.y -= Mathf.Abs(gravity); //Cancel gravity
        }
        else velocity.y += gravity * Time.deltaTime; //Being airborne increases negative y velocity due to gravity

        //Apply player movement
        cControllerRef.Move(playerMovement * speed * Time.deltaTime);

        //Apply other forces
        cControllerRef.Move(velocity * Time.deltaTime);
    }
}