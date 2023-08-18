using System.Collections;
using UnityEngine;
using Utils;

public class Fruit : MonoBehaviour
{
    [SerializeField] EFruitType _fruitType;
    [SerializeField] float _moveSpeed;

    FruitManager _fruitManager;
    GameManager _gameManager;
    MatchFinder _matchFinder;
    Fruit _otherFruit;
    GameObject _destroyEffect;

    Vector2 _firstTouchPos;
    Vector2 _finalTouchPos;
    Vector2 _position;

    int _column;
    int _row;
    int _previousColumn;
    int _previousRow;
    int _targetX;
    int _targetY;
    float _swipeAngle;
    float _swipeResist = 1f;
    bool _isMatch;
    bool _onEffect;

    public EFruitType FruitType { get => _fruitType; }
    public int Column { get => _column; set => _column = value; }
    public int Row { get => _row; set => _row = value; }
    public bool IsMatch { get => _isMatch; set => _isMatch = value; }


    void Start()
    {
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _gameManager = GenericSingleton<GameManager>.Instance;
        _matchFinder = GenericSingleton<MatchFinder>.Instance;
        _destroyEffect = Resources.Load("Prefabs/DestroyEffect") as GameObject;
    }

    void Update()
    {
        MoveFruit();
        MatchFruit();
    }

    void MoveFruit()
    {
        _targetX = _column;
        _targetY = _row;
        if (Mathf.Abs(_targetX - transform.position.x) > 0.1f)
        {
            _position = new Vector2(_targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
            if (_fruitManager.AllFruits[_column, _row] != this)
                _fruitManager.AllFruits[_column, _row] = this;
            _matchFinder.FindAllMatch();
        }

        else
        {
            _position = new Vector2(_targetX, transform.position.y);
            transform.position = _position;
        }

        if (Mathf.Abs(_targetY - transform.position.y) > 0.1f)
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = Vector2.Lerp(transform.position, _position, _moveSpeed);
            if (_fruitManager.AllFruits[_column, _row] != this)
                _fruitManager.AllFruits[_column, _row] = this;
            _matchFinder.FindAllMatch();
        }
        else
        {
            _position = new Vector2(transform.position.x, _targetY);
            transform.position = _position;
            _fruitManager.AllFruits[_column, _row] = this;
        }
    }

    void MatchFruit()
    {
        if (_isMatch)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0.5f, 0.5f, 0.5f, 1);
            if (!_onEffect)
            {
                Instantiate(_destroyEffect, transform.position, Quaternion.identity);
                _onEffect = true;
            }
        }
    }

    void CalculateAngle()
    {
        float x = _finalTouchPos.x - _firstTouchPos.x;
        float y = _finalTouchPos.y - _firstTouchPos.y;
        if (Mathf.Abs(x) > _swipeResist || Mathf.Abs(y) > _swipeResist)
        {
            _swipeAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            SeleteMoveFruit();
            _gameManager.GameState = EGameStateType.Wait;
        }
        else
            _gameManager.GameState = EGameStateType.Move;
    }

    void SeleteMoveFruit()
    {
        if (_swipeAngle > -45 && _swipeAngle <= 45 && _column < _fruitManager.Width)    //Right
        {
            _previousColumn = _column;
            _previousRow = _row;
            _otherFruit = _fruitManager.AllFruits[_column + 1, _row];
            _otherFruit.Column -= 1;
            _column += 1;
        }
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && _row < _fruitManager.Height) //Up
        {
            _previousColumn = _column;
            _previousRow = _row;
            _otherFruit = _fruitManager.AllFruits[_column, _row + 1];
            _otherFruit.Row -= 1;
            _row += 1;
        }
        else if (_swipeAngle > 135 || _swipeAngle <= -135 && _column > 0)               //Left
        {
            _previousColumn = _column;
            _previousRow = _row;
            _otherFruit = _fruitManager.AllFruits[_column - 1, _row];
            _otherFruit.Column += 1;
            _column -= 1;
        }
        else if (_swipeAngle < -45 && _swipeAngle >= -135 && _row > 0)                  //Down
        {
            _previousColumn = _column;
            _previousRow = _row;
            _otherFruit = _fruitManager.AllFruits[_column, _row - 1];
            _otherFruit.Row += 1;
            _row -= 1;
        }

        StartCoroutine(CheckMoveRoutine());
    }

    public IEnumerator CheckMoveRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (_otherFruit != null)
        {
            if (!_isMatch && !_otherFruit.IsMatch)
            {
                _otherFruit.Column = _column;
                _otherFruit.Row = _row;
                _column = _previousColumn;
                _row = _previousRow;
                yield return new WaitForSeconds(0.5f);
                _gameManager.GameState = EGameStateType.Move;
            }
            else
                _fruitManager.CheckMatchsFruit();

            _otherFruit = null;
        }
    }

    private void OnMouseDown()
    {
        if (_gameManager.GameState == EGameStateType.Move)
            _firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (_gameManager.GameState == EGameStateType.Move)
        {
            _finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
}
