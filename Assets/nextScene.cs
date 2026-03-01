using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{

    public string sceneName;

    public void NextScene()
    {
        FadeManager.I.LoadSceneWithFade(sceneName);
    }
}
