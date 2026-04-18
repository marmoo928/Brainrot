using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 12f;

    private SpriteRenderer _renderer;
    private float _timer;
    private int _currentFrame;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        _timer += Time.deltaTime;
        if (_timer >= 1f / fps)
        {
            _timer = 0f;
            _currentFrame = (_currentFrame + 1) % frames.Length;
            _renderer.sprite = frames[_currentFrame];
        }
    }
}
