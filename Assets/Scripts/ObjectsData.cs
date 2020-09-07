using UnityEngine;

public class ObjectsData : MonoBehaviour
{
    //All objects in the game, by their enum identifier:
    public enum objectTypes
    {
        NONE,
        SIMPLE,
        WEIGHT,
        BASE,
        SUPPORT,
        ORB,
        FRACTAL,
        PILLAR,
        TETRAS,
        ATOM,
        ORBOLOID,
        NINJA,
        RINGS,
        POTION,
        PRISM,
        CONTAINER,
        TORNADO,
        MAGIC_ARTIFACT,
        DARK_MATTER,
        MAGNETRON,
        CONNECTOR
    };

    public Material[] objectMaterial_DoorDisplay_Unordered; //Contains all graphics for the door displays in same order as each enum "objectTypes", for unordered door requirements
    public Material[] objectMaterial_DoorDisplay_Ordered; //Contains all graphics for the door displays in same order as each enum "objectTypes", for ordered door requirements
    public GameObject[] objectGameObjects_GridAligned; //Contains GridAligned Objects as gameObjects for each enum "objectTypes"
    public GameObject[] objectGameObjects_Held; //Contains Held Objects as gameObjects for each enum "objectTypes"

    void Start()
    {

    }

    void Update()
    {
        
    }
}
