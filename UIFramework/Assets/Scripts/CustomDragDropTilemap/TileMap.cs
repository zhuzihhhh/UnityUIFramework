using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour, IEnumerable<Tile> {
    public static TileMap Instance;

    public int Row = 11, Col = 11;
    public bool IsAnchorLeft = true;

    public Color legalColor = new Color(0f, 1f, 0f, 0.25f);
    public Color illegalColor = new Color(1f, 0f, 0f, 0.25f);
    public Color defaultColor = new Color(1, 1, 1, .25f);

    [SerializeField] private Tile tilePrfab;
    private List<Tile> tiles = new List<Tile>();

    private float tileOffset => tilePrfab.GetComponent<SpriteRenderer>().bounds.size.x / 2;

    private void Awake() {
        Instance = this;
        tiles.Clear();
        tiles.AddRange(GetComponentsInChildren<Tile>());
    }

    [ExecuteInEditMode]
    [ContextMenu("CreateTileMap")]
    public void CreateTileMap() {
        foreach (var tile in tiles) {
            DestroyImmediate(tile);
        }

        for (int i = 0; i < Row; i++) {
            for (int j = 0; j < Col; j++) {
                var tile = Instantiate(tilePrfab, transform);
                var size = tile.GetComponent<SpriteRenderer>().bounds.size;
                if (IsAnchorLeft)
                    tile.transform.localPosition = new Vector3(size.x * j + tileOffset, size.y * i + tileOffset, 0);
                else
                    tile.transform.localPosition = new Vector3(-size.x * j - tileOffset, size.y * i + tileOffset, 0);
                tiles.Add(tile);
            }
        }
    }


    [ExecuteInEditMode]
    [ContextMenu("deleteMap")]
    public void DeleteMap() {
        while (transform.childCount > 0) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void ResetAllColor() {
        foreach (var tile in tiles) {
            tile.SetColor(defaultColor);
        }
    }

    public IEnumerator<Tile> GetEnumerator() {
        return tiles.GetEnumerator();
    }


    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}