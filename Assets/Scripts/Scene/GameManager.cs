using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook.MiniJSON;
using System;

public class GameManager : MonoBehaviour {

	public UIManager _UIManager;
	public UIButton backButton;

	//Server Setting
	private uint lastestUpdateId = 0;
	private bool forcePull = false;
	private bool first = true;

	//Pile cards
	public UILabel coinsRoom;
	public UILabel remainCard;
	public UITexture pileCard; 
	public UILabel pileId;
	public UILabel pileClubs; 

	//Timer
	public GameObject timeWidget;
	public UILabel time; 
	public UISprite timeSprite; 
	private float startTime = 10f;
	private float timeLeft;
	private bool timer;

	//Object button
	public UIButton[] buttons; 
	private bool[] canMove = new bool[5];

	//Player Setting
	public GameSeat[] seats; 
	private int[] rank = new int[CardGame.MAX_SEAT];  

	private bool bgSound = false;
	private bool onTouch = true; 
	private bool onceResult = true;
	public Color c1, c2;

	[HideInInspector]
	public bool onCompleteDrawCard = false;
	private bool drawCard = false; 
	//Beta
	short cardInRoom = 60;

	private void Awake()
	{
		unsafe { StarManager._INSTANCE.OnReceiveData = DataCallback; }
		StarManager._INSTANCE.OnStatusChange = StatusChangeCallback; 
		StarManager._INSTANCE.OnStarError = ErrorCallback; 

	}

	void Start()
	{	
		CardGameManager._INSTANCE.InitFade(); 

		_UIManager = _UIManager.GetComponent<UIManager> (); 

		EventDelegate.Add(buttons[0].onClick, delegate() { SendData(PlayerActionCode.PLAYER_GAME_ACTION_VALUE_0); });  
		EventDelegate.Add(buttons[1].onClick, delegate() { SendData(PlayerActionCode.PLAYER_GAME_ACTION_VALUE_1); });  
		EventDelegate.Add(buttons[2].onClick, delegate() { SendData(PlayerActionCode.PLAYER_GAME_ACTION_VALUE_2); });  
		EventDelegate.Add(buttons[3].onClick, delegate() { SendData(PlayerActionCode.PLAYER_GAME_ACTION_VALUE_3); });  
		EventDelegate.Add(buttons[4].onClick, delegate() { SendData(PlayerActionCode.PLAYER_GAME_ACTION_VALUE_4); }); 

		EventDelegate.Add(backButton.onClick, BackClick);
		StartCoroutine(Pulling());
	}


	private LeaveRoomRequest lrr = new LeaveRoomRequest() { header = Header.LEAVE_ROOM_HEADER };
	private unsafe void BackClick()
	{
		fixed (void* v = &lrr)
		{
			Star.SendData(v, sizeof(PlayerActionRequest));
		}
	}

	private unsafe void DataCallback(int idx, void* buffer, uint size)
	{
		switch (*(Header*)buffer)
		{
		case Header.PLAYER_GAME_ACTION_HEADER:
			PlayerActionResponseHandle((PlayerActionResponse*)buffer);
			break;
		case Header.GAME_PULLING_HEADER:
			PullingResponseHandler(buffer);
			break;
		}
	}

	private unsafe void PlayerActionResponseHandle(PlayerActionResponse *res)
	{
		switch (res->code)
		{
		case PlayerActionCode.PLAYER_GAME_ACTION_FOLD:
			break;
		case PlayerActionCode.PLAYER_GAME_ACTION_VALUE_0:
			Debug.Log(res->code); 
			break;
		case PlayerActionCode.PLAYER_GAME_ACTION_VALUE_1:
			Debug.Log(res->code);
			break;
		case PlayerActionCode.PLAYER_GAME_ACTION_VALUE_2:
			Debug.Log(res->code);
			break;
		case PlayerActionCode.PLAYER_GAME_ACTION_VALUE_3:
			Debug.Log(res->code); 
			break;
		case PlayerActionCode.PLAYER_GAME_ACTION_VALUE_4:
			Debug.Log(res->code); 
			break;
		}
	}



	private void FixedUpdate()
	{	
		//Timer
		if (timeWidget != null) { 
			if (timer) {
				if (timeLeft >= 0) {
					timeLeft -= Time.fixedDeltaTime;
					timeSprite.fillAmount = timeLeft * 0.1f;
					timeSprite.color = Color.Lerp(c1,c2,timeLeft*.1f); 
				} 
				time.text = Mathf.Floor (timeLeft % 60).ToString ("0");	 
			} else {
				timeLeft = startTime;
				time.text = startTime.ToString ();
				timeSprite.fillAmount = startTime;
			}
		} 

	}


