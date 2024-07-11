using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Grid_pathfinding : MonoBehaviour
{
    public static int MOVE_DIAGONAL_COST = 14;
    public static int MOVE_STRAIGHT_COST = 10;

    public bool DrawGizmos;
    public bool CanCalculate;
    //
    [Header("Grid Info")]
    public Transform pointBound;
    public float sizeCell;
    //
    [Header("A star")]
    public Transform pointSet;
    public Transform pointDestination;
    //
    private float _sizeCellReal;
    private Grid _grid;
    private List<Node> paths;
    public float3 passPosSet;
    public float3 passPosDestination;
    
    
    private void Awake()
    {
        //
        _sizeCellReal = math.max(sizeCell, 0.1f);
        _grid = new Grid()
        {
            sizeCell =_sizeCellReal
        };
        
        var pos = (float3)pointBound.localPosition;
        var w = (int)(pos.x / _sizeCellReal);
        var h = (int)(pos.z / _sizeCellReal);
        var centerPoint = new Node[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                float centerX = (i * _sizeCellReal) + (_sizeCellReal / 2f);
                float centerZ = (j * _sizeCellReal) + (_sizeCellReal / 2f);
                var node = new Node();
                node.Init(new int2(i,j),PositionWorld(new float3(centerX, 0, centerZ)));
                centerPoint[i, j] = node;
            }
        }
        _grid.UpdateGrid(centerPoint);
        passPosSet = pointSet.position;
        passPosDestination = pointDestination.position;
        //
    }

    [ContextMenu("CalculatePath")]
    public void CalculatePath()
    {
        paths = GetPath(PositionLocal(pointSet.position), PositionLocal(pointDestination.position));
    }

    private void Update()
    {
        if (_grid.init && paths != null)
        {
            var passPos = float3.zero;
            bool hasPassPos = false;
            foreach (var node in paths)
            {
                if (!hasPassPos)
                {
                    hasPassPos = true;
                }
                else
                {
                    Debug.DrawLine(passPos,node.centerPoint,Color.green);
                }
                passPos = node.centerPoint;
            }
        }

        if (_grid.init)
        {
            if (!pointSet.position.Equals(passPosSet) || !pointDestination.position.Equals(passPosDestination))
            {
                passPosSet = pointSet.position;
                passPosDestination = pointDestination.position;
                CalculatePath();
            }
        }
    }

    private void OnDrawGizmos()
    {
        HandleDrawGrid();
    }

    private void HandleDrawGrid()
    {
        if(!_grid.init || !DrawGizmos || !pointBound || !pointSet) return;
        DrawGrid();
        DrawBound();
        DrawCellCenters();

        if (pointSet)
        {
            DrawCellNearest(pointSet.position);
        }
        
        if (pointDestination)
        {
            DrawCellNearest(pointDestination.position);
        }

        DrawPathDestination();

    }

    private void DrawPathDestination()
    {
        if(paths == null) return;
        foreach (var node in paths)
        {
            DrawGridCell(node.index,Color.black);
        }
        
    }

    private void DrawBound()
    {
        Gizmos.color = Color.yellow;

        float3 pos1 = PositionWorld(float3.zero);
        float3 pos2 = pointBound.position;
        float3 posA = new float3(pos1.x, 0, pos2.z);
        float3 posB = new float3(pos2.x, 0, pos1.z);

        Gizmos.DrawLine(pos1, posA);
        Gizmos.DrawLine(pos1, posB);
        Gizmos.DrawLine(pos2, posA);
        Gizmos.DrawLine(pos2, posB);
    }

    private void DrawGrid()
    {
        Gizmos.color = Color.white;
        DrawGrid_L(_grid.sizeGrid.x, _grid.sizeGrid.y, _sizeCellReal);

        void DrawGrid_L(int w, int h, float size)
        {
            // Vẽ các đường ngang (Draw horizontal lines)
            for (int i = 0; i <= h; i++)
            {
                Gizmos.DrawLine(PositionWorld(new float3(0, 0, i * size)),
                    PositionWorld(new float3(w * size, 0, i * size)));
            }

            // Vẽ các đường dọc (Draw vertical lines)
            for (int j = 0; j <= w; j++)
            {
                Gizmos.DrawLine(PositionWorld(new float3(j * size, 0, 0)),
                    PositionWorld(new float3(j * size, 0, h * size)));
            }
        }

        
    }
    
    private void DrawCellCenters()
    {
        Gizmos.color = Color.green; // Change color for better visualization
        
        for (int i = 0; i < _grid.sizeGrid.x; i++)
        {
            for (int j = 0; j < _grid.sizeGrid.y; j++)
            {
                float3 center = _grid.GetPositionNode(i,j);
                float3 center1 = center - math.up() * -0.1f;
                float3 center2 = center - math.up() * 0.1f;
                Gizmos.DrawLine(center1,center2);
            }
        }
        
    }
    
    float3 PositionWorld(float3 ltPos)
    {
        return transform.TransformPoint(ltPos);
    }
    float3 PositionLocal(float3 wPos)
    {
        return transform.InverseTransformPoint(wPos);
    }
    
    private void DrawCellNearest(float3 wPos)
    {
        var index = _grid.GetIndexGrid(PositionLocal(wPos));
        if(!_grid.CheckInGrid(index)) return;
        Gizmos.DrawCube(_grid.GetPositionNode(index),new float3(1,1,1) * _sizeCellReal); 
        DrawGridCell(index,Color.red);
    }

    private void DrawGridCell(int2 index,Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(_grid.GetPositionNode(index),new float3(1,1,1) * _sizeCellReal); 
    }
    [BurstCompile]
    public List<Node> GetPath(float3 startPoint, float3 endPoint)
    {
        if (!_grid.init || !CanCalculate) return new List<Node>();
        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();
        //
        open.Add(_grid.GetNodeNearest(startPoint));
        var nodeDestination = _grid.GetNodeNearest(endPoint);
        var nodeStart = _grid.GetNodeNearest(startPoint);
        nodeStart.CalculateHCost(nodeDestination);

        while (open.Count > 0)
        {
            var nodeSet = open[0];
            
            foreach(var nodeCheck in open)
            {
                if (nodeCheck.fCost < nodeSet.fCost || nodeSet.fCost == nodeCheck.fCost && nodeSet.hCost > nodeCheck.hCost)
                {
                    nodeSet = nodeCheck;
                }
            }

            close.Add(nodeSet);
            open.Remove(nodeSet);
            var checkIndex = nodeDestination.index - nodeSet.index;
            if(checkIndex.x == 0 && checkIndex.y == 0)
            {
                break;
            }
            foreach (Node neighbor in _grid.GetNeighBor(nodeSet))
            {
                if(close.Contains(neighbor)) continue;
                int newGCost = nodeSet.gCost + nodeSet.CalculateCost(neighbor.index);
                if (newGCost < neighbor.gCost || !open.Contains(neighbor))
                {
                    neighbor.CalculateGCost(nodeSet);
                    neighbor.CalculateHCost(nodeDestination);
                    neighbor.CalculateFCost();
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }

        }

        return close;
    }
    
    public int GetCost(int2 index1,int2 index2)
    {
        var subtract = math.abs(index1 - index2);
        var remaining = math.abs(subtract.x - subtract.y);

        return Grid_pathfinding.MOVE_DIAGONAL_COST * math.min(subtract.x, subtract.y) +
               Grid_pathfinding.MOVE_STRAIGHT_COST * remaining;
    }
}

