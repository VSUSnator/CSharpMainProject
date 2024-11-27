using Model;
using UnitBrains.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnityEngine;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {
        private UnitCoordinator unitCoordinator;

        // Метод для инициализации с экземпляром UnitCoordinator
        public void Initialize(UnitCoordinator coordinator)
        {
            unitCoordinator = coordinator;

            if (unitCoordinator == null)
            {
                Debug.LogError("UnitCoordinator instance is null. Make sure it is initialized.");
            }
        }

        private void Start()
        {
            // Здесь можно оставить пустым, или добавить другую логику, если необходимо
        }

        protected float DistanceToOwnBase(Vector2Int fromPos) =>
            Vector2Int.Distance(fromPos, runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);

        protected void SortByDistanceToOwnBase(List<Vector2Int> list)
        {
            list.Sort(CompareByDistanceToOwnBase);
        }

        private int CompareByDistanceToOwnBase(Vector2Int a, Vector2Int b)
        {
            var distanceA = DistanceToOwnBase(a);
            var distanceB = DistanceToOwnBase(b);
            return distanceA.CompareTo(distanceB);
        }

        public void UpdateRecommendations()
        {
            if (unitCoordinator == null)
            {
                Debug.LogError("UnitCoordinator is not initialized.");
                return;
            }

            int attackRadius = 1; // Задайте радиус атаки, если он фиксирован
            int mapHalf = runtimeModel.RoMap.Width / 2; // Половина карты

            unitCoordinator.UpdateRecommendations(attackRadius, mapHalf);

            Vector2Int recommendedTarget = unitCoordinator.RecommendedTarget;
            Vector2Int recommendedPosition = unitCoordinator.RecommendedPosition;

            Debug.Log($"Recommended Target: {recommendedTarget}, Recommended Position: {recommendedPosition}");

            // Логика для использования recommendedTarget и recommendedPosition
            SetTarget(recommendedTarget);
            SetPosition(recommendedPosition);
        }

        private void SetTarget(Vector2Int target)
        {
            // Логика установки цели для юнита
            Debug.Log($"Setting target to: {target}");
        }

        private void SetPosition(Vector2Int position)
        {
            // Логика перемещения юнита к рекомендованной позиции
            Debug.Log($"Setting position to: {position}");
        }
    }
}