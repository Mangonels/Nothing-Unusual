using Cinemachine;
using UnityEngine;


public class PlayerHolding : MonoBehaviour
{
    public GameObject[] objects;
    public float maxGrabDistance = 3.0f;
    [SerializeField] private int amountOfHeldObjects = 0;
    public Transform GridBoxCollisionCheckPointRef; //Transform containing reference position from which we check if we're colliding with a "collidingGridBox" and update such reference.
    [SerializeField] Collider collidingGridBox; //The box grid's collider the detection sphere for held objects is colliding with by trigger

    public CinemachineVirtualCamera cVCRef; //Cinemachine virtual camera reference
    void Start()
    {
        
    }

    void Update()
    {
        //Rotate held objects with camera's x & y axis extracted euler angles
        transform.rotation = Quaternion.Euler(0.0f, cVCRef.State.CorrectedOrientation.eulerAngles.y, 0.0f);

        if (Input.GetMouseButtonDown(0)) //Left click
        {
            //Pick up object
            RaycastHit hit;
            Ray ray = new Ray(cVCRef.transform.position, cVCRef.State.CorrectedOrientation * Vector3.forward);
            //Debug.DrawRay(cVCRef.transform.position, (cVCRef.State.CorrectedOrientation * Vector3.forward) * 100, Color.red, 2f);
            if (Physics.Raycast(ray, out hit)) //Raycast hits game object
            {
                if (hit.distance <= maxGrabDistance) //Hit detection is within "maxGrabDistance"
                {
                    if (hit.collider.gameObject.tag == "PickableObject") //Game object is a PickableObject
                    {
                        if (amountOfHeldObjects < objects.Length) 
                        {
                            Destroy(hit.collider.gameObject);
                            objects[amountOfHeldObjects].SetActive(true);
                            amountOfHeldObjects++;
                        }
                    }
                }
            }
        }
            
        if (Input.GetMouseButtonDown(1)) //Right click
        {
            //Drop object
            if (amountOfHeldObjects > 0)
            {
                amountOfHeldObjects--;
                objects[amountOfHeldObjects].SetActive(false);
                //Drop it in current detected grid box from sphere collider
                //collidingGridBox
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Trigger box entered other collider: " + col.tag);
        if (col.gameObject.tag == "GridBox")
        {
            Debug.Log("Trigger box entered GridBox");
            collidingGridBox = col; //The ObjectHolding's trigger sphere has entered collision with a GridBox's BoxCollider, we register it as current droppable grid box from it's collider.
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "GridBox")
        {
            Debug.Log("Trigger box left GridBox");
            collidingGridBox = null; //The ObjectHolding's trigger sphere has left collision with a GridBox's BoxCollider, we unregister it as current droppable grid box from it's collider.
        }
    }

    public void RemoveAllHeldObjects() //Sets all held objects to inactive (gives the appearance that we're holding nothing)
    {
        for (int i = 0; i < objects.Length; i++) 
        {
            objects[i].SetActive(false);
            amountOfHeldObjects = 0;
        }
    }
}
