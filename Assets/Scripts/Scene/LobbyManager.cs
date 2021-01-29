using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;

public class LobbyManager : MonoBehaviour {      

	public ShopManager shop;

	public GameObject DisplayScoreBox;
	public UILabel rankName;
	public UITexture rankTex;
	public UILabel rankLevel;
	public UILabel rankBest;
	public UILabel rankId;
	public UITexture rankAvatar;
	public UISprite rankCup;

	public UIButton quickJoin;
	public UIButton logoutButton; 
	//User data
	public UILabel winCount;
	public UILabel loseCount;
	public UILabel attenedClass;
	public UILabel levelText;

	//Bottom Bar
	public UILabel coinTextBar;
	public UILabel expTextBar;
	public UILabel levelTextBar;
	//Player Info
	public UILabel ProfileName;
	public UILabel AvatarName;
	public UITexture ProfilePicture;
	//Display Box
	private GameObject rankingRoot; 
	private GameObject dialogPanel;
	public List<Ranking> Rank = new List<Ranking>(); 
	public List<GameObject> Box = new List<GameObject>(); 
	public List<Sprite> FBPictures = new List<Sprite>(); 
	private int TotalUsers;
	private uint MaxUser; 
	private int fetch = 0;  
	private float YOffset = 0f;  

	void Awake () {
		unsafe { StarManager._INSTANCE.OnReceiveData = DataCallback; }
		StarManager._INSTANCE.OnStatusChange = StatusChangeCallback;
		StarManager._INSTANCE.OnStarError = ErrorCallback;
	}
		

	private unsafe void Start()
	{	
		rankingRoot = GameObject.Find ("Ranking");   

		SoundControl.shared.PlayBackgroundSound("LobbySound");
		CardGameManager._INSTANCE.InitFade(); 

		EventDelegate.Add(quickJoin.onClick, OnQuickRoomClick);
		EventDelegate.Add(logoutButton.onClick, Logout);
		shop.CheckUnLockItem(); 

		RequestTotalUser(); 
		RequestUser();
	}

	private unsafe void RequestUser(){
		RequestUpdateUserData(delegate(FetchDataCode res) 
			{
				if (res == FetchDataCode.FETCH_DATA_CODE_USERDATA){
					print("Fetch Data");
					SetUserData(CardGame.user);
					shop.UpdateSetUser(CardGame.user);

					CardGameManager._INSTANCE.FadeFirstOpen(); 
				}
				else{	
					ErrorFetch();
				}
			});
	}

	private unsafe void RequestTotalUser()
	{	
		Debug.Log("RequestTotalUser"); 
		LobbyActionRequest lr = new LobbyActionRequest() { header = Header.LOBBY_ACTION_HEADER, code = LobbyActionCode.LOBBY_ACTION_CODE_TOTAL };     
		Star.SendData(&lr, sizeof(LobbyActionRequest));  
	}


	FetchDataRequest reqFetchUserData = new FetchDataRequest() { header = Header.FETCH_DATA_HEADER, code = FetchDataCode.FETCH_DATA_CODE_USERDATA };
	Action<FetchDataCode> fetchUserDataCallback = null;
	private void RequestUpdateUserData(Action<FetchDataCode> response)
	{
		fetchUserDataCallback = response;
		unsafe
		{
			fixed(void* v = &reqFetchUserData) Star.SendData(v, sizeof(RegisterRequest));
		}
	}

	private unsafe void FetchRank(){ 
		RequestFectchRanking(delegate(LobbyActionCode res) 
			{
				if (res == LobbyActionCode.LOBBY_ACTION_CODE_RANK){ 
					print("Fetch Ranking");  
					SetRankData(CardGame.rank);
				}
				else{	
					ErrorFetch();
				}
			}); 
	}
		
