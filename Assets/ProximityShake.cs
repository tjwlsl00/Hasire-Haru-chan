using System.Collections;
using UnityEngine;

public class ProximityShake : MonoBehaviour
{
    #region 내부 변수 
    public Transform targetObject;
    public float shakeDistance = 10.0f;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    private CameraShake cameraShake;
    private bool isShaking = false;
    #endregion

    void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Update()
    {
        if (targetObject == null || cameraShake == null) return;

        // 플레이어와 목표 오브젝트 사이의 거리를 계산
        float distance = Vector3.Distance(transform.position, targetObject.position);

        // 거리가 설정한 값보다 가깝고, 현재 흔들리는 중이 아니라면
        if (distance < shakeDistance && !isShaking)
        {
            StartCoroutine(ShakeAndReset());
        }
    }

    private IEnumerator ShakeAndReset()
    {
        isShaking = true;
        yield return StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        isShaking = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (targetObject != null)
        {
            Gizmos.DrawWireSphere(targetObject.position, shakeDistance);
        }
    }
}
