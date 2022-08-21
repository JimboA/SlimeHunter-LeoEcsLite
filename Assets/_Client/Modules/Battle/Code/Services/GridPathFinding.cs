using System;
using JimmboA.Plugins;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    // A very naive and straightforward implementation of A* algorithm, but for our "turn based" case this is enough 
    public class GridPathFinding
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        public void FindPath(int2 startPos, int2 endPos, int2 gridSize, NativeArray<int2> steps, NativeArray<bool> walkableCells, FastList<int> result)
        {
            var path = new NativeList<int>(32, Allocator.TempJob);
            var job = new PathFindingJob
            {
                StartPosition = startPos,
                EndPosition = endPos,
                GridSize = gridSize,
                Path = path,
                WalkableCells = walkableCells,
                StepOffsets = steps
            };
            
            job.Schedule().Complete(); 

            foreach (var index in path)
            {
                result.Add(index);
            }

            path.Dispose();
        }

        [BurstCompile]
        public struct PathFindingJob : IJob
        {
            public int2 StartPosition;
            public int2 EndPosition;
            public int2 GridSize;
            [DeallocateOnJobCompletion] public NativeArray<bool> WalkableCells;
            [DeallocateOnJobCompletion] public NativeArray<int2> StepOffsets;
            public NativeList<int> Path;

            public void Execute()
            {
                var gridWidth = GridSize.x;
                var cells = new NativeArray<PathNode>(GridSize.x * GridSize.y, Allocator.Temp);
                for (int y = 0; y < GridSize.y; y++)
                {
                    for (int x = 0; x < GridSize.x; x++)
                    {
                        var pathNode = new PathNode
                        {
                            X = x,
                            Y = y,
                            Index = CalculateIndex(x, y, gridWidth),
                            GCost = int.MaxValue,
                            HCost = CalculateDistanceCost(new int2(x, y), EndPosition),
                            CameFromNodeIndex = -1,
                            IsSearched = false
                        };
                        pathNode.CalculateFCost();
                        pathNode.IsWalkable = WalkableCells[pathNode.Index];
                        cells[pathNode.Index] = pathNode;
                    }
                }

                var endNodeIndex = CalculateIndex(EndPosition.x, EndPosition.y, gridWidth);
                var startNode = cells[CalculateIndex(StartPosition.x, StartPosition.y, gridWidth)];
                startNode.GCost = 0;
                startNode.CalculateFCost();
                cells[startNode.Index] = startNode;

                var openList = new NativeList<int>(Allocator.Temp);
                openList.Add(startNode.Index);

                while (openList.Length > 0)
                {
                    var currentNodeIndex = GetLowestCostFNodeIndex(in openList, in cells);
                    var currentNode = cells[currentNodeIndex];
                    if (currentNodeIndex == endNodeIndex)
                        break;

                    for (int i = 0; i < openList.Length; i++)
                    {
                        if (openList[i] == currentNodeIndex)
                        {
                            openList.RemoveAtSwapBack(i);
                            break;
                        }
                    }
                    
                    currentNode.IsSearched = true;

                    for (int i = 0; i < StepOffsets.Length; i++)
                    {
                        var neighbourOffset = StepOffsets[i];
                        var neighbourPosition = new int2(currentNode.X + neighbourOffset.x,
                            currentNode.Y + neighbourOffset.y);

                        if (!IsPositionInsideGrid(neighbourPosition, GridSize))
                            continue;

                        var neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridWidth);
                        var neighbourNode = cells[neighbourNodeIndex];
                        if (neighbourNode.IsSearched || !neighbourNode.IsWalkable)
                            continue;

                        var currentNodePosition = new int2(currentNode.X, currentNode.Y);
                        var tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                        if (tentativeGCost < neighbourNode.GCost)
                        {
                            neighbourNode.CameFromNodeIndex = currentNodeIndex;
                            neighbourNode.GCost = tentativeGCost;
                            neighbourNode.CalculateFCost();
                            cells[neighbourNodeIndex] = neighbourNode;

                            if (!openList.Contains(neighbourNode.Index))
                                openList.Add(neighbourNode.Index);
                        }
                    }
                }
                
                var endNode = cells[endNodeIndex];
                if(endNode.CameFromNodeIndex != -1)
                    CalculatePath(in cells, in endNode);
                
                openList.Dispose();
                cells.Dispose();
            }

            private int CalculateIndex(int x, int y, int gridWidth)
            {
                return x + y * gridWidth;
            }

            private int CalculateDistanceCost(int2 aPosition, int2 bPosition)
            {
                int xDistance = math.abs(aPosition.x - bPosition.x);
                int yDistance = math.abs(aPosition.y - bPosition.y);
                int remaining = math.abs(xDistance - yDistance);
                return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
            }
            
            private int GetLowestCostFNodeIndex(in NativeList<int> openList, in NativeArray<PathNode> pathNodeArray) 
            {
                var lowestCostPathNode = pathNodeArray[openList[0]];
                for (int i = 1; i < openList.Length; i++) 
                {
                    var testPathNode = pathNodeArray[openList[i]];
                    if (testPathNode.FCost < lowestCostPathNode.FCost)
                        lowestCostPathNode = testPathNode;
                }
                return lowestCostPathNode.Index;
            }

            private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize) 
            {
                return
                    gridPosition.x >= 0 && 
                    gridPosition.y >= 0 &&
                    gridPosition.x < gridSize.x &&
                    gridPosition.y < gridSize.y;
            }
            
            private void CalculatePath(in NativeArray<PathNode> pathNodeArray, in PathNode endNode) 
            {
                Path.Add(endNode.Index);
                var currentNode = endNode;
                while (currentNode.CameFromNodeIndex != -1) 
                {
                    var cameFromNode = pathNodeArray[currentNode.CameFromNodeIndex];
                    Path.Add(cameFromNode.Index);
                    currentNode = cameFromNode;
                }
            }

            private struct PathNode
            {
                public int X;
                public int Y;
                public int Index;
                public int GCost;
                public int HCost;
                public int FCost;
                public int CameFromNodeIndex;
                public bool IsWalkable;
                public bool IsSearched;
                
                public void CalculateFCost() => FCost = GCost + HCost;
            }
        }
    }

    public static class GridPathfindingHelpers
    {
        public static NativeArray<int2> GetStepOffsets(StepType type, int range)
        {
            return type switch
            {
                StepType.Square => new NativeArray<int2>(8, Allocator.TempJob)
                {
                    [0] = new int2(-range, 0), // Left
                    [1] = new int2(+range, 0), // Right
                    [2] = new int2(0, +range), // Up
                    [3] = new int2(0, -range), // Down
                    [4] = new int2(-range, -range), // Left Down
                    [5] = new int2(-range, +range), // Left Up
                    [6] = new int2(+range, -range), // Right Down
                    [7] = new int2(+range, +range), // Right Up
                },
                StepType.Cross => new NativeArray<int2>(4, Allocator.TempJob)
                {
                    [0] = new int2(-range, 0), // Left
                    [1] = new int2(+range, 0), // Right
                    [2] = new int2(0, +range), // Up
                    [3] = new int2(0, -range), // Down
                },
                StepType.Diagonal => new NativeArray<int2>(4, Allocator.TempJob)
                {
                    [0] = new int2(-range, -range), // Left Down
                    [1] = new int2(-range, +range), // Left Up
                    [2] = new int2(+range, -range), // Right Down
                    [3] = new int2(+range, +range), // Right Up
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}