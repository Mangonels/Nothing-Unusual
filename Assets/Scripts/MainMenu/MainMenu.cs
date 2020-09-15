using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AudioSource click;
    public GameObject credits;
    public void StartGame(bool skipIntro)
    {
        click.Play();

        SceneManager.LoadScene("BlocksRoom");
    }

    public void ShowCredits(bool show) 
    {
        credits.SetActive(show);

        click.Play();
    }
}
