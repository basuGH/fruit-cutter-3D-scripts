using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityLogIn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;

    string m_ExternalIds;

    async void Awake()
    {
        // await UnityServices.InitializeAsync();

        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            _statusText.text = e.Message;
        }
        //SetupEvents();
        PlayerAccountService.Instance.SignedIn += SignInWithUnity;

        //Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
       // await SignInCachedUserAsync();

    }
    async Task SignInCachedUserAsync()
    {
        // Check if a cached player already exists by checking if the session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // if not, then do nothing
            return;
        }


        //Sign in Anonymously
        //This call will sign in the cached player.
        try
        {
            // await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            SignInWithUnity();
            return;
            Debug.Log("Sign in unity succeeded!");

            // Shows how to get the playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerName}");

        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    void SetException(Exception ex)
    {
        _statusText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
        _statusText.text = ex.Message;
    }
    public async void StartSignInAsync()
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
            SetException(ex);
        }
    }

    public async void SignInWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
            m_ExternalIds = GetExternalIds(AuthenticationService.Instance.PlayerInfo);
            Debug.Log("Player ID    " + AuthenticationService.Instance.PlayerId + "   is Sign IN " + AuthenticationService.Instance.IsSignedIn);

            Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
                Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
                Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
            Debug.Log($"Access Token : {PlayerAccountService.Instance.AccessToken}");
            Debug.Log($"unity id : {AuthenticationService.Instance.PlayerInfo.GetUnityId()}");
            Debug.Log($"AuthenticationService Access Token : {AuthenticationService.Instance.AccessToken}");
            Debug.Log($"Player ID Token : {PlayerAccountService.Instance.IdToken}");

            //SceneManager.LoadScene(1);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);

        }
    }
    string GetExternalIds(PlayerInfo playerInfo)
    {
        if (playerInfo.Identities == null)
        {
            return "None";
        }

        var sb = new StringBuilder();
        foreach (var id in playerInfo.Identities)
        {
            sb.Append(" " + id.TypeId);
        }

        return sb.ToString();
    }
    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        Debug.Log("Player ID " + AuthenticationService.Instance.PlayerId +"  , is Sign IN " + AuthenticationService.Instance.IsSignedIn);
    }
    public async void StartUnlinkAsync()
    {
       await UnlinkUnityAsync();
    }

    async Task UnlinkUnityAsync()
    {
        try
        {

            await AuthenticationService.Instance.UnlinkUnityAsync();
            Debug.Log("Unlink is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
    public void ClearBtnClick()
    {
        AuthenticationService.Instance.ClearSessionToken();
        Debug.Log("Player ID    " + AuthenticationService.Instance.PlayerId + "   is Sign IN " + AuthenticationService.Instance.IsSignedIn);

        Debug.Log($"Is SignedIn: {AuthenticationService.Instance.IsSignedIn}");
        Debug.Log($"Is SignedIn PlayerService: {PlayerAccountService.Instance.IsSignedIn}");
        Debug.Log($"Is Authorized: {AuthenticationService.Instance.IsAuthorized}");
        Debug.Log($"Is Expired: {AuthenticationService.Instance.IsExpired}");
    }
}
