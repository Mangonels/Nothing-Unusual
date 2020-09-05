using UnityEngine;

public class GridBox : MonoBehaviour
{
    public int maxItemsBeforeGameOver = 5;
    [SerializeField] private int currentItemAmmount = 0;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void IncreaseCurrentItemAmmount() 
    { 
        currentItemAmmount++;
        CapacityCheck();
    }
    public void DecreaseCurrentItemAmmount()
    {
        currentItemAmmount--;
        CapacityCheck();
    }

    private void CapacityCheck() 
    {
        if (currentItemAmmount > maxItemsBeforeGameOver) //Box capacity exceeded
        {
            //GAME OVER

        }
    }
}