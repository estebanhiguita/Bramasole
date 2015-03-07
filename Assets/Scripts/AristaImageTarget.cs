using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tatacoa;

public class AristaImageTarget : MonoBehaviour, ITrackableEventHandler 
{
	public TrackableBehaviour trackable;
	public ImageTracker imageTracker;
	public DataSet dataSet;

	public GameObject canvas;


	public bool targetFound = false;

	// Use this for initialization
	public void Awake () {
		if (trackable)
			trackable.RegisterTrackableEventHandler (this);
	}

	public void Start ()
	{

	}

	void OnTargetFound ()
	{
		canvas.SetActive (true);
	}

	void OnTargetLost ()
	{
		canvas.SetActive (false);
	}

	public bool TRACK = false;
	public void Update ()
	{
		if (TRACK)
		{
			TRACK = false;
			tracking = true;
		}
	}



	public void OnTrackableStateChanged (TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
		    newStatus == TrackableBehaviour.Status.TRACKED ||
		    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED )
		{
			OnTargetFound();
		}
		else
		{
			OnTargetLost();
		}
	}

	bool _tracking = true;
	public bool tracking {
		get {
			return _tracking;
		}
		set {
			if (value == _tracking)
				return;

			trackable.enabled = _tracking = value;

		}
	}

}

public class LocalImageTargetJS 
{
	public int version;
	public string imageId;
	public string id;
	public string name;
	public string datId;
	public string xmlId;
}

