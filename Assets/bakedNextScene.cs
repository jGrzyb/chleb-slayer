using UnityEngine;
using UnityEngine.SceneManagement;

public class bakedNextScene : MonoBehaviour
{
    public string sceneName;

    public void NextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}

