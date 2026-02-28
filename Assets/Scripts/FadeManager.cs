using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class FadeManager : MonoBehaviour
{
    public static FadeManager I { get; private set; }
    private Animator animator;
    private Action callback;

    void Awake() {
        if (I != null) {
            Destroy(gameObject);
            return;
        }
        else
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            animator = GetComponent<Animator>();
        }
    }

    public void MakeTransition(Action callback) {
        if(this.callback != null) {
            Debug.LogWarning("Attempting to make a transition while another transition is in progress. Executing callback immediately.");
            callback();
            return;
        }
        this.callback = callback;
        animator.SetBool("IsBlack", true);
    }

    public void LoadSceneWithFade(int buildIndex) {
        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex);
        op.allowSceneActivation = false;
        
        FadeManager.I.MakeTransition(async () => {
            await WaitUntilReady(op);
            op.allowSceneActivation = true;
        });
    }

    private async Task WaitUntilReady(AsyncOperation op) {
        while (op.progress < 0.9f) {
            await Task.Yield();
        }
    }

    // Called as an animation event
    private void ExecuteCallbackEvent() {
        callback();
        animator.SetBool("IsBlack", false);
        callback = null;
    }
}