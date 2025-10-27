using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private int size = 5;

    [SerializeField] private List<GameObject> tiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        WaveFunctionCollapse();
        //SpawnRandom();
    }

    private void SpawnRandom()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Instantiate(tiles[Random.Range(0, tiles.Count)], new Vector2(i, j), Quaternion.identity);
            }
        }

        
        
    }

    private bool CheckComp(Vector4 a, Vector4 b)
    {
        if ((a.y == b.w) ||
            (a.x == b.z) ||
            (a.w == b.y) ||
            (a.z == b.x))
        {
            return true;
        }

        return false;
    }

    private void CheckAdjacent(List<GameObject> maplist, GameObject current)
    {
        int aux = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (current.transform.position.x == i &&
                    current.transform.position.y == j)
                {
                    foreach (GameObject t in tiles)
                    {
                        if (CheckComp(current.GetComponent<CustomTile>().directionType,
                            t.GetComponent<CustomTile>().directionType))
                        {
                            aux++;
                        }
                    }
                    current.GetComponent<CustomTile>().entropy = aux;
                    break;
                }
            }
        }
    }

    private void WaveFunctionCollapse()
    {
        List<GameObject> objList = new List<GameObject>();
        int count = size * size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject t = Instantiate(tiles[Random.Range(0, tiles.Count)],
                    new Vector2(i, j), Quaternion.identity);

                objList.Add(t);
            }
            
        }

        while (count > 0) 
        {
            GameObject temp;

            if (count == size * size)
            {
                objList.Add(Instantiate(tiles[Random.Range(0, tiles.Count)],
                    new Vector2(Random.Range(0, size), Random.Range(0, size)), Quaternion.identity));
                count--;
                continue;
            }

            int smallest = 999;

            for (int i = 0; i < size*size; i++)
            {
                if (objList[i].GetComponent<CustomTile>().entropy < smallest)
                {
                    smallest = objList[i].GetComponent<CustomTile>().entropy;
                }
            }

            Vector3 pos = new Vector2(Random.Range(0, size), Random.Range(0, size));

            for (int i = 0; i < objList.Count; i++)
            {
                if (objList[i].GetComponent<CustomTile>().entropy == smallest)
                {
                    pos = objList[i].transform.position;
                }
            }

            temp = Instantiate(tiles[Random.Range(0, tiles.Count)],
                    pos, Quaternion.identity);

            CheckAdjacent(objList, temp);

            objList.Add(temp);

            count--;
        }

    }
}
