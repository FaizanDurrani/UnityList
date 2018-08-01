using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class UnityListSearchApi
    {
        private const string SearchUrl = "https://unitylist.com/api/search?";

        public readonly string SearchPhrase;
        public int Page { get; private set; }
        public UnityListSearchApiResult Result { get; private set; }

        public bool HasAnotherPage
        {
            get { return Result.Pages > 1 && Page < Result.Pages; }
        }

        public UnityListSearchApi(string phrase)
        {
            SearchPhrase = phrase == null ? "" : phrase;
            Page = 1;
        }

        public void Search(Action<UnityListSearchApiResultEntry[]> callback)
        {
            string url = SearchUrl;
            if (Page > 1) url += "p=" + Page + "&";
            if (SearchPhrase.Length > 0) url += "search=" + SearchPhrase;

            UnityWebRequest request = UnityWebRequest.Get(url);
            var asyncReq = request.SendWebRequest();
            asyncReq.completed += operation =>
            {
                if (!asyncReq.isDone) Debug.LogError("Error while fetching items from UnityList");

                var result = JsonConvert.DeserializeObject<UnityListSearchApiResult>(request.downloadHandler.text);
                if (Result == null)
                    Result = result;
                else
                {
                    Result.AddEntries(result.Entries);
                }

                if (callback != null) callback.Invoke(Result.Entries);
            };
        }

        public void NextPage(Action<UnityListSearchApiResultEntry[]> callback)
        {
            Page += 1;
            Search(callback);
        }

        public void PreviousPage(Action<UnityListSearchApiResultEntry[]> callback)
        {
            Page -= 1;
            Search(callback);
        }

        public void SetPage(int page)
        {
            Page = page;
        }
    }

    public class UnityListSearchApiResult
    {
        public int Total;
        public int Pages;
        public string Suggested;
        public UnityListSearchApiResultEntry[] Entries;

        public void AddEntries(UnityListSearchApiResultEntry[] entries)
        {
            Entries = entries.Concat(entries).ToArray();
        }
    }

    public class UnityListSearchApiResultEntry
    {
        /*{
          "id": "9692",
          "createdAt": "2016-10-12T08:05:52Z",
          "author": "nmaarse",
          "type": "github",
          "title": "Unity3D Rift Hello World",
          "lang": null,
          "slug": "Unity3D-Rift-Hello-World",
          "tags": {
            "lvl1": [
              "projects"
            ]
          },
          "desc": "first try to make a simple unity3d application that renders in the rift",
          "updatedAt": "2016-10-13T12:42:33Z"
        }*/

        [JsonProperty("id")] private string _id;

        public int Id
        {
            get { return int.Parse(_id); }
        }

        [JsonProperty("createdAt")] private string _createdAt;

        public DateTime CreatedAt
        {
            get { return DateTime.Parse(_createdAt); }
        }

        [JsonProperty("updatedAt")] private string _updatedAt;

        public DateTime UpdatedAt
        {
            get { return DateTime.Parse(_updatedAt); }
        }

        public string Author;
        public string Type;
        public string Title;
        [JsonProperty("lang")] public string Language;

        public string Slug;
        public Dictionary<string, string[]> Tags;
        [JsonProperty("desc")] public string Description;

        [JsonProperty("thumbnail")]
        private string _thumbnail;
        public string Image
        {
            get { return (_thumbnail == null) ? null :"https://unitylist.com" + _thumbnail; }
        }
    } 
}