using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapProgress : MonoBehaviour
{
    [Header("내부 변수")]
    public Slider progressSlider;
    public Transform player;
    public List<Transform> Enemies = new List<Transform>();
    public RectTransform playerIcon;
    public List<RectTransform> enemyIcons = new List<RectTransform>();
    // 맵 시작, 끝 지점
    public Transform startPoint;
    public Transform endPoint;
    public bool clampIconsToSlider = true;
    public float smoothSpeed = 10f;
    private float mapTotalLength;
    private RectTransform sliderRect;

    void Start()
    {
        if (startPoint == null || endPoint == null) return;
        if (progressSlider == null)
        {
            progressSlider = GetComponent<Slider>();
        }

        sliderRect = progressSlider.GetComponent<RectTransform>();
        CalculateMapLength();
        SetupSlider();
    }

    void Update()
    {
        UpdateAllPositions();
    }

    #region 맵 길이 측정
    void CalculateMapLength()
    {
        mapTotalLength = Mathf.Abs(endPoint.position.z - startPoint.position.z);
        if (mapTotalLength <= 0)
        {
            Debug.LogWarning("맵 길이가 0 또는 음수입니다. 기본값 100으로 설정합니다.");
            mapTotalLength = 1f;
        }
    }
    #endregion

    #region Slider 세팅
    void SetupSlider()
    {
        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;
        if (player != null)
        {
            progressSlider.value = CaculateZProgress(player.position.z);
        }
    }
    #endregion

    #region 모든 위치 업데이트
    void UpdateAllPositions()
    {
        if (player != null)
        {
            float playerProgress = CaculateZProgress(player.position.z);
            // 슬라이더의 값을 즉시 변경하는 대신, 목표 값(playerProgress)을 향해 부드럽게 이동
            progressSlider.value = Mathf.Lerp(progressSlider.value, playerProgress, smoothSpeed * Time.deltaTime);
            UpdateIconPosition(playerIcon, playerProgress);
        }

        for (int i = 0; i < Mathf.Min(Enemies.Count, enemyIcons.Count); i++)
        {
            if (Enemies[i] != null && enemyIcons[i] != null)
            {
                float objProgress = CaculateZProgress(Enemies[i].position.z);
                UpdateIconPosition(enemyIcons[i], objProgress);
                enemyIcons[i].gameObject.SetActive(objProgress >= 0f && objProgress <= 1f);
            }
        }
    }
    #endregion

    #region Z계산
    public float CaculateZProgress(float zPosition)
    {
        // 맵 시작점을 기준으로 얼마나 진행했는지 계산하여 0과 1사이의 값으로 반화
        return Mathf.Clamp01((zPosition - startPoint.position.z) / mapTotalLength);
    }
    #endregion

    #region 아이콘 업데이트
    void UpdateIconPosition(RectTransform icon, float progress)
    {
        if (icon == null || sliderRect == null) return;

        float sliderWidth = sliderRect.rect.width;
        float iconX;

        if (clampIconsToSlider)
        {
            float padding = icon.rect.width / 2;
            iconX = Mathf.Lerp(padding, sliderWidth - padding, progress);
        }
        else
        {
            iconX = Mathf.Lerp(0f, sliderWidth, progress);
        }

        Vector2 targetPosition = new Vector2(iconX, icon.anchoredPosition.y);
        icon.anchoredPosition = Vector2.Lerp(icon.anchoredPosition, targetPosition, smoothSpeed * Time.deltaTime);
    }
    #endregion

}
