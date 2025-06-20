using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Dan
{
    internal static class ExtensionMethods
    {
        internal static Vector3 ToVector3(this Vec3 vec) => 
            new(vec.x, vec.y, vec.z);

        internal static Quaternion ToQuaternion(this Vec3 vec) => 
            Quaternion.Euler(vec.x, vec.y, vec.z);

        internal static void Handle(this UnityWebRequest webRequest, Action<bool> onComplete)
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest().completed += _ =>
            {
                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onComplete(true);
                }
                else
                {
                    Logger.Log(webRequest.downloadHandler.text, Logger.LogType.Error);
                    onComplete(false);
                }
                webRequest.downloadHandler.Dispose();
                webRequest.Dispose();
            };
        }
    }
}