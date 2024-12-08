using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using utils;

namespace PlayJam.InGame
{
    /// <summary>
    /// 인게임에서 사용할 공용 데이터
    /// </summary>
    public class MiniGameSharedData : Singleton<MiniGameSharedData>
    {
        /// <summary>
        /// 선택된 Config값
        /// </summary>
        public MiniGameConfig Config;

        public Vector2 LeftTopPos;

        public Vector2 RightBottomPos;

        public int StageCount;

        public float CurStageTime;

        public float SpendTime { get => CurStageTime - LeftTime; }

        public float LeftTime;

        public int HeartCount;

        public List<MiniGameData> ReservedMiniGameDatas;

        public List<MiniGameData> AllMiniGameDatas;

        public MiniGameData CurrentMiniGameData;

        private bool _isTestGame;

        private EMiniGame _testMiniGameKind;

        private List<EMiniGame> _newGameStageSelector = new List<EMiniGame>();
        private List<float> _electPercentageList = new List<float>();

        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize(MiniGameConfig inConfig, List<MiniGameData> inAllMiniGames, bool isTestGame, EMiniGame inTestGameKind)
        {
            Config = inConfig;

            _isTestGame = isTestGame;
            _testMiniGameKind = inTestGameKind;

            float curRatio = (float)Screen.width / Screen.height;
            float baseRatio = 720 / 1280f;
            float adaptScale = 1;

            if (baseRatio > curRatio)
            {
                adaptScale = curRatio / baseRatio;
            }

            LeftTopPos = new Vector2(-360, 220 * adaptScale);
            RightBottomPos = new Vector2(360, -640);

            ReservedMiniGameDatas = new List<MiniGameData>();

            StageCount = 1;
            HeartCount = Config.InitialHeartCount;
            AllMiniGameDatas = inAllMiniGames;

            _newGameStageSelector = Enum.GetValues(typeof(EMiniGame)).Cast<EMiniGame>().ToList();
            _electPercentageList = Enumerable.Repeat(10f, inAllMiniGames.Count).ToList();

            ElectMiniGames(5);
            CurrentMiniGameData = ReservedMiniGameDatas[0];
            ReservedMiniGameDatas.RemoveAt(0);
        }

        public void ElectMiniGames(int inTotalElectedCnt)
        {
            while (ReservedMiniGameDatas.Count < inTotalElectedCnt)
            {
                MiniGameData miniGameElected = null;

                if (_isTestGame == true)
                    miniGameElected = AllMiniGameDatas.Where(x => x.GameKind == _testMiniGameKind).FirstOrDefault();
                else
                {
                    if (_newGameStageSelector.Count > 0)
                    {
                        EMiniGame electedEnum = _newGameStageSelector[UnityEngine.Random.Range(0, _newGameStageSelector.Count)];
                        _newGameStageSelector.Remove(electedEnum);
                        miniGameElected = AllMiniGameDatas.Where(x => x.GameKind == electedEnum).FirstOrDefault();
                        Debug.Log($"남은 초기 미니게임 수 : {_newGameStageSelector.Count}");
                    }
                    else
                    {
                        float percentageSum = _electPercentageList.Sum();
                        float rand = UnityEngine.Random.Range(0, percentageSum);
                        float sum = 0;

                        for (int i = 0; i < _electPercentageList.Count; i++)
                        {
                            sum += _electPercentageList[i];
                            if (sum >= rand)
                            {
                                miniGameElected = AllMiniGameDatas[i];
                                float percentageToDecrease = Mathf.Clamp(_electPercentageList[i], 0, 9);
                                float distributeVal = percentageToDecrease / (_electPercentageList.Count - 1);

                                for (int j = 0; j < _electPercentageList.Count; j++)
                                {
                                    if (j == i)
                                        _electPercentageList[j] -= percentageToDecrease;
                                    else
                                        _electPercentageList[j] += distributeVal;
                                }

                                break;
                            }
                        }
                    }
                }

                ReservedMiniGameDatas.Add(miniGameElected);
            }
        }

        public void OnMiniGamePrevStart()
        {
            CurStageTime = Mathf.Max(Config.MinimumPlayTime, Config.PlayTime - Config.DecresedPlayTimePerRound * (StageCount - 1));
            LeftTime = CurStageTime;
            Debug.Log($"MiniGameSharedData.OnMiniGamePrevStart : {CurStageTime}");
        }

        public void OnMiniGameStart()
        {

        }

        public void OnMiniGamePostStart()
        {

        }

        public void OnMiniGameResume()
        {

        }

        public void OnMiniGamePause()
        {

        }

        public void OnMiniGameQuit()
        {
            UserDataHelper.Instance.Coin += Mathf.Max(0, 100 * (Instance.StageCount - 1));
            MiniGameManager.WorldManager.RefreshCoinCount();

            Clear();
        }

        public void OnMiniGameEnd(bool isSuccess)
        {
            if (isSuccess == true)
            {
                StageCount += 1;
            }
            else
            {
                HeartCount--;
            }

            ElectMiniGames(5);
            CurrentMiniGameData = ReservedMiniGameDatas[0];
            ReservedMiniGameDatas.RemoveAt(0);
        }

        /// <summary>
        /// 데이터 초기화 처리
        /// </summary>
        public void Clear()
        {
            Config = null;

            ReservedMiniGameDatas.Clear();
            ReservedMiniGameDatas = null;

            CurrentMiniGameData = null;
        }

        #region Functions

        /// <summary>
        /// Param Pos is in the game screen?
        /// </summary>
        /// <param name="inPos"></param>
        /// <returns></returns>
        public bool ContainsGameScreen(Vector3 inPos)
        {
            if (inPos.x > LeftTopPos.x &&
                inPos.x < RightBottomPos.x &&
                inPos.y < LeftTopPos.y &&
                inPos.y > RightBottomPos.y)
                return true;

            return false;
        }

        #endregion
    }
}