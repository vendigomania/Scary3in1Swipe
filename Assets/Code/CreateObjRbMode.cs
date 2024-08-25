using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjRbMode : MonoBehaviour
{
    public Transform[] Points;
    public GameObject[] SpawnObjArray;
    public float Interval;
    public float CurrentInterval;
    
    private void Start() =>
        Interval = 0;

    private void Update() => 
        Create();

    private void Create()
    {
        Interval -= Time.deltaTime;
        if (Interval <= 0)
        {
            var randomSelectObj = Random.Range(0, SpawnObjArray.Length);
            var randomSelectPointPosition = Random.Range(0, Points.Length);
            var go = Instantiate(SpawnObjArray[randomSelectObj],
                Points[randomSelectPointPosition].transform.position, Quaternion.identity);

            if (go)
            {
                Destroy(go, 20f);
            }

            Interval = CurrentInterval;
        }
    }
}
