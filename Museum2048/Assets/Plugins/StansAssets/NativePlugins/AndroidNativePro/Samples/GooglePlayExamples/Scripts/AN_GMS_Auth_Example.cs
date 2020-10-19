using SA.Android.App;
using UnityEngine;
using UnityEngine.UI;
using SA.Android.GMS.Games;
using SA.Android.GMS.Auth;
using SA.Android.GMS.Common.Images;
using SA.Android.GMS.Drive;
using StansAssets.Foundation;

public class AN_GMS_Auth_Example : MonoBehaviour
{
#pragma warning disable 649
    [Header("User Info")]
    [SerializeField]
    Text m_userName;
    [SerializeField]
    Text m_userMail;
    [SerializeField]
    RawImage m_userAvatar;
    [Header("Buttons")]
    [SerializeField]
    Button m_connect;

#pragma warning restore 649

    //Avoid using API with Awake. The main Android activity may not be ready yet.
    void Start()
    {
        Debug.Log("AN_GMS_Auth_Example Start");

        //Let's see if user has already signed in
        var signedInAccount = AN_GoogleSignIn.GetLastSignedInAccount();
        if (signedInAccount != null) //That's good but we need to re-connect anyway
            SignInNoSilent();
        m_connect.onClick.AddListener(() =>
        {
            var account = AN_GoogleSignIn.GetLastSignedInAccount();
            if (account == null)
                SignInFlow();
            else
                SignOutFlow();
        });
    }

    void SignOutFlow()
    {
        var gso = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN).Build();
        var client = AN_GoogleSignIn.GetClient(gso);
        client.SignOut(ClearAccountUI);
    }

    void ShowGamesSettingsExample()
    {
        var gamesClient = AN_Games.GetGamesClient();
        gamesClient.GetSettingsIntent((result) =>
        {
            if (result.IsSucceeded)
            {
                var intent = result.Intent;
                var proxy = new AN_ProxyActivity();
                proxy.StartActivityForResult(intent, (intentResult) =>
                {
                    proxy.Finish();
                });
            }
            else
            {
                Debug.Log($"Failed to Get Settings Intent {result.Error.FullMessage}");
            }
        });
    }

    void SignInNoSilent()
    {
        var builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
        builder.RequestId();
        builder.RequestEmail();
        builder.RequestProfile();
        var gso = builder.Build();
        var client = AN_GoogleSignIn.GetClient(gso);
        Debug.Log("SignInNoSilent Start");
        client.SignIn((signInResult) =>
        {
            Debug.Log("Sign In StatusCode: " + signInResult.StatusCode);
            if (signInResult.IsSucceeded)
            {
                Debug.Log("SignIn Succeeded");
                UpdateUIWithAccount(signInResult.Account);
            }
            else
            {
                Debug.LogError($"SignIn failed: {signInResult.Error.FullMessage}");
            }
        });
    }

    void SignInFlow()
    {
        Debug.Log("Play Services Sign In started....");
        var builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);

        //Google play documentation syas that
        // you don't need to use this, however, we recommend you still
        // add those option to your Sing In builder. Some version of play service lib
        // may retirn a signed account with all fileds empty if you will not add this.
        // However according to the google documentation this step isn't required
        // So the decision is up to you.
        builder.RequestId();
        builder.RequestEmail();
        builder.RequestProfile();

        // Add the APPFOLDER scope for Snapshot support.
        builder.RequestScope(AN_Drive.SCOPE_APPFOLDER);
        var gso = builder.Build();
        var client = AN_GoogleSignIn.GetClient(gso);
        Debug.Log("Let's try Silent SignIn first");
        client.SilentSignIn((result) =>
        {
            if (result.IsSucceeded)
            {
                Debug.Log("SilentSignIn Succeeded");
                UpdateUIWithAccount(result.Account);
            }
            else
            {
                // Player will need to sign-in explicitly using via UI
                Debug.Log($"SilentSignIn Failed with code: {result.Error.Code}");
                Debug.Log("Starting the default Sign in flow");

                //Starting the interactive sign-in
                client.SignIn((signInResult) =>
                {
                    Debug.Log($"Sign In StatusCode: {signInResult.StatusCode}");
                    if (signInResult.IsSucceeded)
                    {
                        Debug.Log("SignIn Succeeded");
                        UpdateUIWithAccount(signInResult.Account);
                    }
                    else
                    {
                        Debug.LogError($"SignIn filed: {signInResult.Error.FullMessage}");
                    }
                });
            }
        });
    }

    void ClearAccountUI()
    {
        m_connect.GetComponentInChildren<Text>().text = "Connect";

        //Display User info
        m_userName.text = "Signed out";
        m_userMail.text = "Signed out";
        m_userAvatar.texture = null;
    }

    void PrintSignedPlayerInfo()
    {
        var client = AN_Games.GetPlayersClient();
        client.GetCurrentPlayer((result) =>
        {
            if (result.IsSucceeded)
            {
                var player = result.Data;

                //Printing player info:
                Debug.Log($"player.Id: {player.PlayerId}");
                Debug.Log($"player.DisplayName: {player.DisplayName}");
                Debug.Log($"player.HiResImageUri: {player.HiResImageUri}");
                Debug.Log($"player.IconImageUri: {player.IconImageUri}");
                Debug.Log($"player.HasIconImage: {player.HasIconImage}");
                Debug.Log($"player.HasHiResImage: {player.HasHiResImage}");
                if (!player.HasHiResImage)
                {
                    var url = player.HiResImageUri;
                    var manager = new AN_ImageManager();
                    manager.LoadImage(url, (imaheLoadResult) =>
                    {
                        if (imaheLoadResult.IsSucceeded)
                            m_userAvatar.texture = imaheLoadResult.Image;
                        else //Or you may want to assing some default texture here
                            m_userAvatar.texture = null;
                    });
                }
            }
            else
            {
                Debug.LogError("Failed to load Current Player " + result.Error.FullMessage);
            }
        });
    }

    void UpdateUIWithAccount(AN_GoogleSignInAccount account)
    {
        Debug.Log($"account.HashCode: {account.HashCode}");
        Debug.Log(account);
        Debug.Log($"SignIn IsSucceeded. user: {account.GetDisplayName()}");

        //Display User info
        m_userName.text = account.GetDisplayName();
        m_userMail.text = account.GetEmail();
        m_connect.GetComponentInChildren<Text>().text = "Disconnect";
        PrintSignedPlayerInfo();
    }

    [ContextMenu("Test me")]
    void TestMe()
    {
        CachedWebRequest.GetTexture2D("https://lh6.googleusercontent.com/--WOsAL7DUtk/AAAAAAAAAAI/AAAAAAAAAD8/zT7QiKYB2A0/photo.jpg", (texture) =>
        {
            m_userAvatar.texture = texture;
        });
    }
}
