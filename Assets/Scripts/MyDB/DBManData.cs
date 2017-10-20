using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DBVector {
    public float x;
    public float y;
    public float z;
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class DBManData {
    public int id;
    public DBVector start_position;
    public DBVector target_position;
}

[System.Serializable]
public class OneManManager {
    public GameObject gameObject;
    public OneMan oneMan;
}