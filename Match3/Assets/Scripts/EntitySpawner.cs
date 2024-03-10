using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using System.Linq;

public class EntitySpawner : MonoBehaviour
{
    public static EntitySpawner Instance;
    public static readonly Vector3 half = new Vector3(0.5f, 0.5f, 0);

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private Sprite[] _sprites;
    public float DeathWait = 0.3f;
    [SerializeField] private int _comboAmount;
    private bool _tilesFalling;
    private bool _tilesActing;
    private int _debugCount;
    private int _currentType;
    [SerializeField] private int debugint;
    [SerializeField] private Dictionary<Vector3Int, GameTile> _tileDictionary = new Dictionary<Vector3Int, GameTile>();
    [SerializeField] private float _timeScale;
    [SerializeField] private GameObject _obstcleTilemap;
    public List<Obstacle> Obstacles;

    private int _fallingEntities;
    public int FallingEntities
    {
        get 
        {
            return _fallingEntities; 
        }
        set
        {
            _fallingEntities = value;
            CheckFallingCount();
        }
    }
    

    private void Awake()
    {
        DOTween.Init();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        CreateTiles();
        FindObstacles();
        RePopulateTiles();
        CheckFallingCount();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckTileData();
        }
    }

    private void CheckFallingCount()
    {
        if (FallingEntities == 0)
        {
            _tilesFalling = false;
            LevelController.Instance.CheckIfLevelEnd();
        } 
        else _tilesFalling = true;
    }

    private void FindObstacles()
    {
        Obstacles = _obstcleTilemap.GetComponentsInChildren<Obstacle>().ToList();
        for (int i = 0; i < Obstacles.Count; i++)
        {
            Obstacles[i].ConnectToTile();
        }
    }

    private void CheckTileData()
    {
        if (!_tilesActing && !_tilesFalling && LevelController.Instance.TurnsLeft > 0)
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = _tilemap.WorldToCell(mousePos);
            GameTile tile = GetTile(tilePos);

            if (tile != null && tile.CurrentObstacle == null)
            {
                _tilesActing = true;
                LevelController.Instance.DoTurn(1);

                _currentType = tile.CurrentEntity.EntityType;
                List<GameTile> oneTypeTiles = new List<GameTile>() { tile };
                List<Vector3Int> checkedTiles = new List<Vector3Int>() { tilePos };
                CheckOneTypeRecursive(tilePos, oneTypeTiles, checkedTiles);
                List<GameTile> adjacentTiles = GetAdjacentTiles(oneTypeTiles);
                if (oneTypeTiles.Count >= _comboAmount)
                {
                    for (int i = 0; i < oneTypeTiles.Count; i++)
                    {
                        DamageEntity(oneTypeTiles[i]);
                    }

                    for (int i = 0; i < adjacentTiles.Count; i++)
                    {
                        DamageObstacle(adjacentTiles[i]);
                    }

                    StartCoroutine(ActivateFall(oneTypeTiles,DeathWait));
                }
            }
        }
    }

    private List<GameTile> GetAdjacentTiles(List<GameTile> tiles)
    {
        List<GameTile> adjacentTiles = new List<GameTile>();
        foreach (GameTile tile in tiles)
        {
            Vector3Int pos = tile.Pos;

            GameTile tileInCheck = GetTile(pos + Vector3Int.right);
            if (CheckIfAdjacent(tileInCheck, tiles) && !adjacentTiles.Contains(tileInCheck)) adjacentTiles.Add(tileInCheck);

            tileInCheck = GetTile(pos + Vector3Int.up);
            if (CheckIfAdjacent(tileInCheck, tiles) && !adjacentTiles.Contains(tileInCheck)) adjacentTiles.Add(tileInCheck);

            tileInCheck = GetTile(pos + Vector3Int.left);
            if (CheckIfAdjacent(tileInCheck, tiles) && !adjacentTiles.Contains(tileInCheck)) adjacentTiles.Add(tileInCheck);

            tileInCheck = GetTile(pos + Vector3Int.down);
            if (CheckIfAdjacent(tileInCheck, tiles) && !adjacentTiles.Contains(tileInCheck)) adjacentTiles.Add(tileInCheck);
        }
        return adjacentTiles;
    }

    private bool CheckIfAdjacent(GameTile tile, List<GameTile> nonAdjacentTiles)
    {
        if (tile == null) return false;
        if (nonAdjacentTiles.Contains(tile)) return false;
        else return true;
    }

    private void MakeFall(Vector3Int tilePos)
    {
        GameTile tile = GetTile(tilePos);
        if (tile != null && tile.CurrentEntity != null)
        {
            tile.CurrentEntity.Fall();
        }
        else if (tile == null)
        {
            InstantiateEntityCelling(tilePos);
        }
    }

    public bool CheckTileBelow(Vector3Int tilePos)
    {
        tilePos += new Vector3Int(0, -1, 0);
        GameTile tile = GetTile(tilePos);
        if (tile != null && tile.CurrentEntity == null)
        {
            return true;
        }
        else return false;
    }

    private IEnumerator ActivateFall(List<GameTile> tiles,float seconds)
    {
        yield return new WaitForSeconds(seconds);
        tiles.Sort();
        tiles.Reverse();
        DeleteLowerTiles(tiles);
        tiles.Reverse();
        foreach (GameTile tile in tiles)
        {
            MakeFall(tile.Pos + Vector3Int.up);
        }
        _tilesActing = false;
    }

    private List<GameTile> DeleteLowerTiles(List<GameTile> tiles)
    {
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            GameTile otherTile = GetTile(tiles[i].Pos + Vector3Int.up);
            if (tiles.Contains(otherTile))
            {
                tiles.RemoveAt(i);
            }
        }
        return tiles;
    }

    private void DamageEntity(GameTile tile)
    {
        if (tile.CurrentObstacle != null) tile.CurrentObstacle.Damage();
        else
        {
            tile.CurrentEntity.Die(DeathWait);
            tile.CurrentEntity = null;
        } 
    }

    private void DamageObstacle(GameTile tile)
    {
        if (tile.CurrentObstacle != null) tile.CurrentObstacle.Damage();
    }

    private void CheckOneTypeRecursive(Vector3Int tilePos, List<GameTile> oneTypeTiles, List<Vector3Int> checkedTiles)
    {
        Vector3Int tileInCheck = tilePos + Vector3Int.right;
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + Vector3Int.up;
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + Vector3Int.left;
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + Vector3Int.down;
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);
    }

    private void CheckIfIsOneType(Vector3Int tileInCheck, List<GameTile> oneTypeTiles, List<Vector3Int> checkedTiles)
    {
        if (_tilemap.GetTile(tileInCheck) != null && !checkedTiles.Contains(tileInCheck))
        {
            GameTile tile = GetTile(tileInCheck);
            checkedTiles.Add(tileInCheck);
            if (tile.CurrentEntity != null && tile.CurrentEntity.EntityType == _currentType)
            {
                oneTypeTiles.Add(tile);
                CheckOneTypeRecursive(tileInCheck, oneTypeTiles, checkedTiles);
            }
        }
    }

    private List<Vector3Int> GetAllTilesPos(Tilemap tilemap)
    {
        List<Vector3Int> newList = new List<Vector3Int>();

        tilemap.CompressBounds();

        for (int i = tilemap.origin.z; i <= tilemap.origin.z + tilemap.cellBounds.size.z - 1; i++)
        {
            for (int j = tilemap.origin.y; j <= tilemap.origin.y + tilemap.cellBounds.size.y - 1; j++)
            {
                for (int k = tilemap.origin.x; k <= tilemap.origin.x + tilemap.cellBounds.size.x - 1; k++)
                {
                    Vector3Int pos = new Vector3Int(k, j, i);
                    if (_tilemap.GetTile(pos) != null) newList.Add(pos);
                }
            }
        }
        return newList;
    }

    private void CreateTiles()
    {
        foreach (Vector3Int v in GetAllTilesPos(_tilemap))
        {
            _tileDictionary.Add(v, new GameTile(v));
        }
    }

    private void RePopulateTiles()
    {
        foreach (KeyValuePair<Vector3Int, GameTile> pair in _tileDictionary)
        {
            GameTile tile = pair.Value;
            if (tile.CurrentEntity != null) Destroy(tile.CurrentEntity.gameObject);
            InstantiateEntity(tile);
        }
    }

    private void InstantiateEntity(GameTile tile)
    {
        GameObject newObject = Instantiate(_entityPrefab, tile.Pos + half, Quaternion.identity);
        newObject.name = "Tile" + _debugCount++.ToString();
        Entity entity = newObject.GetComponent<Entity>();
        byte entityType = (byte)Random.Range(0, _sprites.Length);

        entity.EntityType = entityType;
        entity.SetSprite(_sprites[entityType]);
        entity.MyTile = tile;
        entity.Pos = tile.Pos;

        tile.CurrentEntity = entity;
    }

    public Entity InstantiateEntityCelling(Vector3Int pos)
    {
        GameObject newObject = Instantiate(_entityPrefab, pos + half, Quaternion.identity);
        newObject.name = "Tile" + _debugCount++.ToString();
        Entity entity = newObject.GetComponent<Entity>();
        byte entityType = (byte)Random.Range(0, _sprites.Length);

        entity.EntityType = entityType;
        entity.SetSprite(_sprites[entityType]);
        entity.Pos = pos;
        entity.MoveDown();

        return entity;
    }

    

    public GameTile GetTile(Vector3Int v)
    {
        _tileDictionary.TryGetValue(v, out GameTile tile);
        return tile;
    }

    public void AbilityReshuffle(int cost)
    {
        RePopulateTiles();
        LevelController.Instance.DoTurn(cost);
    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        Singleton();
    }
}
