
namespace utils
{
    /// <summary>
    /// 일반 object에 대한 singleton
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T>
        where T : class
    {
        static T m_instance;

        public static T Instance
        {
            get
            {
                if (NullChecker.IsNull(m_instance))
                {
                    m_instance = System.Activator.CreateInstance<T>();
                }

                return m_instance;
            }

            protected set
            {
                m_instance = value;
            }
        }
    }
}
