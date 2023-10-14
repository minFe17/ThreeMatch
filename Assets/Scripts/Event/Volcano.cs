using System.Collections;
using UnityEngine;
using Utils;

public class Volcano : Event
{
    Shuffle _shuffle;

    protected override void Start()
    {
        base.Start();
        _eventType = EEventType.Volcano;
        _eventManager.Events.Add(this);
        _shuffle = GenericSingleton<EventManager>.Instance.Shuffle;
    }

    public override void EventEffect()
    {
        _gameManager.GameState = EGameStateType.Event;
        StartCoroutine(VolcanoRoutine());
    }

    void Eruption()
    {
        int creatableTiles = Random.Range(_minCreatableTiles, _maxCreatableTiles);
        for (int i = 0; i <= creatableTiles; i++)
        {
            Debug.Log("CreateLavaTile");

            _tileManager.CreateLavaTiles();
        }
        _shuffle.ShuffleFruit();
    }

    IEnumerator VolcanoRoutine()
    {
        // ui 이미지 보여주기
        yield return new WaitForSeconds(0.5f);
        Eruption();

        while (!_shuffle.EndShuffle)
        {
            yield return new WaitForSeconds(0.1f);
            if (_shuffle.EndShuffle)
                break;
        }
        // ui 이미지 숨기기
        yield return new WaitForSeconds(0.5f);
        _gameManager.GameState = EGameStateType.Move;
    }
}
