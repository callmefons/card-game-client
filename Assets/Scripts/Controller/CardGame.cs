using UnityEngine;


public enum SceneName : int
{
	Splash = 0,
	Login = 1,
	Lobby = 2,
	Game = 3,
	Tutorial = 4
}

public enum GameState : sbyte
{
	WAIT = 0,
	// WAIT 4 PLAYER JOIN
	LOADING = 1,
	//GAME_STATE_LOADING
	INIT_MATCH = 2,
	// INIT GAME SETTING
	FLOD_CARD = 3,
	// PRE MATCH
	IN_MATCH = 4,
	// PLAYING
	CALCULATE_MATCH = 5,
	// CALCULATE RESULT
	POST_MATCH = 6,
	// SHOW RESULT
	SHOW_RESULT = 7,
	// SHOW RESULT
	END_GAME = 8,
	// END GAME
	ERROR = -1
	// ERROR
}


public enum PlayerAction : sbyte
{
	NONE = -10,
	FOLD = -1,
	PLAYER_ACTION_VALUE_0 = 0,
	PLAYER_ACTION_VALUE_1 = 1,
	PLAYER_ACTION_VALUE_2 = 2,
	PLAYER_ACTION_VALUE_3 = 3,
	PLAYER_ACTION_VALUE_4 = 4

}

public class CardGame
{

	public static void LoadScene (SceneName scene)
	{
		DTDebug.Log ("Load Scene: " + scene.ToString ());
		Application.LoadLevel ((int)scene);
	}

	public const string IP = "128.199.223.148";
	public const ushort PORT = 9009;

	public const byte UUID_LENGTH = 128;
	public const byte ROOM_NAME_LENGTH = 32;
	public const byte USER_NAME_LENGTH = 32;
	public const byte MIN_NAME_LENGTH = 4;

	public const int PLAYER_DATA_STRUCT_SIZE = 528; //data struct size 5 cards = 252 // data struct size 80 = 528
	public const sbyte CARDS_DATA_STRUCT_SIZE = 2;
	//
	public const sbyte FREE_USER_SLOT = 0;
	// AVATAR
	public const sbyte DEFAULT_CHARACTER_ID = 1;
	public const sbyte NONE_EQUIP = 0;

	// PLAYING
	public const sbyte NO_GAME = 0;
	public const sbyte NO_ROOM = -1;
	public const sbyte NO_SEAT = -1;
	public const sbyte ONLINE = 1;
	public const sbyte OFFLINE = 0;
	public const sbyte NO_CARD = 0;

	// CONFIG
	public const short MAX_ROOM = 64;
	public const sbyte MAX_CARD = 80; // true 60
	public const sbyte MAX_SEAT = 4;
	public const short MAX_PLAYERS = 2 * MAX_ROOM * MAX_SEAT; 
	public const sbyte MAX_BUTTON = 5;
	public const uint LEVELUP = 1000;

	public const sbyte CARD_ACTIVE_TRUE = 1;
	public const sbyte CARD_ACTIVE_FALSE = 0;
	public const sbyte WINNER = 1;
	public const sbyte LOSER = -1;

	//STATUS ANSWER CARD
	public const int STATUS_DEFAULT = 0;
	public const int STATUS_TRUE = 100;
	public const int STATUS_FALSE = 1;

	public static UserData user;
	public static Ranking rank; 
	public static GameRoomData room;
	public static Cards card; 
}

