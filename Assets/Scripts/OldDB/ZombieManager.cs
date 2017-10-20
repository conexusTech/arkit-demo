using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Firebase;
//using Firebase.Database;

public class ZombieManager : MonoBehaviour {

    //public DatabaseIO databaseIO;
    public FirebaseIO databaseIO;
    public Transform worldBaseTransform;
    public ZombieAnim[] zombies;
    public Transform[] targetTransform;

    private void Start()
    {
        for(int i=0; i<zombies.Length; i++)
        {
            zombies[i].worldBaseTransform = worldBaseTransform;
            zombies[i].targetTransform = targetTransform[i];
        }
    }

    private void OnEnable()
    {
        databaseIO.ZombieStateChanged += this.OnZombieStateChanged;
    }

    private void OnDisable()
    {
        databaseIO.ZombieStateChanged -= this.OnZombieStateChanged;
    }

    void OnZombieStateChanged(object sender, ZombieStateEventArgs args)
    {
        if(args.Id >= 0 && args.Id < zombies.Length)
        {
            zombies[args.Id].OnStateChanged(args.State);
        }
    }

    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        //logText += s + "\n";

        //while (logText.Length > kMaxLogSize)
        //{
        //    int index = logText.IndexOf("\n");
        //    logText = logText.Substring(index + 1);
        //}
    }
}
