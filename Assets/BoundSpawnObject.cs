using UnityEngine;
using System.Collections;

public class BoundSpawnObject : MonoBehaviour
{
    #region 내부 변수 
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.forward;
    private bool isMoving = true;
    public float springBackwardForce = 8f;
    // amplitude: 튀는 높이, frequency: 튀는 속도 
    public float amplitude = 0.2f;
    public float frequency = 4.5f;
    private float baseY;
    private float phase;
    public float rotationPerSecond = 10f;
    // 효과음
    public AudioSource audioSource;
    public AudioClip boundsEffect;
    #endregion

    void Start()
    {
        baseY = transform.position.y;
        // 효과음
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayBoundSound());
    }

    void Update()
    {
        if (!isMoving)
        {
            Vector3 p = transform.position;
            p.y = Mathf.Lerp(p.y, baseY, 10f * Time.deltaTime);
            transform.position = p;
        }

        // <기본적인 공 회전>
        float degreessToRate = rotationPerSecond * 360 * Time.deltaTime;
        transform.Rotate(degreessToRate, 0, 0);

        // 1. 기본 z축 전진
        Vector3 dir = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

        // 2. 누적 위상으로 사인파 계산(=누적시간)
        phase += 2f * Mathf.PI * frequency * Time.deltaTime;
        float y = baseY + amplitude * Mathf.Sin(phase);

        // 3. y만 덮어쓰기 
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
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