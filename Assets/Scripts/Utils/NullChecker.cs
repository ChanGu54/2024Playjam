
/// <summary>
/// 유니티의 fake null 채크 기능을 추가, 유니티 object만 해당 부분 사용
/// 원래 ANIPANGNEXT.Util의 namespace를 가져야 하지만 범용성을 위해 Util은 제거
/// </summary>
public static class NullChecker
{
    /// <summary>
    /// 확장 메소드 버전
    /// </summary>
    /// <param name="unityObj"></param>
    /// <returns></returns>
    public static bool IsNull(this UnityEngine.Object unityObj)
    {
        return ReferenceEquals(unityObj, null) || !unityObj;
    }

    public static bool IsNotNull(this UnityEngine.Object unityObj)
    {
        return !IsNull(unityObj);
    }

    public static bool IsNull(this object obj)
    {
        return ReferenceEquals(obj, null);
    }

    public static bool IsNotNull(this object obj)
    {
        return !ReferenceEquals(obj, null);
    }

    ///// <summary>
    ///// IsNull의 명시적 버전
    ///// </summary>
    ///// <param name="unityObj"></param>
    ///// <returns></returns>
    //public static bool IsN(UnityEngine.Object unityObj)
    //{
    //    return ReferenceEquals(unityObj, null) || !unityObj;
    //}
}
