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
        public int X { get; }
        public int Y { get; }
        public int Cost { get; private set; } = 10; // Стоимость перемещения
        public int Estimate { get; private set; }
        public int Value => Cost + Estimate; // Общая стоимость
        public Node? Parent { get; set; } // Родительский узел

        public Node(int x, int y)
        {
            X = x; 
            Y = y;
        }

        public void CalculateEstimate(int targetX, int targetY)
        {
            Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);
        }

        public void SetCost(int newCost)
        {
            Cost = newCost;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Node node)
                return false;

            return X == node.X && Y == node.Y;
        }
        public override int GetHashCode() => (X, Y).GetHashCode();

    }
}