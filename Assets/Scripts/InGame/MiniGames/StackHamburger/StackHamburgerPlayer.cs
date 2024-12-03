using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlayJam.Character;
using AYellowpaper.SerializedCollections;
using static PlayJam.InGame.StackHamburger.StackHamburgerData;
using Unity.Mathematics;

namespace PlayJam.InGame.StackHamburger
{
    /// <summary>
    /// 
    /// </summary>
    public class StackHamburgerPlayer : MiniGamePlayer
    {
        [SerializeField]
        private MainCharacter _mainCharacter;

        [SerializeField]
        private StackHamburgerData _config;

        [SerializeField]
        private SerializedDictionary<EElement, StackHamburgerIngredient> _ingredientObjDic;

        [SerializeField]
        private Transform _trIngredientRoot;

        private int2 ingredientAppearRangeX = new int2(-240, 240);
        private int ingredinetAppearY = 400;
        private int ingredientDestY = -600;

        private List<StackHamburgerIngredientData> _ingredientDatas = new List<StackHamburgerIngredientData>();
        private List<StackHamburgerIngredient> _ingredients = new List<StackHamburgerIngredient>();

        private List<EElement> _catchList = new List<EElement>();

        private Queue<KeyValuePair<EElement, float>> _elementAppearTime = new Queue<KeyValuePair<EElement, float>>();

        private Dictionary<EElement, float> _elementToElapsedTime = new Dictionary<EElement, float>();