	private void UpdateAll()
	{
		Cards lastcard = CardGame.room.lastCardIndex;
		int offset = CardGame.room.seatId; 
		int active = CardGame.CARD_ACTIVE_TRUE;
		sbyte layout = CardGame.room.layout; 
		//sbyte status = CardGame.room.statusCard;

		//// Setting Room ////
		if (CardGame.room.gameState != GameState.INIT_MATCH) { 
			remainCard.text = "X " + (CardGame.room.remainCard - cardInRoom);  
			coinsRoom.text = CardGame.room.coinsRoom.ToString ();
		} 
		//// Setting Player ////
		for (sbyte i = 0; i < CardGame.MAX_SEAT; i++) 
		{	
			seats[i].userId = CardGame.room.GetPlayer (offset).user.userId;
			seats[i].uname.text = CardGame.room.GetPlayer (offset).user.name;
			seats [i].ulevel.text = CardGame.room.GetPlayer (offset).user.level.ToString();
			seats [i].totalCard.text = CardGame.room.GetPlayer (offset).remainCardPlayer.ToString();

			if (CardGame.room.GetPlayer(offset).user.userId != CardGame.FREE_USER_SLOT) {
				seats[i].upicture.mainTexture = CardGameManager._INSTANCE.charactersData[CardGame.room.GetPlayer(offset).user.character - 1].character;    
			}
			else
			{	
				seats[i].upicture.mainTexture = null; 			
			}
			seats[i].upicture.gameObject.SetActive(CardGame.room.GetPlayer(offset).user.userId != CardGame.FREE_USER_SLOT); 

			offset = (offset + 1) % CardGame.MAX_SEAT;  
		}

		if (CardGame.room.gameState == GameState.WAIT) {
			Debug.Log ("GameState.WAIT");
			_UIManager.gameState.GetComponent<UILocalize> ().key = "StateWait";


		}

		if (CardGame.room.gameState == GameState.LOADING) {
			Debug.Log ("GameState.LOADING");
			_UIManager.gameState.GetComponent<UILocalize> ().key = "StateLoading";
			_UIManager.gameState.GetComponent<UILabel> ().text = Localization.Get (_UIManager.gameState.GetComponent<UILocalize> ().key); 
			Debug.Log (_UIManager.gameState.GetComponent<UILocalize> ().key);

			if( PrefManager.Lifes > 0 ){
				PrefManager._INSTANCE.SpendLife(1); 
				Debug.Log( "SpendLife" );
			} 
			_UIManager.SettingLayout (layout);   	
		}
		if (CardGame.room.gameState == GameState.INIT_MATCH) {
			Debug.Log ("GameState.INIT_MATCH");
			_UIManager.OnInitGame ();
			bgSound = true;
			Debug.Log ("bgSound" + bgSound);


		}
		if (CardGame.room.gameState == GameState.FLOD_CARD) {
			Debug.Log ("GameState.FLOD_CARD");
			_UIManager.ItweenFoldCard (); 
			timer = false;
			timeSprite.gameObject.SetActive(false);
			pileId.text = "";
			pileClubs.text = "";
			drawCard = true; 
			_UIManager.ResetMove ();   

			for (int i = 0; i < buttons.Length; i++) {
				buttons [i].gameObject.SetActive (true);
			}

		}
	
		if (CardGame.room.gameState == GameState.IN_MATCH || CardGame.room.gameState == GameState.CALCULATE_MATCH) {
			Debug.Log ("GameState.IN_MATCH");
			if(drawCard){  
				_UIManager.ItweenDrawCard ();
				pileId.text = lastcard.id.ToString ();
				pileClubs.text = lastcard.club.ToString();
				pileCard.mainTexture = Resources.Load ("Cards/" + layout + "/"+ lastcard.id) as Texture;  

				if(onCompleteDrawCard){
					timer = true; 
					TimerAlarm.LazyTimer (.5f, delegate() {
						timeSprite.gameObject.SetActive(true); 
					});
					for (int i = 0; i < buttons.Length; i++) {
						buttons [i].GetComponent<UIButton> ().enabled = true;
					}
					for (int i = 0; i < canMove.Length; i++) { 
						canMove [i] = true;
					}
					onCompleteDrawCard = false;
				}

				drawCard = false;
					
			}

			if (CardGame.room.GetButton (0) == CardGame.STATUS_DEFAULT && canMove[0]) CheckStatus (0);
			if (CardGame.room.GetButton (1) == CardGame.STATUS_DEFAULT && canMove[1]) CheckStatus (1);
			if (CardGame.room.GetButton (2) == CardGame.STATUS_DEFAULT && canMove[2]) CheckStatus (2);
			if (CardGame.room.GetButton (3) == CardGame.STATUS_DEFAULT && canMove[3]) CheckStatus (3);
			if (CardGame.room.GetButton (4) == CardGame.STATUS_DEFAULT && canMove[4]) CheckStatus (4);


			if(bgSound){
				SoundControl.shared.PlayBackgroundSound ("InMatchSound");
				bgSound = false;
			}

			onTouch = true;
						
		}

		if(CardGame.room.gameState == GameState.CALCULATE_MATCH){ 
			Debug.Log ("GameState.CALCULATE_MATCH");
		}

		if(CardGame.room.gameState == GameState.POST_MATCH){
			Debug.Log ("GameState.POST_MATCH");
			_UIManager.OnPostMatch ();

		}
			
		if(CardGame.room.gameState == GameState.SHOW_RESULT){
			Debug.Log ("GameState.SHOW_RESULT");
			_UIManager.ShowResult (); 
			_UIManager.ResetMove (); 


			for (sbyte i = 0; i < seats.Length; i++) {	 

				seats [i].winnerSeatId = CardGame.room.GetPlayer (offset).winnerSeatId;
				seats [i].xp.text = CardGame.room.GetPlayer (offset).exp.ToString();   
				seats [i].coins.text = CardGame.room.GetPlayer (offset).coins.ToString();   

				_UIManager.SetRank (i,seats[i].winnerSeatId);   
				offset = (offset + 1) % CardGame.MAX_SEAT;  
			}

			if(onceResult){
				onceResult = false;
				SoundControl.shared.PlayEffectSound ("Result");
			}
				
		}
		
	}

