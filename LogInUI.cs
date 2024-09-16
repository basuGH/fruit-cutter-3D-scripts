using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInUI : MonoBehaviour
{
    private static LogInUI _instance;
    public static LogInUI Instance { get { return _instance; } }

    [SerializeField] private TextMeshProUGUI _guestSignInBtnText, _unitySignInBtnText;
    [SerializeField] private Button _guestSignInBtn, _unitySignInBtn;
    [SerializeField] private TextMeshProUGUI _statusText, _previousLinkedAccountDataText;
    [SerializeField] private Button _oldDataBtn, _currentDataBtn;
    [SerializeField] private TextMeshProUGUI _oldDataBtnText, _currentDataBtnText;
    [SerializeField] private TextMeshProUGUI _loadingText;

    public GameObject LinkWarningInfoPanel, ButtonSet;
    [SerializeField] private Toggle _privacyPolicy;

    private void Awake()
    {
        _privacyPolicy.isOn = false;
        _instance = this;
        DefaultUI();
       // SignOutText();
    }
    private void Start()
    {
        SignOutText();
    }
    public void SignOutText()
    {
        if (AuthManager.Instance.IsSignOut)
        {
            _statusText.text = "Successfully SignOut";

        }
    }

    private void DefaultUI()
    {
        LinkWarningInfoPanel.SetActive(false);
        ButtonSet.SetActive(true);
    }

    public void EnableAutoConnectUI()
    {
        ButtonSet.SetActive(false);
        _loadingText.gameObject.SetActive(true);
    }
    public void DisableAutoConnectUI()
    {
        ButtonSet.SetActive(true);
        _loadingText.gameObject.SetActive(false);
    }

    public void EnableGuestBtn()
    {

        _guestSignInBtn.enabled = true;
        _unitySignInBtn.enabled = true;
        _guestSignInBtnText.text = "Retry \nGuest Sign In";
    }
    private void DisableGuestBtn()
    {
        _guestSignInBtn.enabled = false;
        _unitySignInBtn.enabled = false;
        _guestSignInBtnText.text = "Please Wait ...";

    }
    public void EnableUnitySignInBtn()
    {
        _unitySignInBtn.enabled = true;
        _guestSignInBtn.enabled = true;
        _unitySignInBtnText.text = "Retry \nSign In With Unity";
    }
    private void DisableUnitySignInBtn()
    {
        _unitySignInBtn.enabled = false;
        _guestSignInBtn.enabled = false;
        _unitySignInBtnText.text = "Please Wait ...";

    }
    public void GuestBtnClick()
    {
        if (!_privacyPolicy.isOn)
        {
            _statusText.text = "Please read the Privacy Policy and Terms of Service by clicking on the respective words. Tick the box to indicate your acceptance before proceeding.";
            return;
        }
        DisableGuestBtn();
        _statusText.text = string.Empty;
        AuthManager.Instance.SignIn();
    }
    public void UnitySignInBtnClick()
    {
        if (!_privacyPolicy.isOn)
        {
            _statusText.text = "Please read the Privacy Policy and Terms of Service by clicking on the respective words. Tick the box to indicate your acceptance before proceeding.";
            return;
        }
        _statusText.text = string.Empty;
        DisableUnitySignInBtn();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            _statusText.text = Application.internetReachability.ToString() + "\nmaybe there is no INTERNET";
            EnableUnitySignInBtn();
            return;
        }

        AuthManager.Instance.StartUnitySignInAsync();
    }
    public void StatusMessageUI(string msg)
    {
        //_statusText.text = msg; // due to long error Message disabling this
        _statusText.text = "Error !!!\n" + "no internet access or connection failure \n try again";
    }
    public void PreviousDataUI(string info)
    {
        _previousLinkedAccountDataText.text += info + "\n";
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
        _oldDataBtn.enabled = false;
        _currentDataBtnText.text = "Linking...";
    }
    public void EnableCurrentDataBtn()
    {
        _currentDataBtn.enabled = true;
        _oldDataBtn.enabled = true;
        _currentDataBtnText.text = "Error to link, Please try again";
    }
    public void PrivacyPolicyBtnClick()
    {
        AuthManager.Instance.OpenPrivacyPolicy();
    }
    public void TermsOfServiceBtnClick()
    {
        AuthManager.Instance.OpenTermsOfService();
    }

    public void UnityPlayerAccountDeleteBtnClick()
    {
        AuthManager.Instance.OpenUPADeleteLink();


    }
}