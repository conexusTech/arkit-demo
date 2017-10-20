using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnim : MonoBehaviour {

    [HideInInspector]
    public Transform worldBaseTransform;
    [HideInInspector]
    public Transform targetTransform;

    public Transform eyesTransform;

    private Animator animator;

    private Vector3 startPosition = Vector3.zero;
    private float speed;
    private float targetRate;
    private ZombieState prevState;

    public bool IsBlueEyes { get; private set; }

    public bool IsVisible {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnStateChanged(ZombieState newState)
    {
        //if (newState.Equals(prevState))
        //    return;

        bool bTargetChanged = false;

        startPosition = worldBaseTransform.TransformPoint(newState.startPosition);
        if (!this.IsVisible && newState.visibilty)
        {
            transform.position = startPosition;

            Vector3 targetDirection = Camera.main.transform.position - transform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);

            transform.rotation = targetRotation;

            bTargetChanged = true;
        }

        Vector3 targetPosition = worldBaseTransform.TransformPoint(newState.targetPosition);

        Vector3 offset = targetTransform.position - targetPosition;
        offset.y = 0;

        if (offset.magnitude > 0.1f && newState.rate > 0)
            bTargetChanged = true;

        if (bTargetChanged)
        {
            targetTransform.position = targetPosition;
            targetRate = newState.rate;
        }


        this.IsBlueEyes = newState.blueEyes;
        this.IsVisible = newState.visibilty;

        eyesTransform.gameObject.SetActive(IsBlueEyes);
    }

    float speedVelocity = 0;
    // Update is called once per frame
    void Update ()
    {
		if(IsVisible)
        {
            Vector3 targetDirection = targetTransform.position - transform.position;
            targetDirection.y = 0;
            float distance = targetDirection.magnitude;
            targetDirection.Normalize();

            if (distance < 0.25f)
                targetRate = 0;

            if (targetRate > 0)
            {
                //speed = Mathf.SmoothDamp(speed, targetSpeed, ref speedVelocity, 0.5f);
                speed = distance / targetRate;

                transform.position += targetDirection * speed * Time.deltaTime;
                //Quaternion targetRotation = Quaternion.LookRotation(-targetDirection, Vector3.up);
                //Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

                targetRate -= Time.deltaTime;
            }
            else
            {
                speed = 0f;

                targetDirection = Camera.main.transform.position - transform.position;
                targetDirection.y = 0;
                //Quaternion targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
            }

            animator.SetFloat("speed", speed);
        }
    }
}
