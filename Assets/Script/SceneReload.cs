using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReload : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public AudioSource audioSource;
    public AudioClip StartGameEffect;
    public AudioClip ByeClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerMovement.isGameOver || playerMovement.isVictroy)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                PlayRestartEffect();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayMenuEffect();
            }
        }
    }

    #region 게임 재시작
    public void PlayRestartEffect()
    {
        // 리스타트 오디오 재생 
        audioSource.clip = StartGameEffect;
        audioSource.Play();
        // 코루틴으로 오디오 재생후 씬 넘어가도록 
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f);
        RestartCurrentScene();
    }

    public void RestartCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    #endregion

    #region 메뉴로 

    public void PlayMenuEffect()
    {
        audioSource.clip = ByeClip;
        audioSource.Play();
        StartCoroutine(GoBackHome());
    }

    IEnumerator GoBackHome()
    {
        yield return new WaitForSeconds(1f);
        LoadMenuScene();
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}
