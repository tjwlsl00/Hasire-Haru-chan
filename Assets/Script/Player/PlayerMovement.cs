using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    #region 내부 변수
    // 오브젝트 연결
    private Rigidbody rb;
    private Animator animator;
    private Vector3 moveInput;
    public Image PlayerStaminaUI;
    public Image PlayerUltUI;
    public GameObject CanUltPanel;
    public GameObject GoalUI;
    public GameObject GameOverUI;
    public StopWatch stopWatch;
    public TextMeshProUGUI gameOverResultTime;
    public TextMeshProUGUI GoalResultTime;
    public FollowPlayer followPlayer;
    // 플레이어 Raycast
    public LayerMask groundLayer;
    private bool isGrounded;
    // 플레이어 수치 
    public float currentSpeed;
    public float normalSpeed = 0.5f;
    public float dashSpeed = 20f;
    public float accelerationTime = 0.3f;
    public float JumpForce = 10f;
    public float maxStamina = 100f;
    public float currentStamina;
    public float maxUltAmount = 100f;
    public float currentUltAmount;
    public float UltTime = 7f;
    public float UltSpeed = 20f;
    // 오디오
    public AudioSource audioSource;
    public AudioClip Jump_1Clip;
    public AudioClip Jump_2Clip;
    public AudioClip HitClip;
    public AudioClip UltClip;
    public AudioClip GameOver_1Clip;
    public AudioClip GameOver_2Clip;
    public AudioClip VictroyClip;
    // 파티클
    public ParticleSystem speedLinesParticleSystem;
    public ParticleSystem UltParticleSystem;
    public ParticleSystem StunParticleSystem;
    #endregion

    #region 애니메이션 변수
    public bool canMovePlayer = true;
    public bool CanUlt = false;
    public bool isRun;
    private string isRunAnimParam = "isRun";
    public bool isDashing = false;
    public bool isJump;
    private string isJumpAnimParam = "isJump";
    public bool isPlayerUsingUlt = false;
    public bool isdashLocked = false;
    public bool _ignoreInput;
    public bool isGameOver = false;
    public bool isVictroy;
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        currentStamina = maxStamina;
        currentUltAmount = 0f;
        isVictroy = false;
        UpdatePlayerStaminaUI();
        UpdatePlayerUltUI();
        CanUltPanel.SetActive(false);
        GoalUI.SetActive(false);
        GameOverUI.SetActive(false);
        // 오디오
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 게임매니저 카운트다운, 게임 승리, 오버시 입력 무시 
        if (GameManager.currentState == GameState.Countdown || isVictroy || isGameOver) return;

        #region 플레이어 이동
        if (canMovePlayer)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            moveInput = new Vector3(horizontalInput, 0, verticalInput).normalized;

            if (moveInput != Vector3.zero)
            {
                isRun = true;

                Vector3 targetVelocity = moveInput * currentSpeed;
                targetVelocity.y = rb.linearVelocity.y;
                rb.linearVelocity = targetVelocity;

                Quaternion targetRotation = Quaternion.LookRotation(moveInput);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, dashSpeed * Time.fixedDeltaTime);
            }
            else
            {
                isRun = false;

                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }

            animator.SetBool(isRunAnimParam, isRun);
        }
        else
        {
            moveInput = Vector3.zero;
        }
        #endregion

        #region 플레이어 대시 
        if (!CanUlt && !isPlayerUsingUlt)
        {
            // boundObject랑 부딫히면 4초간 대쉬 불가
            if (_ignoreInput) return;

            if (!isdashLocked && Input.GetKey(KeyCode.Z) && currentStamina > 0)
            {
                isDashing = true;
                currentSpeed = dashSpeed;
                currentStamina -= Time.deltaTime * 10f;

                // 대쉬가 0이 되면 일정 시간 이후 다시 사용 가능(스테미나 회복을 위해)
                if (currentStamina <= 0)
                {
                    isdashLocked = true;
                    currentSpeed = normalSpeed;
                    StartCoroutine(WaitforStamina());
                }

                // 대쉬 파티클 생성
                StartDashEffect();
            }
            else
            {
                isDashing = false;
                currentStamina = Mathf.Min(currentStamina + Time.deltaTime * 20f, maxStamina);
                currentSpeed = normalSpeed;
                StopDashEffect();
            }
            // 스테미나 UI 업데이트 
            UpdatePlayerStaminaUI();
        }

        IEnumerator WaitforStamina()
        {
            yield return new WaitForSeconds(1f);
            isdashLocked = false;
            Debug.Log("대쉬 사용 가능 현재 대쉬량:" + currentStamina.ToString());
        }
        #endregion

        #region 플레이어 점프 
        // 레이캐스트 이용 땅을 확인하는 함수 호출
        CheckGrounded();
        if (canMovePlayer && isGrounded && Input.GetKeyDown(KeyCode.LeftControl))
        {
            isJump = true;
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            animator.SetBool(isJumpAnimParam, true);
            PlayJumpSound();
        }
        #endregion

        #region 플레이어 궁극기 
        // 궁극기 시간 지남에 따라서 천천히 차게 
        if (!isPlayerUsingUlt)
        {
            currentUltAmount = Mathf.Min(currentUltAmount + Time.deltaTime * 3.3f, maxUltAmount);
        }

        UpdatePlayerUltUI();

        #region 궁극기 사용 조건
        if (isGrounded && currentStamina >= maxStamina && currentUltAmount >= maxUltAmount && !isPlayerUsingUlt)
        {
            CanUlt = true;
        }
        else
        {
            // 조건 중 하나라도 맞지 않으면 무조건 false로 설정합니다.
            CanUlt = false;
        }
        if (CanUltPanel != null) // 혹시 모를 Null 에러 방지
        {
            CanUltPanel.SetActive(CanUlt);
        }

        if (CanUlt)
        {
            // Shift를 누른 상태에서 Z를 누르는 것으로 입력 로직을 단순화합니다.
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
            {
                WaitforTransform();
                PlayUltSound();
            }
        }
        #endregion
        #endregion
    }

    #region 지면 체크 
    private void CheckGrounded()
    {
        float rayDistance = 0.1f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;

        if (Physics.Raycast(rayOrigin, Vector3.down, rayDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red);
    }
    #endregion

    // 궁 누르고 일정 시간동안 움직임x(변신) -> 이후 이동가능
    public void WaitforTransform()
    {
        // 모든 행동 상태 한번에 취소 
        isRun = false;
        isJump = false;
        isDashing = false;
        animator.SetBool(isRunAnimParam, false);
        animator.SetBool(isJumpAnimParam, false);
        // 궁 모션 
        animator.SetTrigger("isUsingUlt");

        // 카메라 위치 변환(궁극기 시)
        followPlayer.UltCamera(new Vector3(0, 4.15f, -5.49f), 3.73f);

        // 궁 이펙트
        StartUltEffect();

        StartCoroutine(iceageplayer());
    }

    IEnumerator iceageplayer()
    {
        canMovePlayer = false;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(2.0f);
        canMovePlayer = true;
        StartCoroutine(PlayerUsingUlt());
    }

    IEnumerator PlayerUsingUlt()
    {
        Debug.Log("플레이어 궁극기 사용!");
        isPlayerUsingUlt = true;
        currentUltAmount = 0;
        CanUltPanel.SetActive(false);
        currentSpeed = UltSpeed;

        StartDashEffect();
        StartUltEffect();

        yield return new WaitForSeconds(UltTime);

        StopDashEffect();
        StopUltEffect();

        // 10초 이후 궁극기종료
        Debug.Log("플레이어 궁극기 끝!");
        isPlayerUsingUlt = false;
        CanUlt = false;
    }

    public void SetIgnoreInput(bool ignore)
    {
        _ignoreInput = ignore;
        currentSpeed = 5f;
    }

    #region 충돌 이벤트 
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isJump)
            {
                isJump = false;
                animator.SetBool("isJump", false);
            }
        }

        if (collision.gameObject.CompareTag("BoundObj"))
        {
            if (isPlayerUsingUlt) return;
            StartCoroutine(StunEffectCoroutine());
            HitSound();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 게임 오버 혹은 승리 시 충돌 이벤트 무시
        if (isGameOver || isVictroy) return;

        if (other.CompareTag("Enemy"))
        {
            // 게임매니저 상태 GameOver호출
            GameManager.instance.ChangeState(GameState.GameOver);
            GameOver();
        }

        if (other.CompareTag("GoalLine"))
        {
            // 게임매니저 상태 GameOver호출
            GameManager.instance.ChangeState(GameState.Victory);
            Victory();
        }
    }

    public void ApplySlowEffect(float duration)
    {
        StartCoroutine(PlayerGetSlow(duration));
    }

    IEnumerator PlayerGetSlow(float duration)
    {
        Debug.Log(duration + "초간 대쉬불가/속도 ↓");
        SetIgnoreInput(true);
        yield return new WaitForSeconds(duration);
        Debug.Log("속박이 풀렸습니다.");
        SetIgnoreInput(false);
    }

    #endregion

    #region 게임 종료
    public void GameOver()
    {
        isGameOver = true;
        canMovePlayer = false;

        // 모든 행동 상태 한번에 취소 
        isRun = false;
        isJump = false;
        isDashing = false;
        animator.SetBool(isRunAnimParam, false);
        animator.SetBool(isJumpAnimParam, false);

        // 플레이어 물리 영향 X
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        animator.SetTrigger("isGameover");

        // 스톱워치 시간 받아오기
        GetGameOverResultTime();

        // GameOver 패널 띄우기 
        GameOverUI.SetActive(isGameOver);

        // 오디오 재생(두 사운드 중에 랜덤 재생)
        PlayGameOverSound();
    }
    #endregion

    #region 게임 승리
    public void Victory()
    {
        isVictroy = true;
        Debug.Log("승리함");
        canMovePlayer = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float currentX = transform.eulerAngles.x;
        float currentZ = transform.eulerAngles.z;
        transform.rotation = Quaternion.Euler(currentX, 180f, currentZ); //플레이어 z축으로 뒤집기

        animator.SetTrigger("isGoal");

        GetGoalResultTime(); // 시간 값 가져오기 

        followPlayer.OnGoalReached(new Vector3(0, 23.8f, -27.5f), 4.38f); //카메라 offset 값 수정   

        GoalUI.SetActive(true);

        PlayVictorySound();
    }
    #endregion

    #region 스톱워치에서 시간 정보 받아오기 
    public void GetGoalResultTime()
    {
        GoalResultTime.text = stopWatch.timeText.text;
    }

    public void GetGameOverResultTime()
    {
        gameOverResultTime.text = stopWatch.timeText.text;
    }
    #endregion

    #region 플레이어 UI업데이트(스테미나, 분노 게이지)
    public void UpdatePlayerStaminaUI()
    {
        PlayerStaminaUI.fillAmount = currentStamina / maxStamina;
    }

    public void UpdatePlayerUltUI()
    {
        PlayerUltUI.fillAmount = currentUltAmount / maxUltAmount;
    }
    #endregion

    #region 플레이어 효과음(오디오 재생)
    // 점프
    public void PlayJumpSound()
    {
        int randomIndex_Jump = Random.Range(0, 2);
        AudioClip selectedClip = randomIndex_Jump == 0 ? Jump_1Clip : Jump_2Clip;
        audioSource.PlayOneShot(selectedClip);
    }
    // 바운드 오브젝트랑 충돌 시 
    public void HitSound()
    {
        audioSource.clip = HitClip;
        audioSource.Play();
    }
    // 궁극기 사용
    public void PlayUltSound()
    {
        audioSource.clip = UltClip;
        audioSource.Play();
    }
    // 게임오버 
    public void PlayGameOverSound()
    {
        int randomIndex_GameOver = Random.Range(0, 2);
        AudioClip selectedClip = randomIndex_GameOver == 0 ? GameOver_1Clip : GameOver_2Clip;
        audioSource.PlayOneShot(selectedClip);
    }
    // 게임승리 
    public void PlayVictorySound()
    {
        audioSource.clip = VictroyClip;
        audioSource.Play();
    }
    #endregion

    #region 파티클 생성(대쉬/분노/스턴)
    public void StartDashEffect()
    {
        if (speedLinesParticleSystem != null)
        {
            Debug.Log("대쉬 파티클이 생성됩니다.");
            speedLinesParticleSystem.Play();
        }
    }

    public void StopDashEffect()
    {
        if (speedLinesParticleSystem != null)
        {
            Debug.Log("대쉬 파티클이 사라집니다.");
            speedLinesParticleSystem.Stop();
        }
    }

    public void StartUltEffect()
    {
        if (UltParticleSystem != null)
        {
            Debug.Log("궁극기 파티클이 생성됩니다.");
            UltParticleSystem.Play();
        }
    }

    public void StopUltEffect()
    {
        if (UltParticleSystem != null)
        {
            Debug.Log("궁극기 파티클이 사라집니다.");
            UltParticleSystem.Stop();
        }
    }

    IEnumerator StunEffectCoroutine()
    {
        StartStunEffect();
        yield return new WaitForSeconds(2f);
        StopStunEffect();
    }

    public void StartStunEffect()
    {
        if (StunParticleSystem != null)
        {
            Debug.Log("스턴 파티클이 생성됩니다.");
            StunParticleSystem.Play();
        }
    }

    public void StopStunEffect()
    {
        if (StunParticleSystem != null)
        {
            Debug.Log("궁극기 파티클이 사라집니다.");
            StunParticleSystem.Stop();
        }
    }
    #endregion
}
