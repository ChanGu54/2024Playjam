using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

namespace PlayJam.Ranking
{
    public struct RankingData
    {
        public string ID;
        public string UserName;
        public int Score;
        public string TimeStamp;
    }

    public class RankingManager : MonoBehaviour
    {
        private static RankingManager _instance;

        public static RankingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find("RankingManager")?.GetComponent<RankingManager>();
                }

                return _instance;
            }
        }

        public bool IsInitialized = false;

        public void Initialize(Action inCallback)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(async task => {

                // 딜레이 없으면 인스턴스 못불러오네..? 왜지
                await UniTask.WaitForSeconds(0.1f);

                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    IsInitialized = true;
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));

                    IsInitialized = false;
                }

                inCallback?.Invoke();
            });
        }

        // 점수 제출
        public void SubmitScore(string inID, string inUserName, int inScore)
        {
            if (IsInitialized == false)
                return;

            var data = new Dictionary<string, object>
            {
                ["ID"] = inID,
                ["UserName"] = inUserName,
                ["Score"] = inScore,
                ["TimeStamp"] = ServerValue.Timestamp
            };

            FirebaseDatabase.DefaultInstance.RootReference.Child("Rankings").Child(inID).SetValueAsync(data);
        }

        // 상위 10개 랭킹 가져오기
        public void GetTopRankings(int inNum, System.Action<List<RankingData>> callback)
        {
            if (IsInitialized == false)
                return;

            FirebaseDatabase.DefaultInstance.RootReference.Child("Rankings")
                .OrderByChild("Score")
                .LimitToLast(inNum)
                .GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Rankings download failed");
                        return;
                    }

                    var rankings = new List<RankingData>();
                    var snapshot = task.Result;

                    foreach (var childSnapshot in snapshot.Children.Reverse())
                    {
                        rankings.Add(new RankingData
                        {
                            ID = childSnapshot.Child("ID").Value.ToString(),
                            UserName = childSnapshot.Child("UserName").Value.ToString(),
                            Score = int.Parse(childSnapshot.Child("Score").Value.ToString()),
                            TimeStamp = childSnapshot.Child("TimeStamp").Value.ToString()
                        });
                    }

                    callback(rankings);
                });
        }

        public void GetMyScore(System.Action<RankingData> callback)
        {
            FirebaseDatabase.DefaultInstance.RootReference.Child("Rankings")
                .Child(UserDataHelper.Instance.ID)
                .GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Rankings download failed");
                        return;
                    }

                    RankingData rankingData = new RankingData()
                    {
                        ID = UserDataHelper.Instance.ID,
                        UserName = UserDataHelper.Instance.Name,
                    };

                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists)
                    {
                        rankingData.Score = int.Parse(snapshot.Child("Score").Value.ToString());
                        rankingData.TimeStamp = snapshot.Child("TimeStamp").Value.ToString();
                    }
                    else
                    {
                        rankingData.Score = -1;
                        rankingData.TimeStamp = string.Empty;
                    }

                    callback(rankingData);
                });
        }
    }
}