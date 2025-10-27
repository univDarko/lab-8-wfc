using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private int size = 10;
    [SerializeField] private List<GameObject> tilePrefabs = new List<GameObject>();

    private class Cell
    {
        public bool isCollapsed = false;
        public List<GameObject> possibleOptions;
        public Vector2Int position;

        public int Entropy => possibleOptions.Count;

        public Cell(List<GameObject> allTilePrefabs, Vector2Int pos)
        {
            possibleOptions = new List<GameObject>(allTilePrefabs);
            position = pos;
        }
    }

    private Cell[,] grid;
    private List<Cell> uncollapsedCells = new List<Cell>();

    void Start()
    {
        StartGeneration();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopAllCoroutines();
            ClearGrid();
            StartGeneration();
        }
    }

    void StartGeneration()
    {
        uncollapsedCells.Clear();
        InitializeGrid();
        StartCoroutine(RunWFC());
    }

    void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void InitializeGrid()
    {
        grid = new Cell[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Cell newCell = new Cell(tilePrefabs, new Vector2Int(i, j));
                grid[i, j] = newCell;
                uncollapsedCells.Add(newCell);
            }
        }
    }

    private IEnumerator RunWFC()
    {
        while (uncollapsedCells.Count > 0)
        {
            Cell cellToCollapse = GetCellWithLowestEntropy();

            if (cellToCollapse != null)
            {
                CollapseCell(cellToCollapse);

                Propagate(cellToCollapse.position);
            }
            else
            {
                Debug.LogError("WFC fallo");
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private Cell GetCellWithLowestEntropy()
    {
        if (uncollapsedCells.Count == 0) return null;

 � � � �//Ordena la lista por entrop�a
 � � � �uncollapsedCells = uncollapsedCells.OrderBy(c => c.Entropy).ToList();

 � � � �//Filtra las celdas con la entrop�a m�s baja (pueden ser varias)
 � � � �int lowestEntropy = uncollapsedCells[0].Entropy;
        List<Cell> cellsWithLowestEntropy = uncollapsedCells.Where(c => c.Entropy == lowestEntropy).ToList();

 � � � �//Elege una al azar entre las de menor entrop�a
 � � � �return cellsWithLowestEntropy[Random.Range(0, cellsWithLowestEntropy.Count)];
    }

    private void CollapseCell(Cell cell)
    {
 � � � �//Elegir un prefab aleatorio de las opciones restantes
 � � � �GameObject chosenPrefab = cell.possibleOptions[Random.Range(0, cell.Entropy)];

        cell.possibleOptions = new List<GameObject> { chosenPrefab };
        cell.isCollapsed = true;
        uncollapsedCells.Remove(cell);

        Instantiate(chosenPrefab, new Vector2(cell.position.x, cell.position.y), Quaternion.identity, this.transform);
    }

    private void Propagate(Vector2Int collapsedPos)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(collapsedPos);

        while (stack.Count > 0)
        {
            Vector2Int currentPos = stack.Pop();
            Cell currentCell = grid[currentPos.x, currentPos.y];

            Vector2Int[] neighborPositions = new Vector2Int[]
            {
                currentPos + Vector2Int.up, � �// Arriba
 � � � � � � � �currentPos + Vector2Int.down, �// Abajo
 � � � � � � � �currentPos + Vector2Int.left, �// Izquierda
 � � � � � � � �currentPos + Vector2Int.right  // Derecha
                  };

            foreach (Vector2Int neighborPos in neighborPositions)
                {
                if (neighborPos.x < 0 || neighborPos.x >= size || neighborPos.y < 0 || neighborPos.y >= size)
                    continue;

                Cell neighborCell = grid[neighborPos.x, neighborPos.y];

 � � � � � � � �//si el vecino ya est� colapsado, no hace nadaxd
 � � � � � � � �if (neighborCell.isCollapsed) continue;

                int initialEntropy = neighborCell.Entropy;
                List<GameObject> validOptions = new List<GameObject>(neighborCell.possibleOptions);

                List<float> validSockets = GetValidSockets(currentCell, neighborPos - currentPos);

 � � � � � � � �//Elimina opciones no v�lidas del vecino
 � � � � � � � �validOptions.RemoveAll(optionPrefab =>
                {
                    CustomTile optionTile = optionPrefab.GetComponent<CustomTile>();
                    float socketToMatch = GetSocketForDirection(optionTile, currentPos - neighborPos);
                    return !validSockets.Contains(socketToMatch);
                });

                if (validOptions.Count < initialEntropy)
                {
                    neighborCell.possibleOptions = validOptions;

                    if (!stack.Contains(neighborPos))
                    {
                        stack.Push(neighborPos);
                    }
                }
            }
        }
    }
    private List<float> GetValidSockets(Cell cell, Vector2Int direction)
    {
        List<float> sockets = new List<float>();
        foreach (GameObject prefab in cell.possibleOptions)
        {
            float socket = GetSocketForDirection(prefab.GetComponent<CustomTile>(), direction);
            if (!sockets.Contains(socket))
            {
                sockets.Add(socket);
            }
        }
        return sockets;
    }

    private float GetSocketForDirection(CustomTile tile, Vector2Int direction)
    {
        if (direction == Vector2Int.up) return tile.directionType.x; � � �// Arriba (x)
 � � � �if (direction == Vector2Int.right) return tile.directionType.y; � �// Derecha (y)
 � � � �if (direction == Vector2Int.down) return tile.directionType.z; � � // Abajo (z)
 � � � �if (direction == Vector2Int.left) return tile.directionType.w; � � // Izquierda (w)
 � � � �return -999;//Error
 � �}
}