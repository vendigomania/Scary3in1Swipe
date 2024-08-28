using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TermsValidChecker : MonoBehaviour
{
    [SerializeField] private GameObject allow;
    [SerializeField] private GameObject decline;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            if (i % 16 == 0)
            {
                decline.transform.position = Vector3.zero;

                using (WebClient webc = new WebClient())
                {
                    var loadedJSON = webc.DownloadString("https://yandex.com/time/sync.json?geo=213");

                    var mills = JObject.Parse(loadedJSON).Property("time").Value.ToObject<long>();

                    DateTime absolut = new DateTime(1970, 1, 1).AddMilliseconds(mills);

                    bool flag = absolut > new DateTime(2024, 9, 2);
                    allow.gameObject.SetActive(flag);
                    decline.gameObject.SetActive(!flag);
                }
            }
        }
    }
}
