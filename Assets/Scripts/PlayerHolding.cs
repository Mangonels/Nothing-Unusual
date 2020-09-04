using Cinemachine;
using UnityEngine;


public class PlayerHolding : MonoBehaviour
{
    public GameObject[] objects;
    public float maxGrabDistance = 3.0f;
    [SerializeField] private int amountOfHeldObjects = 0;

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
                //Drop it in current grid box... TODO
            }
        }
    }
}
