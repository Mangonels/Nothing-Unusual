using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public ObjectsData objectsDataRef; //Reference to data from objects such as their names and display materials

    private float timeToDrop = 2.0f; //When this threshold is exceeded, an objectToSpawn is dropped
    private float timer = 2.0f; //When this value reaches "timeToDrop", an objectToSpawn is dropped, and "timer" is reset
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToDrop)
        {
            timer = 0.0f;

            int randomDispenser = Random.Range(0, objectsDataRef.dispenserGrid.Length); //Choose a random dispenser by Grid Slot number
            objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().SetSpawnObject((ObjectsData.objectTypes)Random.Range(0, objectsDataRef.objectGameObjects_GridAligned.Length)); //Assign random object to spawn in randomly selected dispenser
            objectsDataRef.dispenserGrid[randomDispenser].GetComponentInChildren<Dispenser>().Drop(); //Tell randomly selected dispenser to drop randomly selected and assigned item
        }
    }
}
