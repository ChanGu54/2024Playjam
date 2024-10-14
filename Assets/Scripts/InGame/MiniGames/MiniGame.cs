using System;
using System.Collections;
using UnityEngine;

namespace PlayJam.InGame
{
    public class MiniGamePlayer : MonoBehaviour
    {
        public bool IsPlaying = false;

        public virtual void Initialize(MiniGameData inData)
        {

        }

        public virtual IEnumerator OnStart()
        {
            yield return null;
        }

        public virtual void OnPostStart()
        {
            IsPlaying = true;
        }

        public virtual IEnumerator OnPause()
        {
            yield return null;
        }

        public virtual IEnumerator OnResume()
        {
            yield return null;
        }

        public virtual IEnumerator OnSuccess(Action inCallback)
        {
            yield return null;
            inCallback?.Invoke();
        }

        public virtual IEnumerator OnFail(Action inCallback)
        {
            yield return null;
            inCallback?.Invoke();
        }

        public virtual void Clear()
        {
            IsPlaying = false;
        }
    }
}
