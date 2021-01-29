using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{

	public float time = .2f;
	// Use this for initialization
	void Start ()
	{
		Destroy (this.gameObject, time);
	}
}
