using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Utilities;
#nullable enable

namespace UnitBrains.Pathfinding
{
    public class Node
    {
        public int X;
        public int Y;
        public int Cost = 10; // Стоимость перемещения
        public int Estimate;
        public int Value;
        public Node Parent;

        public Node(int x, int y)
        {
            X = x; 
            Y = y;
        }

        public void CalculateEstimate(int targetX, int targetY)
        {
            Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);
        }

        public void CalculateValue()
        {
            Value = Cost + Estimate;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Node node)
                return false;

            return X == node.X && Y == node.Y;
        }

        public override int GetHashCode() => (X, Y).GetHashCode(); // Уникальный хэш-код для узла
        
        public List<Node> GetNeighbours(IReadOnlyRuntimeModel runtimeModel)
        {
            var neighbours = new List<Node>();
            var directions = new Vector2Int[]
            {
                new Vector2Int(1, 0),  // Вправо
                new Vector2Int(-1, 0), // Влево
                new Vector2Int(0, 1),  // Вверх
                new Vector2Int(0, -1)  // Вниз
            };

            foreach (var dir in directions)
            {
                int newX = X + dir.x;
                int newY = Y + dir.y;

                // Проверка на проходимость
                if (runtimeModel.IsTileWalkable(new Vector2Int(newX, newY)))
                {
                    neighbours.Add(new Node(newX, newY) { Parent = this }); // Устанавливаем родителя
                }
            }

            return neighbours;
        }

        public static List<Node> ReconstructPath(Node currentNode)
        {
            List<Node> path = new List<Node>();

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path; // Возвращаем путь в правильном порядке
        }
    }
}