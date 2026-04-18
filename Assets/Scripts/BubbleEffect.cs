using UnityEngine;
using System.Collections;

public class BubbleEffect : MonoBehaviour
{
    public SpriteRenderer bubbleRenderer;
    public Sprite bubbleSprite;
    public Sprite[] popFrames;
    public float popFPS = 12f;
    [Range(0f, 1f)]
    public float bubbleAlpha = 0.5f;

    private Coroutine _popCoroutine;

    void Start()
    {
        if (bubbleRenderer != null) bubbleRenderer.enabled = false;
    }

    public void ShowBubble()
    {
        if (_popCoroutine != null) StopCoroutine(_popCoroutine);
        bubbleRenderer.sprite = bubbleSprite;
        Color c = bubbleRenderer.color;
        c.a = bubbleAlpha;
        bubbleRenderer.color = c;
        bubbleRenderer.enabled = true;
    }

    public void PopBubble()
    {
        if (_popCoroutine != null) StopCoroutine(_popCoroutine);
        _popCoroutine = StartCoroutine(PlayPopRoutine());
    }

    private IEnumerator PlayPopRoutine()
    {
        Color c = bubbleRenderer.color;
        c.a = 1f;
        bubbleRenderer.color = c;

        float delay = 1f / popFPS;
        foreach (Sprite frame in popFrames)
        {
            bubbleRenderer.sprite = frame;
            yield return new WaitForSeconds(delay);
        }

        bubbleRenderer.enabled = false;
        _popCoroutine = null;
    }
}
