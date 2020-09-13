using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    
    void Start()
    {

    }

    void Update()
    {

    }

    public void StartGame(bool skipIntro)
    {
        SceneManager.LoadScene("Nothing Unusual");
    }
}
