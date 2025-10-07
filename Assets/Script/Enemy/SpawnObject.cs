using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnObject : MonoBehaviour
{
    #region 내부 변수 
    public float moveSpeed = 5f;
    public Vector3 moveDirection = Vector3.forward;
    private bool isMoving = true;
    // 오디오
    public AudioSource audioSource;
    public AudioClip rhinoClip;
    #endregion
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayrhinoSound());
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    #region 충돌 이벤트
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // 효과음
    IEnumerator PlayrhinoSound()
    {
        while (true)
        {
            audioSource.clip = rhinoClip;
            audioSource.Play();
            yield return new WaitForSeconds(2f);
        }
    }
}