	private void CheckStatus(int i){
		SetActive(buttons[i]);
		if (CardGame.room.GetStatus(i) == CardGame.STATUS_TRUE) {
			canMove[i] = false;
			_UIManager.MoveTrue(i, CardGame.room.GetListener(i));
		}
		if (CardGame.room.GetStatus(i) == CardGame.STATUS_FALSE) {
			canMove[i] = false;
			_UIManager.MoveFalse(i, CardGame.room.GetListener(i));
		}
	}

	private void SetActive(UIButton btn){ 
		btn.gameObject.SetActive (false); 
	}


	PlayerActionRequest pgr = new PlayerActionRequest() { header = Header.PLAYER_GAME_ACTION_HEADER };
	private unsafe void SendData(PlayerActionCode playerAction)
	{	
		int offset = CardGame.room.seatId; 

		if(CardGame.room.GetPlayer(offset).action != PlayerAction.FOLD && onTouch) {

			onTouch = false;
			pgr.code = playerAction;
			fixed (void* v = &pgr)
			{
				Star.SendData(v, sizeof(PlayerActionRequest));
			}	
		}
	}

	private IEnumerator Pulling()
	{
		float interval = 2;
		float next = Time.realtimeSinceStartup - 1;

		while (CardGame.room.roomId != CardGame.NO_ROOM && CardGame.room.seatId != CardGame.NO_SEAT)
		{
			if (Time.realtimeSinceStartup > next || forcePull)
			{
				forcePull = false;
				next = Time.realtimeSinceStartup + interval;
				SendPulling();
			}
			yield return null;
		}

		// leave
		Debug.Log("Leave??"); 
		CardGameManager._INSTANCE.FadeScreen(delegate()
			{
				CardGame.LoadScene(SceneName.Lobby); 
		});
	}

	GamePullingGameRequest ppr = new GamePullingGameRequest() { header = Header.GAME_PULLING_HEADER, lastestUpdateId = 0 };
	private unsafe void SendPulling()
	{
		ppr.lastestUpdateId = lastestUpdateId;
		fixed (void* v = &ppr)
		{
			Star.SendData(v, sizeof(GamePullingGameRequest));
		}
	}

	private unsafe void PullingResponseHandler(void* buffer)
	{
		GamePullingLatestGameResponse* latest = (GamePullingLatestGameResponse*)buffer;
		switch (latest->code)
		{
		case GamePullingGameResponseCode.GAME_PULLING_CODE_UPDATE:
			GamePullingUpdateGameResponse* update = (GamePullingUpdateGameResponse*)buffer;

			if (lastestUpdateId != update->room.lastestUpdateId)
			{
				CardGame.room = update->room;
				Debug.Log("roomId -> " + update->room.roomId);

				lastestUpdateId = update->room.lastestUpdateId;
				if (CardGame.room.roomId != CardGame.NO_ROOM && CardGame.room.seatId != CardGame.NO_SEAT)
				{
					UpdateAll();
				}

				if (first)
				{
					first = false;
					CardGameManager._INSTANCE.FadeFirstOpen(); 

				}
			}
			forcePull = true;
			break;
		case GamePullingGameResponseCode.GAME_PULLING_CODE_LATEST: 
			forcePull = true;
			break;
		case GamePullingGameResponseCode.GAME_PULLING_CODE_ERROR:
			forcePull = true;
			break;
		}
	}

	private void StatusChangeCallback(SCStatus changed)
	{
		print("GameManager StatusChangeCallback: " + changed);
		switch (changed)
		{
		case SCStatus.ERROR:
			break;
		}
	}

	private void ErrorCallback(SCError error, string msg)
	{
		print("GameManager ErrorCallback: " + error);
		switch (error)
		{
		case SCError.SERVER_FAIL:
		case SCError.DISCONNECTED_FROM_SERVER:
		case SCError.CONNECTION_TIMEOUT:
		case SCError.CREATE_CONNECTION_FAIL:
			StarManager._INSTANCE.ResetClientStatus(delegate()
				{
					print("reconnected");
					BoxDialog._INSTANCE.ShowMessageDialog("error", "", "can't connect to server", "", "ok", delegate() 
					{
						CardGameManager._INSTANCE.FadeScreen(delegate()
						{
							CardGame.LoadScene(SceneName.Login);
						});
					});
				});
			break;
		}
	}
}
