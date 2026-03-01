using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        SoundManager.I.PlayMusic(SoundManager.I.MenuMusic);
    }
    public string sceneName = "Game";
    public void PlayGame()
    {
        FadeManager.I.LoadSceneWithFade(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

}
