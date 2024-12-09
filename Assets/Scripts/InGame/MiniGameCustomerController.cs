using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PlayJam.Character.NPC;
using PlayJam.Utils;
using UnityEngine;
using static PlayJam.InGame.MiniGameRuntimeController;

namespace PlayJam.InGame
{
    /// <summary>
    /// 미니게임 손님 처리
    /// </summary>
    public class MiniGameCustomerController : BaseMiniGameController
    {
        [SerializeField]
        private Transform _customerRoot;

        [SerializeField]
        private List<Customer> _allCustomers;

        [SerializeField]
        private GameObject _carrotPrefab;

        [SerializeField]
        private RectTransform _rtDish;

        private List<GameObject> _carrotList = new List<GameObject>();

        private List<Customer> _customerList = new List<Customer>();

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            for (int i = 0; i < _customerList.Count; i++)
            {
                Destroy(_customerList[i].gameObject);
            }

            _customerList.Clear();

            for (int i = 0; i < _carrotList.Count; i++)
            {
                Destroy(_carrotList[i].gameObject);
            }

            _carrotList.Clear();
        }

        /// <summary>
        /// 게임 초기화
        /// </summary>
        public override void Initialize()
        {
            List<MiniGameData> electedMiniGames = MiniGameSharedData.Instance.ReservedMiniGameDatas;
            List<MiniGameData> allElectedMiniGames = new List<MiniGameData>();
            allElectedMiniGames.Add(MiniGameSharedData.Instance.CurrentMiniGameData);
            allElectedMiniGames.AddRange(electedMiniGames);

            float maxDuration = 1f;
            float durationPerDist = maxDuration / 880f;
            float moveTimePerCustomerPos = 140 * durationPerDist;
            float walkAnimDuration = moveTimePerCustomerPos / 2;

            for (int i = 0; i < allElectedMiniGames.Count; i++)
            {
                Customer electedCustomerPrefab = _allCustomers[UnityEngine.Random.Range(0, _allCustomers.Count)];
                Customer customerInstance = Instantiate(electedCustomerPrefab.gameObject, _customerRoot).GetComponent<Customer>();
                customerInstance.transform.localPosition = new Vector3(600, 0, 0);
                customerInstance.ShowFoodResource(allElectedMiniGames[i].Food);
                float endX = (i - 2) * 140; // -280 -140 0 140 280
                float moveDist = endX - 600;
                float moveTime = durationPerDist * Mathf.Abs(moveDist);
                float waitTime = i == 0 ? 0 : maxDuration - moveTime;

                StartCoroutine(Co_MoveLocal(customerInstance.transform, customerInstance.transform.localPosition, new Vector3(endX, customerInstance.transform.localPosition.y, customerInstance.transform.localPosition.z), waitTime, moveTime, (int)(moveTime / walkAnimDuration), 25));

                _customerList.Add(customerInstance);
            }

            MiniGameManager.OnMiniGamePrevStart.AddListener(OnMiniGamePrevStart);
            MiniGameManager.OnMiniGameStart.AddListener(OnMiniGameStart);
            MiniGameManager.OnMiniGamePostStart.AddListener(OnMiniGamePostStart);
            MiniGameManager.OnMiniGameEnd.AddListener(OnMiniGameEnd);
            MiniGameManager.OnMiniGamePause.AddListener(OnMiniGamePause);
            MiniGameManager.OnMiniGameResume.AddListener(OnMiniGameResume);
            MiniGameManager.OnMiniGameQuit.AddListener(OnMiniGameQuit);
        }

        private void OnMiniGamePrevStart()
        {
        }

        private void OnMiniGameStart()
        {
        }

        private void OnMiniGamePostStart()
        {
        }

        private void OnMiniGameResume()
        {
        }

        private void OnMiniGamePause()
        {
        }

