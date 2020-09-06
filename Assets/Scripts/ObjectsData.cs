using UnityEngine;

public class ObjectsData : MonoBehaviour
{
    //All objects in the game, by their enum identifier:
    public enum objectTypes
    {
        NONE,
        HOURGLASS,
        SQUARE_WEIGHT,
        TENNISBALL,
        HYPERCUBE,
        WEIGHT,
        DORIC_PILLAR,
        CUBE,
        TETRAS
    };

    public Material[] objectMaterial_DoorDisplay; //Contains all graphics for the door displays in same order as each enum "objectTypes"
    public GameObject[] objectGameObjects; //Contains all gameObjects for each enum "objectTypes"

    void Start()
    {

    }

    void Update()
    {
        
    }
}
