using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private int size = 5;

    [SerializeField] private List<Tile> tiles = new List<Tile>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnRandom();
    }

    private void SpawnRandom()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                tilemap.SetTile(pos, tiles[Random.Range(0, tiles.Count)]);
            }
        }
        
    }
}
