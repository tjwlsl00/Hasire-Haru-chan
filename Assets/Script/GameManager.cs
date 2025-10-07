using System.Collections;
using UnityEngine;

public enum GameState { Countdown, Playing, Victory, GameOver }

public class GameManager : MonoBehaviour
{
    #region 내부 변수 
    public static GameState currentState;
    public static GameManager instance;
    // 효과음
    public AudioSource audioSource;
    public AudioClip CountClip;
    public AudioClip BGM;
    public AudioClip GameOverBGM;
    
    #endregion

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        PlayBGM();
        ChangeState(GameState.Countdown);
        StartCoroutine(WaitForGameStart());
    }

    public IEnumerator WaitForGameStart()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("카운팅 종료");
        ChangeState(GameState.Playing);
    }

    #region 게임 매니저 상태 바뀌는 경우(BGM변경-한번만 실행)
    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        Debug.Log(newState + "상태로 변경 되었습니다.");

        if (currentState == GameState.GameOver)
        {
            PlayGameoverBGM();
        }
    }
    #endregion

    #region 효과음
    public void PlayBGM()
    {
        audioSource.clip = BGM;
        audioSource.Play();
    }
  
    public void PlayGameoverBGM()
    {
        audioSource.clip = GameOverBGM;
        audioSource.Play();
    }
    #endregion
}
