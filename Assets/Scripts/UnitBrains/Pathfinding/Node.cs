using System;
using System.Collections.Generic;
using Model;
using UnityEngine;
using Utilities;
#nullable enable

namespace UnitBrains.Pathfinding
{
    public class Nodes
    {
        public int X { get; }
        public int Y { get; }
        public int Cost { get; private set; } = 10;
        public int Estimate { get; private set; }
        public int Value => Cost + Estimate;
        public Nodes? Parent { get; set; }

        public Nodes(int x, int y)
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
            if (obj is not Nodes nodes)
                return false;

            return X == nodes.X && Y == nodes.Y;
        }
        public override int GetHashCode() => (X, Y).GetHashCode();

    }
}