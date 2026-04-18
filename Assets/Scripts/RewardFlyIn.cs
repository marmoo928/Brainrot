using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardFlyIn : MonoBehaviour
{
    [Tooltip("Odkial priletí (lokálna pozícia v Canvase).")]
    public Vector2 launchPosition = new Vector2(-600f, 0f);

    [Tooltip("Ako dlho trvá animácia (sekundy).")]
    public float duration = 0.8f;

    [Tooltip("Krivka pohybu (nastav na EaseOut pre prirodzený dolet).")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Začne so scale 0 a vyrastie na 1.")]
    public bool scaleIn = true;

    private RectTransform _rect;
    private Image _image;
    private Vector2 _targetPosition;
    private Vector3 _originalScale;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _targetPosition = _rect.anchoredPosition;
        _originalScale = _rect.localScale;
        if (_image != null) _image.enabled = false;
    }

    public void PlayFlyIn(Sprite sprite)
    {
        if (_image != null)
        {
            _image.sprite = sprite;
            _image.enabled = true;
        }
        StopAllCoroutines();
        StartCoroutine(FlyInRoutine());
    }

    private IEnumerator FlyInRoutine()
    {
        float elapsed = 0f;
        _rect.anchoredPosition = launchPosition;
        if (scaleIn) _rect.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(Mathf.Clamp01(elapsed / duration));
            _rect.anchoredPosition = Vector2.Lerp(launchPosition, _targetPosition, t);
            if (scaleIn) _rect.localScale = Vector3.Lerp(Vector3.zero, _originalScale, t);
            yield return null;
        }

        _rect.anchoredPosition = _targetPosition;
        if (scaleIn) _rect.localScale = _originalScale;
    }
}
