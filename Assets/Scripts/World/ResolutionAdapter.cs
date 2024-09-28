using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionAdapter : MonoBehaviour
{
    private enum EAnchor
    {
        Up,
        Down,
    }

    [SerializeField]
    private float _baseResolutionWidth = 720;

    [SerializeField]
    private float _baseResolutionHeight = 1280;

    [SerializeField]
    private bool _isActivated = false;

    [SerializeField]
    private EAnchor _anchor;

    private Vector3 _basePos;

    private float _cachedRatio;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        _basePos = transform.position;
        Update();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (_isActivated == false)
            return;

        float curRatio = (float)Screen.width / Screen.height;
        if (curRatio == _cachedRatio)
            return;

        _cachedRatio = curRatio;

        float ratio = _baseResolutionWidth / _baseResolutionHeight;
        float adaptScale = 1;
        float multiplyRate = _baseResolutionWidth / Screen.width;
        float fixedHeight = Screen.height * multiplyRate;
        float yVal = 0;

        if (ratio > curRatio)
        {
            adaptScale = curRatio / ratio;
            yVal = (fixedHeight - _baseResolutionHeight) / 2f * adaptScale;
        }
        else
        {
        }

        transform.localScale = Vector3.one * adaptScale;

        switch (_anchor)
        {
            case EAnchor.Up:
                transform.position = _basePos + Vector3.up * yVal;
                break;
            case EAnchor.Down:
                transform.position = _basePos - Vector3.up * yVal;
                break;
        }
    }
}
