using Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Pathfinding;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

#nullable enable

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };
        private int MaxLength => runtimeModel.RoMap.Width * runtimeModel.RoMap.Height;
        private const int MoveCost = 10;

        public AStarUnitPath(int x, int y, IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
            : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            var currentPoint = startPoint;
            var result = new List<Vector2Int> { startPoint };
            var visited = new HashSet<Vector2Int> { startPoint }; // Для отслеживания посещённых точек
            var counter = 0;

            while (currentPoint != endPoint && counter++ < MaxLength)
            {
                var currentNode = new Node(currentPoint.x, currentPoint.y);
                currentNode.CalculateEstimate(endPoint.x, endPoint.y);
                currentNode.SetCost(10);

                // Ищем следующий шаг
                var nextPoint = CalcNextStepTowards(currentPoint, endPoint);

                // Проверяем, если шаг уже посещён или не проходим
                if (!visited.Contains(nextPoint) && runtimeModel.IsTileWalkable(nextPoint))
                {
                    result.Add(nextPoint);
                    visited.Add(nextPoint); // Добавляем в посещённые
                    currentPoint = nextPoint; // Обновляем текущую точку
                }
                else
                {
                    // Если прямая линия не проходима, пробуем другие варианты
                    if (!TryStep(currentPoint, visited, result))
                    {
                        // Если нет доступных шагов, выходим из цикла
                        Debug.LogWarning($"No available steps from {currentPoint}. Stopping calculation.");
                        break;
                    }
                }
            }

            path = result.ToArray();
            Debug.Log($"Calculating path from {startPoint} to {endPoint}");
        }

        private static readonly Vector2Int[] possibleSteps = {
             new Vector2Int(1, 0), // Перемещение вправо
             new Vector2Int(-1, 0), // Перемещение влево
             new Vector2Int(0, 1), // Перемещение вверх
             new Vector2Int(0, -1) // Перемещение вниз
        };

        private bool TryStep(Vector2Int currentPoint, HashSet<Vector2Int> visited, List<Vector2Int> result)
        {
            foreach (var step in possibleSteps)
            {
                Vector2Int newStep = new Vector2Int(currentPoint.x + step.x, currentPoint.y + step.y);
                if (!visited.Contains(newStep) && runtimeModel.IsTileWalkable(newStep))
                {
                    result.Add(newStep);
                    visited.Add(newStep); // Добавляем в посещённые
                    return true; // Успех
                }
            }

            return false; // Если ни один шаг не сработал
        }

        private bool IsValid(Vector2Int cell)
        {
            bool isValidX = cell.x >= 0 && cell.x < runtimeModel.RoMap.Width;
            bool isValidY = cell.y >= 0 && cell.y < runtimeModel.RoMap.Height;

            return isValidX && isValidY && runtimeModel.IsTileWalkable(cell);
        }

        private Vector2Int CalcNextStepTowards(Vector2Int current, Vector2Int target)
        {
            // Определяем следующий шаг, основываясь на целевой точке
            Vector2Int nextStep = current; // Начнем с текущей позиции

            // Пробуем переместиться по всем направлениям
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = current.x + dx[i];
                int newY = current.y + dy[i];

                Vector2Int potentialStep = new Vector2Int(newX, newY);

                // Проверяем, что следующая позиция проходима
                if (IsValid(potentialStep))
                {
                    nextStep = potentialStep; // Обновляем следующую позицию, если она допустима
                    break; // Если нашли допустимую позицию, выходим из цикла
                }
            }

            return nextStep; // Возвращаем следующую допустимую позицию
        }

    }
}