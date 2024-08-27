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
            OneSignal.Initialize("b1d07c88-c850-4f61-9487-7cb0e8f22d20");
    }

    public static void SubscribeOff()
    {
        OneSignal.Notifications?.ClearAllNotifications();
        OneSignal.Logout();
    }
}
