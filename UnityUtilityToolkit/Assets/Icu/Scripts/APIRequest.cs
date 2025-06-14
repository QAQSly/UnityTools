using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Icu;
using Newtonsoft.Json;

public class APIRequest : MonoBehaviour
{


    public void GetSteamData(string uri, System.Action<List<Friend>> callback)
    {
        StartCoroutine(GetRequest(uri, callback));
    }

    IEnumerator GetRequest(string uri, System.Action<List<Friend>> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                callback(default(List<Friend>)); // 在发生错误时调用回调，传递 null
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                
                // Debug.Log(text);
                FriendsList res = JsonUtility.FromJson<FriendsList>(text);
                // List<Friend> res = JsonConvert.DeserializeObject<List<Friend>>(text);
                // Debug.Log(res.friends[0].steamid);
                callback(res.friends); // 在成功时调用回调，传递解析后的数据
            }
        }
    }
}