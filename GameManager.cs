using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Transform> _wave2 = new List<Transform>();
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    [SerializeField] private GameObject[] _targetPrefab;
    [SerializeField] private int _countPerPrefab = 5;

    [SerializeField] private Vector2 _targetSpawnDelayRange;
    [SerializeField] private Vector2Int _bombSpawnDelayRange;
    [SerializeField] private Vector2 _targeSpawnWaveRange, _targeSpawnWave2Range;
    [SerializeField] private int _maxTargetSpawnRange, _maxBombSpawnRange;
    [SerializeField] public Transform _objectPooler;
    [SerializeField] private int _spawnCount = 0;
    [SerializeField] private float _maxPlayTime = 50f, _maxPlayTimeIncreaseValue;
    [field: SerializeField] public int RenderQueue { get; set; }
    public float TargetScale = 1.3f;
    [SerializeField] private bool _isTargetSpawnRangeXDecreasing;
    [SerializeField] private bool _isBombSpawnRangeXDecreasing;
    [SerializeField] private int _totalTargetSpawn, _totalBombSpawn;
    public int TotalTargetSpawn {get { return _totalTargetSpawn; } }

    [SerializeField] private int _maxTotalTargetSpawn, _maxTotalBombSpawn;
    [SerializeField] private int _life;
    [field : SerializeField]public bool IsGameOver { get; set; }
    [field: SerializeField] public bool IsAdPlayed { get; set; }
    [SerializeField] private float _time = 0;

    private void OnValidate()
    {
        _maxTargetSpawnRange = Mathf.Max(1, _maxTargetSpawnRange);
        _maxBombSpawnRange = Mathf.Max(1, _maxBombSpawnRange);
    }
    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        
        ScoreManager.Instance.ResetScore();
        StartCoroutine(UiManager.Instance.ActionUI("Ready", Color.green));
        _maxPlayTimeIncreaseValue = _maxPlayTime;
        CreateTarget();
        AudioManager.Instance.AudioSource.Stop();
        StartCoroutine(Init(1.5f));
    }
    private IEnumerator Init(float delay)
    {
        IsGameOver = false;
        yield return new WaitForSeconds(delay);
        StartCoroutine(TargetPool());
        StartCoroutine(BombPool());
        StartCoroutine(TargetWave());
        StartCoroutine(TargetWave2());
    }
    private IEnumerator TargetPool()
    {

        while (!IsGameOver)
        {
            int maxSpawnTarget = Random.Range(1, (_maxTargetSpawnRange + 1));
            int id = Random.Range(2, System.Enum.GetValues(typeof(TargetType)).Length);

            foreach (Transform child in _objectPooler)
            {

                if (_spawnCount >= maxSpawnTarget)
                {
                    _spawnCount = 0;
                    break;
                }

                if (!child.gameObject.activeInHierarchy)
                {
                    Target target = child.GetComponent<Target>();
                    TargetType typeToCheck = (TargetType)id;

                    if (target.Type == typeToCheck)
                    {
                        child.gameObject.SetActive(true);
                        yield return new WaitForSeconds(0.3f);
                        _spawnCount++;
                        _totalTargetSpawn++;
                        //Debug.Log(maxSpawnTarget+", "+ _spawnCount);
                    }
                }
            }
            float delay;
            delay = Random.Range(_targetSpawnDelayRange.x, _targetSpawnDelayRange.y);

            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator BombPool()
    {
        yield return new WaitForSeconds(_bombSpawnDelayRange.x);
        while (!IsGameOver)
        {
            int maxSpawnTarget = Random.Range(1, (_maxBombSpawnRange + 1));
            int id = Random.Range(0, 1);

            foreach (Transform child in _objectPooler)
            {

                if (_spawnCount >= maxSpawnTarget)
                {
                    _spawnCount = 0;
                    break;
                }

                if (!child.gameObject.activeInHierarchy)
                {
                    Target target = child.GetComponent<Target>();
                    TargetType typeToCheck = (TargetType)id;

                    if (target.Type == typeToCheck)
                    {
                        child.gameObject.SetActive(true);
                        yield return new WaitForSeconds(0.3f);
                        _spawnCount++;
                        _totalBombSpawn++;
                        //Debug.Log(maxSpawnTarget + ", " + _spawnCount);
                    }
                }
            }
            float delay;

            delay = Random.Range(_bombSpawnDelayRange.x, (_bombSpawnDelayRange.y + 1));

            yield return new WaitForSeconds(delay);
        }
    }
    private void CreateTarget()
    {
        for (int i = 0; i < _countPerPrefab; i++)
        {
            foreach (GameObject target in _targetPrefab)

            {
                GameObject obj = Instantiate(target, _objectPooler);
                obj.transform.localScale = Vector3.one * TargetScale;
                obj.SetActive(false);
            }
        }
    }

    private IEnumerator TargetWave()
    {
        while (!IsGameOver)
        {
            float delay;

            delay = Random.Range(_targeSpawnWaveRange.x, (_targeSpawnWaveRange.y));

            yield return new WaitForSeconds(delay);

            for (int i = 0; i< _targetPrefab.Length-2;i++)
            {
                if (!IsGameOver)
                {
                    GameObject target = Instantiate(_targetPrefab[i], transform.position, Quaternion.identity);
                    target.transform.localScale = Vector3.one * TargetScale;
                    _totalTargetSpawn++;

                    Destroy(target, 10f);
                    yield return new WaitForSeconds(0.3f);
                }
            }

        }
    }

    private IEnumerator TargetWave2()
    {

        while (!IsGameOver)
        {
            float delay;

            delay = Random.Range(_targeSpawnWave2Range.x, (_targeSpawnWave2Range.y));

            yield return new WaitForSeconds(delay);

            int count = Random.Range(_objectPooler.childCount - 16, _objectPooler.childCount - 7);
            //int i = 0;

            for (int i = _objectPooler.childCount-1; i > count; i--)
            {
                GameObject child = _objectPooler.GetChild(i).gameObject;
               // Debug.Log(count + ", " +  i);
                if (IsGameOver)
                {
                    break;
                }
                if (child.GetComponent<Target>().Type != TargetType.Bomb && child.GetComponent<Target>().Type != TargetType.Bomb2)
                {
                    if (!child.activeInHierarchy)
                    {

                        child.SetActive(true);
                        _totalTargetSpawn++;

                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
        }
    }
    public void GameDifficulty()
    {
        if (_time > _maxPlayTime)
        {
            if (_targetSpawnDelayRange.y > _targetSpawnDelayRange.x && !_isTargetSpawnRangeXDecreasing)
            {
                _targetSpawnDelayRange.y -= 0.20f;
            }
            else
            {
                _isTargetSpawnRangeXDecreasing = true;

                if (_targetSpawnDelayRange.x > 0)
                {
                    _targetSpawnDelayRange.x -= 0.20f;
                }
            }

            if (_bombSpawnDelayRange.y > _bombSpawnDelayRange.x && !_isBombSpawnRangeXDecreasing)
            {
                _bombSpawnDelayRange.y--;
            }
            else
            {
                _isBombSpawnRangeXDecreasing = true;

                if (_bombSpawnDelayRange.x > 0)
                {
                    _bombSpawnDelayRange.x--;
                }
            }
            _maxPlayTime += _maxPlayTimeIncreaseValue;

            if (_totalTargetSpawn > _maxTotalTargetSpawn)
            {
                _maxTotalTargetSpawn += _maxTotalTargetSpawn;

                if (_maxTargetSpawnRange <= _countPerPrefab)
                {
                    _maxTargetSpawnRange++;
                }
            }

            if (_totalBombSpawn > _maxTotalBombSpawn)
            {
                _maxTotalBombSpawn += _maxTotalBombSpawn;
                if (_maxBombSpawnRange <= _countPerPrefab)
                {
                    _maxBombSpawnRange++;
                }
            }
        }
    }
    public void LifeDeduct()
    {
        if (!IsGameOver)
        {
            _life--;
            UiManager.Instance.LifeUIUpdate(_life);

            if (_life < 1)
            {
                GameOver();
            }
        }

    }
    public void GameOver()
    {
        IsGameOver = true;
        UiManager.Instance.LifeUIUpdate(0);

        StartCoroutine(GameOverProcess());
    }

    private IEnumerator GameOverProcess()
    {
        StartCoroutine(UiManager.Instance.ActionUI("Game Over", Color.red));

        yield return new WaitForSeconds(1.5f);

        if (!IsAdPlayed)
        {
            IsAdPlayed = true;
            UiManager.Instance.ActiveAdPanel();
        }
        else
        {
            UiManager.Instance.GameOverUI();
        }
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GameDifficulty();
        if (!IsGameOver && Input.GetKey(KeyCode.Escape))
        {
            UiManager.Instance.PauseBtnClick();
        }
    }

    public void RevivePlayer()
    {
        UiManager.Instance.DeActiveAdPanel();
        _life = 1;
        UiManager.Instance.LifeUIUpdate(_life);
        StartCoroutine(Init(0.5f));
    }
}