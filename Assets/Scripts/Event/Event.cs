using UnityEngine;
using Utils;

public abstract class Event : MonoBehaviour
{
    protected EEventType _eventType;
    protected EventManager _eventManager;
    protected FruitManager _fruitManager;
    protected GameManager _gameManager;
    protected TileManager _tileManager;
    protected int _width;
    protected int _height;
    protected int _maxCreatableTiles = 10;

    protected virtual void Start()
    {
        _eventManager = GenericSingleton<EventManager>.Instance;
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _tileManager = GenericSingleton<TileManager>.Instance;

        _width = _fruitManager.Width;
        _height = _fruitManager.Height;
    }

    public abstract void EventEffect();
}