using UnityEngine;

public class Spin : MonoBehaviour
{
    public float spinSpeed = 360f;

    private float _spinAngle = 0f;
    private ObstacleMover _mover;

    void Start()
    {
        _mover = GetComponent<ObstacleMover>();
    }

    void LateUpdate()
    {
        _spinAngle += spinSpeed * Time.deltaTime;
        float gravAngle = _mover != null
            ? Vector2.SignedAngle(Vector2.down, _mover.gravityDir)
            : Vector2.SignedAngle(Vector2.down, Physics2D.gravity.normalized);
        transform.rotation = Quaternion.Euler(0f, 0f, gravAngle + _spinAngle);
    }
}
