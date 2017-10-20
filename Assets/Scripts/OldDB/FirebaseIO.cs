using UnityEngine;
using SimpleFirebaseUnity;
using SimpleFirebaseUnity.MiniJSON;

using System.Collections.Generic;
using System.Collections;
using System;
using System.Text.RegularExpressions;

[System.Serializable]
public class ZombieState
{
    public int id;
    public Vector3 startPosition;
    public Vector3 targetPosition;
    public float rate;
    public bool blueEyes;
    public bool visibilty;

    public override bool Equals(System.Object obj)
    {
        // Check for null values and compare run-time types.
        if (obj == null || GetType() != obj.GetType())
            return false;

        ZombieState p = (ZombieState)obj;
        return (id == p.id) && (visibilty == p.visibilty) && (targetPosition == p.targetPosition) && (blueEyes == p.blueEyes);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class ZombieStateEventArgs : EventArgs
{
    public int Id { get; private set; }
    public ZombieState State { get; private set; }

    public ZombieStateEventArgs(int index, ZombieState state)
    {
        this.Id = index;
        this.State = state;
    }
}

public class FirebaseIO : MonoBehaviour
{

    public string databaseUrl = "got-window.firebaseio.com";
    public string userId = "";

    public string DBPath = "people";

    [SerializeField]
    TextMesh textMesh;

    [System.Serializable]
    public class StartPosition
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public struct  TargetPosition
    {
        public float rate;
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class OneItem
    {
        public bool eyes;
        public TargetPosition start_position;
        public TargetPosition target_position;
        public bool visible;
    }

    public event EventHandler<ZombieStateEventArgs> ZombieStateChanged;

    private List<ZombieState> zombieStateList = null;

    Dictionary<int, ZombieState> zombieStatesMap = new Dictionary<int, ZombieState>();

    private SimpleFirebaseUnity.Firebase firebase;
    private SimpleFirebaseUnity.Firebase peopleDB;
    private SimpleFirebaseUnity.FirebaseObserver dbObserver;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    IEnumerator Start()
    {
        InitializeFirebase();

        yield return null;

        zombieStateList = new List<ZombieState>();
        zombieStatesMap.Clear();
    }

    // Initialize the Firebase database:
    void InitializeFirebase()
    {
        firebase = SimpleFirebaseUnity.Firebase.CreateNew(databaseUrl);
        peopleDB = firebase.Child(DBPath);

        // Init callbacks
        firebase.OnGetSuccess += GetOKHandler;
        firebase.OnGetFailed += GetFailHandler;
        firebase.OnSetSuccess += SetOKHandler;
        firebase.OnSetFailed += SetFailHandler;
        firebase.OnUpdateSuccess += UpdateOKHandler;
        firebase.OnUpdateFailed += UpdateFailHandler;
        firebase.OnPushSuccess += PushOKHandler;
        firebase.OnPushFailed += PushFailHandler;
        firebase.OnDeleteSuccess += DelOKHandler;
        firebase.OnDeleteFailed += DelFailHandler;

        // Get child node from firebase, if false then all the callbacks are not inherited.
        //SimpleFirebaseUnity.Firebase temporary = firebase.Child("temporary", true);
        SimpleFirebaseUnity.Firebase lastUpdate = firebase.Child("lastUpdate");

        lastUpdate.OnGetSuccess += GetTimeStamp;

        peopleDB.OnGetSuccess += OnChangeDB;
        peopleDB.GetValue();

        // Make observer on "last update" time stamp
        dbObserver = new FirebaseObserver(peopleDB, 1f);
        dbObserver.OnChange += this.OnChangeDB;
        dbObserver.Start();

        //DoDebug("[OBSERVER] FirebaseObserver on " + lastUpdate.FullKey + " started!");

        // Print details
        //DoDebug("Firebase endpoint: " + peopleDB.Endpoint);
        //DoDebug("Firebase key: " + peopleDB.Key);
        //DoDebug("Firebase fullKey: " + peopleDB.FullKey);
        //DoDebug("Firebase child key: " + peopleDB.Key);
        //DoDebug("Firebase child fullKey: " + peopleDB.FullKey);
    }

    void GetTimeStamp(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        long timeStamp = snapshot.Value<long>();
        DateTime dateTime = SimpleFirebaseUnity.Firebase.TimeStampToDateTime(timeStamp);

        DoDebug("[OK] Get on timestamp key: <" + sender.FullKey + ">");
        DoDebug("Date: " + timeStamp + " --> " + dateTime.ToString());
    }

    void OnChangeDB(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OBSERVER] Last updated changed to: " + snapshot.RawJson);

        foreach (var key in snapshot.Keys)
        {
            string strItem = JsonHelper.GetJsonObject(snapshot.RawJson, key);

            string numbersOnly = Regex.Replace(key, "[^0-9]", "");
            if (string.IsNullOrEmpty(numbersOnly))
                continue;

            int index = int.Parse(numbersOnly);

            OneItem item = JsonUtility.FromJson<OneItem>(strItem);
            if (zombieStatesMap.ContainsKey(index))
            {
                ParseItem(item, index, zombieStatesMap[index]);
            }
            else
            {
                ZombieState zombie = new ZombieState();
                if (ParseItem(item, index, zombie))
                {
                    zombieStatesMap[zombie.id] = zombie;
                    zombieStateList.Add(zombie);
                }
            }
        }


        //Do something with the data in args.Snapshot
        //if (db != null && db.items.Count > 0)
        //{
        //    foreach (var item in db.items)
        //    {
        //        if (zombieStatesMap.ContainsKey(item.id))
        //        {
        //            ParseItem(item, zombieStatesMap[item.id]);
        //        }
        //        else
        //        {
        //            ZombieState zombie = new ZombieState();
        //            if (ParseItem(item, zombie))
        //            {
        //                zombieStatesMap[zombie.id] = zombie;
        //                zombieStateList.Add(zombie);
        //            }
        //        }
        //    }
        //}

        if (ZombieStateChanged != null)
        {
            foreach (var pair in zombieStatesMap)
            {
                ZombieStateChanged(this, new ZombieStateEventArgs(pair.Key, pair.Value));
            }
        }
    }


    bool ParseItem(OneItem item, int index, ZombieState state)
    {
        state.id = index;
        state.blueEyes = item.eyes;
        state.visibilty = item.visible;

        state.startPosition = new Vector3(item.start_position.x, item.start_position.y, item.start_position.z);
        state.targetPosition = new Vector3(item.target_position.x, item.target_position.y, item.target_position.z);
        state.rate = item.target_position.rate;

        return true;
    }

    void GetOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Get from key: <" + sender.FullKey + ">");
        DoDebug("[OK] Raw Json: " + snapshot.RawJson);

        Dictionary<string, object> dict = snapshot.Value<Dictionary<string, object>>();
        List<string> keys = snapshot.Keys;

        if (keys != null)
            foreach (string key in keys)
            {
                DoDebug(key + " = " + dict[key].ToString());
            }
    }

    void GetFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Get from key: <" + sender.FullKey + ">,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void SetOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Set from key: <" + sender.FullKey + ">");
    }

    void SetFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Set from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void UpdateOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Update from key: <" + sender.FullKey + ">");
        DoDebug(snapshot.RawJson);
    }

    void UpdateFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Update from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void DelOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Del from key: <" + sender.FullKey + ">");
    }

    void DelFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Del from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void PushOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] Push from key: <" + sender.FullKey + ">");
    }

    void PushFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] Push from key: <" + sender.FullKey + ">, " + err.Message + " (" + (int)err.Status + ")");
    }

    void GetRulesOKHandler(SimpleFirebaseUnity.Firebase sender, DataSnapshot snapshot)
    {
        DoDebug("[OK] GetRules");
        DoDebug("[OK] Raw Json: " + snapshot.RawJson);
    }

    void GetRulesFailHandler(SimpleFirebaseUnity.Firebase sender, FirebaseError err)
    {
        DoDebug("[ERR] GetRules,  " + err.Message + " (" + (int)err.Status + ")");
    }

    void DoDebug(string str)
    {
        Debug.Log(str);

        //if (textMesh != null)
        //{
        //    textMesh.text += str + "\n";
        //}
    }

}