	private unsafe void SetUserData(UserData user)
	{
		
		//Profile
		levelText.text  = user.level.ToString();
		winCount.text = user.winCount.ToString();
		loseCount.text = user.loseCount.ToString();
		attenedClass.text = (user.winCount + user.loseCount).ToString();

		ProfileName.text = "" + user.name;

		if(ProfilePicture.mainTexture != null){ 
			ProfilePicture.mainTexture = CardGameManager._INSTANCE.charactersData[user.character - 1].character;	  
			rankAvatar.mainTexture = CardGameManager._INSTANCE.charactersData[user.character - 1].character;	        
			AvatarName.text = CardGameManager._INSTANCE.charactersData[user.character - 1].name.ToString();   
		}
			
		//Bottom Bar
		coinTextBar.text   = user.gold.ToString("##,#");   
		double percent =  (double)((user.level * CardGame.LEVELUP)/100d);
		double exp = (double)(user.exp / 100d);
		expTextBar.text    = exp + "/" + percent;
		levelTextBar.text = user.level.ToString();

		//At Rank
		rankName.text = user.name;    
		FB.API ("/"+ user.uuid+ "/picture?redirect=false", HttpMethod.GET, ProfilePhotoCallback); 
		rankLevel.text = user.level.ToString();   
		rankBest.text = user.bestScore.ToString();  
		rankId.text = user.rankId.ToString();

		if (user.rankId != 0 && user.rankId <= 3) {
			rankCup.enabled = true;
			rankCup.spriteName =  user.rankId.ToString();

		}else{
			rankCup.enabled = false;
		}
			
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
		rankTex.mainTexture = www.texture; 
	}
		

	private void SetRankData (Ranking rank) { 

		if(rank.rankId != CardGame.FREE_USER_SLOT){
			Rank.Add(rank);     
			TotalUsers += 1; 
		}

		fetch++;
		LoadProcess();   
	}

	void CreateDisplayBox (int Total) {

		float YOffset = 0f;
		for (int i = 0; i < Total; i++) {

			GameObject box = NGUITools.AddChild (rankingRoot, DisplayScoreBox);   
			Box.Add (box);  

			Transform itemtrans = box.transform;       
			itemtrans.localPosition = new Vector2 (0f, YOffset);  

			Box [i].gameObject.transform.GetChild(0).GetComponent<UILabel>().text = Rank [i].name;
			Box [i].gameObject.transform.GetChild(2).GetComponent<UILabel>().text = Rank [i].level.ToString(); 
			Box [i].gameObject.transform.GetChild(3).GetComponent<UILabel>().text = Rank [i].bestScore.ToString(); 
			Box [i].gameObject.transform.GetChild(4).GetComponent<UILabel>().text = Rank [i].rankId.ToString(); 
			Box [i].gameObject.transform.GetChild(5).GetComponent<UITexture>().mainTexture = CardGameManager._INSTANCE.charactersData[Rank[i].characterId - 1].character; 	 


			if (Rank[i].rankId != 0 && Rank[i].rankId <= 3) {
				Box [i].gameObject.transform.GetChild(6).GetComponent<UISprite>().spriteName =  Rank[i].rankId.ToString();

			}else{
				Box [i].gameObject.transform.GetChild(6).GetComponent<UISprite>().enabled = false; 

			}

			YOffset -= 105f;
		}

	}

	IEnumerator FacebookPicture (int TotalUsers) {

		Texture2D _tempTexture;
		sbyte count = 0;

		for (int i = 0; i < TotalUsers; i++) {
			WWW url = new WWW("https" + "://graph.facebook.com/v2.5/" + Rank[i].uuid + "/picture?type=square");  
			yield return url;
			_tempTexture = new Texture2D(60, 60, TextureFormat.RGB24, false); 
			url.LoadImageIntoTexture(_tempTexture); 
			FBPictures.Add(Sprite.Create(_tempTexture,new Rect(0,0,_tempTexture.width,_tempTexture.height),new Vector2(0.5f,0.5f)));
			UITexture _tempsprite = Box[i].gameObject.transform.GetChild(1).GetComponent<UITexture>();
			_tempsprite.mainTexture = FBPictures[i].texture; 
			count++; 
		}
	}
		
	private void LoadProcess(){ 

		if (fetch >= MaxUser)   
		{	
			CreateDisplayBox (TotalUsers);
			StartCoroutine (FacebookPicture (TotalUsers));  
		}
	}

	 
	private unsafe void FetchDataResponseHandler(FetchUserDataResponse* res)
	{
		switch (res->code)
		{
		case FetchDataCode.FETCH_DATA_CODE_USERDATA:
			CardGame.user = res->user;
			if (fetchUserDataCallback != null) fetchUserDataCallback(res->code);
			break;
		}
	}


	private unsafe void LobbyActionResponseHandler(LobbyActionResponse* res) 
	{
		switch (res->code)
		{
		case LobbyActionCode.LOBBY_ACTION_CODE_TOTAL:
			Debug.Log("Total user " + res->total); 
			MaxUser = res->total;
			FetchRank(); 
			break;
		case LobbyActionCode.LOBBY_ACTION_CODE_RANK:
			CardGame.rank = res->rank;
			RequestUser();
			if (fetchRankingDataCallback != null) fetchRankingDataCallback(res->code);  
			break;
		}
	}

