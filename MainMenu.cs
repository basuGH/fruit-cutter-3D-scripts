using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static MainMenu _instance;
    public static MainMenu Instance { get { return _instance; } }

    [SerializeField] private GameObject _mainPanel, _settingPanel, _creditPanel, _storePanel, _playerInfoPanel;
    [SerializeField] private Slider _volumeSlider;
   //[SerializeField] private float _volume;
    [SerializeField] private TextMeshProUGUI _fruitsPowerUpCountText;
    [SerializeField] private TextMeshProUGUI _bombDefuseCountText;
    [SerializeField] private TextMeshProUGUI _juiceText_main, _juiceText_Store;
    [SerializeField] private TextMeshProUGUI _totalExpText, _levelText;
    [SerializeField] private TextMeshProUGUI _playerIdText;
    [SerializeField] private GameObject _linkedUI, _unlinkInfoUI, _linkButtonSet;
    [SerializeField] private TextMeshProUGUI _playerIDText, _loginSignInTypeText, _idText_PlayerInfo;

    [SerializeField] private Button _oldDataBtn, _currentDataBtn;
    [SerializeField] private TextMeshProUGUI _oldDataBtnText, _currentDataBtnText;

    public GameObject LinkWarningInfoPanel;
    public Button LinkBtn, UnlinkYesBtn,UnlinkBtnNo, PlayerInfoBackBtn, SignOutBtn;
    [SerializeField] private TextMeshProUGUI _linkBtnText, _unlinkYesBtnText, _statusText;

    public GameObject NetworkErrorPanel;
    public TextMeshProUGUI ErrorText;
    public Button NetworkRetryBtn;

    [SerializeField] private Image[] _rewardsPrefabImage;
    [SerializeField] private Button _adButton;
    [SerializeField] private TextMeshProUGUI _aapVersionText;
    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _aapVersionText.text = Application.version.ToString();
        _volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.75f);
       
        RewardedAdsButton.Instance.SetAdButton(_adButton);
        BackBtnClick();
        AudioManager.Instance.PlayBackgroundMusic();

        _playerIdText.text = "Player ID " + AuthManager.Instance.PlayerID;
        _idText_PlayerInfo.text = "ID " + AuthManager.Instance.PlayerID;
    }

    public void UpdateUI()
    {
        _juiceText_main.text = ScoreManager.Instance.TotalJuice.ToString();
        _juiceText_Store.text = ScoreManager.Instance.TotalJuice.ToString();

        _fruitsPowerUpCountText.text = ScoreManager.FruitsPowerUpCount.ToString();
        _bombDefuseCountText.text = ScoreManager.BombDefuseCount.ToString();

        _totalExpText.text = "XP " + ScoreManager.Instance.TotalExp.ToString();
        _levelText.text = "Level " + ScoreManager.Instance.Level.ToString();
       // ScoreManager.Instance.DebugLogScore();
    }
    public void BackBtnClick()
    {
        _mainPanel.SetActive(true);
        _storePanel.SetActive(false);
        _creditPanel.SetActive(false);
        _settingPanel.SetActive(false);
        _playerInfoPanel.SetActive(false);
        LinkWarningInfoPanel.SetActive(false);
        UpdateUI();
    }
    public void StoreBackBtnClick()
    {
        BackBtnClick();
        ScoreManager.Instance.SavePowerupData();
        ScoreManager.Instance.SaveJuiceData();
    }
    public void PlayBtnClick()
    {
        SceneManager.LoadScene(2);
    }
    public void QuitBtnClick()
    {
        Application.Quit();
    }
    public void SettingBtnClick()
    {
        _settingPanel.SetActive(true);
        _mainPanel.SetActive(false);
       // _volumeSlider.value = _volume;

    }
    public void CreditBtnClick()
    {
        Application.OpenURL("https://1drv.ms/b/s!AkjL3NnSW_CJiVDtfxE7HQgR-YP5?e=5GY5Ox");

    }
    public void VolumeSlider()
    {
        //_volume = _volumeSlider.value;
        AudioManager.Instance.AudioSource.volume = _volumeSlider.value;
        AudioManager.Instance.BladeAudio.volume = _volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", _volumeSlider.value);

    }
    public void StoreBtnClick()
    {
        _storePanel.SetActive(true);
        _mainPanel.SetActive(false);
    }
    public void BombDefuseBtnClick()
    {
        ScoreManager.Instance.BombDefusePurchase();
        _bombDefuseCountText.text = "You Have " + ScoreManager.BombDefuseCount.ToString();
        _juiceText_main.text = ScoreManager.Instance.TotalJuice.ToString();
        _juiceText_Store.text = ScoreManager.Instance.TotalJuice.ToString();
    }
    public void FruitsPowerUpClick()
    {
        ScoreManager.Instance.FruitsPowerUpPurchase();
        _fruitsPowerUpCountText.text = "You Have " + ScoreManager.FruitsPowerUpCount.ToString();
        _juiceText_main.text = ScoreManager.Instance.TotalJuice.ToString();
        _juiceText_Store.text = ScoreManager.Instance.TotalJuice.ToString();

    }
    public void PlayerInfoBtnClick()
    {
        _playerInfoPanel.SetActive(true);
        _mainPanel.SetActive(false);

        Debug.Log("Linked " + AuthManager.Instance.IsLinkedWithUnity(AuthenticationService.Instance.PlayerInfo));

        if (AuthManager.Instance.IsLinkedWithUnity(AuthenticationService.Instance.PlayerInfo))
        {
            _linkedUI.SetActive(true);
            _unlinkInfoUI.SetActive(false);
            LinkBtn.gameObject.SetActive(false);
            _loginSignInTypeText.text = "Authentication Type Unity Player Accounts";
            PlayerInfoBackBtn.gameObject.SetActive(true);

        }
        else
        {
            LinkBtn.gameObject.SetActive(true);
            _linkedUI.SetActive(false);
            _loginSignInTypeText.text = "Authentication Type Anonymous";
            PlayerInfoBackBtn.gameObject.SetActive(true);

        }

    }
    public void SignOutBtnClick()
    {
        AuthManager.Instance.SignOut();
    }

    public void LinkBtnClick()
    {
        _statusText.text = string.Empty;
        DisableLinkBtn();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _statusText.text = Application.internetReachability.ToString() + ", maybe there is no INTERNET";
            EnableLinkBtn();
            return;
        }

        AuthManager.Instance.StartUnitySignInAsync();
    }
    private void DisableLinkBtn()
    {
        LinkBtn.enabled = false;
        _linkBtnText.text = "please wait";
    }
    public void EnableLinkBtn()
    {
        LinkBtn.enabled = true;
        _linkBtnText.text = "Retry";
    }
    public void UnlinkBtnClick()
    {
        _unlinkInfoUI.SetActive(true);
        PlayerInfoBackBtn.gameObject.SetActive(false);
        _linkButtonSet.SetActive(false);
    }
    public void UnlinkYesBtnClick()
    {
        UnlinkYesBtnDisable();
        AuthManager.Instance.UnlinkUnityPlayerAccount();
    }
    public void UnlinkYesBtnEnable()
    {
        UnlinkYesBtn.enabled = true;
        _unlinkYesBtnText.text = "Retry";
        UnlinkBtnNo.enabled = true;
    }
    public void UnlinkYesBtnDisable()
    {
        UnlinkYesBtn.enabled = false;
        _unlinkYesBtnText.text = "Please Wait...";
        UnlinkBtnNo.enabled = false;
    }
    public void UnlinkNoBtnClick()
    {
        _unlinkInfoUI.SetActive(false);
        PlayerInfoBackBtn.gameObject.SetActive(true);
        _linkButtonSet.SetActive(true);
    }
    public void OldDataBtn()
    {
        DisableOldDataBtn();
        AuthManager.Instance.LinkWithUnityOld();
    }
    private void DisableOldDataBtn()
    {
        _oldDataBtn.enabled = false;
        _currentDataBtn.enabled = false;
        _oldDataBtnText.text = "Linking...";
    }
    public void EnableOldDataBtn()
    {
        _oldDataBtn.enabled = true;
        _currentDataBtn.enabled = true;
        _oldDataBtnText.text = "Error fetching old data, Please try again";
    }
    public void CurrentDataBtn()
    {
        DisableCurrentDataBtn();
        AuthManager.Instance.LinkWithCurrentData();

    }
    private void DisableCurrentDataBtn()
    {
        _currentDataBtn.enabled = false;
        _oldDataBtn.enabled = false ;
        _currentDataBtnText.text = "Linking...";
    }
    public void EnableCurrentDataBtn()
    {
        _currentDataBtn.enabled = true;
        _oldDataBtn.enabled = true;

        _currentDataBtnText.text = "Error to link, Please try again";
    }
    public void StatusMessageUI(string msg)
    {
        //_statusText.text = msg; // due to long error Message disabling this
        _statusText.text = "Error\n" + "no internet access or connection failure \n try again";
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
        ScoreManager.Instance.LoadDataFromServer();
        ScoreManager.Instance.SaveJuiceData();
        ScoreManager.Instance.SavePowerupData();
    }
    public void EnableNetworkRetryBtn()
    {
        NetworkRetryBtn.enabled = true;
    }
    public void RewardUI(int count)
    {
        int index = Random.Range(0, _rewardsPrefabImage.Length);
        Image reward = Instantiate(_rewardsPrefabImage[index], transform);
        TextMeshProUGUI rewardText = reward.GetComponentInChildren<TextMeshProUGUI>();
        if (reward.CompareTag("Juice"))
        {
            rewardText.text = "+" + count.ToString();
            ScoreManager.Instance.TotalJuice += count;
            ScoreManager.Instance.SaveJuiceData();
        }
        else if (reward.CompareTag("BombDefuse"))
        {
            rewardText.text = "+ 1";
            ScoreManager.BombDefuseCount++;
            ScoreManager.Instance.SavePowerupData();

        }
        else if (reward.CompareTag("FruitsPowerup"))
        {
            rewardText.text = "+ 1";
            ScoreManager.FruitsPowerUpCount++;
            ScoreManager.Instance.SavePowerupData();
        }

        Destroy(reward.gameObject, 1.2f);
    }

}
