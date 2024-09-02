using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

namespace PawsOfFire.MonoSystem
{
    public class LeaderBoards : MonoBehaviour
    {
        public RectTransform viewport;

        private GameObject _entryPrefab;

        // Start is called before the first frame update
        void Awake()
        {
            _entryPrefab = (GameObject)Resources.Load("Prefabs/Entry");
        }

        [System.Serializable]
        public class Entry
        {
            public string name;
            public int score;
        }

        [System.Serializable]
        public class LeaderBoardData
        {
            public List<Entry> entries;

            public static LeaderBoardData CreateFromJSON(string jsonString)
            {
                return JsonUtility.FromJson<LeaderBoardData>(jsonString);
            }
        }

        private IEnumerator GetRequest()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://plasmaclash.com:3002/get-players"))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    LeaderBoardData e = LeaderBoardData.CreateFromJSON(webRequest.downloadHandler.text);
                    LoadLeaderBoards(e.entries);
                }
            }
        }

        public void Reload()
        {
            if (!PawsOfFireGameManager.isOnline)
            {
                int childCount = viewport.transform.childCount;
                for (int j = 1; j < childCount; j++)
                {
                    DestroyImmediate(viewport.transform.GetChild(1).gameObject);
                }
                return;
            }
            StartCoroutine(GetRequest());
        }

        public void LoadLeaderBoards(List<Entry> entries)
        {
            int childCount = viewport.transform.childCount;
            for (int j = 1; j < childCount; j++)
            {
                DestroyImmediate(viewport.transform.GetChild(1).gameObject);
            }

            List<Entry> sorted = entries.OrderBy(e => -e.score).ToList();
            sorted = sorted.Where(e => e.score > 0).ToList();

            viewport.sizeDelta = new Vector2(0, 32 + sorted.Count * _entryPrefab.GetComponent<RectTransform>().rect.height);

            for (int i = 0; i < sorted.Count; i++)
            {
                Entry entry = sorted[i];
                GameObject go = GameObject.Instantiate(_entryPrefab, viewport);
                RectTransform rt = go.GetComponent<RectTransform>();

                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{i + 1}.";
                go.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = entry.name;
                go.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = entry.score.ToString();
            }
        }
    }
}