using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PlayJam.Popup
{
    public class PopupComingSoon : BasePopup
    {
        public override void PrevOpen()
        {
            base.PrevOpen();
            StartCoroutine(Co_CloseTimer(1f, () =>
            {
                PopupManager.Instance.Exit<PopupComingSoon>();
            }));
        }

        private IEnumerator Co_CloseTimer(float inTime, Action inCallback)
        {
            yield return new WaitForSeconds(inTime);
            inCallback.Invoke();
        }
    }
}
