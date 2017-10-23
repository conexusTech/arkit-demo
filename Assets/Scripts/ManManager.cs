using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManManager : MonoBehaviour {

    public Transform rootHolder;
    public GameObject manPrefab;

    private Dictionary<string, OneManManager> peoples = new Dictionary<string, OneManManager>(); 

    private void Awake()
    {
        FireDB.OnManData += WhenManData;
    }

    private void OnDestroy()
    {
        FireDB.OnManData += WhenManData;
    }

    private void WhenManData(string keyname, DBManData dbmandata)
    {
        if (peoples.ContainsKey(keyname))
        {
            peoples[keyname].oneMan.UpdateData(rootHolder, dbmandata.start_position.GetVector3(), dbmandata.target_position.GetVector3()); 
        }
        else
        {
            Vector3 spawnPoint = dbmandata.start_position.GetVector3();
            spawnPoint = rootHolder.TransformPoint(spawnPoint);
            GameObject go = Instantiate(manPrefab, spawnPoint, Quaternion.identity) as GameObject;
            go.name = keyname;
            go.transform.parent = rootHolder;
            OneMan oneMan = go.GetComponent<OneMan>();
            OneManManager onemanmanager = new OneManManager();
            onemanmanager.gameObject = go;
            onemanmanager.oneMan = oneMan;
            oneMan.UpdateData(rootHolder, dbmandata.start_position.GetVector3(), dbmandata.target_position.GetVector3());
            peoples.Add(keyname, onemanmanager);
        }
    }
}
