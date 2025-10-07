using UnityEngine;
using TMPro;

public class StopWatch : MonoBehaviour
{
    #region 내부 변수 
    public TextMeshProUGUI timeText;
    private bool useUnscaledTime = true;
    public bool IsRunning { get; private set; }
    private float elapsed;
    #endregion

    void Update()
    {
        if (GameManager.currentState == GameState.Playing)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            UpdateUI();
        }
    }

    public void ResetWatch()
    {
        elapsed = 0f;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (!timeText) return;

        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed) % 60;
        int centi = Mathf.FloorToInt((elapsed * 100f) % 100f);

        timeText.text = $"Time: {minutes:00}:{seconds:00}:{centi:00}";
    }
}
