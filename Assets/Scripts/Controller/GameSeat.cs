using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameSeat : MonoBehaviour
{
	public UILabel uname;
	public UILabel ulevel;
	public UITexture upicture;
	public UISprite totalCardSrite;
	public UILabel totalCard;
	public UISprite wrongIcon;
	public UI2DSprite correctIcon;
	//Result Score
	public UILabel xp;
	public UILabel coins;

	private uint _userId;
	private sbyte _winnerSeatId;
	private uint _winCount;

	public uint userId {
		get { return _userId; }
		set { _userId = value; }
	}

	public sbyte winnerSeatId {
		get { return _winnerSeatId; }
		set { _winnerSeatId = value; }
	}

	public uint winCount {
		get{ return _winCount; }
		set{ _winCount = value; }
	}


}
