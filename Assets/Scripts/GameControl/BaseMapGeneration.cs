using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class BaseMapGeneration : MonoBehaviour
{
    [SerializeField] protected float objectGridScale = 1;
    [SerializeField] protected float noiseScale = 10;
    [SerializeField] protected float fieldSize = 10;
    [SerializeField] private float seed = 0;

    protected List<Transform> objects = new List<Transform>();
    protected Dictionary<Vector2Int, bool> occupationMap = new Dictionary<Vector2Int, bool>();

    void Start()
    {
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Generate();
        }
    }

    public abstract void Generate();

    float NoisePoint(Vector2Int coordinates)
    {
        return Mathf.PerlinNoise(noiseScale * coordinates.x + seed, noiseScale * coordinates.y + seed);
    }

    public void SpawnPrefab(GameObject prefab, Vector2Int coordinates)
    {
        GameObject instance = Instantiate(prefab, transform);
        instance.transform.localPosition = new Vector3(coordinates.x, 0, coordinates.y);
    }

    public bool WaveCheckCells(Vector2Int coordinates, float range = 4)
    {
        HashSet<Vector2Int> occupationDelta = new HashSet<Vector2Int>();
        List<Vector2Int> cellQueue = new List<Vector2Int> { coordinates };
        var it = cellQueue.GetEnumerator();

        while (it.MoveNext())
        {
            Vector2Int cell = it.Current;
            if (CheckCellOverlapped(cell))
            {
                return false;
            }
            if (Vector2Int.Distance(cell ,coordinates) < range - 1)
            {
                cellQueue.AddRange(NeighbourCells(cell));
            }
        }
        
        it.Dispose();
        return false;
    }

    protected bool CheckCellOverlapped(Vector2Int coordinates)
    {
        return Physics.CheckBox(PointOnCoordinates(coordinates), Vector3.one * objectGridScale / 2);
    }

    private List<Vector2Int> NeighbourCells(Vector2Int cell)
    {
        return new List<Vector2Int>
        {
            new(cell.x, cell.y + 1),
            new(cell.x, cell.y - 1),
            new(cell.x + 1, cell.y),
            new(cell.x - 1, cell.y)
        };
    }

    public Vector3 PointOnCoordinates(Vector2Int coordinates)
    {
        return new Vector3(coordinates.x, transform.localPosition.y, coordinates.y);
    }
}