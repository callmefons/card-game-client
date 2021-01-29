using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour {

	public UIButton BuyComfirmBtn;
	public UIButton ConfirmButton; 

	//Player
	public UITexture player_characterTexture; 

	private sbyte currentCharcater = 1; 

	//Shop Backdrop
	public GameObject[] characters;  
	public GameObject[] charactersDes;      


	private void Awake() 
	{	
		EventDelegate.Add(ConfirmButton.onClick, delegate() {  
			Confirm();        
		}); 	

		for (sbyte i = 1; i <= characters.Length; i++) {  
			AddEventSystem (characters[i-1].transform.FindChild("Select Button").GetComponent<UIButton>(), i);    	       
		}  
	}


	void AddEventSystem(UIButton btn, sbyte index) {  
	 	
		if(btn.name == "Select Button"){     
			EventDelegate.Add(btn.onClick, delegate() { 
				SelectBackDrop(index);      
			}); 	
		}
	}

	public unsafe void CheckUnLockItem(){  
		
		for (sbyte i = 1; i <= characters.Length; i++) {    
			ShopRequest backdrop_req = new ShopRequest() { header = Header.SHOP_ACTION_HEADER, code = ShopCode.SHOP_ACTION_CODE_SHOP, charcater_id = i};   
			Star.SendData(&backdrop_req, sizeof(ShopRequest));  	 
		}
	}

	public void UpdateSetUser(UserData user){ 
		if(player_characterTexture.mainTexture != null){  
			player_characterTexture.mainTexture = CardGameManager._INSTANCE.charactersData[user.character - 1].character; 
			SelectedTexture.transform.SetParent (characters[user.character-1].transform);   
			SelectedTexture.transform.localPosition = new Vector2(0f,0f);  
			currentCharcater = user.character;     
		}

		for(sbyte i = 0; i < charactersDes.Length; i++){
			if(i == user.character - 1){
				charactersDes[i].gameObject.SetActive(true);
			}else{
				charactersDes[i].gameObject.SetActive(false);
			}
		}

				
	}
		
		
	public void UnlockedItem(sbyte id, sbyte level_status, sbyte gold_status, sbyte character_status){

		if (level_status == 1) {  
			characters [id - 1].transform.FindChild ("LockTexture").gameObject.SetActive (false);    
		} 

		//
		if (character_status == 1) {  
			characters [id - 1].transform.FindChild ("Buy Button").gameObject.SetActive (false); 
			characters [id - 1].transform.FindChild ("Select Button").gameObject.SetActive (true);  
		} else {
			characters [id - 1].transform.FindChild ("Buy Button").gameObject.SetActive (true); 
			characters [id - 1].transform.FindChild ("Select Button").gameObject.SetActive (false);  
		}

		//
		if (gold_status == 1 && level_status == 1) { 
			characters [id - 1].transform.FindChild ("Buy Button").GetComponent<UIButton> ().enabled = true;  
			characters [id - 1].transform.FindChild ("Buy Button").GetComponent<UI2DSprite> ().alpha = 1f; 
		} else { 
			characters [id - 1].transform.FindChild ("Buy Button").GetComponent<UIButton> ().enabled = false; 
			characters [id - 1].transform.FindChild ("Buy Button").GetComponent<UI2DSprite> ().alpha = .5f;   
		}
	}

	public UI2DSprite SelectedTexture; 

	void SelectBackDrop(sbyte i)  
	{	 
		//Debug.Log ("Select Backdrop " + i);
		player_characterTexture.mainTexture = CardGameManager._INSTANCE.charactersData[i-1].character; 
		SelectedTexture.transform.SetParent (characters[i-1].transform); 
		SelectedTexture.transform.localPosition = new Vector2(0f,0f); 

		for(sbyte j = 0; j < characters.Length; j++){
			if(j == (i-1)){
				charactersDes[j].gameObject.SetActive(true);
			}else{
				charactersDes[j].gameObject.SetActive(false);
			}
		}	

		currentCharcater = i;   
	}

	public void Confirm()   
	{	
		Debug.Log ("Confirm"); 
		
		unsafe
		{	
			ShopRequest backdrop_req = new ShopRequest() { header = Header.SHOP_ACTION_HEADER, code = ShopCode.SHOP_ACTION_CODE_CHARCATER, charcater_id = (sbyte)(currentCharcater)};       
			Star.SendData(&backdrop_req, sizeof(ShopRequest));  
		}	 
	}

	
		
}
