using UnityEngine;
using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication.PlayerAccounts;


public class AuthManager : MonoBehaviour
{
    private static AuthManager _instance;
    public static AuthManager Instance { get { return _instance; } }

    public bool IsSignOut;

    [SerializeField] private string _privacyPolicyLink;
    [SerializeField] private string _termsOfServiceLink;
    [SerializeField] private string _upaDeleteLink;

    public string PlayerID { get; private set; }
    private bool _isAnonymous;
    [SerializeField] private bool _isLinked;
    async void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            LogInUI.Instance.StatusMessageUI(e.ToString());
        }
        SetupEvents();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;


    }

    private void Start()
    {
        _isLinked = bool.Parse(PlayerPrefs.GetString("IsLinked", "false"));

        AutoConnect();

    }

    // Setup authentication event handlers if desired
    void SetupEvents()
    {

        AuthenticationService.Instance.SignedIn += () =>
        {
            PlayerID = AuthenticationService.Instance.PlayerId;

            if (!IsLinkedWithUnity(AuthenticationService.Instance.PlayerInfo))
            {
                _isAnonymous = true;
            }
            // Shows how to get a playerID
            //Debug.Log($"PlayerID @Event: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            //Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
            LogInUI.Instance.StatusMessageUI(err.ToString());
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            PlayerAccountService.Instance.SignedIn -= SignInWithUnity;

            Debug.Log("Player signed out.");
        };

        AuthenticationService.Instance.Expired += () =>
        {
            LogInUI.Instance.StatusMessageUI("Player session could not be refreshed and expired.");
            Debug.Log("Player session could not be refreshed and expired.");
        };
    }
    private async void AutoConnect()
    {
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            LogInUI.Instance.EnableAutoConnectUI();

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();


                // Debug.Log("Auto Connect succeeded!");
                SceneManager.LoadScene(1);
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                LogInUI.Instance.DisableAutoConnectUI();

                Debug.LogException(ex);
                LogInUI.Instance.StatusMessageUI(ex.ToString());

            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                LogInUI.Instance.DisableAutoConnectUI();

                Debug.LogException(ex);
                LogInUI.Instance.StatusMessageUI(ex.ToString());
            }
        }

    }
    public async void SignIn()
    {
        await SignInAnonymouslyAsync();
    }
    async Task SignInAnonymouslyAsync()
    {

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();


            Debug.Log("Sign in anonymously succeeded!");
            SceneManager.LoadScene(1);
            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log("Linked " + IsLinkedWithUnity(AuthenticationService.Instance.PlayerInfo));

            //PlayerID = AuthenticationService.Instance.PlayerId;

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            LogInUI.Instance.EnableGuestBtn();

            Debug.LogException(ex);
            LogInUI.Instance.StatusMessageUI(ex.ToString());

        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            LogInUI.Instance.EnableGuestBtn();
            Debug.LogException(ex);
            LogInUI.Instance.StatusMessageUI(ex.ToString());
        }
    }
    public async void StartUnitySignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();

        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.StatusMessageUI(ex.ToString());
                LogInUI.Instance.EnableUnitySignInBtn();

            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.StatusMessageUI(ex.ToString());
                MainMenu.Instance.EnableLinkBtn();
            }
        }
    }

    private async void SignInWithUnity()
    {
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // Debug.Log("No Session Token");
            await UnitySignInProcess();
            return;
        }

        if (!_isLinked)
        {

            // Debug.Log("Available Session Token");
            // Debug.Log("Unity Link Process");


            if (!_isAnonymous)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            }
            LinkWithUnity();
        }

        if (_isLinked)
        {
            await UnitySignInProcess();
        }
    }

    private async Task UnitySignInProcess()
    {
        //Debug.Log("Unity Sign Process");
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            //Debug.Log("before is linked " + _isLinked);
            if (!_isLinked)
            {
                _isLinked = true;
                // Debug.Log(" Inside is linked " + _isLinked);

                SaveIsLinkedValue();
            }
            // Debug.Log(" after is linked " + _isLinked);


            SceneManager.LoadScene(1);
            // PlayerID = AuthenticationService.Instance.PlayerId;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.StatusMessageUI(ex.Message);
                LogInUI.Instance.EnableOldDataBtn();
                LogInUI.Instance.EnableUnitySignInBtn();

            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.EnableLinkBtn();
                MainMenu.Instance.EnableOldDataBtn();
            }

        }
    }

    private async void StartLinkWithUnity()
    {

        //if (PlayerAccountService.Instance.IsSignedIn)
        //{
        //    LinkWithUnity();
        //    return;
        //}

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }

        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            LogInUI.Instance.StatusMessageUI(ex.Message);

        }
    }
    public void LinkWithUnityOld()
    {
        AuthenticationService.Instance.SignOut();
        _isLinked = true;
        SaveIsLinkedValue();

        StartUnitySignInAsync();
    }
    public void LinkWithCurrentData()
    {
        ForceLinkWithUnityAsync();
    }

    private async void LinkWithUnity()
    {

        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            _isLinked = true;
            SaveIsLinkedValue();

            SceneManager.LoadScene(1);
            Debug.Log("Link is successful.");
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            //PlayerInfo info = await AuthenticationService.Instance.GetPlayerInfoAsync();


            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.LinkWarningInfoPanel.SetActive(true);
                LogInUI.Instance.ButtonSet.SetActive(false);
                //LogInUI.Instance.PreviousDataUI("ID " + info.Id);
                //LogInUI.Instance.PreviousDataUI("Created At " + info.CreatedAt.ToString());
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.LinkWarningInfoPanel.SetActive(true);
                MainMenu.Instance.LinkBtn.gameObject.SetActive(false);
                MainMenu.Instance.PlayerInfoBackBtn.gameObject.SetActive(false);
            }
            // Prompt the player with an error message.
            Debug.Log("This user is already linked with another account. Log in instead.");
        }

        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.EnableUnitySignInBtn();
                LogInUI.Instance.StatusMessageUI(ex.ToString());
            }
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.LinkWarningInfoPanel.SetActive(true);
                MainMenu.Instance.EnableLinkBtn();
            }

            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {

            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.EnableUnitySignInBtn();
                LogInUI.Instance.StatusMessageUI(ex.ToString());
            }
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    public async void ForceLinkWithUnityAsync()
    {
        Debug.Log(" Force LinkMethod");
        LinkOptions options = new LinkOptions();
        options.ForceLink = true;
        try
        {
            await AuthenticationService.Instance.LinkWithUnityAsync(PlayerAccountService.Instance.AccessToken, options);
            SceneManager.LoadScene(1);
            _isLinked = true;
            SaveIsLinkedValue();

            Debug.Log("Link is successful.");
        }

        catch (AuthenticationException ex)
        {
            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.EnableCurrentDataBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.EnableCurrentDataBtn();
            }
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message

            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            if (LogInUI.Instance != null)
            {
                LogInUI.Instance.EnableCurrentDataBtn();
            }

            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.EnableCurrentDataBtn();
            }
            Debug.LogException(ex);
        }
    }
    public async void UnlinkUnityPlayerAccount()
    {
        await UnlinkUnityAsync();

    }
    async Task UnlinkUnityAsync()
    {
        try
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
            PlayerAccountService.Instance.SignedIn -= SignInWithUnity;
            ScoreManager.Instance.ResetAllData();
            _isLinked = false;
            SaveIsLinkedValue();

            PlayerAccountService.Instance.SignOut();
            AuthenticationService.Instance.ClearSessionToken();
            _isAnonymous = false;
            SceneManager.LoadScene(0);

            LogInUI.Instance.StatusMessageUI("Unlink is successful.");

            Debug.Log("Unlink is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.UnlinkYesBtnEnable();
            }
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            if (MainMenu.Instance != null)
            {
                MainMenu.Instance.UnlinkYesBtnEnable();
            }
            Debug.LogException(ex);
        }
    }
    public bool IsLinkedWithUnity(PlayerInfo playerInfo)
    {
        //Debug.Log("Identity count : " + playerInfo.Identities.Count);

        if (playerInfo.Identities.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void SignOut()
    {
        IsSignOut = true;
        //LogInUI.Instance.SignOutText();

        _isLinked = false;
        ScoreManager.Instance.ResetAllData();
        SaveIsLinkedValue();
        AuthenticationService.Instance.SignOut(true);
        PlayerAccountService.Instance.SignOut();
        AuthenticationService.Instance.ClearSessionToken();
        SceneManager.LoadScene(0);
      //  LogInUI.Instance.SignOutText();

    }
    private void SaveIsLinkedValue()
    {
        PlayerPrefs.SetString("IsLinked", _isLinked.ToString());
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL(_privacyPolicyLink);
    }
    public void OpenTermsOfService()
    {
        Application.OpenURL(_termsOfServiceLink);
    }

    internal void OpenUPADeleteLink()
    {
        Application.OpenURL(_upaDeleteLink);
    }
}