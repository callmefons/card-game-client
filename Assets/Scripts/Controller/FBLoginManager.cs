using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;

public class FBLoginManager : MonoBehaviour
{
	public LoginManager loginManager;
	public UIButton LoginWithFacebook;
	public TweenAlpha logoAlpha;

	private string _userId;
	private string _userEmail;
	private string _userName;

	public string userId {
		get { return _userId; }
		set { _userId = value; }
	}

	public string userEmail {
		get { return _userEmail; }
		set { _userEmail = value; }
	}

	public string userName {
		get { return _userName; }
		set { _userName = value; }
	}


	static FBLoginManager instance = null;

	public static FBLoginManager Instance(){
		return instance;
	}

	// Awake function from Unity's MonoBehavior
	void Awake ()
	{	
		if(instance == null)
			instance = this;

		DontDestroyOnLoad(gameObject);

		FBInit();
		//CheckLogin ();
		loginManager = loginManager.GetComponent<LoginManager> ();
		EventDelegate.Add (LoginWithFacebook.onClick, Login);
		EventDelegate.Add (logoAlpha.onFinished, OnSplashFinished, true); 
	}


	void FBInit ()
	{
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
			CheckLogin ();
		}
	}

	private void OnSplashFinished ()
	{
		StartCoroutine (Wait (0.1f, delegate() {	 
			FBInit ();
			Debug.Log ("OnSplashFinished");
		}));
	}

	private IEnumerator Wait (float sec, Action action)
	{
		while (StarManager._INSTANCE.ClientStatus != SCStatus.READY) {
			yield return null;
		}
		yield return new WaitForSeconds (sec);
		if (action != null) {
			action ();
		}
	}


	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp ();
			// Continue with Facebook SDK
			// ...
			CheckLogin ();
			Debug.Log ("Facebook status " + FB.IsLoggedIn);

		} else {

			BoxDialog._INSTANCE.ShowMessageDialog ("Error", "Failed to Initialize", "the Facebook SDK", "please login again", "OK", delegate() {
				CardGameManager._INSTANCE.FadeScreen (delegate() {
					CardGame.LoadScene (SceneName.Login);
				}); 
			});
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	private void GetAPI ()
	{
		FB.API ("/me?fields=first_name,email", HttpMethod.GET, ProfileNameCallback);
		//FB.API ("/me/picture?redirect=false", HttpMethod.GET, ProfilePhotoCallback);
	}

	void CheckLogin ()
	{	
		Debug.Log ("CheckLogin");
		if (FB.IsLoggedIn) {
			loginManager.loginWidget.SetActive (true);
			LoginWithFacebook.gameObject.SetActive (false); 
			GetAPI ();
		} else {
			loginManager.loginWidget.SetActive (false); 
			LoginWithFacebook.gameObject.SetActive (true);
		}


	}



	public void Login ()
	{	
		SoundControl.shared.PlayEffectSound ("Click");
		var perms = new List<string> (){ "public_profile", "email", "user_friends" };
		FB.LogInWithReadPermissions (perms, AuthCallback);
	}

	public void Connect ()
	{	
		//loginManager.OnLoginClick ();
		loginManager.loginWidget.SetActive (true);
		LoginWithFacebook.gameObject.SetActive (false); 
	}


	private void AuthCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);
			Debug.Log ("Facebook status " + FB.IsLoggedIn);
			// Print current access token's granted permissions

			GetAPI ();

		} else {

			BoxDialog._INSTANCE.ShowMessageDialog ("Error", "User cancelled login", "facebook", "please login again", "OK", delegate() {
				CardGameManager._INSTANCE.FadeScreen (delegate() {
					CardGame.LoadScene (SceneName.Login);
				}); 
			});
		}
	}

	private void ProfileNameCallback (IGraphResult result)
	{
		if (result.ResultDictionary != null) {
			foreach (string key in result.ResultDictionary.Keys) {
				Debug.Log (key + " : " + result.ResultDictionary [key].ToString ());
			}
		}
	
		userId = result.ResultDictionary ["id"].ToString ();
		userEmail = result.ResultDictionary ["email"].ToString ();
		userName = result.ResultDictionary ["first_name"].ToString ();

		Debug.Log ("userId " + userId + " User Email " + userEmail + " Username " + userName); 

		Connect ();
	}

	private void ProfilePhotoCallback (IGraphResult result)
	{
		if (String.IsNullOrEmpty (result.Error) && !result.Cancelled) {
			IDictionary data = result.ResultDictionary ["data"] as IDictionary;
			string photoURL = data ["url"] as String;

			StartCoroutine (fetchProfilePic (photoURL));
		}
	}

	private IEnumerator fetchProfilePic (string url)
	{
		WWW www = new WWW (url);
		yield return www;
		//this.profilePic.mainTexture = www.texture;
	}

}
