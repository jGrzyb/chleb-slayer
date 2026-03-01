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
        GameManager.I.ResetProgression();
        FadeManager.I.LoadSceneWithFade(GameManager.I.GetNextLevelName());
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

}
