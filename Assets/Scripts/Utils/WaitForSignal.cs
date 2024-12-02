using System.Collections;
using UnityEngine;

namespace PlayJam.Utils
{
    /// <summary>
    /// 시그널 대기 로직
    /// </summary>
    public class WaitForSignal
    {
        /// <summary>
        /// 시그널 Flag
        /// </summary>
        private bool _flag;

        /// <summary>
        /// 생성자
        /// </summary>
        public WaitForSignal()
        {
            _flag = false;
        }

        /// <summary>
        /// 플래그 값 변경
        /// </summary>
        public void Signal()
        {
            _flag = true;
        }

        /// <summary>
        /// 시그널 값 왔는지 체크
        /// </summary>
        /// <returns></returns>
        public bool HasSignal()
        {
            return _flag;
        }

        /// <summary>
        /// 플래그 값 변경 대기
        /// </summary>
        /// <returns></returns>
        public IEnumerator Wait()
        {
            while (true)
            {
                if (HasSignal() == true)
                {
                    _flag = false;
                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _flag = false;
        }
    }
}
