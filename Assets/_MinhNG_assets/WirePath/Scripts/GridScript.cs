using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridScript : MonoBehaviour
{
    public float spaceMin;
    public Color[] pathColors;
    public List<CellInfo> cells;
    public int maxCountRen;
    [Header("--- SETUP ---")]
    public GameObject cellPrefab;
    public Vector2 gridSpace;
    public Vector2 gridSize;
    public List<path> paths;
    
    #region SETUP
    [ContextMenu("Build Grid")]
    public void BuildGrid()
    {
        if (cells == null)
        {
            cells = new List<CellInfo>();
        }

        var totalCel = gridSize.x * gridSize.y;

        var subtract = totalCel - cells.Count;
        
        Debug.Log(subtract);
        
        var sign = Mathf.Sign(subtract);
        
        if (subtract != 0)
        {
            for (int i = 0; i < Mathf.Abs(subtract); i++)
            {
                if (sign < 0)
                {
                    DestroyImmediate(cells[0].spriteRenderer.gameObject);
                    cells.RemoveAt(0);
                }
                else
                {
                    var instance = Instantiate(cellPrefab,transform);
                    var gridInfo = instance.GetComponent<CellInfo>();
                    cells.Add(gridInfo);
                }
                
            }
        }
        
        
        for (int i = 0; i < cells.Count; i++)
        {
            var posGrid = GetGridPosition(i);
            cells[i].Init(posGrid, gridSpace);
        }
    }

    [ContextMenu("Clear All Cell")]
    public void ClearAllCell()
    {
        var list = GameObject.FindObjectsOfType<CellInfo>();
        
        for (int i = 0; i < list.Length; i++)
        {
            DestroyImmediate(list[i].gameObject);
        }
        cells.Clear();
    }
    private Vector2 GetGridPosition(int index)
    {
        int y = (int)(index / gridSize.x);
        var x = index - y * gridSize.x; 
        //
        Vector2 pos = new Vector2(x,y);
        return pos;
    }

    #endregion

    [ContextMenu("Reset State Cell")]
    public void ResetStateCell ()
    {
        foreach (var cell in cells)
        {
            cell.ResetState();
        }
    }
    
    [ContextMenu("Generate")]
    public void Generate()
    {
        paths = new List<path>();
        ResetStateCell();
        Dictionary<Color, path> pathDictionary = new Dictionary<Color, path>();
        for (int i = 0; i < pathColors.Length; i++)
        {
            Color a = pathColors[i];
            int index = 0;
            bool check = false;
            int count = 0;
            while (!check)
            {
                count++;
                check = true;
                index = Random.Range(0,cells.Count);
                var posGrid = cells[index].gridPosition;
                foreach (var path in pathDictionary)
                {
                    if (Vector2.Distance(posGrid, cells[path.Value.GetTail()].gridPosition) < spaceMin)
                    {
                        check = false;
                        break;
                    }
                }

                if (check)
                {
                    var cell = cells[index].gridPosition;
                    check = (cell.x > 0 && cell.x + 1 < gridSize.x && cell.y > 0 && cell.y + 1 < gridSize.y);
                }

                if (count > maxCountRen)
                {
                    Debug.Log("max count : " + i );
                    break;
                }
            }

            cells[index].isReady = false;
            path newPath = new path();
            newPath.AddStart(index);
            
            pathDictionary.Add(a,newPath);
        }

        int indexRen = 0;
        while (!CheckFull())
        {
            indexRen++;
            foreach (var pathD in pathDictionary)
            {
                var indexNextHead = GetIndexPriority(pathD.Value.GetHead());
                if (indexNextHead >= 0)
                {
                    pathD.Value.AddHead(indexNextHead);
                    cells[indexNextHead].isReady = false;
                }

                var indexNextTail = GetIndexPriority(pathD.Value.GetTail());
                if (indexNextTail >= 0)
                {
                    pathD.Value.AddTail(indexNextTail);
                    cells[indexNextTail].isReady = false;
                }
            }

            if (indexRen > maxCountRen)
            {
                Debug.Log("is stack over flow");
                break;
            }
        }

        foreach (var pathD in pathDictionary)
        {
            paths.Add(pathD.Value);
            foreach (var iCell in pathD.Value.pathList)
            {
                cells[iCell].Draw(pathD.Key);
            }
        }
        
    }

    private int GetIndexPriority(int index)
    {
        
        int indexNext = -1;

        var cellGridPos = cells[index].gridPosition;
        List<CellPriority> cellNext = new List<CellPriority>();
        int priority = 0;
        int priorityMax = 0;
        if (CheckCellReady(cellGridPos + Vector2.up))
        {
            var gridPos = cellGridPos + Vector2.up;
            priority = GetPriority(gridPos);
            var indexInGrid = GetIndexInGrid(gridPos);
            cellNext.Add(new CellPriority()
            {
                priority = priority,
                index = indexInGrid,
            });
            priorityMax = priority;
        }
        
        if (CheckCellReady(cellGridPos - Vector2.up))
        {
            var gridPos = cellGridPos - Vector2.up;
            priority = GetPriority(gridPos);
            var indexInGrid = GetIndexInGrid(gridPos);
            cellNext.Add(new CellPriority()
            {
                priority = priority,
                index = indexInGrid,
            });
            if (priorityMax < priority)
            {
                priorityMax = priority;
            }
        }
        
        if (CheckCellReady(cellGridPos + Vector2.right))
        {
            var gridPos = cellGridPos + Vector2.right;
            priority = GetPriority(gridPos);
            var indexInGrid = GetIndexInGrid(gridPos);
            cellNext.Add(new CellPriority()
            {
                priority = priority,
                index = indexInGrid,
            });
            if (priorityMax < priority)
            {
                priorityMax = priority;
            }
        }
        
        if (CheckCellReady(cellGridPos - Vector2.right))
        {
            var gridPos = cellGridPos - Vector2.right;
            priority = GetPriority(gridPos);
            var indexInGrid = GetIndexInGrid(gridPos);
            cellNext.Add(new CellPriority()
            {
                priority = priority,
                index = indexInGrid,
            });
            if (priorityMax < priority)
            {
                priorityMax = priority;
            }
        }

        if (cellNext.Count > 0)
        {
            cellNext.RemoveAll(x => x.priority < priorityMax);
            var randomIndex = Random.Range(0, cellNext.Count);
            indexNext = cellNext[randomIndex].index;
        }
        
        return indexNext;
    }

    private int GetPriority(Vector2 cellGridPos)
    {
        int priority = 0;

        if (!CheckCellReady(cellGridPos + Vector2.up))
        {
            priority++;
        }
        
        if (!CheckCellReady(cellGridPos - Vector2.up))
        {
            priority++;
        }
        
        if (!CheckCellReady(cellGridPos + Vector2.right))
        {
            priority++;
        }
        
        if (!CheckCellReady(cellGridPos - Vector2.right))
        {
            priority++;
        }

        return priority;
    }

    private bool CheckCellReady(Vector2 cellGridPos)
    {
        if (cellGridPos.x < 0 || cellGridPos.y < 0 || cellGridPos.x + 1 > gridSize.x ||
            cellGridPos.y + 1 > gridSize.y) return false;
        bool isReady = true;

        foreach (var cell in cells)
        {
            if (cell.gridPosition.Equals(cellGridPos))
            {
                isReady = cell.isReady;
                break;
            }
        }
        return isReady;
    }

    private int GetIndexInGrid(Vector2 cellGridPos)
    {
        for(int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            if (cell.gridPosition.Equals(cellGridPos))
            {
                return i;
            }
        }

        return -1;
    }
    
    private struct CellPriority
    {
        public int index;
        public int priority;
    }

    private bool CheckFull()
    {
        foreach (var cell in cells)
        {
            if (cell.isReady) return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if(paths == null) return;
        Gizmos.color = Color.red;
        foreach (var path in paths)
        {
            Vector3 pos = default;
            bool check = false;
            foreach (var iCell in path.pathList)
            {
                var nextPos = cells[iCell].transform.position;
                if (!check)
                {
                    pos = nextPos;
                    check = true;
                    continue;
                }
                Gizmos.DrawLine(pos,nextPos);
                pos = nextPos;
            }
        }
    }
}

public class path
{
    public List<int> pathList;

    public void AddHead(int index)
    {
        pathList.Add(index);
    }

    public void AddTail(int index)
    {
        pathList.Insert(0,index);
    }

    public void AddStart(int index)
    {
        pathList = new List<int>();
        pathList.Add(index);
    }

    public int GetTail()
    {
        return pathList[0];
    }

    public int GetHead()
    {
        return pathList[pathList.Count - 1];
    }
    
}

