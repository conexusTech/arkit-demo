using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartPointer : MonoBehaviour {

    public MyPlanePlacement myPlanePlacement;
    public GameObject winPlaceHelp;

    private void OnEnable()
    {
        myPlanePlacement.onPlace.AddListener(WhenPlaced);
        myPlanePlacement.onHide.AddListener(WhenHided);
    }

    private void OnDisable()
    {
        myPlanePlacement.onPlace.RemoveListener(WhenPlaced);
        myPlanePlacement.onHide.RemoveListener(WhenHided);
    }

    private void WhenPlaced()
    {
        winPlaceHelp.SetActive(false);
    }

    private void WhenHided()
    {
        winPlaceHelp.SetActive(true);
    }

}
