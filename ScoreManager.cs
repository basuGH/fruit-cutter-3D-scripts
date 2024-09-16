using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }
    [SerializeField] private int _score;
    [SerializeField] private static int _best;
    [SerializeField] private int _combo, _comboCount;
    [SerializeField] private bool _isCombo;
    [SerializeField] private float _comboDeactivationTime, _comboDefaultValue;
    [SerializeField] private Vector3 _lastComboPos;
    [SerializeField] private static int _currentJuice, _totalJuice;
    public int TotalJuice { get { return _totalJuice; } set { _totalJuice = value; } }
    [SerializeField] private int _currentEarnExp;
    private static int _currentLevelExp, _targetExp = 150, _level = 1;//data store
    public int Level { get { return _level; } }
    [field: SerializeField] private static int _totalExp;
    public int TotalExp { get { return _totalExp; } }

   // [SerializeField] private int _currentLevelExp; ////data store
    public static int BombDefuseCount, FruitsPowerUpCount;
    public bool IsBombDefuseActive { get; set; }
    public bool IsFruitsPowerUpActive { get; set; }


    private void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
       // Debug.Log("Score Start method");
        _comboDefaultValue = _comboDeactivationTime;
        LoadDataFromServer();
        //DebugLogScore();
    }
    public void DebugLogScore()
    {
        Debug.Log("Level :"+_level + ", Total Exp :" + _totalExp + ", CurrentLevelExp : "+ _currentLevelExp+", Target Exp : "+_targetExp);
    }
    public void ResetScore()
    {
        _score = 0;
        _comboCount = 0;

        UiManager.Instance.ScoreTextUpdate(_score, _best);

    }
    public void ScoreUpdate(int score)
    {
        _score += score;

        if (_score >= _best)
        {
            _best = _score;
        }
        UiManager.Instance.ScoreTextUpdate(_score, _best);
    }
    public void ComboActivation(Vector3 pos)
    {
        if (!_isCombo)
        {
            _isCombo = true;
            StartCoroutine(ComboDeactivation());

        }

        if (_isCombo)
        {

            _combo++;
            _lastComboPos = pos;

            if (_combo > 2)
            {
                _comboDeactivationTime += 0.05f;
            }
        }
    }
    private IEnumerator ComboDeactivation()
    {
        // Debug.Log("Before While  : " + Time.time + ", _comb Deactivation Time : " + _comboDeactivationTime);
        float elapsedTime = 0;
        while (elapsedTime < _comboDeactivationTime)
        {
            // Debug.Log("inside While  : " + Time.time + ", _comb Deactivation Time : " + _comboDeactivationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Debug.Log("end While  : " + Time.time + ", _comb Deactivation Time : " + _comboDeactivationTime);

        _isCombo = false;
        if (_combo > 2)
        {
            _comboCount++;
            UiManager.Instance.ShowCombo(_lastComboPos, _combo);
            ScoreUpdate(_combo);
            AudioManager.Instance.ComboAudio();
        }
        _combo = 0;
        _comboDeactivationTime = _comboDefaultValue;
    }
    public IEnumerator UpdateJuice()
    {
        int juice = 0;
        _currentJuice = Mathf.RoundToInt((_score / 25) + (_comboCount / 5));

        while (juice < _currentJuice)
        {
            juice++;
            UiManager.Instance.JuiceUiUpdate(juice);
            yield return new WaitForSeconds(0.2f);
        }
        _totalJuice += _currentJuice;
    }
    public IEnumerator UpdateExp()
    {
        UiManager.Instance.ButtonSet.SetActive(false);
        int xp = 0;
        float progressBarValue;
        _currentEarnExp = _score + _comboCount * _level + Mathf.Max(1, (_score - GameManager.Instance.TotalTargetSpawn)) * _level;
        // _totalExp += _currentEarnExp;
        while (xp < _currentEarnExp)
        {
            xp++;

            _currentLevelExp++;
            _totalExp++;
            if (_currentLevelExp > _targetExp)
            {
                _level++;
                _currentLevelExp = 0;
                _targetExp += Mathf.RoundToInt(_level * _targetExp * 0.02f);
            }

            progressBarValue = (float)_currentLevelExp / _targetExp;
            //Debug.Log(_totalExp + "," + _targetExp + "," + progressBarValue);

            UiManager.Instance.ExpUIUpdate(xp, progressBarValue, _level);
            yield return null;
        }
        //Debug.Log("combo count" + _comboCount);
        SaveAllData();
        UiManager.Instance.ButtonSet.SetActive(true);
    }
    public void BombDefusePurchase()
    {
        if (_totalJuice > 110)
        {
            BombDefuseCount++;
            _totalJuice -= 110;
        }
    }
    public void FruitsPowerUpPurchase()
    {
        if (_totalJuice > 75)
        {
            FruitsPowerUpCount++;
            _totalJuice -= 75;
        }
    }
    public IEnumerator BombDefuseActive()
    {
        if (BombDefuseCount > 0 && !GameManager.Instance.IsGameOver)
        {
            IsBombDefuseActive = true;
            UiManager.Instance.BombDefuse.interactable = false;
            BombDefuseCount--;
            UiManager.Instance.BombDefuseCountText.text = BombDefuseCount.ToString();

            yield return new WaitForSeconds(7);

            IsBombDefuseActive = false;
            UiManager.Instance.BombDefuse.interactable = true;
        }
    }
    public IEnumerator FruitsPowerUpActive()
    {
        if (FruitsPowerUpCount > 0 && !GameManager.Instance.IsGameOver)
        {
            IsFruitsPowerUpActive = true;
            UiManager.Instance.FruitsPowerUp.interactable = false;
            FruitsPowerUpCount--;
            UiManager.Instance.FruitPowerUpCountText.text = FruitsPowerUpCount.ToString();

            yield return new WaitForSeconds(7);

            IsFruitsPowerUpActive = false;
            UiManager.Instance.FruitsPowerUp.interactable = true;
        }
    }
    public void LoadDataFromServer()
    {
        LoadGameplayData();
        LoadJuiceData();
        LoadPowerupData();
    }
    public void SaveAllData()
    {
        SaveGameplayData();
        SaveJuiceData();
        SavePowerupData();
    }

    private async void SaveGameplayData()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }

        try
        {
            var gameplayData = new Dictionary<string, object>() {

            { "Level", _level },
            { "TotalExp", _totalExp },
            { "TargetExp", _targetExp },
            { "CurrentLevelExp", _currentLevelExp },
            { "Best", _best }

        };
           // Debug.Log("Before Save");

            await CloudSaveService.Instance.Data.Player.SaveAsync(gameplayData);
            //Debug.Log($"Saved data {string.Join(',', gameplayData)}");
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
            }
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }


        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }

    }
    private async void LoadGameplayData()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }

        try
        {
            var gameplayData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {

            { "Level" },
            { "TotalExp" },
            { "TargetExp" },
            { "CurrentLevelExp" },
            { "Best" }
            });

            if (gameplayData.TryGetValue("Level", out var level))
            {
                //Debug.Log("level " + level);
                _level = level.Value.GetAs<int>();
            }
            if (gameplayData.TryGetValue("TotalExp", out var totalExp))
            {
                _totalExp = totalExp.Value.GetAs<int>();
            }
            if (gameplayData.TryGetValue("TargetExp", out var targetExp))
            {
                _targetExp = targetExp.Value.GetAs<int>();
            }

            if (gameplayData.TryGetValue("CurrentLevelExp", out var currentLevelExp))
            {
                _currentLevelExp = currentLevelExp.Value.GetAs<int>();
            }

            if (gameplayData.TryGetValue("Best", out var best))
            {
                _best = best.Value.GetAs<int>();
            }

            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
                MainMenu.Instance.UpdateUI();
            }
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }
    }

    public async void SaveJuiceData()
    {

        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }

        try
        {
            var juiceData = new Dictionary<string, object>() {

            { "Juice", _totalJuice }
        };

            await CloudSaveService.Instance.Data.Player.SaveAsync(juiceData);
           // Debug.Log($"Saved data {string.Join(',', juiceData)}");

            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
                MainMenu.Instance.UpdateUI();
            }
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }


        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }

    }
    private async void LoadJuiceData()
    {

        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }

        try
        {
            var juiceData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {

            { "Juice" },

        });

            if (juiceData.TryGetValue("Juice", out var juice))
            {
               // Debug.Log("juice " + juice.ToString());
                _totalJuice = juice.Value.GetAs<int>();
            }

            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
                MainMenu.Instance.UpdateUI();
            }

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }


        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }


    }
    public async void SavePowerupData()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Please wait while we upload data to the server...");
        }

        try
        {

            var powerup = new Dictionary<string, object>() {

              { "BombDefuseCount", BombDefuseCount },
              { "FruitsPowerUpCount", FruitsPowerUpCount }

            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(powerup);
           // Debug.Log($"Saved data {string.Join(',', powerup)}");

            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
                MainMenu.Instance.UpdateUI();
            }

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }


        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }


    }
    private async void LoadPowerupData()
    {
        if (UiManager.Instance != null)
        {
            UiManager.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.ActiveNetworkPanel("Fetching data from the server. This may take a moment, please wait...");
        }

        try
        {

            var powerupData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {


            { "BombDefuseCount" },
            { "FruitsPowerUpCount" }

        });

            if (powerupData.TryGetValue("BombDefuseCount", out var bombDefuseCount))
            {
                BombDefuseCount = bombDefuseCount.Value.GetAs<int>();
            }

            if (powerupData.TryGetValue("FruitsPowerUpCount", out var fruitsPowerUpCount))
            {
                FruitsPowerUpCount = fruitsPowerUpCount.Value.GetAs<int>();
            }
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkErrorPanel.SetActive(false);
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkErrorPanel.SetActive(false);
                MainMenu.Instance.UpdateUI();
            }

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.NetworkError();
                UiManager.Instance.EnableNetworkRetryBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.NetworkError();
                MainMenu.Instance.EnableNetworkRetryBtn();
            }
        }
    }

    public void GenerateReward()
    {
        int count = Random.Range(2, 10);
        MainMenu.Instance.RewardUI(count);
    }

    public void ResetAllData()
    {
        _level = 1;
        _totalExp = 0;
        _targetExp = 150;
        _currentLevelExp = 0;
        _best = 0;
        _totalJuice = 0;
        BombDefuseCount = 0;
        FruitsPowerUpCount = 0;
    }
}