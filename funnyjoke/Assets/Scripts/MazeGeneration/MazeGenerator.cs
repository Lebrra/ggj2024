using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    [Space]
    [SerializeField] private Vector2Int numRoomsPerDirection;
    [SerializeField] private float verticalOffset;
    [SerializeField] private Vector2 spacingMultiplier;

    private readonly List<(int x, int z)> potentialRooms = new List<(int x, int z)>();
    private readonly Dictionary<GameObject, int> roomIndices = new Dictionary<GameObject, int>();
    private readonly Dictionary<(int, int), GameObject> rooms = new Dictionary<(int, int), GameObject>();
    private readonly List<Vector3> usedWallPositions = new List<Vector3>();

    private bool[,] roomLayout;
    private int roomIndex;
    private UnionFind unionFind;

    private GameObject roomParent;
    private GameObject wallsParent;
    private bool entrance;

    private void Start()
    {
        InitializeLayout();
        Destroy(roomParent);
        wallsParent.transform.position = transform.position + verticalOffset * Vector3.up;
    }

    public void InitializeLayout()
    {
        ResetValues();
        CreateRoomPlacements();
        roomParent = new GameObject("Rooms");
        wallsParent = new GameObject("Walls");
        unionFind = new UnionFind(numRoomsPerDirection.x * numRoomsPerDirection.y);

        for (int i = 0; i < roomLayout.GetLength(0); i++)
        {
            for (int j = 0; j < roomLayout.GetLength(1); j++)
            {
                if (!roomLayout[i, j])
                    continue;
                PlaceRoom(i, j);
            }
        }

        CreateMaze();
    }

    private void ResetValues()
    {
        entrance = false;
        roomIndex = 0;
        potentialRooms.Clear();
        roomIndices.Clear();
        rooms.Clear();
        usedWallPositions.Clear();
    }

    private void CreateRoomPlacements()
    {
        roomLayout = new bool[numRoomsPerDirection.x, numRoomsPerDirection.y];

        (int x, int z) roomCoordinates = (Random.Range(0, numRoomsPerDirection.x - 1), Random.Range(0, numRoomsPerDirection.y - 1));

        roomLayout[roomCoordinates.x, roomCoordinates.z] = true;
        AddNeighbors(roomCoordinates.x, roomCoordinates.z);

        for (int i = 0; i < numRoomsPerDirection.x * numRoomsPerDirection.y - 1; i++)
        {
            roomCoordinates = potentialRooms[Random.Range(0, potentialRooms.Count)];
            roomLayout[roomCoordinates.x, roomCoordinates.z] = true;
            AddNeighbors(roomCoordinates.x, roomCoordinates.z);
            potentialRooms.Remove(roomCoordinates);
        }
        potentialRooms.Clear();
    }

    private void AddNeighbors(int xIndex, int zIndex)
    {
        if (xIndex < numRoomsPerDirection.x - 1)
            if (!potentialRooms.Contains((xIndex + 1, zIndex)) && !roomLayout[xIndex + 1, zIndex])
                potentialRooms.Add((xIndex + 1, zIndex));

        if (xIndex > 0)
            if (!potentialRooms.Contains((xIndex - 1, zIndex)) && !roomLayout[xIndex - 1, zIndex])
                potentialRooms.Add((xIndex - 1, zIndex));

        if (zIndex < numRoomsPerDirection.y - 1)
            if (!potentialRooms.Contains((xIndex, zIndex + 1)) && !roomLayout[xIndex, zIndex + 1])
                potentialRooms.Add((xIndex, zIndex + 1));

        if (zIndex > 0)
            if (!potentialRooms.Contains((xIndex, zIndex - 1)) && !roomLayout[xIndex, zIndex - 1])
                potentialRooms.Add((xIndex, zIndex - 1));
    }

    private void PlaceRoom(int xIndex, int zIndex)
    {
        int angleMultiplier = Random.Range(0, 4);
        var rotation = Quaternion.Euler(0, angleMultiplier * 90, 0);
        rooms[(xIndex, zIndex)] = new GameObject("Room")
        {
            transform =
            {
                parent = roomParent.transform,
            },
        };
        roomIndices.Add(rooms[(xIndex, zIndex)], roomIndex);
        roomIndex++;
    }

    private void PlaceWalls(int xIndex, int zIndex)
    {
        var box = wallPrefab.GetComponentInChildren<BoxCollider>(); 
        var roomSizes = new Vector2(box.size.x * box.transform.localScale.x * spacingMultiplier.x, box.size.z * box.transform.localScale.z * spacingMultiplier.y);
        int currentRoomIndex = roomIndices[rooms[(xIndex, zIndex)]];
        (float x, float z) coordinates = (xIndex * roomSizes.x, zIndex * roomSizes.y);
        var offsets = new Vector3(roomSizes.x / 2, 0, roomSizes.y / 2);

        var wallData = new List<BuildWallData>
        {
            new BuildWallData(() => zIndex < numRoomsPerDirection.y - 1, (() => xIndex, () => zIndex + 1), currentRoomIndex,
                (xIndex, zIndex + 1), new Vector3(coordinates.x, 0, coordinates.z + offsets.z), Quaternion.Euler(0, 180, 0)),
            new BuildWallData(() => zIndex > 0, (() => xIndex, () => zIndex - 1), currentRoomIndex,
                (xIndex, zIndex - 1), new Vector3(coordinates.x, 0, coordinates.z - offsets.z), Quaternion.Euler(0, 0, 0)),
            new BuildWallData(() => xIndex < numRoomsPerDirection.x - 1, (() => xIndex + 1, () => zIndex), currentRoomIndex,
                (xIndex + 1, zIndex), new Vector3(coordinates.x + offsets.x, 0, coordinates.z), Quaternion.Euler(0, 270, 0)),
            new BuildWallData(() => xIndex > 0, (() => xIndex - 1, () => zIndex), currentRoomIndex,
                (xIndex - 1, zIndex), new Vector3(coordinates.x - offsets.x, 0, coordinates.z), Quaternion.Euler(0, 90, 0)),
        };

        for (int i = 0; i < wallData.Count; i++)
            BuildWall(wallData[i]);
    }

    private void BuildWall(BuildWallData data)
    {
        if (usedWallPositions.Contains(data.WallPosition))
            return;
        bool buildPathway = false;
        if (data.OnEdge())
            buildPathway = roomLayout[data.Pathway.xIndex(), data.Pathway.zIndex()];
        int neighborIndex = rooms.TryGetValue(data.RoomsIndex, out var room) ? roomIndices[room] : -1;
        ReduceToHamiltonianPath(ref buildPathway, data.RoomIndex, neighborIndex);
        if (!buildPathway)
        {
            if (entrance)
                Instantiate(wallPrefab, data.WallPosition, data.WallRotation, wallsParent.transform);
            entrance = true;
        }
        usedWallPositions.Add(data.WallPosition);
    }

    private void ReduceToHamiltonianPath(ref bool direction, int roomIndex, int neighborIndex)
    {
        if (!direction)
            return;
        if (neighborIndex == -1)
            return;
        if (unionFind.AreUnioned(roomIndex, neighborIndex))
            direction = false;
        else
            unionFind.Union(roomIndex, neighborIndex);
    }

    private void CreateMaze()
    {
        int x = 0;
        int y = 0;

        var opened = new List<Vector2Int>();
        var closed = new List<Vector2Int>();
        for (int i = 0; i < numRoomsPerDirection.x * numRoomsPerDirection.y; i++)
        {
            PlaceWalls(x, y);
            closed.Add(new Vector2Int(x, y));

            var indices = new Vector2Int(x + 1, y);
            if (indices.x < numRoomsPerDirection.x && !closed.Contains(indices) && !opened.Contains(indices))
                opened.Add(indices);

            indices = new Vector2Int(x - 1, y);
            if (indices.x >= 0 && !closed.Contains(indices) && !opened.Contains(indices))
                opened.Add(indices);

            indices = new Vector2Int(x, y + 1);
            if (indices.y < numRoomsPerDirection.y && !closed.Contains(indices) && !opened.Contains(indices))
                opened.Add(indices);

            indices = new Vector2Int(x, y - 1);
            if (indices.y >= 0 && !closed.Contains(indices) && !opened.Contains(indices))
                opened.Add(indices);

            if (opened.Count == 0)
                return;
            int index = Random.Range(0, opened.Count);
            x = opened[index].x;
            y = opened[index].y;
            opened.RemoveAt(index);
        }
    }

    private struct BuildWallData
    {
        public Func<bool> OnEdge { get; }
        public (Func<int> xIndex, Func<int> zIndex) Pathway { get; }
        public int RoomIndex { get; }
        public (int, int) RoomsIndex { get; }
        public Vector3 WallPosition { get; }
        public Quaternion WallRotation { get; }

        public BuildWallData(Func<bool> onEdge, (Func<int>, Func<int>) pathway, int roomIndex, (int, int) roomsIndex, Vector3 wallPosition, Quaternion wallRotation)
        {
            OnEdge = onEdge;
            Pathway = pathway;
            RoomIndex = roomIndex;
            RoomsIndex = roomsIndex;
            WallPosition = wallPosition;
            WallRotation = wallRotation;
        }
    }
}
