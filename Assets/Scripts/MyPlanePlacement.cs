using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MyPlanePlacement : MonoBehaviour
{
    public UnityEvent onPlace = new UnityEvent();
    public UnityEvent onHide = new UnityEvent();

	public bool rotateToCamera = false;
	public GameObject holder;

	bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
		if (hitResults.Count > 0)
		{
			foreach (var hitResult in hitResults)
			{
				//Debug.Log("Got hit!");
				transform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
				transform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
				//Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", transform.position.x, transform.position.y, transform.position.z));

				holder.SetActive(!holder.activeSelf);
                if (holder.activeSelf)
                {
                    if (onPlace != null)
                        onPlace.Invoke();
                }
                else
                {
                    if (onHide != null)
                        onHide.Invoke();
                }

				if (holder.activeSelf && rotateToCamera)
				{
					Quaternion roomRotation = Camera.main.transform.rotation;
					holder.transform.rotation = Quaternion.Euler(new Vector3(0f, roomRotation.eulerAngles.y + 180f, 0f));
				}

				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update()
	{
		ARPoint? hitTestPosition = null;
        //Vector2 touchPosition;

        if (Input.touchCount > 0 && holder != null)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began /*|| touch.phase == TouchPhase.Moved*/)
            {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                // Make a safe area
                if (screenPosition.x > 0.2f &&
                    screenPosition.x < 0.8f &&
                    screenPosition.y > 0.2f &&
                    screenPosition.y < 0.8f)
                {
                    hitTestPosition = new ARPoint
                    {
                        x = screenPosition.x,
                        y = screenPosition.y,
                    };
                }
            }
        }

			if (hitTestPosition.HasValue)
			{
				// prioritize reults types
				ARHitTestResultType[] resultTypes = {
					ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
					// if you want to use infinite planes use this:
					//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
					ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
					ARHitTestResultType.ARHitTestResultTypeFeaturePoint
				};

				foreach (ARHitTestResultType resultType in resultTypes)
				{
					if (HitTestWithResultType(hitTestPosition.Value, resultType))
					{
						return;
					}
				}
			}
		}
	}