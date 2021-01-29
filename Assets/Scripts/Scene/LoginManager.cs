using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections;

public class LoginManager : MonoBehaviour {

	public FBLoginManager FBLogin; 

	//login widget
	public GameObject loginWidget;
	public UIButton TouchScreenBtn;  
	//register widget
	public GameObject regisWidget;
	public UILabel[] character_name;
	public UITexture[] character_image;
	 
	//register
	public UIButton okNameButton;
	public UILabel name;
	public UIButton girlBtn;
	public UIButton boyBtn;

	private int characterIndex = 0;

	private CharactersData[] charactersData
	{
		get { return CardGameManager._INSTANCE.charactersData; }  
	}
		

	private void Awake()
	{	
		FBLogin = FBLogin.GetComponent<FBLoginManager> ();

		unsafe { StarManager._INSTANCE.OnReceiveData    = DataCallback; }
		StarManager._INSTANCE.OnStatusChange            = StatusChangeCallback;
		StarManager._INSTANCE.OnStarError               = ErrorCallback;
	}

	private void Start()
	{	
		CardGameManager._INSTANCE.InitFade(); 
		CardGameManager._INSTANCE.FadeFirstOpen(); 

		//loginWidget.SetActive(true); 
		regisWidget.SetActive(false);

		EventDelegate.Add(TouchScreenBtn.onClick, OnLoginClick); 
		EventDelegate.Add(okNameButton.onClick, OnNameOkClick);

		EventDelegate.Add(girlBtn.onClick, delegate() {
			OnSelect(0);
		});

		EventDelegate.Add(boyBtn.onClick, delegate() {
			OnSelect(1);
		}); 

		UpdateCharacterInfo(); 
	}



	private unsafe void DataCallback(int idx, void* buffer, uint size)
	{
		switch (*(Header*)buffer)
		{
		case Header.LOGIN_HEADER:
			LoginResponseHandler((LoginResponse*)buffer);
			break;
		case Header.REGISTER_HEADER:
			RegisResponseHandler((RegisterResponse*)buffer);
			break;
		}
	}

	private unsafe void LoginResponseHandler(LoginResponse* res)
	{
		print("LoginCode: " + res->code);
		switch (res->code)
		{
		case LoginResponseCode.LOGIN_CODE_OK:
		case LoginResponseCode.LOGIN_CODE_REJOIN_END:
			CardGame.user = res->user;
			CardGameManager._INSTANCE.FadeScreen(delegate() 
				{
					CardGame.LoadScene(SceneName.Lobby); 
				});
			break;
		case LoginResponseCode.LOGIN_CODE_NEW_USER:
			CardGameManager._INSTANCE.FadeScreen(delegate()
				{
					loginWidget.SetActive(false);
					regisWidget.SetActive(true);
					name.text = "Welcome, " + FBLogin.userName; 
				});
			break;
		case LoginResponseCode.LOGIN_CODE_REJOIN_OK:
			Debug.Log("Login Rejoin OK!");
			CardGameManager._INSTANCE.FadeScreen(delegate() 
				{
					CardGame.LoadScene(SceneName.Game);
				});
			break;
		case LoginResponseCode.LOGUN_CODE_ALREADY_LOGIN:
			BoxDialog._INSTANCE.ShowMessageDialog("Error", "can't login", "this id already logged in", "bye~~", "OK", delegate() 
				{
					Application.Quit();
				});
			break;
		case LoginResponseCode.LOGIN_CODE_ERROR: 
			break;
		}
	}

	private unsafe void RegisResponseHandler(RegisterResponse* res)
	{
		switch (res->code)
		{
		case RegisterResponseCode.REGISTER_CODE_OK:
			break;
		case RegisterResponseCode.REGISTER_CODE_NAME_DUPLICATE:
			BoxDialog._INSTANCE.ShowMessageDialog("Error", "", "name already use", "", "OK", delegate() 
				{
				});
			break;
		case RegisterResponseCode.REGISTER_CODE_NAME_SHORT:
			break;
		case RegisterResponseCode.REGISTER_CODE_ERROR:
			break;
		}
	}

	private void StatusChangeCallback(SCStatus changed)
	{
		print("LoginManager StatusChangeCallback: " + changed);
		switch (changed)
		{
		case SCStatus.CHILD:
			SendLogin();
			break;
		}
	}


	private void ErrorCallback(SCError error, string msg)
	{
		print("Login ErrorCallback: " + error);
		switch (error)
		{
		case SCError.SERVER_FAIL:
		case SCError.DISCONNECTED_FROM_SERVER:
		case SCError.CONNECTION_TIMEOUT:
		case SCError.CREATE_CONNECTION_FAIL:
		case SCError.NO_INTERNET: 
			Recon();
			break;
		}
	}


	private void Recon()
	{
		StarManager._INSTANCE.ResetClientStatus(delegate()
		{
			print("reconnected");
				BoxDialog._INSTANCE.ShowMessageDialog("Error", "", "can't connect to server", "", "ok", delegate()
			{
				CardGame.LoadScene(SceneName.Login);  
			});
		});
	}



	public void OnLoginClick()
	{	
		SoundControl.shared.PlayEffectSound ("Click");
		if (StarManager._INSTANCE.ClientStatus == SCStatus.READY)
		{
			print("connect");
			CardGameManager._INSTANCE.Connect();
		}
	}

		
	private unsafe void SendLogin()
	{	
		Debug.Log("SEND LOGIN");
		Debug.Log("FBLogin.userId" + FBLogin.userId);
		LoginRequest lr = new LoginRequest() { header = Header.LOGIN_HEADER, email = FBLogin.userEmail };  
		Star.SendData(&lr, sizeof(LoginRequest));  
	}

	private void OnSelect(sbyte _index)
	{	
		SoundControl.shared.PlayEffectSound ("Click"); 
		characterIndex = _index;
	}
	private void UpdateCharacterInfo()
	{	
		for (int i = 0; i < 2; i++) { 

			character_image[i].mainTexture = charactersData[i].character;  
			character_name[i].text = charactersData[i].name;  
		}
	}
		

	public void OnNameOkClick()
	{	
		SoundControl.shared.PlayEffectSound ("Click"); 

		if (StarManager._INSTANCE.ClientStatus == SCStatus.CHILD)
		{
			unsafe
			{
				RegisterRequest lr = new RegisterRequest() { header = Header.REGISTER_HEADER, uuid = FBLogin.userId,email = FBLogin.userEmail, name = FBLogin.userName, character = (sbyte)(characterIndex + 1)};  
				Star.SendData(&lr, sizeof(RegisterRequest));
			}
		}
	}


}

[System.Serializable]
public class CharactersData 
{
	public Texture character;
	public string name;
	[TextArea]
	public string info;
}
