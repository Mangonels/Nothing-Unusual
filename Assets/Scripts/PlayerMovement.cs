using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public AudioSource jump;
    public AudioSource boost;
    public AudioSource fall;

    public float gravity = -18f;

    public CharacterController cControllerRef; //Character controller reference
    [SerializeField] private Vector3 playerMovement; //Picks up movement input as movement
    public float speed = 8f;
    public float jumpHeight = 2.5f;
    public float airBoostStrength = 14f;
    public int maxAirboosts = 3;
    [SerializeField] private int usedAirboosts = 0;
    [SerializeField] private Vector3 forcesMovement; //Picks up all additional speeds
    [SerializeField] bool canPlayFallSound = false; //Stores if the fall sound can be played. Is set to true when airborne, and false on first grounded frame

    public CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    Vector3 camdir; //Camera direction/forward vector
    Vector3 camright; //Camera right vector

    void Update()
    {
        //----------------------
        //Get camera information
        //----------------------
        //Update forward and right camera vectors for references
        camdir = cVCRef.State.CorrectedOrientation * Vector3.forward;
        camright = cVCRef.State.CorrectedOrientation * Vector3.right;
        //Debug.Log("Camera direction vector:" + camdir);
        //Debug.Log("Camera right vector:" + camright);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camdir, Color.red);
        //Debug.DrawLine(cVCRef.transform.position, cVCRef.transform.position + camright, Color.blue);

        //---------
        //Get input
        //---------

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //-----------------------------------------------------
        //Calculate and apply movement to character controller:
        //-----------------------------------------------------

        //The part of the movement influenced by the player's horizontal move input is relative to the camera vectors
        Vector2 horizontalInputMove = new Vector2((camright * x + camdir * z).x, (camright * x + camdir * z).z);
        
        //Player movement calculation
        playerMovement = new Vector3(horizontalInputMove.x, 0.0f, horizontalInputMove.y);

        if (Input.GetButtonDown("Jump") && cControllerRef.isGrounded) //Jumping from ground
        {
            //Jump movement calculation
            forcesMovement.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jump.Play();
        }

        if (Input.GetButtonDown("Jump") && !cControllerRef.isGrounded) //Jumping airborne
        {
            if (usedAirboosts < maxAirboosts) 
            {
                usedAirboosts++;
                //Air movement force movement boost calculation
                forcesMovement.z = playerMovement.z * airBoostStrength;
                forcesMovement.x = playerMovement.x * airBoostStrength;
                forcesMovement.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //This is pretty much the same as a second jump impulse

                boost.Play();
            }
        }

        if (cControllerRef.isGrounded) //Character controler grounded
        {
            usedAirboosts = 0; //Reset airboost availability

            //Being grounded reduces airBoost in x & z due to ground friction
            if (forcesMovement.x != 0f)
            {
                forcesMovement.x = 0f; //Stop any force movement in X due to ground friction
            }
            if (forcesMovement.z != 0f)
            {
                forcesMovement.z = 0f; //Stop any force movement in Z due to ground friction
            }
            if (forcesMovement.y < 0)
            {
                forcesMovement.y = -2f; //Cancel most gravity (leaves a bit of gravity so that the player doesn't unstick from the ground)
            }

            //Apply player movement from ground impulse
            cControllerRef.Move(playerMovement * speed * Time.deltaTime);

            if (canPlayFallSound) {
                canPlayFallSound = false;
                fall.Play();
            }
        }
        else //Character controller airborne
        {
            canPlayFallSound = true;

            //Diferent movement in air
            forcesMovement.x += playerMovement.x * 0.1f;
            forcesMovement.z += playerMovement.z * 0.1f;

            //Increase negative y forcesMovement due to gravity
            forcesMovement.y += gravity * Time.deltaTime; 
        } 

        //Apply other movements derived from forces
        cControllerRef.Move(forcesMovement * Time.deltaTime);
    }
}