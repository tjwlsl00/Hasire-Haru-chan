using System.Collections;
using UnityEngine;

public class WarningEvent : MonoBehaviour
{
    #region 내부 변수
    // 경고 오브젝트 
    public GameObject WarningObj;
    // 오디오
    public AudioSource audioSource;
    public AudioClip SirenClip;
    #endregion

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        WarningObj.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            WarningObj.SetActive(true);
            Debug.Log("충돌");
            // 오디오 재생
            PlayWorningSound();
            StartCoroutine(UnvisibleWarning());
        }

    }

    public void PlayWorningSound()
    {
        audioSource.clip = SirenClip;
        audioSource.Play();
    }

    //적 소환시 경고 표시(UI) -> 2초 경과 후 비활성화
    IEnumerator UnvisibleWarning()
    {
        yield return new WaitForSeconds(2f);
        WarningObj.SetActive(false);
    }

}
