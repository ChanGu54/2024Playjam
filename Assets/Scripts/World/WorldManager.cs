using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private enum EAnchor
    {
        Up,
        Center,
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

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        _basePos = transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (_isActivated == false)
            return;

        float ratio = _baseResolutionWidth / _baseResolutionHeight;
        float curRatio = (float)Screen.width / Screen.height;
        float adaptScale = 1;

        if (ratio > curRatio)
        {
            adaptScale = curRatio / ratio;
        }

        transform.localScale = Vector3.one * adaptScale;

        if (adaptScale == 1 || _anchor == EAnchor.Center)
        {
            transform.position = _basePos;
        }
        else
        {
            float multiplyRate = _baseResolutionWidth / Screen.width;
            float fixedHeight = Screen.height * multiplyRate;
            float yVal = (fixedHeight - _baseResolutionHeight) / 2f * adaptScale;

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
}
