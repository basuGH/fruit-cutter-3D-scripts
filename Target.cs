using UnityEngine;
using UnityEngine.Profiling;

public class Target : MonoBehaviour
{
    [SerializeField] private float _torqueRange;
    [SerializeField] private Vector2 _forceRange;
    [SerializeField] private float _xRangePos;
    [SerializeField] private float _yPos;
    [SerializeField] private float _rotationAngleRange;
    [SerializeField] private TargetType _targetType;
    public TargetType Type { get { return _targetType; } }
    public bool IsSpawned { get; set; }

    private Rigidbody _rb;
    private bool _isDisable;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        Profiler.BeginSample("Target Start");

        float zAngle = Random.Range(-_rotationAngleRange, _rotationAngleRange);
        _rb.rotation = Quaternion.Euler(0f, 0f, zAngle);
        SpawnPos(_xRangePos, zAngle);
        _rb.AddRelativeForce(Vector3.up * RandomForce(), ForceMode.Impulse);
        _rb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        RenderQueue();
        if (_isDisable)
        {
            AudioManager.Instance.ThrowAudio();
        }

        Profiler.EndSample();
    }
    private void Start()
    {

    }
    private float RandomTorque()
    {
        return Random.Range(-_torqueRange, _torqueRange);
    }
    private float RandomForce()
    {
        return Random.Range(_forceRange.x, _forceRange.y);
    }
    private void SpawnPos(float xPosRange, float zAngel)
    {
        float min_X = -xPosRange;
        float max_X = xPosRange;
        if (zAngel >= -0.01)
        {
            min_X = -1;
        }
        else if ( zAngel <= 0.1)
        {
            max_X = 1;
        }

        _rb.position = new Vector3(Random.Range(min_X, max_X), _yPos, 0);
    }
    private void RenderQueue()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                material.renderQueue += GameManager.Instance.RenderQueue;
            }
        }

        GameManager.Instance.RenderQueue++;

        if (GameManager.Instance.RenderQueue >= 50)
        {
            GameManager.Instance.RenderQueue = 0;
        }
    }
    private void OnDisable()
    {
        _isDisable = true;
       IsSpawned = false;
        _rb.velocity = Vector3.zero;
    }
}
public enum TargetType
{
    Bomb,
    Bomb2,
    Grapefruit,
    Lemon,
    lime,
    Watermelon,
    Orange,
    Papaya
}
