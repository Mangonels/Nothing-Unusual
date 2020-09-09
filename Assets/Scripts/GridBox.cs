using UnityEngine;

public class GridBox : MonoBehaviour
{
    public int gameOverObjects = 10;
    [SerializeField] private int currentObjectAmmount = 0;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void IncreaseCurrentObjectAmmount() 
    { 
        currentObjectAmmount++;
        CapacityCheck();
    }
    public void DecreaseCurrentObjectAmmount()
    {
        currentObjectAmmount--;
        CapacityCheck();
    }

    private void CapacityCheck() 
    {
        if (currentObjectAmmount >= gameOverObjects) //Box capacity exceeded
        {
            //GAME OVER

        }
    }

    public int GetCurrentObjectsAmmount()
    {
        return currentObjectAmmount;
    }
}