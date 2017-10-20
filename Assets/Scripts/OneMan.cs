using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneMan : MonoBehaviour {

    public Transform mTransform;
    public Animator mAnimator;
    public string keyASpeed = "speed";
    public Transform rootHolder;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public float speed = 1f;

    private void Update() {
        Vector3 localStart = rootHolder.TransformPoint(startPosition);
        Vector3 localTarget = rootHolder.TransformPoint(targetPosition);

        Vector3 localDirection = localTarget - localStart;
        localDirection.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(localDirection.normalized, Vector3.up);
        mTransform.localRotation = targetRotation;

        Vector3 nposition = Vector3.MoveTowards(mTransform.localPosition, localTarget, speed * Time.deltaTime);
        mTransform.localPosition = nposition;

        float distance = Vector3.Distance(localTarget, nposition);
        mAnimator.SetFloat(keyASpeed, Mathf.Clamp01(distance));
    }
}
