using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Board : MonoBehaviour
{
    [Header("Board Size")]
    [SerializeField] int _width;
    [SerializeField] int _height;
    [SerializeField] int _offset;

    List<TileType> _boardLayout = new List<TileType>();
    GameObject[,] _allTiles;
    bool[,] _blankSpaces;
    IceTile[,] _iceTiles;

    GameObject _cameraPrefab;
    GameObject _backgroundPrefab;

    [Header("Tile Prefabs")]
    GameObject _tilePrefab;
    GameObject _iceTilePrefab;

    FruitManager _fruitManager;

    public bool[,] BlankSpaces { get => _blankSpaces; }
    public IceTile[,] IceTiles { get => _iceTiles; }

    void Start()
    {
        Init();
        CreateCamera();
        CreateBoard();
    }

    void Init()
    {
        SetTileArray();
        LoadResource();

        GenericSingleton<GameManager>.Instance.Init();
        _fruitManager = GenericSingleton<FruitManager>.Instance;
        _fruitManager.Init(_width, _height, this);
        _fruitManager.Offset = _offset;
    }

    void CreateBoard()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector2Int position = new Vector2Int(i, j + _offset);
                Transform tilePos = CreateTile(i, j);
                _fruitManager.CreateFruit(tilePos, position);
            }
        }
        GenericSingleton<HintManager>.Instance.Init();
    }

    void SetTileArray()
    {
        _allTiles = new GameObject[_width, _height];
        _blankSpaces = new bool[_width, _height];
        _iceTiles = new IceTile[_width, _height];
    }

    void LoadResource()
    {
        _cameraPrefab = Resources.Load("Prefabs/Main Camera") as GameObject;
        _backgroundPrefab = Resources.Load("Prefabs/Background") as GameObject;
        _tilePrefab = Resources.Load("Prefabs/Tile/Tile") as GameObject;
        _iceTilePrefab = Resources.Load("Prefabs/Tile/IceTile") as GameObject;
    }

    Transform CreateTile(int width, int height)
    {
        Vector2 position = new Vector2(width, height);
        GameObject tile = Instantiate(_tilePrefab, position, Quaternion.identity);
        tile.transform.parent = this.transform;
        tile.name = $"Tile ({width}, {height})";
        _allTiles[width, height] = tile;
        return tile.transform;
    }

    void GenerateTiles()
    {
        for (int i = 0; i < _boardLayout.Count; i++)
        {
            if (_boardLayout[i].TileKindType == ETileKindType.Blank)
            {
                GenerateBlankSpaces(_boardLayout[i]);
            }
            else if (_boardLayout[i].TileKindType == ETileKindType.Ice)
            {
                GenerateIceSpaces(_boardLayout[i]);
            }
        }
    }

    void GenerateBlankSpaces(TileType tile)
    {
        int x = tile.X;
        int y = tile.Y;
        _blankSpaces[x, y] = true;
        if (_allTiles[x, y] != null)
        {
            Destroy(_allTiles[x, y]);
            _allTiles[x, y] = null;
        }
    }

    void GenerateIceSpaces(TileType tile)
    {
        Vector2 position = new Vector3(tile.X, tile.Y);
        GameObject iceTile = Instantiate(_iceTilePrefab, position, Quaternion.identity);
        _iceTiles[tile.X, tile.Y] = iceTile.GetComponent<IceTile>();
        iceTile.GetComponent<IceTile>().Init(this, tile.X, tile.Y);
    }

    void CreateCamera()
    {
        GameObject mainCamera = Instantiate(_cameraPrefab);
        mainCamera.GetComponent<CameraScalar>().SettingCameraPosition(_width, _height);
        CreateBackground(mainCamera.GetComponent<Camera>());
    }

    void CreateBackground(Camera mainCamera)
    {
        GameObject background = Instantiate(_backgroundPrefab);
        background.GetComponent<Canvas>().worldCamera = mainCamera;
        GenericSingleton<UIManager>.Instance.CreateUI();
    }
}