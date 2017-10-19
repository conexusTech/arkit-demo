using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationLock : MonoBehaviour
{
	public ScreenOrientation setScreenOrientation = ScreenOrientation.AutoRotation;
	public bool isDestroyGOAfterSet = true;

	private void Awake()
	{
		Screen.orientation = setScreenOrientation;
		if (isDestroyGOAfterSet)
		{
			Destroy(gameObject);
		}
	}
}
