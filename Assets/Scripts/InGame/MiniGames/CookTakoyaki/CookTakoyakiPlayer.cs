using DG.Tweening;
using PlayJam.Sound;
using PlayJam.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayJam.InGame.CookTakoyaki
{
    /// <summary>
    /// 
    /// </summary>
    public class CookTakoyakiPlayer : MiniGamePlayer
    {
        [SerializeField]
        private List<Collider2D> _touchPoints;

        [SerializeField]
        private List<GameObject> _takoyakiObjs;

        [SerializeField]
        private List<GameObject> _trapObjs;

        [SerializeField]
        private GameObject _takoyakiStick;

        [SerializeField]
        private CookTakoyakiData _config;

        private List<GameObject> _cachedClonedObjs = new List<GameObject>();

        private int _activeTakoyakiCount;
        private int _activeTrapCount;

        public override void Clear()
        {
            base.Clear();

            _config = null;
            _activeTakoyakiCount = 0;
            _activeTrapCount = 0;

            for (int i = 0; i < _cachedClonedObjs.Count; i++)
            {
                Destroy(_cachedClonedObjs[i]);
            }

            StopAllCoroutines();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as CookTakoyakiData;

            if (_config == null)
                return;

            _activeTakoyakiCount = UnityEngine.Random.Range(_config.TakoyakiMinCount, _config.TakoyakiMaxCount + 1);
            int deactiveTakoyakiCount = _config.TakoyakiMaxCount - _activeTakoyakiCount;
            _activeTrapCount = 0;
            int trapAppearPer = _config.TrapAppearStage <= MiniGameSharedData.Instance.StageCount ? _config.TrapAppearPercent + _config.TrapAppearPercentWeight * (MiniGameSharedData.Instance.StageCount - _config.TrapAppearStage) : 0;

            for (int i = 0; i < deactiveTakoyakiCount; i++)
            {
                int percent = UnityEngine.Random.Range(1, 100 + 1);
                if (percent <= trapAppearPer)
                {
                    _activeTrapCount++;
                }
            }

            for (int i = 0; i < _takoyakiObjs.Count; i++)
            {
                _takoyakiObjs[i].SetActive(false);
            }

            for (int i = 0; i < _trapObjs.Count; i++)
            {
                _trapObjs[i].SetActive(false);
            }

            List<int> indicesToActiveTako = Enumerable.Range(0, _takoyakiObjs.Count).ToList();
            List<int> indicesToActiveTrap = new List<int>();

            while (deactiveTakoyakiCount > 0)
            {
                int removeIndex = UnityEngine.Random.Range(0, indicesToActiveTako.Count);
                indicesToActiveTrap.Add(indicesToActiveTako[removeIndex]);
                indicesToActiveTako.RemoveAt(removeIndex);
                deactiveTakoyakiCount--;
            }

            for (int i = 0; i < indicesToActiveTako.Count; i++)
            {
                _takoyakiObjs[indicesToActiveTako[i]].SetActive(true);
            }

            while (_activeTrapCount != indicesToActiveTrap.Count)
            {
                int removeIndex = UnityEngine.Random.Range(0, indicesToActiveTrap.Count);
                indicesToActiveTrap.RemoveAt(removeIndex);
            }

            for (int i = 0; i < indicesToActiveTrap.Count; i++)
            {
                _trapObjs[indicesToActiveTrap[i]].SetActive(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            yield return null;
            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (IsPlaying == false)
            {
                return;
            }

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) == false)
            {
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                return;
            }

            Touch[] touches = Input.touches;
#endif

            for (int i = 0; i < touches.Length; i++)
            {
                Touch touch = touches[i];

                if (touch.phase != TouchPhase.Began)
                {
                    continue;
                }

                Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);
                // 이미지의 충돌 범위 확인
                Collider2D hit = Physics2D.OverlapPoint(pos);

                for (int j = 0; j < _touchPoints.Count; j++)
                {
                    if (hit != null && hit == _touchPoints[j])
                    {
                        if (_takoyakiObjs[j].activeSelf == true)
                            StartCoroutine(Co_TakeOutTakoyaki(j));
                        else if (_trapObjs[j].activeSelf == true)
                            StartCoroutine(Co_TakeOutTrap(j));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Co_TakeOutTakoyaki(int inIndex)
        {
            _activeTakoyakiCount--;
            bool isClearStage = false;

            if (_activeTakoyakiCount <= 0)
                isClearStage = true;

            if (isClearStage == true)
            {
                // 일단 미니게임 시간 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();
            }

            GameObject clonedTakoyaki = Instantiate(_takoyakiObjs[inIndex], _takoyakiObjs[inIndex].transform.parent);
            clonedTakoyaki.transform.localPosition = Vector3.zero;
            _takoyakiObjs[inIndex].SetActive(false);

            _cachedClonedObjs.Add(clonedTakoyaki);

            WaitForSignal flag = new WaitForSignal();
            Vector3 startPos = new Vector3(clonedTakoyaki.transform.position.x / 2f, -700, -3);

            GameObject clonedTakoyakiStick = Instantiate(_takoyakiStick, _takoyakiStick.transform.parent);
            clonedTakoyakiStick.transform.localPosition = startPos;
            _cachedClonedObjs.Add(clonedTakoyakiStick);

            clonedTakoyakiStick.transform.DOMove(clonedTakoyaki.transform.position, 0.3f).SetEase(Ease.InOutQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            SoundManager.Instance.Play(ESoundType.SFX, "CookTakoyaki");

            clonedTakoyaki.transform.SetParent(clonedTakoyakiStick.transform);
            clonedTakoyaki.transform.localPosition = new(0, 0, -1);

            clonedTakoyakiStick.transform.DOMove(startPos, 0.3f).SetEase(Ease.InQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            if (isClearStage == true)
            {
                // 연출 보여줄거면 보여주고 게임 종료
                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator Co_TakeOutTrap(int inIndex)
        {
            _activeTrapCount--;
            MiniGameManager.OnMiniGamePause.Invoke();

            GameObject clonedTrap = Instantiate(_trapObjs[inIndex], _trapObjs[inIndex].transform.parent);
            clonedTrap.transform.localPosition = Vector3.zero;
            _trapObjs[inIndex].SetActive(false);

            _cachedClonedObjs.Add(clonedTrap);

            WaitForSignal flag = new WaitForSignal();
            Vector3 startPos = new Vector3(clonedTrap.transform.position.x / 2f, -700, -3);

            GameObject clonedTakoyakiStick = Instantiate(_takoyakiStick, _takoyakiStick.transform.parent);
            clonedTakoyakiStick.transform.localPosition = startPos;
            _cachedClonedObjs.Add(clonedTakoyakiStick);

            clonedTakoyakiStick.transform.DOMove(clonedTrap.transform.position, 0.3f).SetEase(Ease.InOutQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            clonedTrap.transform.SetParent(clonedTakoyakiStick.transform);
            clonedTrap.transform.localPosition = new(0, 0, -1);

            clonedTakoyakiStick.transform.DOMove(startPos, 0.3f).SetEase(Ease.InQuad).onComplete = flag.Signal;
            yield return flag.Wait();

            // 연출 보여줄거면 보여주고 게임 종료
            StartCoroutine(OnFail(() => MiniGameManager.OnMiniGameEnd.Invoke(false)));
        }
    }
}
