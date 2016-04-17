using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DoorController : MonoBehaviour {

	private Dictionary<string, Vector3> _initialDoorPositions = new Dictionary<string, Vector3>();
    private GameObject _rightDoor;
    private GameObject _leftDoor;

	public float DOOR_OPENING_TIME_SECONDS = 1;
	private List<IEnumerator> _coroutines = new List<IEnumerator>();

    // Use this for initialization
    void Start () 
	{
		//get initial positions of doors
		_leftDoor = transform.FindChild("leftDoor").gameObject;
		_rightDoor = transform.FindChild("rightDoor").gameObject;
		
		_initialDoorPositions.Add("leftDoor", _leftDoor.transform.position);
		_initialDoorPositions.Add("rightDoor", _rightDoor.transform.position);				
	}
	
		
	public void Open()
	{
		foreach(var coroutine in _coroutines)
		{
			StopCoroutine(coroutine);
		}
		_coroutines.Clear();

		var renderer = _leftDoor.GetComponent<Renderer>();		
		_coroutines.Add(OpenDoor(_leftDoor, _leftDoor.transform.position.x - renderer.bounds.size.x));
		renderer = _rightDoor.GetComponent<Renderer>();		
		_coroutines.Add(OpenDoor(_rightDoor, _rightDoor.transform.position.x + renderer.bounds.size.x));
		
		foreach(var coroutine in _coroutines)
		{
			StartCoroutine(coroutine);
		}
	}
	public void Close()
	{
		foreach(var coroutine in _coroutines)
		{
			StopCoroutine(coroutine);
		}
		_coroutines.Clear();
		
		_coroutines.Add(OpenDoor(_leftDoor, _initialDoorPositions["leftDoor"].x));
		_coroutines.Add(OpenDoor(_rightDoor, _initialDoorPositions["rightDoor"].x));
		
		foreach(var coroutine in _coroutines)
		{
			StartCoroutine(coroutine);
		}
	}
	
	
	IEnumerator OpenDoor(GameObject door, float xDestination)
	{
		Func<GameObject, Boolean> notThereYet;
		
		if(xDestination < door.transform.position.x)
			notThereYet = (gameObject) => door.transform.position.x > xDestination;
		else
			notThereYet = (gameObject) => door.transform.position.x < xDestination;
		
		var increment = (xDestination - door.transform.position.x) / DOOR_OPENING_TIME_SECONDS;
			
		while(notThereYet(door))
		{
			door.transform.position = door.transform.position + new Vector3(increment * Time.deltaTime, 0,0);
			yield return null;
		}	
		
		door.transform.position = new Vector3(xDestination, door.transform.position.y,door.transform.position.z);
	}

	
	// Update is called once per frame
	void Update () {
	    if (Input.GetButtonDown("Fire1")) {
			Debug.Log("open!");
			Open();
		}
		 if (Input.GetButtonDown("Fire2")) {
			Debug.Log("close!");
			Close();
		}
	}
}
