using UnityEngine;
public class Door : MonoBehaviour
{
    public PlayerHolding holdInformationScriptRef; //References the script which contained held objects information
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials
    public GameBehaviour gameBehaviourRef; //Reference to game behaviour script

    public bool wantsObjects; //Is this door accepting objects?
    [SerializeField] private bool wantsObjectsInOrder; //Does the door want the objects to be presented in the same stack order?
    [SerializeField] private float willRemainOpenFor;
    [SerializeField] private float hasBeenOpenFor = 0.0f;
    [SerializeField] private ObjectsData.objectTypes[] wantedObjectTypes; //Current wanted objects by the door
    [SerializeField] private int wantedObjectsAmount = 0;
    public GameObject[] requestDisplays;

    void Start()
    {
        
    }

    void Update()
    {
        hasBeenOpenFor += Time.deltaTime;
        if (hasBeenOpenFor >= willRemainOpenFor) Close(); //Close the door if "willRemainOpenFor" has been exceeded
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (wantsObjects) 
            {
                //Validate brought objects
                GameObject[] heldObjectArray = col.gameObject.GetComponentInChildren<PlayerHolding>().GetHeldObjectsArray(); //Get the player's held object array

                bool allObjectsFound = true; //We asume true, until one of them isn't found
                if (!wantsObjectsInOrder) //No order for objects required
                {
                    //Check types one by one and compare with existing
                    for (int i = 0; i < wantedObjectTypes.Length; i++) //Each wanted object
                    {
                        if (wantedObjectTypes[i] == ObjectsData.objectTypes.NONE) goto End; //There shouldn't be any more wanted objects since we found first NONE, exit loop and jump to "End:"
                        for (int j = 0; j < heldObjectArray.Length; j++) //See if it's in any of the held objects array
                        {
                            if (wantedObjectTypes[i] == heldObjectArray[j].GetComponent<HeldObject>().objectType) 
                            {
                                break; //Object found, next item?
                            } 
                            else if (j == heldObjectArray.Length-1) //Last possible slot where we could find coinciding object
                            {
                                allObjectsFound = false; //We reached the end of "heldObjectArray" without finding this object, so not all objects where found
                                goto End; //Exit entire nested loop and jump to "End:"
                            } 
                        }
                    }
                }
                else //Ordered objects required
                {
                    //Check types one by one and compare with existing
                    for (int i = 0; i < wantedObjectTypes.Length; i++) //Each wanted object
                    {
                        if (wantedObjectTypes[i] != heldObjectArray[i].GetComponent<HeldObject>().objectType)
                        {
                            //We didn't find the same object in this slot. State it, and break loop
                            allObjectsFound = false;
                            break;
                        }
                    }
                }
            End:
                if (allObjectsFound) //Held objects found and approved
                {
                    if (!wantsObjectsInOrder) holdInformationScriptRef.RemoveAndReorderHeldObjects(wantedObjectTypes); //Remove specific types of objects from held items (unordered case)
                    else holdInformationScriptRef.RemoveAndReorderHeldObjectsByAmount(wantedObjectsAmount); //Remove batch held items up to wantedObjectsAmount (ordered case)

                    gameBehaviourRef.AddPoints(100 * wantedObjectsAmount); //Add points to score depending on handed in objects amount

                    for (int i = 0; i < 5; i++) 
                    {
                        if (wantedObjectTypes[i] != ObjectsData.objectTypes.NONE) gameBehaviourRef.RemoveDoorAccountableObject(wantedObjectTypes[i]); //Since we delivered the object/s, doors won't ask for this/these object/s again
                        else break;
                    }
                }
                else //Held objects not found and rejected
                {
                
                }
            }
        }
    }

    public void Open(ObjectsData.objectTypes[] objectTypes, int slotAmountThisTime, float remainOpenFor, bool inOrder)
    {
        wantedObjectTypes = objectTypes;
        wantedObjectsAmount = slotAmountThisTime;
        wantsObjects = true;
        wantsObjectsInOrder = inOrder;
        willRemainOpenFor = remainOpenFor;

        if (!inOrder) 
        {
            for (int i = 0; i < objectTypes.Length; i++)
            {
                requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Unordered[(int)objectTypes[i]]; //Set unordered requests in displays
            }
        }
        else 
        {
            for (int i = 0; i < objectTypes.Length; i++)
            {
                requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Ordered[(int)objectTypes[i]]; //Set ordered requests in displays
            }
        }

        gameObject.GetComponentInChildren<Animator>().SetBool("DoorOpen", true); //Animate door opening
    }

    public void Close()
    {
        wantsObjects = false;
        hasBeenOpenFor = 0f;

        for (int i = 0; i < requestDisplays.Length; i++)
        {
            requestDisplays[i].GetComponent<MeshRenderer>().material = objectsDataRef.objectMaterial_DoorDisplay_Unordered[0]; //Set displays to none (no requests)
        }

        gameObject.GetComponentInChildren<Animator>().SetBool("DoorOpen", false); //Animate door closing
    }
}