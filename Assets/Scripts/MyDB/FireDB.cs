using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;
using System.Text.RegularExpressions;

public class FireDB : MonoBehaviour {
    public static Action<string, DBManData> OnManData;

    public string dbname = "";
    public string dbroot = "";
    private SimpleFirebaseUnity.FirebaseObserver dbObserver;
    public Dictionary<string, DBManData> manData = new Dictionary<string, DBManData>();

    private void Start()
    {
        Firebase firebase = Firebase.CreateNew(dbname);
        Firebase mydb = firebase.Child(dbroot);
        //Like init
        mydb.OnGetSuccess += OnChangeDB;
        mydb.GetValue();
        //Conect observer
        dbObserver = new FirebaseObserver(mydb, 1f);
        dbObserver.OnChange += this.OnChangeDB;
        dbObserver.Start();
    }

    void OnChangeDB(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        //Debug.Log("[OBSERVER] Last updated changed to: " + snapshot.RawJson);

        foreach (var key in snapshot.Keys)
        {
            string strItem = JsonHelper.GetJsonObject(snapshot.RawJson, key);
            DBManData item = JsonUtility.FromJson<DBManData>(strItem);

            if (manData.ContainsKey(key))
            {
                DBManData oneman = manData[key];
                oneman = item;
            }
            else
            {
                manData.Add(key, item);
            }
            if (OnManData != null)
            {
                OnManData.Invoke(key, item);
            }
        }
    }
}
