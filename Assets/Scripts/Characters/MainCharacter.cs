using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace PlayJam.Character
{
    public enum ECostume
    {
        IDLE,
        BEAR,
        CAT,
        CHEF,
        COOLCHEF,
        CUTE,
        FAIRY,
        FLOWER,
        HONEYBEE,
        MICHELIN,
        TOMATO,
        WAITER,
        WHITECHEF,
    }

    public enum EAnim
    {
        IDLE,
        HAMBURGER,
        FRIEDRICE_STIR_SUCCESS,
        FRIEDRICE_STIR_FAIL,
    }

    public enum EHat
    {
        IDLE,
        BEAR,
        CAT,
        CHEF,
        COOLCHEF,
        CUTE,
        FAIRY,
        FLOWER,
        HONEYBEE,
        TOMATO,
    }

    public enum EAddOn
    {
        IDLE,
        HAMBURGER,
        PAN,
        SPOON,
        COOLCHEF,
        CUTE,
        FAIRY,
    }

    public enum EEmotion
    {
        IDLE,
        SMILE,
        SAD,
        EMBARRASSED,
    }

    public enum EArm
    {
        IDLE,
        BEAR,
        CAT,
        CHEF,
        COOLCHEF,
        CUTE,
        MICHELIN,
        TOMATO,
        WAITER,
        WHITECHEF,
    }

    public enum EBody
    {
        IDLE,
        APRON,
        BEAR,
        CAT,
        CHEF,
        COOLCHEF,
        CUTE,
        FAIRY,
        HONEYBEE,
        MICHELIN,
        TOMATO,
        WAITER,
        WHITECHEF,
    }

    /// <summary>
    /// 
    /// </summary>
    public class MainCharacter : MonoBehaviour
    {
        [SerializeField]
        private Animator _characterAnim;

        public Animator CharacterAnimator => _characterAnim;

        [SerializeField]
        private GameObject _objEar;

        [SerializeField]
        private SerializedDictionary<EEmotion, GameObject> _emotionDic;

        [SerializeField]
        private SerializedDictionary<EArm, List<GameObject>> _armDic;

        [SerializeField]
        private SerializedDictionary<EBody, GameObject> _clothDic;

        [SerializeField]
        private SerializedDictionary<EHat, GameObject> _hatDic;

        [SerializeField]
        private SerializedDictionary<EAddOn, GameObject> _addOnDic;

        [SerializeField]
        private ECostume _costume;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inEmotion"></param>
        /// <param name="inArm"></param>
        /// <param name="inCloth"></param>
        public void Initialize(EEmotion inEmotion, ECostume inCostume)
        {
            ChangeEmotion(inEmotion);
            SetCostume(inCostume);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inEmotion"></param>
        /// <param name="inArm"></param>
        /// <param name="inCloth"></param>
        public void Initialize(EEmotion inEmotion, EArm inArm, EBody inCloth, EHat inHat, EAddOn inAddOn, bool hasEar)
        {
            ChangeEmotion(inEmotion);
            ChangeArm(inArm);
            ChangeCloth(inCloth);
            ChangeHat(inHat);
            ChangeAddOn(inAddOn);
            SetActiveEar(hasEar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inEmotion"></param>
        /// <param name="inArm"></param>
        /// <param name="inCloth"></param>
        public void Initialize(EArm inArm, EBody inCloth, EHat inHat, EAddOn inAddOn, bool hasEar)
        {
            ChangeArm(inArm);
            ChangeCloth(inCloth);
            ChangeHat(inHat);
            ChangeAddOn(inAddOn);
            SetActiveEar(hasEar);
        }

        public void SetActiveEar(bool inActive)
        {
            _objEar.SetActive(inActive);
        }

        public void SetCostume(ECostume inCostume)
        {
            _costume = inCostume;

            switch (inCostume)
            {
                case ECostume.IDLE:
                    Initialize(EArm.IDLE, EBody.IDLE, EHat.IDLE, EAddOn.IDLE, true);
                    break;
                case ECostume.BEAR:
                    Initialize(EArm.BEAR, EBody.BEAR, EHat.BEAR, EAddOn.IDLE, false);
                    break;
                case ECostume.CAT:
                    Initialize(EArm.CAT, EBody.CAT, EHat.CAT, EAddOn.IDLE, false);
                    break;
                case ECostume.CHEF:
                    Initialize(EArm.CHEF, EBody.CHEF, EHat.CHEF, EAddOn.IDLE, false);
                    break;
                case ECostume.COOLCHEF:
                    Initialize(EArm.COOLCHEF, EBody.COOLCHEF, EHat.COOLCHEF, EAddOn.COOLCHEF, false);
                    break;
                case ECostume.CUTE:
                    Initialize(EArm.CUTE, EBody.CUTE, EHat.CUTE, EAddOn.CUTE, false);
                    break;
                case ECostume.FAIRY:
                    Initialize(EArm.IDLE, EBody.FAIRY, EHat.FAIRY, EAddOn.FAIRY, false);
                    break;
                case ECostume.FLOWER:
                    Initialize(EArm.IDLE, EBody.IDLE, EHat.FLOWER, EAddOn.IDLE, false);
                    break;
                case ECostume.HONEYBEE:
                    Initialize(EArm.IDLE, EBody.HONEYBEE, EHat.HONEYBEE, EAddOn.IDLE, false);
                    break;
                case ECostume.MICHELIN:
                    Initialize(EArm.MICHELIN, EBody.MICHELIN, EHat.IDLE, EAddOn.IDLE, true);
                    break;
                case ECostume.TOMATO:
                    Initialize(EArm.TOMATO, EBody.TOMATO, EHat.TOMATO, EAddOn.IDLE, true);
                    break;
                case ECostume.WAITER:
                    Initialize(EArm.WAITER, EBody.WAITER, EHat.IDLE, EAddOn.IDLE, true);
                    break;
                case ECostume.WHITECHEF:
                    Initialize(EArm.WHITECHEF, EBody.WHITECHEF, EHat.IDLE, EAddOn.IDLE, true);
                    break;
            }
        }

        public void ChangeEmotion(EEmotion inEmotion)
        {
            foreach (var item in _emotionDic)
            {
                if (item.Value == null)
                    continue;

                if (item.Key == inEmotion)
                    item.Value.SetActive(true);
                else
                    item.Value.SetActive(false);
            }
        }

        public void ChangeCloth(EBody inCloth)
        {
            foreach (var item in _clothDic)
            {
                if (item.Value == null)
                    continue;

                if (item.Key == inCloth)
                    item.Value.SetActive(true);
                else
                    item.Value.SetActive(false);
            }
        }

        public void ChangeArm(EArm inArm)
        {
            foreach (var item in _armDic)
            {
                if (item.Value == null)
                    continue;

                if (item.Key == inArm)
                    item.Value.ForEach(x => x.SetActive(true));
                else
                    item.Value.ForEach(x => x.SetActive(false));
            }
        }

        public void ChangeAddOn(EAddOn inAddOn)
        {
            foreach (var item in _addOnDic)
            {
                if (item.Value == null)
                    continue;

                if (item.Key == inAddOn)
                    item.Value.SetActive(true);
                else
                    item.Value.SetActive(false);
            }
        }

        public void ChangeHat(EHat inHat)
        {
            foreach (var item in _hatDic)
            {
                if (item.Value == null)
                    continue;

                if (item.Key == inHat)
                    item.Value.SetActive(true);
                else
                    item.Value.SetActive(false);
            }
        }

        public void PlayAnimator(EAnim inAnim)
        {
            _characterAnim.Play(inAnim.ToString());
        }
    }
}