public struct Grid
{
    public Node[,] pointGrid;
    public int2 sizeGrid;
    public float sizeCell;
    public bool init;

    public void UpdateGrid(Node[,] grid)
    {
        init = true;
        pointGrid = grid;
        sizeGrid = new int2(pointGrid.GetLength(0), pointGrid.GetLength(1));
    }

    public Node GetNodeNearest(float3 localPosition)
    {
        var index = GetIndexGrid(localPosition);
        if(index.x < 0 || index.y < 0 || index.x >= sizeGrid.x || index.y >= sizeGrid.y) return new Node();
        return pointGrid[index.x, index.y];
    }
    public float3 GetPositionNearest(float3 localPosition)
    {
        var index = GetIndexGrid(localPosition);
        if(index.x < 0 || index.y < 0 || index.x >= sizeGrid.x || index.y >= sizeGrid.y) return float3.zero;

        return GetPositionNode(index);
    }
    public float3 GetPositionNode(int2 index)
    {
        return GetPositionNode(index.x,index.y);
    }
    
    public float3 GetPositionNode(int x,int y)
    {
        return pointGrid[x, y].centerPoint;
    }
    
    public int2 GetIndexGrid(float3 localPos)
    {
        var i = (int)Mathf.Floor((localPos.x / sizeCell));
        var j = (int)Mathf.Floor((localPos.z / sizeCell));
        return new int2(i, j);
    }

    public bool CheckInGrid(float3 localPos)
    {
        var index = GetIndexGrid(localPos);
        return !(index.x < 0 || index.y < 0 || index.x >= sizeGrid.x || index.y >= sizeGrid.y);
    }
    
    public bool CheckInGrid(int2 index)
    {
        return CheckInGrid(index.x,index.y);
    }
    
    public bool CheckInGrid(int x,int y)
    {
        return !(x < 0 || y < 0 || x >= sizeGrid.x || y >= sizeGrid.y);
    }

    public List<Node> GetNeighBor(Node node)
    {
        var x = node.index.x;
        var y = node.index.y;
        var neighbors = new List<Node>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                var xNew = x + i;
                var yNew = y + j;
                if(i == 0 && j == 0 || !CheckInGrid(xNew,yNew)) continue;
                neighbors.Add(pointGrid[xNew,yNew]);
            }
        }

        return neighbors;
    }
    
}

public struct Node
{
    public bool walkable;
    public int2 index;
    public float3 centerPoint;
    public int gCost;
    public int hCost;
    public int fCost;
    public int2 indexNodeConnect;

    public void Init(int2 index,float3 centerPoint)
    {
        this.centerPoint = centerPoint;
        this.index = index;
        
    }
    public void CalculateGCost(Node node)
    {
        indexNodeConnect = node.index;
        
        var cost = CalculateCost(node.index);

        gCost = cost + node.gCost;
    }
    public void CalculateHCost(Node node)
    {
        hCost = CalculateCost(node.index);
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public int CalculateCost(int2 indexCheck)
    {
        var subtract = math.abs(indexCheck - index);
        var remaining = math.abs(subtract.x - subtract.y);

        return Grid_pathfinding.MOVE_DIAGONAL_COST * math.min(subtract.x, subtract.y) +
               Grid_pathfinding.MOVE_STRAIGHT_COST * remaining;
    }
}