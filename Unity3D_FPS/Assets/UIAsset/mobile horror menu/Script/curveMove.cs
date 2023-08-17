using UnityEngine;
using System.Collections;

public class curveMove : MonoBehaviour
{

	public AnimationCurve curve ;
	[Range(0,1)]
	public float
		speed = 1;
	Vector3 targetToReach;

	Vector3 currentPosition;
	float incrementStep;
	Transform thisTransform;
	public bool QuickSlideOnEnable = true ;
	public Vector3 quickSlideDistance = new Vector3 (-200, 0, 0);
	void Start ()
	{

		thisTransform = GetComponent<Transform> ();
		targetToReach = thisTransform.position;
		if (QuickSlideOnEnable) {

			thisTransform.Translate (quickSlideDistance);
		}



	}
	
	// Update is called once per frame
	void Update ()
	{


		thisTransform.position = Vector3.Lerp (thisTransform.position, targetToReach, curve.Evaluate (incrementStep));


		incrementStep += speed * Time.deltaTime;

		if (incrementStep >= 1)
			Destroy (this);
	
	}
}
