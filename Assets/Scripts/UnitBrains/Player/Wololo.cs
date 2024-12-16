using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime;
using UnitBrains;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using View;

namespace UnitBrains.Player
{
    public class FourUnitBrain : DefaultPlayerUnitBrain
    {
        private static int IdBilling = 0;
        public int Id { get; private set; }

        public override string TargetUnitName => "Wololo";

        private const int MaxTargetCount = 3;
        private Vector2Int _targetPosition;
        private float _buffRadius = 3f;
        private float _nextBuffTime;
        private float _buffCooldown = 5f;
        private float _stopDuration = 0.5f;
        private float _buffStartTime;
        private bool _isBuffing;

        public FourUnitBrain()
        {
            Id = IdBilling++;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            var allTargetEnemies = GetAllTargets().ToList();

            if (allTargetEnemies.Count == 0)
            {
                _targetPosition = Vector2Int.zero;
                return allTargetEnemies;
            }

            SortByDistanceToOwnBase(allTargetEnemies);

            allTargetEnemies = allTargetEnemies.Take(MaxTargetCount).ToList();
            _targetPosition = allTargetEnemies[(Id - 1) % allTargetEnemies.Count];

            return allTargetEnemies;
        }

        public override Vector2Int GetNextStep()
        {
            return unit.Pos.CalcNextStepTowards(_targetPosition);
        }

        public override void Update(float deltaTime, float time)
        {
            // Если юнит в состоянии баффа, проверяем, истекло ли время остановки
            if (_isBuffing)
            {
                if (time >= _buffStartTime + _stopDuration)
                {
                    _isBuffing = false; // Завершаем состояние баффа
                    unit.StartMoving(); // Возобновляем движение юнита после баффа
                }
                return; // Останавливаем выполнение, если юнит буффится
            }

            // Если пришло время для применения баффов
            if (time >= _nextBuffTime)
            {
                _nextBuffTime = time + _buffCooldown; // Устанавливаем время следующего применения
                ApplyBuffsToAllies(); // Применяем баффы к союзникам
                _buffStartTime = time; // Запоминаем время начала баффа
                _isBuffing = true; // Устанавливаем состояние баффа

                // Останавливаем движение текущего юнита
                unit.StopMoving(); // Остановка движения только для юнита, который применяет бафф
                return; // Выход из метода
            }

            // Логика для выбора следующего шага, если юнит не в состоянии баффа
            var allTargets = GetAllTargets();

            // Движение к цели, если юнит не в состоянии баффа
            if (!_isBuffing)
            {
                unit.Move(GetNextStep()); // Теперь мы вызываем Move с целевой позицией
            }
        }

        private void ApplyBuffsToAllies()
        {
            var vfxView = ServiceLocator.Get<VFXView>();

            foreach (var ally in GetAllAlliesInRadius(_buffRadius))
            {
                // Проверяем, есть ли у союзника уже активные баффы
                if (ally.GetActiveBuffs().All(b => b.GetType() != typeof(SpeedBuff)))
                {
                    ally.ApplyBuff(new SpeedBuff(5f, 1.5f)); // Применяем бафф

                    vfxView.PlayVFX(ally.Pos, VFXView.VFXType.BuffApplied); // Воспроизводим VFX
                }
            }
        }

        private List<Unit> GetAllAlliesInRadius(float radius)
        {
            var unitCoordinator = ServiceLocator.Get<UnitCoordinator>();
            return unitCoordinator
                .GetUnitsInRadius(unit.Pos, radius)
                .Where(u => u != unit && !u.IsDead)
                .ToList();
        }
    }
}