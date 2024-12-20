using System;
using System.Collections;
using UnityEngine;

namespace PlayJam.InGame.UI
{
    public class BaseMiniGameUI : MonoBehaviour
    {
        public virtual void Initialize()
        {

        }

        public virtual void OnMiniGameStart()
        {

        }

        public virtual void OnMiniGamePause()
        {

        }

        public virtual void OnMiniGameResume()
        {

        }

        public virtual IEnumerator OnPostInitialize(Action inCallback)
        {
            yield return null;
            inCallback?.Invoke();
        }

        public virtual IEnumerator Co_OnMiniGameEnd(bool isSuccess, Action inCallback)
        {
            yield return null;
            inCallback?.Invoke();
        }

        public virtual IEnumerator Co_OnMiniGameQuit(Action inCallback)
        {
            yield return null;
            inCallback?.Invoke();
        }

        public virtual void Clear()
        {

        }
    }
}
