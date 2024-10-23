using System.Collections.Generic;
using Model;
using UnityEngine;
using Utilities;
#nullable enable

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        public int X { get; }
        public int Y { get; }
        public int Cost { get; set; } = 10; // ��������� ����������� �� ���� ����
        public int Estimate { get; set; } // ������������� ������ ���������� �� ����
        public int Value => Cost + Estimate; // ����� ���������
        public AStarUnitPath Parent { get; set; } // ������������ ����
        private const int MaxLength = 100;

        public AStarUnitPath(int x, int y, IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
                    : base(runtimeModel, startPoint, endPoint)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj is AStarUnitPath node && X == node.X && Y == node.Y;
        }

        public override int GetHashCode() => (X, Y).GetHashCode(); // ���������� ���-��� ��� ����



        protected override void Calculate()
        {
            var currentPoint = startPoint;
            var result = new List<Vector2Int> { startPoint };
            var visited = new HashSet<Vector2Int> { startPoint }; // ��� ������������ ���������� �����
            var counter = 0;

            while (currentPoint != endPoint && counter++ < MaxLength)
            {
                Node currentNode = new Node(currentPoint.x, currentPoint.y);
                currentNode.CalculateEstimate(endPoint.x, endPoint.y);
                currentNode.CalculateValue();

                // ���������� ����� ��� ��������� ���������� ����
                var nextPoint = CalcNextStepTowards(currentPoint, endPoint);

                // ���������, ���� ��� ��� ������� ��� �� ��������
                if (visited.Contains(nextPoint))
                {
                    break; // ���� ��������� ����� ��� � ����, ������� �� �����
                }

                if (!visited.Contains(nextPoint) && runtimeModel.IsTileWalkable(nextPoint))
                {
                    result.Add(nextPoint);
                    visited.Add(nextPoint); // ��������� � ����������
                    currentPoint = nextPoint; // ��������� ������� �����
                }
                else
                {
                    // ���� ������ ����� �� ���������, ������� ������ ��������
                    Vector2Int partStep0 = currentPoint + new Vector2Int(nextPoint.x - currentPoint.x, 0);

                    if (!visited.Contains(partStep0) && runtimeModel.IsTileWalkable(partStep0))
                    {
                        result.Add(partStep0);
                        visited.Add(partStep0);
                        currentPoint = partStep0;
                        continue; // ������� � ��������� �������� �����
                    }

                    Vector2Int partStep1 = currentPoint + new Vector2Int(0, nextPoint.y - currentPoint.y);
                    if (!visited.Contains(partStep1) && runtimeModel.IsTileWalkable(partStep1))
                    {
                        result.Add(partStep1);
                        visited.Add(partStep1);
                        currentPoint = partStep1;
                        continue; // ������� � ��������� �������� �����
                    }
                    break; // ���� �� ���� ��� �� ��������, ������� �� �����
                }
            }

            path = result.ToArray();
            Debug.Log($"Calculating path from {startPoint} to {endPoint}");
        }
        private bool IsUnitAtPosition(Vector2Int unitPos)
        {
            foreach (var cell in GetPath())
            {
                if (cell == unitPos)
                {
                    return true; // ���� ��������� �� �������
                }
            }
            return false; // ���� �� �� �������
        }

        private Vector2Int CalcNextStepTowards(Vector2Int current, Vector2Int target)
        {
            // ���������� ��������� ���, ����������� �� ������� �����
            int nextX = current.x + (current.x < target.x ? 1 : (current.x > target.x ? -1 : 0));
            int nextY = current.y + (current.y < target.y ? 1 : (current.y > target.y ? -1 : 0));

            var nextStep = new Vector2Int(nextX, nextY);

            // ���������, ��� ��������� ������� ���������
            if (runtimeModel.IsTileWalkable(nextStep))
                return nextStep;


            // ���� ������ ����� �� ���������, ������� ������ ��������
            if (Mathf.Abs(nextX - current.x) > 1 || Mathf.Abs(nextY - current.y) > 1)
            {
                var partStep0 = current + new Vector2Int(nextX - current.x, 0);
                if (runtimeModel.IsTileWalkable(partStep0))
                    return partStep0;

                var partStep1 = current + new Vector2Int(0, nextY - current.y);
                if (runtimeModel.IsTileWalkable(partStep1))
                    return partStep1;
            }
            // ���� �� ���� ��� �� ��������, ���������� ������� ���������
            return current; // ���������� ������� ���������, ���� �� ���� ��� �� ��� ��������
        }


        public List<AStarUnitPath> GetNeighbours(IReadOnlyRuntimeModel runtimeModel)
        {
            var neighbours = new List<AStarUnitPath>();
            var directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), // ������
                new Vector2Int(-1, 0), // �����
                new Vector2Int(0, 1), // �����
                new Vector2Int(0, -1) // ����
            };

            foreach (var dir in directions)
            {
                int newX = X + dir.x;
                int newY = Y + dir.y;

                // �������� �� ������������
                if (runtimeModel.IsTileWalkable(new Vector2Int(newX, newY)))
                {
                    neighbours.Add(new AStarUnitPath(newX, newY, runtimeModel, new Vector2Int(newX, newY), new Vector2Int(newX, newY)));
                }
            }
            return neighbours;
        }



        private List<AStarUnitPath> ReconstructPath(AStarUnitPath currentNode)
        {
            // ��������������� ����, ������� � �������� ����
            List<AStarUnitPath> path = new List<AStarUnitPath>();

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
            // ������������� ����, ����� �������� ��� � ���������� �������
            // ��������� ���� ��� ��������� ����������� �������� � ���
        }
    }
}