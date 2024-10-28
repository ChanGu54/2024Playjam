using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace PlayJam.Character
{
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
    }

    public enum EAddOn
    {
        IDLE,
        HAMBURGER,
        PAN,
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
        SPOON,
    }

    public enum ECloth
    {
        IDLE,
        APRON,
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
        private SerializedDictionary<EEmotion, GameObject> _emotionDic;

        [SerializeField]
        private SerializedDictionary<EArm, GameObject> _armDic;

        [SerializeField]
        private SerializedDictionary<ECloth, GameObject> _clothDic;

        [SerializeField]
        private SerializedDictionary<EHat, GameObject> _hatDic;

        [SerializeField]
        private SerializedDictionary<EAddOn, GameObject> _addOnDic;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inEmotion"></param>
        /// <param name="inArm"></param>
        /// <param name="inCloth"></param>
        public void Initialize(EEmotion inEmotion, EArm inArm, ECloth inCloth, EHat inHat, EAddOn inAddOn)
        {
            ChangeEmotion(inEmotion);
            ChangeArm(inArm);
            ChangeCloth(inCloth);
            ChangeHat(inHat);
            ChangeAddOn(inAddOn);
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

        public void ChangeCloth(ECloth inCloth)
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
                    item.Value.SetActive(true);
                else
                    item.Value.SetActive(false);
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