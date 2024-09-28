using System;
using System.Collections;
using UnityEngine;

public class BaseMiniGameController : MonoBehaviour
{
    /// <summary>
    /// 게임 초기화
    /// </summary>
    public virtual void Initialize()
    {
        
    }

    /// <summary>
    /// 데이터 삭제 처리
    /// </summary>
    public virtual void Clear()
    {

    }

    /// <summary>
    /// 게임 시작 전 호출
    /// </summary>
    /// <param name="inEndCallback"></param>
    /// <returns></returns>
    public virtual IEnumerator Co_PrevStartGame(Action inEndCallback)
    {
        yield return null;
        inEndCallback?.Invoke();
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    /// <param name="inEndCallback"></param>
    /// <returns></returns>
    public virtual IEnumerator Co_StartGame(Action inEndCallback)
    {
        yield return null;
        inEndCallback?.Invoke();
    }

    /// <summary>
    /// 게임 종료 ㅈ
    /// </summary>
    /// <param name="inEndCallback"></param>
    /// <returns></returns>
    public virtual IEnumerator Co_PrevEndGame(Action inEndCallback)
    {
        yield return null;
        inEndCallback?.Invoke();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    /// <param name="inEndCallback"></param>
    /// <returns></returns>
    public virtual IEnumerator Co_EndGame(Action inEndCallback)
    {
        yield return null;
        inEndCallback?.Invoke();
    }
}
