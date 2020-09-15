using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public AudioSource trueLoop;
    [SerializeField] private float realMusicTimer = 0.0f;
    [SerializeField] private float realMusicTrheshold = 90.0f;
    [SerializeField] private bool doTimer = true;


    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials
    public GameObject gameOverWindowRef;

    public bool gameActive = true;
    [SerializeField] private int score = 0;
    public TextMeshProUGUI uiScore;

    //[SerializeField] private float totalTimer = 0.0f;
    [SerializeField] private float phaseOfDropSpeedTimer = 0.0f;
    [SerializeField] private float phaseOfDoorRequestsTimer = 0.0f;
    [SerializeField] private float newDropTimer = 0.0f;
    [SerializeField] private float newDropExtraTimer = 0.0f;
    [SerializeField] private float newDoorTimer = 0.0f;

    [SerializeField] private int percentileChanceOfDroppingWhenItsTime = 85;
    [SerializeField] private int percentileChanceOfDroppingExtraWhenItsTime = 65;
    [SerializeField] private int percentileChanceOfDoorOpeningWhenItsTime = 90;

    //[SerializeField] private int speedPhase = 0;
    [SerializeField] private int doorRequestsPhase = 0;
    [SerializeField] private float phaseOfSpeedTimerDurationThreshold = 90f; //Threshold after which drop timeToDropThreshold is reduced
    [SerializeField] private float phaseOfDoorRequestsTimerDurationThreshold = 45f; //Threshold after which drop timeToDropThreshold is reduced
    [SerializeField] private float timeToDropThreshold = 20f;
    [SerializeField] private float timeToDropExtraThreshold = 25f;
    [SerializeField] private float timeToDropThresholdReducer = 0.6f;
    [SerializeField] private float timeToDropExtraThresholdReducer = 0.8f;
    [SerializeField] private float timeToOpenDoorThreshold = 20f;

    [SerializeField] private bool firstTimeToDropThresholdReached = true;
    [SerializeField] private float maxTimeDoorsOpen = 80f;
    [SerializeField] private float minTimeDoorsOpen = 70f;
    [SerializeField] private int maxAmountOfObjectsRequestedPerDoor = 2;
    [SerializeField] private int minAmountOfObjectsRequestedPerDoor = 1;
    [SerializeField] private int objectsOrderMode = 0; //0 unordered only, 1, ordered and unordered, 2 ordered only

    [SerializeField] private List<ObjectsData.objectTypes> doorAccountableSpawnedObjects = null;

    public GameObject firstDispenser;
    public ObjectsData.objectTypes firstObject;
    public GameObject firstDoor;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Drop first item in first dispenser
        firstDispenser.GetComponent<Dispenser>().SetSpawnObject(firstObject);
        firstDispenser.GetComponent<Dispenser>().Drop();

        //Open first door with first required objects
        ObjectsData.objectTypes[] objectTypes = { ObjectsData.objectTypes.TETRAS, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE };
        firstDoor.GetComponent<Door>().Open(objectTypes, 1, 30f, false);
    }

    void Update()
    {
        if (doTimer) 
        {
            realMusicTimer += Time.deltaTime;

            if (realMusicTimer > realMusicTrheshold) 
            {
                trueLoop.Play();
                doTimer = false;
            }
        }

        if (gameActive)
        {
            //totalTimer += Time.deltaTime;
            phaseOfDropSpeedTimer += Time.deltaTime;
            phaseOfDoorRequestsTimer += Time.deltaTime;
            newDropTimer += Time.deltaTime;
            newDropExtraTimer += Time.deltaTime;
            newDoorTimer += Time.deltaTime;

            if (phaseOfDropSpeedTimer >= phaseOfSpeedTimerDurationThreshold)
            {
                phaseOfDropSpeedTimer = 0.0f; //Reset timer
                timeToDropThreshold *= timeToDropThresholdReducer; //Reduce time in between drops
                timeToDropExtraThreshold *= timeToDropExtraThresholdReducer; //Reduce time in between extra drops
                //speedPhase++; //Next speed phase
            }

            if (phaseOfDoorRequestsTimer >= phaseOfDoorRequestsTimerDurationThreshold) //Incrementing "objectsPhase" affects door progressions such as: max/minAmountOfObjectsRequestedPerDoor, max/minTimeDoorsOpen & objectsOrderMode
            {
                phaseOfDoorRequestsTimer = 0.0f;

                //-------------------------
                //Per phase configurations
                //-------------------------

                doorRequestsPhase++; //Next objects phase
                if (doorRequestsPhase == 1)
                {
                    timeToOpenDoorThreshold = 15f;
                    phaseOfDoorRequestsTimerDurationThreshold = 90f; //Phases after 0 will last 2 min instead of 1
                    maxAmountOfObjectsRequestedPerDoor = 3;
                }
                else if (doorRequestsPhase == 2)
                {
                    minTimeDoorsOpen = 55f;
                    maxAmountOfObjectsRequestedPerDoor = 4;
                }
                else if (doorRequestsPhase == 3) 
                {
                    objectsOrderMode = 2;
                    maxAmountOfObjectsRequestedPerDoor = 5;
                }
                else if (doorRequestsPhase == 4)
                {
                    maxTimeDoorsOpen = 70f;
                }
                else if (doorRequestsPhase == 5)
                {
                    minTimeDoorsOpen = 50f;
                }
                else if (doorRequestsPhase == 6)
                {
                    objectsOrderMode = 3;
                }
                else if (doorRequestsPhase == 8)
                {
                    //objectsOrderMode = 3 preserved from previous odd phase
                    minTimeDoorsOpen = 45f;
                    maxTimeDoorsOpen = 55f;
                }
                else if (doorRequestsPhase == 12)
                {
                    //objectsOrderMode = 3 preserved from previous odd phase
                    minTimeDoorsOpen = 40f;
                    maxTimeDoorsOpen = 50f;
                }
                else if (doorRequestsPhase == 16)
                {
                    //objectsOrderMode = 3 preserved from previous odd phase
                    minTimeDoorsOpen = 35f;
                    maxTimeDoorsOpen = 45f;
                }
                else if (doorRequestsPhase == 20)
                {
                    //objectsOrderMode = 3 preserved from previous odd phase
                    minTimeDoorsOpen = 25f;
                    maxTimeDoorsOpen = 35f;
                }

                else if ((doorRequestsPhase % 2) != 0) //Every odd number
                {
                    objectsOrderMode = 2;
                }
                else if ((doorRequestsPhase % 2) == 0) //Every even number
                {
                    objectsOrderMode = 3;
                }
            }

            if (newDropTimer >= timeToDropThreshold) //It's time to drop an item on a random tile
            {
                newDropTimer = 0.0f;

                if (!firstTimeToDropThresholdReached) //Not first drop
                {
                    if (Random.Range(1, 100) < percentileChanceOfDroppingWhenItsTime) //Reduced chance of dropping depending on "percentileChanceOfDroppingWhenItsTime"?
                    {
                        int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
                        ObjectsData.objectTypes spawnedObject = (ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length);
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject(spawnedObject); //Assign random object (NONE not included) to spawn in randomly selected dispenser
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned item
                        doorAccountableSpawnedObjects.Add(spawnedObject);
                    }
                }
                else //Drop a first batch of objects so the room doesn't feel so empty at the beginning
                {
                    firstTimeToDropThresholdReached = false;
                    for (int i = 0; i < 3; i++) //Drop 3 objects
                    {
                        int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
                        ObjectsData.objectTypes spawnedObject = (ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length);
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject(spawnedObject); //Assign random object (NONE not included) to spawn in randomly selected dispenser
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned object
                        doorAccountableSpawnedObjects.Add(spawnedObject);
                    }
                }
            }

            if (newDropExtraTimer >= timeToDropExtraThreshold) //It's time to drop an item on a random tile
            {
                newDropExtraTimer = 0.0f;

                if (Random.Range(1, 100) < percentileChanceOfDroppingExtraWhenItsTime) //Reduced chance of dropping extra depending on "percentileChanceOfDroppingExtraWhenItsTime"?
                {
                    int randomAmount = Random.Range(1, 3);
                    for (int i = 0; i < randomAmount; i++) //Drop 1 to 3 objects
                    {
                        int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
                        ObjectsData.objectTypes spawnedObject = (ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length);
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject(spawnedObject); //Assign random object (NONE not included) to spawn in randomly selected dispenser
                        objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned object
                        doorAccountableSpawnedObjects.Add(spawnedObject);
                    }
                }
            }

                if (newDoorTimer >= timeToOpenDoorThreshold) //It's time to open another door
            {
                newDoorTimer = 0.0f;
                if (Random.Range(1, 100) < percentileChanceOfDoorOpeningWhenItsTime) //Reduced chance of door opening depending on "percentileChanceOfDoorOpeningWhenItsTime"? [IMPORTANT] Already open doors oclude doors from opening (if they are still open)
                {
                    int randomDoor = Random.Range(0, objectsDataRef.doors.Length); //Choose a random door by doors slot number
                    if (!objectsDataRef.doors[randomDoor].GetComponent<Door>().wantsObjects) //Obviously, only open the door if it isn't already open
                    {
                        //----------------------
                        //New door configuration
                        //----------------------

                        //Fill up 5 object types that will be requested by door
                        ObjectsData.objectTypes[] objectTypes = { ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE, ObjectsData.objectTypes.NONE }; //Array which will contain all required objects by name for the door, for reference
                        int slotAmountThisTime = Random.Range(minAmountOfObjectsRequestedPerDoor, maxAmountOfObjectsRequestedPerDoor); //Amount of required slots starting from 0 the door will have requirements for
                        for (int i = 0; i < slotAmountThisTime; i++) //Configure slots up to "slotAmountThisTime", the rest will be "NONE" objects by default
                        {
                            if (doorAccountableSpawnedObjects.Count != 0) //Accountable objects exist
                            {
                                objectTypes[i] = doorAccountableSpawnedObjects[Random.Range(0, doorAccountableSpawnedObjects.Count)];
                            }
                            else objectTypes[i] = (ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length); //Make up an item (this should be a rare situation where no items have probably spawned)
                        }

                        //Define a duration for the door to remain open
                        float randomDoorDuration = Random.Range(minTimeDoorsOpen, maxTimeDoorsOpen);

                        //Define if objects will be asked for in order or not
                        bool inOrder = false;
                        if (objectsOrderMode == 0) { }
                        else if (objectsOrderMode == 1)
                        {
                            if (Random.Range(0, 1) == 0) inOrder = true;
                            else inOrder = false;
                        }
                        else if (objectsOrderMode == 2) { inOrder = true; }

                        //Finally open the door with final input, which is usually going to be dependant on "doorRequestsPhase"
                        objectsDataRef.doors[randomDoor].GetComponent<Door>().Open(objectTypes, slotAmountThisTime, randomDoorDuration, inOrder);
                    }
                }
            }
        }
        else if (Input.GetAxis("Submit") > 0.5) //Restart game by pressing enter only when inactive
        {
            SceneManager.LoadScene("BlocksRoom");
        }
    }

    public void RemoveDoorAccountableObject(ObjectsData.objectTypes type) 
    {
        doorAccountableSpawnedObjects.Remove(type);
    }
    public void AddPoints(int points) 
    {
        score += points;
        uiScore.text = score.ToString();
    }

    public void GameOver()
    {
        Debug.Log("GameOver called");

        //Stop game behaviour updating
        gameActive = false;

        //Force all doors to close
        for (int i = 0; i<objectsDataRef.doors.Length; i++) 
        {
            objectsDataRef.doors[i].GetComponent<Door>().Close();
        }

        //Show game over window
        gameOverWindowRef.SetActive(true);
    }
}