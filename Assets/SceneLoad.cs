using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip StartBtn;
    public AudioClip commonBtnSound;
    public GameObject GameExplainPanel;
    public GameObject[] panels;
    private int currenIndex = 0;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameExplainPanel.SetActive(false);
    }

    #region GameStart버튼
    public void LoadGameScene()
    {
        StartCoroutine(PlayStartBtnSound());
    }

    IEnumerator PlayStartBtnSound()
    {
        audioSource.clip = StartBtn;
        audioSource.Play();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(1);
    }
    #endregion

    #region GameExplain 버튼
    public void OpenPanel()
    {
        if (GameExplainPanel != null)
        {
            GameExplainPanel.SetActive(true);
            ShowPanelAtIndex(currenIndex);
        }
        else
        {
            Debug.LogError("오류: targetPanel이 연결되지 않았습니다!");
        }

        audioSource.clip = commonBtnSound;
        audioSource.Play();
    }

    public void ClosePanel()
    {
        if (GameExplainPanel != null)
        {
            GameExplainPanel.SetActive(false);
            currenIndex = 0;
        }
        else
        {
            Debug.LogError("오류: targetPanel이 연결되지 않았습니다!");
        }
        audioSource.clip = commonBtnSound;
        audioSource.Play();
    }
    #endregion


    #region 패널 전환
    public void ShowPanelAtIndex(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);
        }
        audioSource.clip = commonBtnSound;
        audioSource.Play();
    }

    public void ShowNextPanel()
    {
        currenIndex++;

        if (currenIndex >= panels.Length)
        {
            currenIndex = 0;
        }

        ShowPanelAtIndex(currenIndex);
    }
    #endregion
}
