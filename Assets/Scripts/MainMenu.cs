using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public Button bStart;
    public Text tHelp;
    public RawImage rawImage;

    private void OnEnable()
    {
        bStart.onClick.AddListener(WhenStart); 
    }

    private void OnDisable()
    {
        bStart.onClick.RemoveListener(WhenStart);
    }

    private void Start()
    {
        StartCoroutine(CoCameraActivate());
        WebCamTexture webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        //rawImage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    private void WhenStart()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator CoCameraActivate()
    {
        bStart.interactable = false;
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        while (true)
        {
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                bStart.interactable = true;
                tHelp.gameObject.SetActive(false);
            }
            else
            {
                bStart.interactable = false;
                tHelp.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
