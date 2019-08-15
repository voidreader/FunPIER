using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SA.Android.GMS.Games;
using SA.Android.GMS.Auth;
using SA.Android.GMS.Common.Images;
using SA.Android.Utilities;

using SA.Android.GMS.Drive;

using SA.Foundation.Network.Web;

public class AN_GMS_Auth_Example : MonoBehaviour {


#pragma warning disable 649
    [Header("User Info")]
    [SerializeField] Text m_userName;
    [SerializeField] Text m_userMail;
    [SerializeField] RawImage m_userAvatar;

    [Header("Buttons")]
    //[SerializeField] Button m_checkAvalibility;
    [SerializeField] Button m_connect;
    
#pragma warning restore 649


    //Avoid using API with Awake. The main Android activity may not be ready yet.
    private void Start() {

        AN_Logger.Log("AN_GMS_Auth_Example Start");
        //Let's see if user has already signed in
        var signedInAccount = AN_GoogleSignIn.GetLastSignedInAccount();
        if (signedInAccount != null) {
            //That's good but we need to re-connect anyway

            SignInNoSilent();
            //UpdateUIWithAccount(signedInAccount);
        }

        /*
        m_checkAvalibility.onClick.AddListener(() => {
            int result = AN_GoogleApiAvailability.IsGooglePlayServicesAvailable();
            if (result != AN_ConnectionResult.SUCCESS) {
                Debug.Log("Play Services does not available on this device. Resolving....");

                AN_GoogleApiAvailability.MakeGooglePlayServicesAvailable((resolution) => {
                    if (resolution.IsSucceeded) {
                        Debug.Log("Resolved! Play Services is available on this device");
                    } else {
                        Debug.Log("Failed to resolve: " + resolution.Error.Message);
                    }
                });
            } else {
                Debug.Log("Play Services is available on this device");
            }
        });

        */


        m_connect.onClick.AddListener(() => {
            var account = AN_GoogleSignIn.GetLastSignedInAccount();
            if (account == null) {
                SignInFlow();
            } else {
                SignOutFlow();
            }
        });

    }

    private void SignOutFlow() {

      

        AN_GoogleSignInOptions gso = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN).Build(); 
        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);

        client.SignOut(() => {
            ClearAccountUI();
        });
    }



    private void SignInNoSilent() {


        AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
        builder.RequestId();
        builder.RequestEmail();
        builder.RequestProfile();
       
        AN_GoogleSignInOptions gso = builder.Build();
        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);
        AN_Logger.Log("SignInNoSilent Start ");

        client.SignIn((signInResult) => {
            AN_Logger.Log("Sign In StatusCode: " + signInResult.StatusCode);
            if (signInResult.IsSucceeded) {
                AN_Logger.Log("SignIn Succeeded");
                UpdateUIWithAccount(signInResult.Account);
            } else {
                AN_Logger.Log("SignIn filed: " + signInResult.Error.FullMessage);
            }
        });

      
    }

    private void SignInFlow() {
        AN_Logger.Log("Play Services Sign In started....");
        AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);

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

        AN_GoogleSignInOptions gso = builder.Build();
        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);

        AN_Logger.Log("Let's try Silent SignIn first");
        client.SilentSignIn((result) => {
            if (result.IsSucceeded) {
                AN_Logger.Log("SilentSignIn Succeeded");
                UpdateUIWithAccount(result.Account);
            } else {

                // Player will need to sign-in explicitly using via UI

                AN_Logger.Log("SilentSignIn Failed with code: " + result.Error.Code);
                AN_Logger.Log("Starting the default Sign in flow");

                //Starting the interactive sign-in
                client.SignIn((signInResult) => {
                    AN_Logger.Log("Sign In StatusCode: " + signInResult.StatusCode);
                    if (signInResult.IsSucceeded) {
                        AN_Logger.Log("SignIn Succeeded");
                        UpdateUIWithAccount(signInResult.Account);
                    } else {
                        AN_Logger.Log("SignIn filed: " + signInResult.Error.FullMessage);
                    }
                });


            }
        });
    }

    private void ClearAccountUI() {
        m_connect.GetComponentInChildren<Text>().text = "Connect";

        //Display User info
        m_userName.text = "Signed out";
        m_userMail.text = "Signed out";

        m_userAvatar.texture = null;


    }

    private void PrintSignedPlayerInfo() {
        AN_PlayersClient client = AN_Games.GetPlayersClient();
        client.GetCurrentPlayer((result) => {
            if(result.IsSucceeded) {
                AN_Player player = result.Data;
                //Printing player info:
                AN_Logger.Log("player.Id: " + player.PlayerId);
                AN_Logger.Log("player.DisplayName: " + player.DisplayName);
                AN_Logger.Log("player.HiResImageUri: " + player.HiResImageUri);
                AN_Logger.Log("player.IconImageUri: " + player.IconImageUri);
                AN_Logger.Log("player.HasIconImage: " + player.HasIconImage);
                AN_Logger.Log("player.HasHiResImage: " + player.HasHiResImage);



                if (!player.HasHiResImage) {
                    var url = player.HiResImageUri;
                    AN_ImageManager manager = new AN_ImageManager();
                    manager.LoadImage(url, (imaheLoadResult) => {
                        if (imaheLoadResult.IsSucceeded) {
                            m_userAvatar.texture = imaheLoadResult.Image;
                        } else {
                            //Or you may want to assing some default texture here
                            m_userAvatar.texture = null;
                        }
                    });
                }

            } else {
                AN_Logger.Log("Failed to load Current Player " + result.Error.FullMessage);
            }


        });
    }

    private void UpdateUIWithAccount(AN_GoogleSignInAccount account) {
        AN_Logger.Log("account.HashCode:" + account.HashCode);
        AN_Logger.Log(account);
        AN_Logger.Log("SignIn IsSucceeded. user: " + account.GetDisplayName());

        //Display User info
        m_userName.text = account.GetDisplayName();
        m_userMail.text = account.GetEmail();



        m_connect.GetComponentInChildren<Text>().text = "Disconnect";

        PrintSignedPlayerInfo();
    }
    

    [ContextMenu("Test me")]
    void TestMe() {


        SA_CachedRequestsFactory.GetTexture2D("https://lh6.googleusercontent.com/--WOsAL7DUtk/AAAAAAAAAAI/AAAAAAAAAD8/zT7QiKYB2A0/photo.jpg", (texture) => {
            m_userAvatar.texture = texture;
        });
        
       
    }
}
