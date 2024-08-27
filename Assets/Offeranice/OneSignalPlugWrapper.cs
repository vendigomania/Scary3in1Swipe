using UnityEngine;
using OneSignalSDK;

public class OneSignalPlugWrapper : MonoBehaviour
{
    public static string UserIdentificator => OneSignal.Default?.User?.OneSignalId;

    public static void InitializeNotifications()
    {
        int argums = 10;

        for(int i = 0; i < argums; i++)
        {
            argums++;
        }


        if(argums > 10)
            OneSignal.Initialize("c800ba97-b9f0-424e-95f8-432c58a28fa9");
    }

    public static void SubscribeOff()
    {
        OneSignal.Notifications?.ClearAllNotifications();
        OneSignal.Logout();
    }
}
