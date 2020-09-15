using UnityEngine;

public class GridBox : MonoBehaviour
{
    public AudioSource alarm;

    public GameBehaviour gameBehaviourRef; //Reference to game behaviour script

    public int gameOverObjects = 5;
    [SerializeField] private int currentObjectAmmount = 0;

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
        if (currentObjectAmmount == gameOverObjects - 1) alarm.Play();
        if (currentObjectAmmount >= gameOverObjects && gameBehaviourRef.gameActive) //Box capacity exceeded
        {
            //GAME OVER
            gameBehaviourRef.GameOver();
        }
    }

    public int GetCurrentObjectsAmmount()
    {
        return currentObjectAmmount;
    }
}