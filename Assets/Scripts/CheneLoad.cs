using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static string TargetSceneName { get; private set; }
    public string animationStateName = "Load";
    private Animator animator;

    public static void LoadScene(string sceneName)
    {
        TargetSceneName = sceneName;
        SceneManager.LoadScene("Load");
    }

    private void Start()
    {
        animator = FindAnyObjectByType<Animator>();

        if (animator != null)
        {
            animator.Play(animationStateName);
            StartCoroutine(WaitForAnimator());
        }
        else
        {
            SwitchToTargetScene();
        }
    }

    private IEnumerator WaitForAnimator()
    {
        yield return null;
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(length);
        SwitchToTargetScene();
    }

    private void SwitchToTargetScene()
    {
        SceneManager.LoadScene(TargetSceneName);
        TargetSceneName = null;
    }
}