public unsafe struct UserData
{
	private uint _userId;
	private fixed byte _uuid[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _email[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _name[CardGame.USER_NAME_LENGTH * sizeof(byte)];
	private sbyte _character;
	private uint _rankId;
	private uint _gold;
	private uint _exp;
	private uint _level;
	private uint _winCount;
	private uint _loseCount;
	private uint _timeRemainGold;
	private uint _coins;
	private uint  _bestScore;

	public uint userId {
		get { return _userId; }
	}

	public string uuid {
		get {
			fixed (byte* b = _uuid) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}

	public string email { 
		get {
			fixed (byte* b = _email) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}


	public string name {
		get {
			fixed (byte* b = _name) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}

	public sbyte character {
		get { return _character; } 
	}

	public uint rankId {
		get { return _rankId; }
	}

	public uint gold {
		get { return _gold; }
	}

	public uint exp {
		get { return _exp; }
	}

	public uint level {
		get { return _level; }
	}

	public uint winCount {
		get { return _winCount; }
	}

	public uint loseCount {
		get { return _loseCount; }
	}

	public uint timeRemainGold {
		get { return _timeRemainGold; }
	}

	public uint coins{
		get { return _coins; }
	}


	public uint bestScore{
		get { return _bestScore; }
	}
		
}

public unsafe struct Cards
{	
	private sbyte _id;
	private sbyte _club;

	public sbyte id {
		get{ return _id; }
	}

	public sbyte club {
		get{ return _club; }
	}

}

public unsafe struct PlayerData
{
	private UserData _user;

	private sbyte _online;
	private fixed byte _card[CardGame.MAX_CARD * CardGame.CARDS_DATA_STRUCT_SIZE];
	private PlayerAction _action;
	private uint _exp;
	private uint _level;
	private uint _winCount;
	private uint _loseCount;
	private short _nextCardIndexPlayer;
	private uint _remainCardPlayer;
	private uint  _coins;
	private sbyte _winnerSeatId;

	public UserData user {
		get { return _user; }
	}

	public sbyte online { 
		get { return _online; } 
	}


	public Cards GetCard (int card)
	{	
		if (CardGame.CARDS_DATA_STRUCT_SIZE != sizeof(Cards))
			DTDebug.LogWarning ("Config 'CARDS_DATA_STRUCT_SIZE'(" + CardGame.CARDS_DATA_STRUCT_SIZE + ") != sizeof(Cards)(" + sizeof(Cards) + ")");

		fixed (byte* u = _card)
			return *(Cards*)&u [card * sizeof(Cards)];
	}

	public PlayerAction action {
		get { return _action; } 
	}

	public uint exp {
		get { return _exp; }
	}

	public uint level {
		get { return _level; }
	}

	public uint winCount {
		get{ return _winCount; }
	}

	public uint loseCount {
		get{ return _loseCount; }
	}

		
	public short nextCardIndex {
		get{ return _nextCardIndexPlayer; }
	}

	public uint remainCardPlayer {
		get{ return _remainCardPlayer; }
	}

	public uint coins{
		get { return _coins; }
	}

	public sbyte winnerSeatId { 
		get { return _winnerSeatId; } 
	}

}

public unsafe struct GameRoomData
{
	private short _roomId;
	private short _seatId;
	private uint _gameId;
	private fixed byte _name[CardGame.ROOM_NAME_LENGTH * sizeof(byte)];
	private sbyte _minChip;
	private sbyte _gameState;
	private sbyte _currentPlayer;
	private short _remainCard;
	private Cards _lastCardIndex;
	private uint _lastestUpdateId;
	private fixed byte _players[CardGame.MAX_SEAT * CardGame.PLAYER_DATA_STRUCT_SIZE];
	private fixed byte _btn[CardGame.MAX_BUTTON * sizeof(byte)]; 
	private fixed byte _lsn[CardGame.MAX_BUTTON * sizeof(byte)]; 
	private uint  _coinsRoom;
	//private sbyte _statusCard;
	private fixed byte _status[CardGame.MAX_BUTTON * sizeof(byte)]; 
	private sbyte _layout;

	public short roomId {
		get { return _roomId; }
	}

	public short seatId {
		get { return _seatId; }
	}

	public uint gameId {
		get { return _gameId; }
	}

	public string name {
		get {
			fixed (byte* b = _name) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}

	public sbyte minChip {
		get { return _minChip; }
	}


	public GameState gameState {
		get { return (GameState)_gameState; }
	}

	public sbyte currentPlayer {
		get { return _currentPlayer; }
	}


	public short remainCard {
		get{ return _remainCard; }	
	}

	public Cards lastCardIndex
	{
		get{ return _lastCardIndex;}
	}

	public uint lastestUpdateId {
		get { return _lastestUpdateId; }
	}


	public PlayerData GetPlayer (int seat)
	{	
		if (CardGame.PLAYER_DATA_STRUCT_SIZE != sizeof(PlayerData))
			DTDebug.LogWarning ("Config 'PLAYER_DATA_STRUCT_SIZE'(" + CardGame.PLAYER_DATA_STRUCT_SIZE + ") != sizeof(PlayerData)(" + sizeof(PlayerData) + ")");

		fixed (byte* u = _players)
			return *(PlayerData*)&u [seat * sizeof(PlayerData)];
	}

	public byte GetButton (int button)
	{
		fixed (byte* b = _btn)
		return *(byte*)&b [button * sizeof(byte)];
	}


	public byte GetListener (int lsn)
	{
		fixed (byte* b = _lsn)
		return *(byte*)&b [lsn * sizeof(byte)]; 
	}

	public uint coinsRoom{
		get { return _coinsRoom; }
	}


	public byte GetStatus (int status)
	{
		fixed (byte* b = _status) 
		return *(byte*)&b [status * sizeof(byte)];
	}

	public sbyte layout {
		get { return _layout; } 
	}
}


public unsafe struct RegisterRequest
{
	private Header _header;
	private short _ignore16;
	private fixed byte _uuid[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _email[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _name[CardGame.USER_NAME_LENGTH * sizeof(byte)];
	private sbyte _character; 

	public Header header {
		get { return _header; }
		set { _header = value; }
	}

	public string uuid {
		get {
			fixed (byte* b = _uuid) {
				return Memory.UTF8_FromPointer (b);
			}
		}
		set {
			fixed (byte* b = _uuid) {
				Memory.UTF8_ToPointer (value, b, CardGame.UUID_LENGTH);
			}
		}
	} 

	public string email {
		get {
			fixed (byte* b = _email) {
				return Memory.UTF8_FromPointer (b);
			}
		}
		set {
			fixed (byte* b = _email) {
				Memory.UTF8_ToPointer (value, b, CardGame.UUID_LENGTH); 
			}
		}
	}


	public string name {
		get {
			fixed (byte* b = _name) {
				return Memory.UTF8_FromPointer (b);
			}
		}
		set {
			fixed (byte* b = _name) {
				Memory.UTF8_ToPointer (value, b, CardGame.USER_NAME_LENGTH);
			}
		}
	}

	public sbyte character {
		get { return _character; }
		set { _character = value; }
	}


}

public unsafe struct RegisterResponse
{
	private Header _header;
	private RegisterResponseCode _code;

	public Header header {
		get { return _header; }
	}

	public RegisterResponseCode code {
		get { return _code; }
	}
}

public unsafe struct LoginRequest
{
	private Header _header;
	private short _ignore16;
	private fixed byte _uuid[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _email[CardGame.UUID_LENGTH * sizeof(byte)];

	public Header header {
		get { return _header; }
		set { _header = value; }
	}

	public string uuid {
		get {
			fixed (byte* b = _uuid) {
				return Memory.UTF8_FromPointer (b);
			}
		}
		set {
			fixed (byte* b = _uuid) {
				Memory.UTF8_ToPointer (value, b, CardGame.UUID_LENGTH);
			}
		}
	}

	public string email {
		get {
			fixed (byte* b = _email) {
				return Memory.UTF8_FromPointer (b);
			}
		}
		set {
			fixed (byte* b = _email) {
				Memory.UTF8_ToPointer (value, b, CardGame.UUID_LENGTH);
			}
		}
	}
}

public unsafe struct LoginResponse
{
	private Header _header;
	private LoginResponseCode _code;
	private uint _latestGameId;
	private short _latestRoomId;
	private short _latestSeatId;
	private UserData _user;

	public Header header {
		get { return _header; }
	}

	public LoginResponseCode code {
		get { return _code; }
	}

	public uint latestGameId {
		get { return _latestGameId; }
	}

	public short latestRoomId {
		get { return _latestRoomId; }
	}

	public short latestSeatId {
		get { return _latestSeatId; }
	}

	public UserData user {
		get { return _user; }
	}
}

public unsafe struct QuickJoinRoomRequest
{
	private Header _header;
	private short _ignore16;

	public Header header {
		get { return _header; }
		set { _header = value; }
	}
}

public unsafe struct QuickJoinRoomResponse
{
	private Header _header;
	private QuickJoinRoomResponseCode _code;
	private GameRoomData _room;

	public Header header {
		get { return _header; }
	}

	public QuickJoinRoomResponseCode code {
		get { return _code; }
	}

	public GameRoomData room {
		get { return _room; }
	}
}

public unsafe struct LeaveRoomRequest
{
	private Header _header;
	private short _ignore;

	public Header header {
		get { return _header; }
		set { _header = value; }
	}
}

public unsafe struct FetchDataRequest
{
	private Header _header;
	private FetchDataCode _code;

	public Header header {
		get { return _header; }
		set { _header = value; }
	}

	public FetchDataCode code {
		get { return _code; }
		set { _code = value; }
	}
}

public unsafe struct FetchUserDataResponse
{
	private Header _header;
	private FetchDataCode _code;
	private UserData _user;

	public Header header {
		get { return _header; }
	}

	public FetchDataCode code {
		get { return _code; }
	}

	public UserData user {
		get { return _user; }
	}
}

public unsafe struct LobbyActionRequest
{
	private Header _header;
	private LobbyActionCode _code;
	private sbyte _id;

	public Header header
	{
		get { return _header; }
		set { _header = value; }
	}
	public LobbyActionCode code
	{
		get { return _code; }
		set { _code = value; }
	}

	public sbyte id {
		get{ return _id; }
		set { _id = value; }
	}

}


public unsafe struct LobbyActionResponse
{
	private Header _header;
	private LobbyActionCode _code; 
	private Ranking _rank;
	private uint _total; 

	public Header header
	{
		get { return _header; }
	}

	public LobbyActionCode code
	{
		get { return _code; }
	}

	public Ranking rank {
		get { return _rank; }
	}


	public uint total {
		get{ return _total; }
		set { _total = value; }
	}


}

public unsafe struct Ranking
{	

	private fixed byte _uuid[CardGame.UUID_LENGTH * sizeof(byte)];
	private fixed byte _email[CardGame.UUID_LENGTH * sizeof(byte)];
	private uint _rankId;
	private fixed byte _name[CardGame.USER_NAME_LENGTH * sizeof(byte)];
	private sbyte _characterId;
	private uint _level;
	private uint  _bestScore; 

	public string uuid {
		get {
			fixed (byte* b = _uuid) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}


	public string email {
		get {
			fixed (byte* b = _email) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}

	public uint rankId {
		get { return _rankId; }
	}


	public string name {
		get {
			fixed (byte* b = _name) {
				return Memory.UTF8_FromPointer (b);
			}
		}
	}

	public sbyte characterId {
		get { return _characterId; }
	}

	public uint level {
		get { return _level; }
	}


	public uint bestScore{
		get { return _bestScore; }
	}
		
}



public unsafe struct LobbyActionGoldHourlyResponse
{
	private Header _header;
	private LobbyActionCode _code;
	private UserData _user;

	public Header header
	{
		get { return _header; }
	}
	public LobbyActionCode code
	{
		get { return _code; }
	}
	public UserData user
	{
		get { return _user; }
	}
}




public unsafe struct PlayerActionRequest
{
	private Header _header;
	private PlayerActionCode _code;
	private sbyte _drawCardId;

	public Header header {
		get { return _header; }
		set { _header = value; }
	}

	public PlayerActionCode code {
		get { return _code; }
		set { _code = value; }
	}

	public sbyte drawCardId {
		get { return _drawCardId; }
		set {_drawCardId = value; }
	}
}

public unsafe struct PlayerActionResponse
{
	private Header _header;
	private PlayerActionCode _code;


	public Header header {
		get { return _header; }
	}

	public PlayerActionCode code {
		get { return _code; }
	}
}


public unsafe struct GamePullingGameRequest
{
	private Header _header;
	private short _ignore16;
	private uint _lastestUpdateId;

	public Header header {
		get { return _header; }
		set { _header = value; }
	}

	public uint lastestUpdateId {
		get { return _lastestUpdateId; }
		set { _lastestUpdateId = value; }
	}
}

public unsafe struct GamePullingUpdateGameResponse
{
	private Header _header;
	private GamePullingGameResponseCode _code;
	private GameRoomData _room;

	public Header header {
		get { return _header; }
	}

	public GamePullingGameResponseCode code {
		get { return _code; }
	}

	public GameRoomData room {
		get { return _room; }
	}
}

public unsafe struct GamePullingLatestGameResponse
{
	private Header _header;
	private GamePullingGameResponseCode _code;

	public Header header {
		get { return _header; }
	}

	public GamePullingGameResponseCode code {
		get { return _code; }
	}
}


public unsafe struct ShopRequest
{ 
	private Header _header;
	private ShopCode _code; 
	private sbyte _charcater_id; 

	public Header header {
		get { return _header; }
		set { _header = value; } 
	}

	public ShopCode code {  
		get { return _code; }
		set { _code = value; }  
	}

	public sbyte charcater_id { 
		get { return _charcater_id; }
		set { _charcater_id = value; }   
	}


}

public unsafe struct ShopResponse{

	private Header _header;  
	private ShopCode _code; 
	private sbyte _charcater_id;   
	private sbyte _level_status;  
	private sbyte _gold_status;  
	private sbyte _charcater_status;   

	public Header header {
		get { return _header; }
	}

	public ShopCode code {  
		get { return _code; }
	}

	public sbyte charcater_id {  
		get { return _charcater_id; }   
	}

	public sbyte level_status { 
		get { return _level_status; } 
	} 

	public sbyte gold_status { 
		get { return _gold_status; }  
	} 

	public sbyte charcater_status {  
		get { return _charcater_status; } 
	}

}



public enum Header : short
{
	LOGIN_HEADER = 1000,
	REGISTER_HEADER = 1001,

	CREATE_ROOM_HEADER = 2000,
	JOIN_ROOM_HEADER = 2001,
	QUICK_JOIN_ROOM_HEADER = 2002,
	LEAVE_ROOM_HEADER = 2003,

	PLAYER_GAME_ACTION_HEADER = 5000,
	GAME_PULLING_HEADER = 5001,

	FETCH_DATA_HEADER = 6000,
	LOBBY_ACTION_HEADER = 7000,
	SHOP_ACTION_HEADER = 8000
}

public enum RegisterResponseCode : short
{
	REGISTER_CODE_OK = 100,

	REGISTER_CODE_NAME_DUPLICATE = 300,
	REGISTER_CODE_NAME_SHORT = 301,

	REGISTER_CODE_ERROR = 404,
}

public enum LoginResponseCode : short
{
	LOGIN_CODE_OK = 100,
	LOGIN_CODE_REJOIN_OK = 101,
	LOGIN_CODE_REJOIN_END = 102,
	LOGIN_CODE_NEW_USER = 103,

	LOGIN_CODE_BANNED = 300,
	LOGUN_CODE_ALREADY_LOGIN = 301,

	LOGIN_CODE_ERROR = 404,
}

public enum QuickJoinRoomResponseCode : short
{
	QUICK_JOIN_ROOM_CODE_OK = 100,
	// QUICK JOIN OK

	QUICK_JOIN_ROOM_CODE_NOT_ENOUGH_GOLD = 300,
	// NOT ENOUGH GOLD
	QUICK_JOIN_ROOM_CODE_FULL = 301,
	// NO EMPTY ROOM FOR CREATE

	QUICK_JOIN_ROOM_CODE_ERROR = 404,
	// ERROR UNKNOWN CASE
}


public enum ShopCode : short
{
	SHOP_ACTION_CODE_SHOP = 100,
	SHOP_ACTION_CODE_CHARCATER = 200,
	SHOP_ACTION_CODE_ERROR = 404
}

public enum PlayerActionCode : short
{	
	PLAYER_GAME_ACTION_FOLD = 6000,
	PLAYER_GAME_ACTION_VALUE_0 = 100,
	PLAYER_GAME_ACTION_VALUE_1 = 101,
	PLAYER_GAME_ACTION_VALUE_2 = 102,
	PLAYER_GAME_ACTION_VALUE_3 = 103,
	PLAYER_GAME_ACTION_VALUE_4 = 104,
}

public enum GamePullingGameResponseCode : short
{
	GAME_PULLING_CODE_UPDATE = 100,
	GAME_PULLING_CODE_LATEST = 101,

	GAME_PULLING_CODE_ERROR = 404
}

public enum FetchDataCode : short
{
	FETCH_DATA_CODE_USERDATA = 100,

	FETCH_DATA_CODE_ERROR = 404
}

public enum LobbyActionCode : short
{
	LOBBY_ACTION_CODE_TOTAL = 100,
	LOBBY_ACTION_CODE_RANK = 200,
	LOBBY_ACTION_CODE_ERROR = 404
}