        public override void Clear()
        {
            base.Clear();

            _config = null;

            _ingredientDatas.Clear();
            _ingredients.Clear();

            _catchList.Clear();

            _elementAppearTime.Clear();

            _elementToElapsedTime.Clear();

            foreach (var pair in _ingredientObjDic)
            {
                pair.Value.IsCatched = false;
                pair.Value.Initialize();
                pair.Value.transform.SetParent(_trIngredientRoot);
            }

            if (_mainCharacter != null)
            {
                Destroy(_mainCharacter.gameObject);
                _mainCharacter = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inConfig"></param>
        public override void Initialize(MiniGameData inConfig)
        {
            base.Initialize(inConfig);

            _config = inConfig as StackHamburgerData;

            if (_config == null)
                return;

            StackHamburgerIngredientData trapData = null;

            for (int i = 0; i < _config.Targets.Count; i++)
            {
                if (_config.Targets[i].Element == EElement.Trap)
                {
                    trapData = _config.Targets[i];
                    continue;
                }

                StackHamburgerIngredientData data = _config.Targets[i];
                _ingredientDatas.Add(data);
                _ingredients.Add(_ingredientObjDic[data.Element]);
            }

            if (_ingredientDatas.Count <= 0 || _ingredients.Count <= 0)
                return;

            if (_config.TrapAppearLevel <= MiniGameSharedData.Instance.StageCount)
            {
                int insertIndex = UnityEngine.Random.Range(0, _ingredientDatas.Count - 1);
                _ingredientDatas.Insert(insertIndex, trapData);
                _ingredients.Add(_ingredientObjDic[trapData.Element]);
            }

            for (int i = 0; i < _ingredients.Count; i++)
            {
                _ingredients[i].Initialize();
                _ingredients[i].OnTrigger.AddListener(OnTrigger);
                _ingredients[i].OnFail.AddListener(OnFail);
                _ingredients[i].transform.position = new Vector3(0, ingredinetAppearY, 0);
            }

            _mainCharacter = CharacterManager.Instance.GetMainCharacter(transform);
        }

        public void OnFail(StackHamburgerIngredient inIngredient)
        {
            if (IsPlaying == false)
            {
                return;
            }

            if (inIngredient.Element == EElement.Trap)
            {
                return;
            }

            // 일단 미니게임 일시정지
            MiniGameManager.OnMiniGamePause.Invoke();

            // 연출 보여줄거면 보여주고 게임 종료
            StartCoroutine(OnFail(() => MiniGameManager.OnMiniGameEnd.Invoke(false)));
        }

        public void OnTrigger(StackHamburgerIngredient inIngredient)
        {
            if (IsPlaying == false)
            {
                return;
            }

            if (_catchList.Contains(inIngredient.Element))
            {
                return;
            }    

            inIngredient.transform.SetParent(_mainCharacter.transform);
            inIngredient.IsCatched = true;
            inIngredient.Tween.Kill();
            inIngredient.Tween = null;

            if (inIngredient.Element == EElement.Trap)
            {
                // 일단 미니게임 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();

                // 연출 보여줄거면 보여주고 게임 종료
                StartCoroutine(OnFail(() => MiniGameManager.OnMiniGameEnd.Invoke(false)));
                return;
            }

            _catchList.Add(inIngredient.Element);

            if (_catchList.Count == _ingredientObjDic.Count - 1)
            {
                // 일단 미니게임 일시정지
                MiniGameManager.OnMiniGamePause.Invoke();

                // 연출 보여줄거면 보여주고 게임 종료
                StartCoroutine(OnSuccess(() => MiniGameManager.OnMiniGameEnd.Invoke(true)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerator OnStart()
        {
            _mainCharacter.ChangeAddOn(EAddOn.HAMBURGER);
            _mainCharacter.transform.position = new Vector3(0, -460, 0);

            yield return null;
            _mainCharacter.PlayAnimator(EAnim.HAMBURGER);

            yield return new WaitForSeconds(0.5f);

            MiniGameManager.OnMiniGamePostStart.Invoke();
        }

        public override void OnPostStart()
        {
            base.OnPostStart();

            float nativeTotalTime = MiniGameSharedData.Instance.Config.PlayTime;
            float totalTime = MiniGameSharedData.Instance.CurStageTime;
            float durationDecresedRate = totalTime / nativeTotalTime;
            int ingredientCount = _ingredientDatas.Count;

            List<StackHamburgerIngredientData> clonedData = new List<StackHamburgerIngredientData>(_ingredientDatas.ToArray());

            float totalDropTime = 0;
            for (int i = 0; i < ingredientCount; i++)
            {
                float dropTime = clonedData[i].DropTIme * durationDecresedRate;
                totalDropTime += dropTime;
                _elementToElapsedTime.Add(clonedData[i].Element, dropTime);
            }

            float spareTime = totalTime - totalDropTime;

            float startTIme = UnityEngine.Random.Range(spareTime / (ingredientCount), spareTime / (ingredientCount + 2));
            spareTime -= startTIme;

            clonedData = ShuffleList(clonedData);

            for (int i = 0; i < ingredientCount; i++)
            {
                if (clonedData[i].Element == EElement.Bread)
                    continue;

                _elementAppearTime.Enqueue(new(clonedData[i].Element, startTIme));
                startTIme += _elementToElapsedTime[clonedData[i].Element];
                startTIme += UnityEngine.Random.Range(spareTime / (ingredientCount - 1), spareTime / (ingredientCount + 1));
            }

            for (int i = 0; i < ingredientCount; i++)
            {
                if (clonedData[i].Element == EElement.Bread)
                {
                    _elementAppearTime.Enqueue(new(clonedData[i].Element, startTIme));
                    break;
                }
            }
        }

        private List<T> ShuffleList<T>(List<T> list)
        {
            int random1, random2;
            T temp;

            for (int i = 0; i < list.Count; ++i)
            {
                random1 = UnityEngine.Random.Range(0, list.Count);
                random2 = UnityEngine.Random.Range(0, list.Count);

                temp = list[random1];
                list[random1] = list[random2];
                list[random2] = temp;
            }

            return list;
        }

        private Vector3 _lastTouchPos;

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (IsPlaying == false)
            {
                return;
            }

            if (_elementAppearTime.Count > 0 && _elementAppearTime.Peek().Value < MiniGameSharedData.Instance.CurStageTime - MiniGameSharedData.Instance.LeftTime)
            {
                var pair = _elementAppearTime.Dequeue();
                float randX = UnityEngine.Random.Range(ingredientAppearRangeX.x, ingredientAppearRangeX.y);
                float y = ingredinetAppearY;

                _ingredientObjDic[pair.Key].Move(new Vector3(randX, ingredinetAppearY, 0), new Vector3(randX, ingredientDestY, 0), _elementToElapsedTime[pair.Key]);
            }

#if UNITY_EDITOR
            if (Input.GetMouseButton(0) == false)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = new Touch[] {
                new Touch()
                {
                    position = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    phase = _lastTouchPos == Vector3.zero ? TouchPhase.Began : TouchPhase.Moved,
                }
            };
#else
            if (Input.touchCount <= 0)
            {
                _lastTouchPos = Vector3.zero;
                return;
            }

            Touch[] touches = Input.touches;
#endif
            Touch touch = touches[0];
            Vector3 pos = Camera.main.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _lastTouchPos = pos;
                return;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (_lastTouchPos == Vector3.zero)
                    _lastTouchPos = pos;

                Vector3 moveDist = pos - _lastTouchPos;
                _mainCharacter.transform.Translate(new Vector3(moveDist.x, 0, 0), Space.World);
                _lastTouchPos = pos;

                if (_mainCharacter.transform.localPosition.x > ingredientAppearRangeX.y)
                    _mainCharacter.transform.localPosition = new Vector3(240, _mainCharacter.transform.localPosition.y, _mainCharacter.transform.localPosition.z);
                if (_mainCharacter.transform.localPosition.x < ingredientAppearRangeX.x)
                    _mainCharacter.transform.localPosition = new Vector3(-240, _mainCharacter.transform.localPosition.y, _mainCharacter.transform.localPosition.z);
            }
        }
    }
}
