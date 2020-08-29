using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    public Object objectToSpawn;

    void Start()
    {
        Instantiate(objectToSpawn, transform.position, Quaternion.Euler(0, 0, 0));
    }

    void Update()
    {
        
    }
}