	Action<LobbyActionCode> fetchRankingDataCallback = null;
	private unsafe void RequestFectchRanking(Action<LobbyActionCode> response)  
	{	
		fetchRankingDataCallback = response; 
		for (sbyte i = 1; i <= MaxUser; i++) {
			LobbyActionRequest lobbyActionReq = new LobbyActionRequest() { header = Header.LOBBY_ACTION_HEADER, code = LobbyActionCode.LOBBY_ACTION_CODE_RANK, id = i}; 
			Star.SendData(&lobbyActionReq, sizeof(LobbyActionRequest));		
		} 
	}

	private void OnQuickRoomClick()
	{	
		
		
			unsafe
			{
			QuickJoinRoomRequest qjr = new QuickJoinRoomRequest() { header = Header.QUICK_JOIN_ROOM_HEADER };
			Star.SendData(&qjr, sizeof(QuickJoinRoomRequest));
			}
	
	} 

	private void ErrorFetch(){
		BoxDialog._INSTANCE.ShowMessageDialog("error", "can't fecth ranking", "from server", "please login again", "OK", delegate() 
		{
			CardGameManager._INSTANCE.FadeScreen(delegate() 
			{
				CardGame.LoadScene(SceneName.Login);
			}); 
		});
	}

	private unsafe void DataCallback(int idx, void* buffer, uint size)
	{
		print("LobbyManager DataCallback: " + *(Header*)buffer);
		switch (*(Header*)buffer)
		{
		case Header.FETCH_DATA_HEADER:
			FetchDataResponseHandler((FetchUserDataResponse*)buffer);
			break; 
		case Header.QUICK_JOIN_ROOM_HEADER:
			QuickJoinRoomHandler((QuickJoinRoomResponse*)buffer);
			break;
		case Header.LOBBY_ACTION_HEADER:
			LobbyActionResponseHandler((LobbyActionResponse*)buffer);  
			break;
		case Header.SHOP_ACTION_HEADER:
			ShopResponseHandler((ShopResponse*)buffer);    
			break;
		}
	}

	private unsafe void ShopResponseHandler(ShopResponse* res) 
	{
		print("ShopResponseHandler: " + res->code);
		switch (res->code)
		{
		case ShopCode.SHOP_ACTION_CODE_SHOP:  
			Debug.Log("Backdrop Id " + res->charcater_id + " level status " + res->level_status + " gold status " + res->gold_status + " character status " + res->charcater_status); 
			shop.UnlockedItem(res->charcater_id, res->level_status, res->gold_status,res->charcater_status);    
			break;
		case ShopCode.SHOP_ACTION_CODE_CHARCATER:    
			Debug.Log("SHOP_ACTION_CODE_CHARCATER"); 
			break;
		}
	}


	private unsafe void QuickJoinRoomHandler(QuickJoinRoomResponse* res)
	{
		switch (res->code)
		{
		case QuickJoinRoomResponseCode.QUICK_JOIN_ROOM_CODE_OK:
			CardGame.room = res->room;
			CardGameManager._INSTANCE.FadeScreen(delegate() 
				{
					CardGame.LoadScene(SceneName.Game); 
				});
			break;
		case QuickJoinRoomResponseCode.QUICK_JOIN_ROOM_CODE_NOT_ENOUGH_GOLD:
			break;
		case QuickJoinRoomResponseCode.QUICK_JOIN_ROOM_CODE_FULL:
			break;
		case QuickJoinRoomResponseCode.QUICK_JOIN_ROOM_CODE_ERROR:
			break;
		}
	}


	private void StatusChangeCallback(SCStatus changed)
	{
		print("LobbyManager StatusChangeCallback");
	}


	private void ErrorCallback(SCError error, string msg)
	{
		print("LobbyManager ErrorCallback: " + error);
		switch (error)
		{
		case SCError.SERVER_FAIL:
		case SCError.DISCONNECTED_FROM_SERVER:
		case SCError.CONNECTION_TIMEOUT:
		case SCError.CREATE_CONNECTION_FAIL:
			Recon();
			break;
		}
	}

	public static void Logout(){

		CardGameManager._INSTANCE.FadeScreen(delegate() 
			{	
				FB.LogOut();
				if (Application.platform == RuntimePlatform.Android){Application.Quit();}else{Application.Quit();}
		});

	}

	private void Recon()
	{
		StarManager._INSTANCE.ResetClientStatus(delegate()
			{
				print("reconnected");
				CardGame.LoadScene(SceneName.Login);
			});
	}


}