        private void OnMiniGameEnd(bool isSuccess)
        {
            float maxDuration = 3f;
            float durationPerDist = maxDuration / 880f;
            float moveTimePerCustomerPos = 140 * durationPerDist;
            float walkAnimDuration = moveTimePerCustomerPos / 6;

            List<MiniGameData> electedMiniGames = MiniGameSharedData.Instance.ReservedMiniGameDatas;

            Customer electedCustomerPrefab = _allCustomers[UnityEngine.Random.Range(0, _allCustomers.Count)];
            Customer customerInstance = Instantiate(electedCustomerPrefab.gameObject, _customerRoot).GetComponent<Customer>();
            customerInstance.transform.localPosition = new Vector3(480, 0, 0);
            customerInstance.ShowFoodResource(electedMiniGames[electedMiniGames.Count - 1].Food);
            _customerList.Add(customerInstance);

            if (_customerList != null && _customerList.Count == 7)
            {
                Destroy(_customerList[0].gameObject);
                _customerList.RemoveAt(0);
            }

            for (int i = 0; i < _customerList.Count; i++)
            {
                if (i == 0 || i == _customerList.Count - 1)
                    StartCoroutine(Co_MoveLocal(_customerList[i].transform, _customerList[i].transform.localPosition, new Vector3(_customerList[i].transform.localPosition.x - 200, _customerList[i].transform.localPosition.y, _customerList[i].transform.localPosition.z), 0, moveTimePerCustomerPos, (int)(moveTimePerCustomerPos / walkAnimDuration), 25));
                else
                    StartCoroutine(Co_MoveLocal(_customerList[i].transform, _customerList[i].transform.localPosition, new Vector3(_customerList[i].transform.localPosition.x - 140, _customerList[i].transform.localPosition.y, _customerList[i].transform.localPosition.z), 0, moveTimePerCustomerPos, (int)(moveTimePerCustomerPos / walkAnimDuration), 25));
            }

            if (isSuccess == true && _carrotList.Count <= 20 && _carrotPrefab != null)
            {
                GameObject carrot = Instantiate(_carrotPrefab, _carrotPrefab.transform.parent);
                carrot.SetActive(true);
                (carrot.transform as RectTransform).anchoredPosition = new Vector2(UnityEngine.Random.Range(-5f, 35f), UnityEngine.Random.Range(0, 10f));
                carrot.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
                _carrotList.Add(carrot);
            }
        }

        private void OnMiniGameQuit()
        {
            float fixedDuration = 1f;
            
            for (int i = 0; i < _customerList.Count; i++)
            {
                _customerList[i].transform.DOLocalMoveX(-600, fixedDuration).SetEase(Ease.Linear);
            }

            StartCoroutine(TimerCoroutine(fixedDuration, () =>
            {
                Clear();
            }));
        }

        private IEnumerator TimerCoroutine(float duration, System.Action onComplete)
        {
            yield return new WaitForSeconds(duration);
            onComplete?.Invoke();
        }

        private IEnumerator Co_MoveLocal(Transform inTransform, Vector3 inPrevPos, Vector3 inCurPos, float inReservedDuration, float inDuration, int inBounceCount, int inBounceHeight)
        {
            inTransform.localPosition = inPrevPos;

            float time = 0;

            while (inReservedDuration != 0 && time / inReservedDuration < 1)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time -= inReservedDuration;
            float cachedBounceWeight = 0;
            float bounceWeight = 0;
            bool isAscending = true;

            while (inDuration != 0 && time / inDuration < 1)
            {
                bounceWeight = ((time / inDuration) % (1f / inBounceCount)) * inBounceCount;
                if (cachedBounceWeight > bounceWeight)
                {
                    isAscending = !isAscending;
                }

                cachedBounceWeight = bounceWeight;

                if (isAscending == false)
                {
                    bounceWeight = 1 - bounceWeight;
                }

                inTransform.localPosition = Vector3.Lerp(inPrevPos, inCurPos, time / inDuration) + Vector3.up * inBounceHeight * bounceWeight;
                time += Time.deltaTime;
                yield return null;
            }

            inTransform.localPosition = inCurPos;
        }
    }
}