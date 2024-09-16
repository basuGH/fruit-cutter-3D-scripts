using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{

    private static UiManager _instance;
    public static UiManager Instance { get { return _instance; } }
    [SerializeField] private TextMeshProUGUI _scoreText, _bestText;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Sprite[] _lives;
    [SerializeField] private Image _lifeImage;
    [SerializeField] private GameObject _gamePlayPanel, _adPanel, _gameOverPanel, _resumePanel;
    [SerializeField] private TextMeshProUGUI _juiceText, _expText, _levelText;
    [SerializeField] private TextMeshProUGUI _scoreText_gameOver, _bestText_gameOver;
    [SerializeField] private Image _expProgressBar;
    public GameObject ButtonSet;
    public Button FruitsPowerUp, BombDefuse;
    public TextMeshProUGUI FruitPowerUpCountText, BombDefuseCountText;
    [SerializeField] private TextMeshProUGUI _actionText;

    public GameObject NetworkErrorPanel;
    public TextMeshProUGUI ErrorText;
    public Button NetworkRetryBtn;

    [SerializeField] private Button _adButton;



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        RewardedAdsButton.Instance.SetAdButton(_adButton);
        UpdatePowerUpUI();
        _gameOverPanel.SetActive(false);
        _adPanel.SetActive(false);
        _resumePanel.SetActive(false);
        _gamePlayPanel.SetActive(true);
        ButtonSet.SetActive(false);
        NetworkErrorPanel.SetActive(false);
    }
    public void ScoreTextUpdate(int score, int best)
    {


        _scoreText.text = score.ToString();
        _scoreText_gameOver.text = score.ToString();

        _bestText.text = "BEST " + best.ToString();
        _bestText_gameOver.text = "BEST " + best.ToString();

    }
    public void ShowCombo(Vector3 Pos, int comboPoint)
    {
        TextMeshProUGUI comboText = Instantiate(_comboText, _canvas.transform);
        comboText.transform.position = Camera.main.WorldToScreenPoint(Pos);
        comboText.text = "COMBO\n" + "+" + comboPoint.ToString();
        Destroy(comboText.gameObject, 2f);
    }
    public void LifeUIUpdate(int life)
    {
        _lifeImage.sprite = _lives[life];
    }
    public void DeActiveAdPanel()
    {
        _adPanel.SetActive(false);
    }
    public void ActiveAdPanel()
    {
        _adPanel.SetActive(true);
    }
    public void GameOverUI() // SkipBtnClick
    {
        _gameOverPanel.SetActive(true);
        _adPanel.SetActive(false);
        _gamePlayPanel.SetActive(false);

        StartCoroutine(ScoreManager.Instance.UpdateJuice());
        StartCoroutine(ScoreManager.Instance.UpdateExp());

        AudioManager.Instance.PlayBackgroundMusic();

    }

    public void ExpUIUpdate(int currentExp, float progressBarValue, int level)
    {
        _expText.text = "+" + currentExp.ToString();
        _expProgressBar.fillAmount = progressBarValue;
        _levelText.text = "Level " + level.ToString();
    }
    public void JuiceUiUpdate(int juice)
    {
        _juiceText.text = "+" + juice.ToString();
    }


    public void RetryBtnClick()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenuBtnClick()
    {
        //play ad
        SceneManager.LoadScene(1);
    }
    public void BombDefuseBtnClick()
    {
        StartCoroutine(ScoreManager.Instance.BombDefuseActive());
    }
    public void FruitsPowerBtnClick()
    {
        StartCoroutine(ScoreManager.Instance.FruitsPowerUpActive());
    }
    public void PauseBtnClick()
    {
        _resumePanel.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeBtnClick()
    {
        Time.timeScale = 1;
        _resumePanel.SetActive(false);
    }
    public void UpdatePowerUpUI()
    {
        FruitPowerUpCountText.text = ScoreManager.FruitsPowerUpCount.ToString();
        BombDefuseCountText.text = ScoreManager.BombDefuseCount.ToString();

    }
    public IEnumerator ActionUI(string text, Color color)
    {
        _actionText.gameObject.SetActive(true);
        _actionText.text = text;
        _actionText.color = color;
        yield return new WaitForSeconds(1f);

        _actionText.gameObject.SetActive(false);

    }

    public void ActiveNetworkPanel(string msg)
    {
        //Debug.Log("Active Network panel");
        NetworkErrorPanel.SetActive(true);
        NetworkErrorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        ErrorText.text = msg;
        //"Fetching data from the server. This may take a moment, please wait...";
        //"Please wait while we upload data to the server..."
        NetworkRetryBtn.gameObject.SetActive(false);
    }
    public void NetworkError()
    {
        NetworkErrorPanel.SetActive(true);
        NetworkErrorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        ErrorText.text = "Unable to reach the server due to a network error.\n Please check your connection and try again.";

        NetworkRetryBtn.gameObject.SetActive(true);
    }
    public void NetworkRetryBtnClick()
    {
        NetworkRetryBtn.enabled = false;
        ScoreManager.Instance.SaveAllData();
    }
    public void EnableNetworkRetryBtn()
    {
        NetworkRetryBtn.enabled = true;

    }

}
