using System.Collections;
using System.Collections.Generic;
using PlayJam.InGame;
using UnityEngine;

public class TestGamePlayer : MonoBehaviour
{
    [SerializeField]
    private MiniGameManager _miniGameManager;

    private void Awake()
    {
        StartCoroutine(_miniGameManager.Co_StartGame(true));
    }
}
