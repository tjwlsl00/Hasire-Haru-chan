using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    #region 내부 변수 
    public Transform player;
    public float initialDistance;
    public Vector3 initialOffset;
    public float targetDistance;
    public Vector3 targetOffset;
    private bool isGoalReached = false;
    private float moveSpeed = 2.0f;
    // 효과음 
    public AudioSource audioSource;
    public AudioClip CountClip;
    #endregion

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform not assigned!");
            return;
        }

        // 초기 거리 계산 및 저장
        initialOffset = transform.position - player.position;
        initialDistance = initialOffset.magnitude;
        targetOffset = initialOffset;
        targetDistance = initialDistance;

        // 효과음
        audioSource = GetComponent<AudioSource>();
        PlayCountSound();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 항상 현재 값(current)을 목표 값(target)으로 부드럽게 이동시킴
        initialOffset = Vector3.Lerp(initialOffset, targetOffset, moveSpeed * Time.deltaTime);
        initialDistance = Mathf.Lerp(initialDistance, targetDistance, moveSpeed * Time.deltaTime);
        
        // 최종 카메라 위치 계산 및 적용
        transform.position = player.position + initialOffset.normalized * initialDistance;

    }

    #region 카메라 위치 변화(궁극기 사용기->평소, Victroy시)
    public void UltCamera(Vector3 ultOffset, float ultDistance)
    {
        StartCoroutine(UltCameraRoutine(ultOffset, ultDistance));
    }

    IEnumerator UltCameraRoutine(Vector3 ultOffset, float ultDistance)
    {
        targetOffset = ultOffset;
        targetDistance = ultDistance;
        yield return new WaitForSeconds(2f);
        NormalPosition(new Vector3(0, 3.13f, -5.49f), 6.32f);
    }

    public void NormalPosition(Vector3 newOffset, float newDistance)
    {
        targetOffset = newOffset;
        targetDistance = newDistance;
    }

    public void OnGoalReached(Vector3 goalOffset, float goalDistance)
    {
        targetOffset = goalOffset;
        targetDistance = goalDistance;
    }
    #endregion

    #region 효과음
    public void PlayCountSound()
    {
        audioSource.clip = CountClip;
        audioSource.Play();
    }
    #endregion
}
