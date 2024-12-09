using System;
using System.Linq;
using AYellowpaper.SerializedCollections;
using PlayJam.Ranking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayJam.Popup
{
    [Serializable]
    public class Rank
    {
        public TextMeshProUGUI TXT_Rank;
        public TextMeshProUGUI TXT_Name;
        public TextMeshProUGUI TXT_Score;

        public void SetActive(bool inActive)
        {
            TXT_Rank.gameObject.SetActive(inActive);
            TXT_Name.gameObject.SetActive(inActive);
            TXT_Score.gameObject.SetActive(inActive);
        }
    }

    public class PopupRanking : BasePopup
    {
        [SerializeField]
        private SerializedDictionary<int, Rank> _rankDic;

        [SerializeField]
        private TextMeshProUGUI _txtMyRank;

        [SerializeField]
        private Button _btnX;

        public override void PrevOpen()
        {
            _btnX.onClick.RemoveAllListeners();
            _btnX.onClick.AddListener(PopupManager.Instance.Exit<PopupRanking>);

            foreach (Rank rank in _rankDic.Values)
            {
                rank.SetActive(false);
                _txtMyRank.text = "Loading From Server...";
            }

            if (RankingManager.Instance.IsInitialized == false)
            {
                RankingManager.Instance.Initialize(() =>
                {
                    SetUI();
                });
            }
            else
            {
                SetUI();
            }
        }

        private void SetUI()
        {
            RankingManager.Instance.GetTopRankings(100, (datas) =>
            {
                for (int i = 0; i < _rankDic.Count; i++)
                {
                    if (datas.Count >= i + 1)
                    {
                        _rankDic[i + 1].SetActive(true);
                        _rankDic[i + 1].TXT_Rank.text = i == 0 ? "1ST" : i == 1 ? "2ND" : i == 2 ? "3RD" : $"{(i+1)}TH";
                        _rankDic[i + 1].TXT_Name.text = datas[i].UserName;
                        _rankDic[i + 1].TXT_Score.text = datas[i].Score.ToString();
                    }
                    else
                    {
                        _rankDic[i + 1].SetActive(false);
                    }
                }

                RankingData myRankData = datas.Where(x => x.ID == UserDataHelper.Instance.ID).FirstOrDefault();
                if (string.IsNullOrEmpty(myRankData.ID))
                {
                    _txtMyRank.text = "YOUR RANK : N/A";
                }
                else
                {
                    int myRank = datas.IndexOf(myRankData) + 1;
                    _txtMyRank.text = $"YOUR RANK : {myRank}";
                }
            });
        }
    }
}