using UnityEngine;
using System.Collections;

public class ForwardSpawnObject : MonoBehaviour
{
    #region 내부 변수 
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.forward;
    private bool isMoving = true;
    public float springBackwardForce = 8f;
    public float rotationPerSecond = 10f;
    // 효과음
    public AudioSource audioSource;
    public AudioClip boundsEffect;
    #endregion
    void Start()
    {
        // 효과음
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayBoundSound());
    }

     void Update()
    {
        if (!isMoving) return;

        // <기본적인 공 회전>
        float degreessToRate = rotationPerSecond * 360 * Time.deltaTime;
        transform.Rotate(degreessToRate, 0, 0);

        // 1. 기본 z축 전진
        Vector3 dir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
    

    #region 충돌 이벤트 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                // 궁극기 사용 중 x일때, 충돌 허용
                if (!playerMovement.isPlayerUsingUlt)
                {
                    // 플레이어 스턴 효과
                    playerMovement.ApplySlowEffect(2f);

                    // 플레이어 분노 게이지 증가 
                    Debug.Log("플레이어 분노 게이지 +20");
                    playerMovement.currentUltAmount += 20f;
                }
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    #endregion

    IEnumerator PlayBoundSound()
    {
        while (true)
        {
            audioSource.clip = boundsEffect;
            audioSource.Play();
            yield return new WaitForSeconds(2f);
        }
    }

}
