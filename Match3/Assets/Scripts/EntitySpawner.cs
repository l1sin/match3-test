using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Xml;

public class EntitySpawner : MonoBehaviour
{
    public static EntitySpawner Instance;

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _deathWait;
    private bool _turnAvaliable;
    private int _debugCount;
    private int _currentType;
    [SerializeField] private int debugint;
    [SerializeField] private Dictionary<Vector3Int, GameTile> _tileDictionary = new Dictionary<Vector3Int, GameTile>();

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        _turnAvaliable = true;
        CreateTiles();
        RePopulateTiles();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckTileData();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RePopulateTiles();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject go = GameObject.Find(debugint.ToString());
            if (go != null) Debug.Log("Alive");
            else Debug.Log("Dead");
        }
    }

    private void CheckTileData()
    {
        if (_turnAvaliable)
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = _tilemap.WorldToCell(mousePos);

            if (_tilemap.GetTile(tilePos) != null)
            {
                _tileDictionary.TryGetValue(tilePos, out GameTile tile);
                _currentType = tile.CurrentEntity.EntityType;
                List<GameTile> oneTypeTiles = new List<GameTile>() { tile };
                List<Vector3Int> checkedTiles = new List<Vector3Int>() { tilePos };
                CheckOneTypeRecursive(tilePos, oneTypeTiles, checkedTiles);
                if (oneTypeTiles.Count >= 2)
                {
                    _turnAvaliable = false;
                    for (int i = 0; i < oneTypeTiles.Count; i++)
                    {
                        StartCoroutine(WaitDeath(oneTypeTiles[i], _deathWait * i));
                    }
                    float time = _deathWait * (oneTypeTiles.Count - 1);
                    StartCoroutine(TurnReset(time));
                    StartCoroutine(ActivateFall(oneTypeTiles, time + _deathWait));
                }
            }
        }
    }

    private void MakeFall(Vector3Int tilePos)
    {
        GameTile tile = GetTile(tilePos);
        if (tile != null && tile.CurrentEntity != null)
        {
            tile.CurrentEntity.Fall();
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

    private IEnumerator TurnReset(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _turnAvaliable = true;
    }
    private IEnumerator ActivateFall(List<GameTile> tiles,float seconds)
    {
        yield return new WaitForSeconds(seconds);
        foreach (GameTile tile in tiles)
        {
            MakeFall(tile.Pos + Vector3Int.up);
        }
    }

    private IEnumerator WaitDeath(GameTile tile, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(tile.CurrentEntity.gameObject);
        tile.CurrentEntity = null;
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
                    newList.Add(new Vector3Int(k, j, i));
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
        GameObject newObject = Instantiate(_entityPrefab, tile.Pos, Quaternion.identity);
        newObject.name = _debugCount++.ToString();
        Entity entity = newObject.GetComponent<Entity>();
        byte entityType = (byte)Random.Range(0, _sprites.Length);

        entity.EntityType = entityType;
        entity.SetSprite(_sprites[entityType]);
        entity.MyTile = tile;
        entity.Pos = tile.Pos;

        tile.CurrentEntity = entity;

    }

    private void Singleton()
    {
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        else Instance = this;
    }

    public GameTile GetTile(Vector3Int v)
    {
        _tileDictionary.TryGetValue(v, out GameTile tile);
        return tile;
    }
}
