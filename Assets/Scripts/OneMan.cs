using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneMan : MonoBehaviour {

    public Transform mTransform;
    public Animator mAnimator;
    public string keyASpeed = "speed";
    public float moveSpeed = 1f;
    public float rotateSpeed = 180f;

    private Transform _rootHolder;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _isUpdated = false;

    public void UpdateData(Transform holder, Vector3 start, Vector3 target) {
        _isUpdated = true;
        if (start == _startPosition && target == _targetPosition && holder == _rootHolder)
            return;
        
        _rootHolder = holder;
        _startPosition = start;
        _targetPosition = target;

        if (start != _startPosition)
        {
            Vector3 worldStart = _rootHolder.TransformPoint(_startPosition);
            mTransform.position = worldStart;
        }
    }

    public bool IsUpdated()
    {
        if (_isUpdated)
        {
            _isUpdated = false;
            return true;
        }

        return false;
    }

    private void Update() {
        Vector3 worldStart = _rootHolder.TransformPoint(_startPosition);
        Vector3 worldTarget = _rootHolder.TransformPoint(_targetPosition);

        Vector3 worldDirection = worldTarget - worldStart;
        worldDirection.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(worldDirection.normalized, Vector3.up);
        mTransform.rotation = Quaternion.RotateTowards(mTransform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        Vector3 nposition = Vector3.MoveTowards(mTransform.position, worldTarget, moveSpeed * Time.deltaTime);
        mTransform.position = nposition;

        float distance = Vector3.Distance(worldTarget, nposition);
        mAnimator.SetFloat(keyASpeed, Mathf.Clamp01(distance));
    }

    private void OnDrawGizmos()
    {
        Vector3 localStart = _rootHolder.TransformPoint(_startPosition);
        Vector3 localTarget = _rootHolder.TransformPoint(_targetPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(localStart, localTarget);
    }
}
