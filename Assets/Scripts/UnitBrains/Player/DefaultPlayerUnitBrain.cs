using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {

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

        public void UpdateBrain()
        {
            var target = UnitCoordinator.Instance.RecommendedTarget;
            var recommendedPosition = UnitCoordinator.Instance.RecommendedPosition;

            // Логика для выбора цели и движения
            if (Vector2Int.Distance(CurrentUnit.Pos, target) <= attackRadius)
            {
                AttackTarget(target);
            }
            else
            {
                MoveTo(recommendedPosition);
            }
        }

        public void UpdateUnit()
        {
            var target = UnitCoordinator.Instance.RecommendedTarget;
            var recommendedPosition = UnitCoordinator.Instance.RecommendedPosition;

            // Проверяем расстояние до рекомендуемой цели
            if (target != null)
            {
                if (Vector2Int.Distance(CurrentUnit.Pos, target) <= attackRadius)
                {
                    AttackTarget(target);
                }
                else
                {
                    MoveTo(recommendedPosition);
                }
            }
        }
    }
}