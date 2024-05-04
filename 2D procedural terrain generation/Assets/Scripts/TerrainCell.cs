using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCell : MonoBehaviour
{
    public Vector2 coordinates;
    public int terrainId;
    public Sprite terrainSprite;

    public void InitializeTile()
    {
        GetComponent<SpriteRenderer>().sprite = terrainSprite;
    }
}
