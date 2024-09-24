
using UnityEngine;

namespace utils
{
    /// <summary>
    /// 어디서든 접근 가능한 singleton 객체
    /// DontDestroyOnLoad() 가 기본으로 들어가 있다
    /// 최대한 적게 사용하는게 좋을듯...
    /// - 기존 처리를 참고하여 동시접근 관련된 처리 추가
    /// - OnDestroy에서는 절대 MonoSingleton.Instance에 접근해서는 안된다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        static T m_instance;
        static bool m_isReceiveUnityQuitEvent;
        static object m_lock = new object();

        /// <summary>
        /// false 일 경우 DontDestroy를 사용하지 않음.
        /// scene 전환시 메모리에서 제거됨
        /// </summary>
        protected virtual bool IsDontDestroy => true;

        /// <summary>
        /// 해당 singleton 객체가 유니티 applicationQuit 이벤트를 받았는지 확인한다.
        /// </summary>
        protected bool IsReceiveUnityQuitEvent
        {
            get => m_isReceiveUnityQuitEvent;
            set => m_isReceiveUnityQuitEvent = value;
        }

        public static T Instance
        {
            get
            {
                // 이미 quit 이벤트를 받아서 처리중인데 생성 get을 하려고 할때 처리
                // 사실상 이미 instance가 null 이지만 thread safe상에서 접근 할 수도 있으니..
                // NOTE @changu.lee  런타임이 아닐 때 호출할 경우 인스턴스 생성 가능해야 함
                if (m_isReceiveUnityQuitEvent && Application.isPlaying == true)
                    m_instance = null;
                else
                {
                    // instance가 없으면
                    if (NullChecker.IsNull(m_instance))
                    {
                        lock (m_lock)
                        {
                            // type으로 찾아보고
                            var findObjects = FindObjectsOfType<T>();

                            // 찾은 object가 없거나 1개보다 많은 경우
                            // 새로 생성
                            if (findObjects == null || findObjects.Length <= 0)
                            {
                                GameObject g = new GameObject($"[S] {typeof(T).FullName}");
                                g.hideFlags = HideFlags.None;

                                m_instance = g.AddComponent<T>();

                                // don't destroy에 대한 옵션 기능 추가
                                if (m_instance is MonoSingleton<T> monoInstance && monoInstance.IsDontDestroy && Application.isPlaying)
                                    DontDestroyOnLoad(g);
                            }
                            else
                            {
                                m_instance = findObjects[0];

                                if (findObjects.Length > 1)
                                {
                                    Debug.LogWarning($"{nameof(T)} is more than 2!!!!");
                                }
                            }
                        }
                    }
                }

                return m_instance;
            }

            protected set
            {
                m_instance = value;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            m_isReceiveUnityQuitEvent = true;
            m_instance = null;
        }

        protected virtual void Awake()
        {
            m_isReceiveUnityQuitEvent = false;
        }
    }
}
