using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject emptyTilePrefab;
    [SerializeField] private List<BiomeSet> biomeSets;
    
    [SerializeField] private int terrainWidth;
    [SerializeField] private int terrainHeight;

    private List<List<TerrainCell>> _cells = new List<List<TerrainCell>>();

    [SerializeField] private float mapDetails = 5f;
    [SerializeField] private Sprite volcanoSprite;
    [SerializeField] private int volcanoAmount;
    [SerializeField] private Sprite citySprite;
    [SerializeField] private int cityAmount;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            GenerateTerrain();
        }
    }

    private void Start()
    {
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        GenerateBiomesMap();

        while (volcanoAmount != 0)
        {
            PlantVolcanoes(Random.Range(2, terrainWidth - 2), Random.Range(2, terrainHeight - 2));
        }

        while (cityAmount != 0)
        {
            PlantCities(Random.Range(2, terrainWidth - 2), Random.Range(2, terrainHeight - 2));
        }
    }

    private void GenerateBiomesMap()
    {
        for (int x = 0; x < terrainWidth; x++)
        {
            _cells.Add(new List<TerrainCell>());
            for (int y = 0; y < terrainHeight; y++)
            {
                int tileId = GetTileId(x, y);
                var newTile = CreateTile(tileId, x, y);
                _cells[x].Add(newTile);
            }
        }
    }

    private TerrainCell CreateTile(int tileId, int x, int y)
    {
        var newTile = Instantiate(emptyTilePrefab, transform);
        
        Sprite tileSprite = null;
        for (int i = 0; i < biomeSets.Count; i++)
        {
            if (tileId == i)
            {
                tileSprite = biomeSets[i].biomeTileSprite;
                newTile.GetComponent<TerrainCell>().terrainSprite = tileSprite;
                newTile.gameObject.name = $"{biomeSets[i].biomeName}-{x}:{y}";
                newTile.GetComponent<TerrainCell>().coordinates = new Vector2(x, y);
            }
        }
        
        if (tileSprite != null)
        {
            Vector2 spriteSize = tileSprite.bounds.size;
            newTile.transform.position = new Vector3(x * spriteSize.x, y * spriteSize.x, 0);
        }
    
        newTile.GetComponent<TerrainCell>().terrainId = tileId;
        newTile.GetComponent<TerrainCell>().InitializeTile();
        newTile.GetComponent<SpriteRenderer>().sortingOrder = -y;

        return newTile.GetComponent<TerrainCell>();
    }

    private int GetTileId(int x, int y)
    {
        
        float rawPerlin = Mathf.PerlinNoise(x / mapDetails, y/ mapDetails);

        float clampPerlin = Mathf.Clamp(rawPerlin, 0.0f, 1.0f);
        float rescaledPerlin = clampPerlin * biomeSets.Count;

        if (Math.Abs(rescaledPerlin - biomeSets.Count) < 0.1f)
        {
            rescaledPerlin = biomeSets.Count - 1;
        }

        return Mathf.FloorToInt(rescaledPerlin);
    }

    private void PlantVolcanoes(int x, int y)
    {
        if (_cells[x][y].terrainId == 2)
        {
            if (_cells[x - 1][y].terrainId == 2 && _cells[x + 1][y].terrainId == 2 && _cells[x][y - 1].terrainId == 2 && _cells[x][y + 1].terrainId == 2)
            {
                Debug.Log("Volcano created!");
                _cells[x][y].terrainSprite = volcanoSprite;
                _cells[x][y].GetComponent<SpriteRenderer>().sprite = volcanoSprite;
                _cells[x][y].gameObject.name = $"Volcano-{x}-{y}";
                volcanoAmount -= 1;
            }
        }
    }

    private void PlantCities(int x, int y)
    {
        if (_cells[x][y].terrainId == 1)
        {
            if (_cells[x - 1][y].terrainId == 1 && _cells[x + 1][y].terrainId == 1 && _cells[x][y - 1].terrainId == 1 && _cells[x][y + 1].terrainId == 1)
            {
                Debug.Log("City created!");
                _cells[x][y].terrainSprite = citySprite;
                _cells[x][y].GetComponent<SpriteRenderer>().sprite = citySprite;
                _cells[x][y].gameObject.name = $"City-{x}-{y}";
                cityAmount -= 1;
            }
        }
    }
}
