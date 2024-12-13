using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class FourUnitBrain : DefaultPlayerUnitBrain
    {
        public static int IdBilling = 0;
        public int Id;

        public override string TargetUnitName => "Wololo";

        private const int MaxTarget = 3;
        private Vector2Int _notRangeEnemyPosition;
        private List<Vector2Int> allTargetEnemies = new List<Vector2Int>();

        public FourUnitBrain()
        {
            Id = IdBilling++;
        }

        private BuffSystem _buffSystem;

        public FourUnitBrain(BuffSystem buffSystem)
        {
            Id = IdBilling++;
            _buffSystem = buffSystem; // Получаем ссылку на систему баффов
        }


        protected override List<Vector2Int> SelectTargets()
        {
            allTargetEnemies.Clear();
            allTargetEnemies = GetAllTargets().ToList();

            Vector2Int targetBaseEnemy = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            allTargetEnemies.Remove(targetBaseEnemy);

            // Если нужно оставить только одну цель
            if (allTargetEnemies.Count > 0)
            {
                SortByDistanceToOwnBase(allTargetEnemies);

                if (allTargetEnemies.Count > MaxTarget)
                {
                    allTargetEnemies = allTargetEnemies.GetRange(0, MaxTarget);
                }

                int idEnemy = (Id - 1) % allTargetEnemies.Count;
                Vector2Int target = allTargetEnemies[idEnemy];
                allTargetEnemies.Clear();

                if (!HasTargetsInRange())
                {
                    _notRangeEnemyPosition = target;
                }
                else
                {
                    allTargetEnemies.Add(target);
                }
            }
            else
            {
                allTargetEnemies.Clear();

                if (!HasTargetsInRange())
                {
                    _notRangeEnemyPosition = targetBaseEnemy;
                }
                else
                {
                    allTargetEnemies.Add(targetBaseEnemy);
                }
            }
            return allTargetEnemies;
        }

        public override Vector2Int GetNextStep()
        {
            // Возвращаем следующий шаг к цели
            return unit.Pos.CalcNextStepTowards(_notRangeEnemyPosition);
        }

        public override void Update(float deltaTime, float time)
        {
            if (CanMove())
            {
                Move();
            }
        }

        private void Move()
        {
            Vector2Int nextStep = GetNextStep();
            // Логика для движения к цели
        }

        private bool CanMove()
        {
            return HasTargetsInRange();
        }
    }
}