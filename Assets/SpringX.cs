using UnityEngine;

public class SpringX : MonoBehaviour
{
    #region 내부 변수 
    public float springBackwardForce = 8f;
    public float springUpForce = 0.3f;
    public float selfBounceForce = 5f;
    private Rigidbody rigidbody;
    #endregion

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (playerMovement != null)
            {
                #region 플레이어 튕기기
                // 2) 접근 방향(플레이어가 들어온 방향)
                Vector3 incoming = -collision.relativeVelocity;

                // 3) 너무 느리면 법선으로 보정
                ContactPoint contact = collision.GetContact(0);
                Vector3 normal = contact.normal;
                Vector3 approachDir = (incoming.sqrMagnitude > 0.1f) ? incoming.normalized : -normal;

                // 4) 반대방향으로 밀기
                Vector3 pushDir = approachDir;

                // 5) 뒤로 임펄스
                playerRigidbody.AddForce(pushDir * springBackwardForce, ForceMode.Impulse);
                playerRigidbody.AddForce(Vector3.up * springUpForce, ForceMode.Impulse);

                // 6) 스프링도 튕김(자기 연출)
                rigidbody.AddForce(Vector3.up * selfBounceForce, ForceMode.Impulse);
                #endregion

                #region 플레이어 분노 게이지 증가
                Debug.Log("플레이어 분노 게이지 +20");
                playerMovement.currentUltAmount += 100f;
                #endregion
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 플레이어와 충돌 1초 후 오브젝트 파괴 
        Destroy(gameObject, 1.0f);
    }
}
