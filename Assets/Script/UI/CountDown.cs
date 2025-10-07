using UnityEngine;
using TMPro;
using System.Collections;

public class CountDown : MonoBehaviour
{
    #region 내부 변수 
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI newText;
    public float CountTime = 3f;
    // 애니메이션
    public float animationDuration = 0.3f;
    public float moveDistance = 50f;
    public float delayBetweenCounts = 1.0f;
    #endregion

    void Start()
    {
        StartCoroutine(StartTheCountdown());
    }

    IEnumerator StartTheCountdown()
    {
        if (GameManager.currentState == GameState.Countdown)
        {
            // 3표시
            yield return StartCoroutine(AnimateTextChange("3"));
            yield return new WaitForSeconds(delayBetweenCounts);

            // 2표시
            yield return StartCoroutine(AnimateTextChange("2"));
            yield return new WaitForSeconds(delayBetweenCounts);

            // 1표시
            yield return StartCoroutine(AnimateTextChange("1"));
            yield return new WaitForSeconds(delayBetweenCounts);

            // start표시
            yield return StartCoroutine(AnimateTextChange("START!"));
            yield return new WaitForSeconds(delayBetweenCounts);

            // Start표시 이후 화면 내 텍스트 비활성화
            currentText.gameObject.SetActive(false);
            newText.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateTextChange(string newString)
    {
        // 새로운 숫자 넣어주기, 위에서 내려오기 위한 설정
        newText.text = newString;
        RectTransform newRect = newText.rectTransform;
        newRect.localPosition = new Vector3(0, moveDistance, 0);

        // currenText Alpha값 1로 리셋
        SetTextAlpha(currentText, 1f);

        float elapsedTime = 0f;

        // 애니메이션 실행
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;

            // current 점점 투명하게 
            float newAlpha = Mathf.Lerp(1f, 0f, t);
            SetTextAlpha(currentText, newAlpha);

            // newText 위에서 내려오는 연출
            newRect.localPosition = Vector3.Lerp(new Vector3(0, moveDistance, 0), Vector3.zero, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentText.text = newString;
        currentText.rectTransform.localPosition = Vector3.zero;

        // 숫자 다시 보이도록 Alpha값 1로 조절
        SetTextAlpha(currentText, 1f);

        newText.text = "";
    }

    void SetTextAlpha(TextMeshProUGUI textMeshPro, float alpha)
    {
        // 기존 색상 정보 
        Color newColor = textMeshPro.color;
        // Alpha값 변경
        newColor.a = alpha;
        // 변경된 색상 정보 다시 적용
        textMeshPro.color = newColor;
    }
}
