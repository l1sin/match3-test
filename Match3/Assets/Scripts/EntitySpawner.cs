using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _entityPrefab;
    [SerializeField] private Sprite[] _sprites;
    private int _debugCount;
    private int _currentType;

    [SerializeField] private Dictionary<Vector3Int, GameTile> _tileDictionary = new Dictionary<Vector3Int, GameTile>();

    private void Start()
    {
        PopulateTiles();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckTileData();
        }
    }

    private void CheckTileData()
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
            foreach (GameTile gt in oneTypeTiles)
            {
                Destroy(gt.CurrentEntity.gameObject);
            }
        }
    }

    private void CheckOneTypeRecursive(Vector3Int tilePos, List<GameTile> oneTypeTiles, List<Vector3Int> checkedTiles)
    {
        Vector3Int tileInCheck = tilePos + new Vector3Int(1, 0, 0);
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + new Vector3Int(0, 1, 0);
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + new Vector3Int(-1, 0, 0);
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);

        tileInCheck = tilePos + new Vector3Int(0, -1, 0);
        CheckIfIsOneType(tileInCheck, oneTypeTiles, checkedTiles);
    }

    private void CheckIfIsOneType(Vector3Int tileInCheck, List<GameTile> oneTypeTiles, List<Vector3Int> checkedTiles)
    {
        if (_tilemap.GetTile(tileInCheck) != null && !checkedTiles.Contains(tileInCheck) )
        {
            _tileDictionary.TryGetValue(tileInCheck, out GameTile tile);
            checkedTiles.Add(tileInCheck);
            if (tile.CurrentEntity.EntityType == _currentType)
            {
                oneTypeTiles.Add(tile);
                CheckOneTypeRecursive(tileInCheck, oneTypeTiles, checkedTiles);
            }
        }
    }

    private void DestroyEntity(Vector3Int tilePos)
    {
        if (_tilemap.GetTile(tilePos) != null)
        {
            _tileDictionary.TryGetValue(tilePos, out GameTile tile);
            Destroy(tile.CurrentEntity.gameObject);
            Debug.Log(tile.CurrentEntity.name);
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

    private void PopulateTiles()
    {
        foreach (Vector3Int v in GetAllTilesPos(_tilemap))
        {
            _tileDictionary.Add(v, new GameTile());
            CreateEntity(v);
        }
    }

    private void CreateEntity(Vector3Int v)
    {
        GameObject go = Instantiate(_entityPrefab, v, Quaternion.identity);
        go.name = _debugCount++.ToString();
        Entity en = go.GetComponent<Entity>();
        byte entityType = (byte)Random.Range(0, _sprites.Length);
        en.EntityType = entityType;
        en.SetSprite(_sprites[entityType]);
        _tileDictionary.TryGetValue(v, out GameTile tile);
        tile.CurrentEntity = en;
    }
}
