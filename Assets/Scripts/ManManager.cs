using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManManager : MonoBehaviour {

    public Transform rootHolder;
    public GameObject manPrefab;

    private Dictionary<string, OneManManager> peoples = new Dictionary<string, OneManManager>(); 

    private void OnEnable()
    {
        FireDB.OnManData += WhenManData;
    }

    private void OnDisable()
    {
        FireDB.OnManData += WhenManData;
    }

    private void WhenManData(string keyname, DBManData dbmandata)
    {
        if (peoples.ContainsKey(keyname))
        {
            peoples[keyname].oneMan.startPosition = dbmandata.start_position.GetVector3();
            peoples[keyname].oneMan.targetPosition = dbmandata.target_position.GetVector3(); 
        }
        else
        {
            Vector3 spawnPoint = dbmandata.start_position.GetVector3();
            //spawnPoint = rootHolder.InverseTransformVector(spawnPoint);
            GameObject go = Instantiate(manPrefab, spawnPoint, Quaternion.identity) as GameObject;
            go.name = keyname;
            go.transform.parent = transform;
            OneMan oneMan = go.GetComponent<OneMan>();
            OneManManager onemanmanager = new OneManManager();
            onemanmanager.gameObject = go;
            onemanmanager.oneMan = oneMan;
            oneMan.startPosition = dbmandata.start_position.GetVector3();
            oneMan.targetPosition = dbmandata.target_position.GetVector3();
            oneMan.rootHolder = rootHolder;
            peoples.Add(keyname, onemanmanager);
        }
    }
}
