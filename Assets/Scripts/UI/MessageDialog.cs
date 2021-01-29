using UnityEngine;
using System.Collections;
using System;

public class MessageDialog : MonoBehaviour
{

	public UILabel title;
	public UILabel message;
	public UILabel buttonLabel;
	public UIButton okButton;


	private UIPanel cache;

	public UIPanel Panel {
		get {
			if (cache == null) {
				cache = GetComponent<UIPanel> ();
			}
			return cache;
		}
	}

	public void Setup (string title, string line_1, string line_2, string line_3, string okBtnLabel, Action okCallback)
	{
		this.title.text = title;
		this.message.text = line_1 + "\n" + line_2 + "\n" + line_3;
		this.buttonLabel.text = okBtnLabel;
		EventDelegate.Add (this.okButton.onClick, delegate() {
			if (okCallback != null)
				okCallback ();
			Destroy (gameObject);
		});
	}


}
