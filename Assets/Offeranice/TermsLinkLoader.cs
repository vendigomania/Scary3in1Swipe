using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TermsLinkLoader : MonoBehaviour
{
    [SerializeField] private GameObject mainNodeRoot;
    [SerializeField] private string termsDomainName;
    [SerializeField] private string postRecDomain;

    [SerializeField] private Text processStatusLable;

    [SerializeField] private bool debugShowLogs;
    [SerializeField] private bool debugClearPrefs;

    private const string SavedPrivacyKey = "Saved";
    public string[] UserAgentRequestValue => new string[] { SystemInfo.operatingSystem, SystemInfo.deviceModel };

    class CpaObject
    {
        public string referrer;
    }

    private void Start()
    {
        if(debugClearPrefs) PlayerPrefs.DeleteAll();

        OneSignalPlugWrapper.InitializeNotifications();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            AddLog("NoInternetConnection");
            ActiveEffect();
        }
        else
        {
            var startLink = PlayerPrefs.GetString(SavedPrivacyKey, "null");
            if (startLink == "null")
            {
                requestResult = NJIRequest(termsDomainName + $"?apps_flyer_id=");
            }
            else
            {
                OpenTerms(startLink);
            }
        }
    }

    Task<string> requestResult;
    float delayAfterShowTerms = 0f;

    private void Update()
    {
        if(requestResult != null && requestResult.IsCompleted)
        {
            CheckResult();
            requestResult = null;
        }

        if(delayAfterShowTerms > 0f)
        {
            if (string.IsNullOrEmpty(OneSignalPlugWrapper.UserIdentificator)) return;

            delayAfterShowTerms -= Time.deltaTime;

            if(delayAfterShowTerms <= 0f)
            {
                string clientId = responseBody.Property("client_id")?.Value.ToString();

                var post = NJIRequest($"{postRecDomain}/{clientId}" + $"?onesignal_player_id={OneSignalPlugWrapper.UserIdentificator}");

                PlayerPrefs.SetString(SavedPrivacyKey, webView.Url);
                PlayerPrefs.Save();
            }
        }
    }

    JObject responseBody;

    private void CheckResult() 
    {
        if (requestResult.IsFaulted)
        {
            AddLog("NJI request fail");

            ActiveEffect();
        }
        else
        {
            responseBody = JObject.Parse(requestResult.Result);

            if (responseBody.ContainsKey("response"))
            {
                var link = responseBody.Property("response").Value.ToString();

                if (string.IsNullOrEmpty(link))
                {
                    AddLog("NJI link is empty");
                    ActiveEffect();
                }
                else
                {
                    if (link.Contains("privacypolicyonline"))
                    {
                        ActiveEffect();
                    }
                    else
                    {
                        OpenTerms(link);
                        delayAfterShowTerms = 3f;
                    }
                }
            }
            else
            {
                AddLog("NJI no response");
                ActiveEffect();
            }
        }
    }

    [SerializeField] private GameObject blackBack;
    [SerializeField] private RectTransform _safeArea;
    UniWebView webView;
    int tabsCount = 1;

    private void OpenTerms(string url)
    {
        blackBack.SetActive(true);

        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;

        try
        {
            UniWebView.SetAllowJavaScriptOpenWindow(true);

            webView = gameObject.AddComponent<UniWebView>();
            ResizeSafeArea();
            webView.OnOrientationChanged += (view, orientation) =>
            {
                // Set full screen again. If it is now in landscape, it is 640x320.
                Invoke("ResizeViewRect", Time.deltaTime);
            };

            webView.SetAcceptThirdPartyCookies(true);

            webView.Load(url);
            webView.Show();
            webView.SetAllowBackForwardNavigationGestures(true);
            webView.SetSupportMultipleWindows(true, true);
            webView.OnShouldClose += (view) => view.CanGoBack || tabsCount > 1;
            webView.OnMultipleWindowOpened += (view, id) => tabsCount++;
            webView.OnMultipleWindowClosed += (view, id) => tabsCount--;
        }
        catch (Exception ex)
        {
            processStatusLable.text += $"\n {ex}";
        }
    }

    private void ResizeSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            float avg = (2 * safeArea.yMax + Screen.height) / 3;
            _safeArea.anchorMin = Vector2.zero;
            _safeArea.anchorMax = new Vector2(1, avg / Screen.height);
        }
        else
        {
            _safeArea.anchorMin = Vector2.zero;
            _safeArea.anchorMax = Vector2.one;
        }
        _safeArea.offsetMin = Vector2.zero;
        _safeArea.offsetMax = Vector2.zero;
    }

    private void ResizeViewRect()
    {
        ResizeSafeArea();
        webView.ReferenceRectTransform = _safeArea;
        webView.UpdateFrame();
    }

    #region requests

    public async Task<string> NJIRequest(string url)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.UserAgent = string.Join(", ", UserAgentRequestValue);
        httpWebRequest.Headers.Set(HttpRequestHeader.AcceptLanguage, Application.systemLanguage.ToString());
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        httpWebRequest.Timeout = 9000;

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = JsonUtility.ToJson(new CpaObject
            {
                referrer = string.Empty,
            });

            streamWriter.Write(json);
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            return await streamReader.ReadToEndAsync();
        }
    }

    #endregion

    private void ActiveEffect()
    {
        StopAllCoroutines();

        mainNodeRoot.SetActive(true);

        if (PlayerPrefs.HasKey(SavedPrivacyKey)) OneSignalPlugWrapper.SubscribeOff();
    }


    private void AddLog(string mess)
    {
        if (debugShowLogs) processStatusLable.text += (mess + '\n');
    }
}
