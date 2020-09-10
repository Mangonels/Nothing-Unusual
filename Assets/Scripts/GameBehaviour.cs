using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    [SerializeField] private float totalTimer = 0.0f;
    [SerializeField] private float phaseOfSpeedTimer = 0.0f;
    [SerializeField] private float phaseOfObjectsTimer = 0.0f;
    [SerializeField] private float dropTimer = 0.0f;

    [SerializeField] private int speedPhase = 0;
    [SerializeField] private int objectsPhase = 0;
    [SerializeField] private float phaseOfSpeedTimerDurationThreshold = 60f;
    [SerializeField] private float phaseOfObjectsTimerDurationThreshold = 210f;
    [SerializeField] private float timeToDropThreshold = 10f;
    [SerializeField] private float timeToDropThresholdReducer = 0.8f;

    public GameObject firstDispenser;
    public ObjectsData.objectTypes firstObject;
    public GameObject firstDoor;

    void Start()
    {
        Cursor.visible = false;

        //Drop first item in first dispenser
        firstDispenser.GetComponent<Dispenser>().SetSpawnObject(firstObject);
        firstDispenser.GetComponent<Dispenser>().Drop();

        ObjectsData.objectTypes[] objectTypes = { ObjectsData.objectTypes.TETRAS };
        firstDoor.GetComponent<Door>().OpenDoor(objectTypes, 30f, false);
    }

    void Update()
    {
        totalTimer += Time.deltaTime;
        phaseOfSpeedTimer += Time.deltaTime;
        phaseOfObjectsTimer += Time.deltaTime;
        dropTimer += Time.deltaTime;

        if (phaseOfSpeedTimer >= phaseOfSpeedTimerDurationThreshold)
        {
            phaseOfSpeedTimer = 0.0f; //Reset timer
            timeToDropThreshold *= timeToDropThresholdReducer; //Reduce time in between drops (difficulty increase)
            if (speedPhase == 0) phaseOfSpeedTimerDurationThreshold = 300f; //Phases after the first will last 5 min instead of 1
            speedPhase++; //Next speed phase
        }

        if (phaseOfObjectsTimer >= phaseOfObjectsTimerDurationThreshold)
        {
            phaseOfObjectsTimer = 0.0f;

            if (objectsPhase == 0) phaseOfObjectsTimerDurationThreshold = 300f; //Phases after the first will last 5 min instead of 1
            objectsPhase++; //Next objects phase
        }

        if (dropTimer >= timeToDropThreshold)
        {
            dropTimer = 0.0f;

            int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
            objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject((ObjectsData.objectTypes)Random.Range(1, objectsDataRef.objectGameObjects_GridAligned.Length)); //Assign random object (NONE not included) to spawn in randomly selected dispenser
            objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned item
        }
    }
}