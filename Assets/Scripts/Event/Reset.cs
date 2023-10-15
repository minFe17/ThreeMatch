using System.Collections;
using UnityEngine;

public class Reset : Event
{
    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Reset;
        _eventManager.Reset = this;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(ResetRoutine());
    }
    
    IEnumerator ResetRoutine()
    {
        // ui 리셋 이미지 보여주기
        yield return new WaitForSeconds(_eventDelay);
        _tileManager.ResetTile();
        // ui 리셋 이미지 숨기기
        yield return new WaitForSeconds(_eventDelay);
        _gameManager.GameState = EGameStateType.Move;
    }
}
