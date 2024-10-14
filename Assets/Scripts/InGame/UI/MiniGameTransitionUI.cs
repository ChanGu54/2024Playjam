using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace PlayJam.InGame.UI
{
    public class MiniGameTransitionUI : BaseMiniGameUI
    {
        [SerializeField]
        private TextMeshProUGUI _txtTitle;

        [SerializeField]
        private TextMeshProUGUI _txtDesc;

        [SerializeField]
        private Animator[] _animHearts;

        [SerializeField]
        private Animator _animRoot;

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            for (int i = 0; i < _animHearts.Length; i++)
            {
                string animClipName = i < MiniGameSharedData.Instance.Config.InitialHeartCount ? "On" : "Off";
                _animHearts[i].Play(animClipName);
            }

            StartCoroutine(Co_OnMiniGameEnd(true, null));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnMiniGameStart()
        {
            _animRoot.Play("Disappear");
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnMiniGamePause()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnMiniGameResume()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override IEnumerator Co_OnMiniGameEnd(bool isSuccess, Action inCallback)
        {
            _txtTitle.text = $"STAGE {MiniGameSharedData.Instance.StageCount}";

            _animRoot.Play("Appear");

            switch (MiniGameSharedData.Instance.CurrentMiniGameData.InputMethod)
            {
                case EInputMethod.DRAG:
                    _txtDesc.text = "DRAG";
                    break;
                case EInputMethod.TOUCH:
                    _txtDesc.text = "TOUCH";
                    break;
                case EInputMethod.TOUCH_DRAG:
                    _txtDesc.text = "TOUCH & DRAG";
                    break;
            }

            for (int i = 0; i < _animHearts.Length; i++)
            {
                string animClipName = i < MiniGameSharedData.Instance.HeartCount ? "On" : i == MiniGameSharedData.Instance.HeartCount ? isSuccess == false ? "Broken" : "Off" : "Off";
                _animHearts[i].Play(animClipName);
            }

            yield return null;

            float duration = 0;

            for (int i = 0; i < _animHearts.Length; i++)
            {
                duration = Mathf.Max(duration, _animHearts[i].GetCurrentAnimatorClipInfo(0)[0].clip.length);
            }

            yield return new WaitForSeconds(duration);

            inCallback?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {

        }
    }
}