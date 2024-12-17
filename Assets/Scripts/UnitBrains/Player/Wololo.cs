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
            // ���� ���� � ��������� �����, ���������, ������� �� ����� ���������
            if (_isBuffing)
            {
                if (time >= _buffStartTime + _stopDuration)
                {
                    _isBuffing = false; // ��������� ��������� �����
                    unit.StartMoving(); // ������������ �������� ����� ����� �����
                }
                return; // ������������� ����������, ���� ���� ��������
            }

            // ���� ������ ����� ��� ���������� ������
            if (time >= _nextBuffTime)
            {
                _nextBuffTime = time + _buffCooldown; // ������������� ����� ���������� ����������
                //ApplyBuffsToAllies(); // ��������� ����� � ���������
                _buffStartTime = time; // ���������� ����� ������ �����
                _isBuffing = true; // ������������� ��������� �����

                // ������������� �������� �������� �����
                unit.StopMoving(); // ��������� �������� ������ ��� �����, ������� ��������� ����
                return; // ����� �� ������
            }

            // ������ ��� ������ ���������� ����, ���� ���� �� � ��������� �����
            var allTargets = GetAllTargets();

            // �������� � ����, ���� ���� �� � ��������� �����
            if (!_isBuffing)
            {
                unit.Move(GetNextStep()); // ������ �� �������� Move � ������� ��������
            }
        }

        //private void ApplyBuffsToAllies()
        //{
        //    var vfxView = ServiceLocator.Get<VFXView>();
        //    if (vfxView == null)
        //    {
        //        Debug.LogError("VFXView �� ��������������� � ServiceLocator!");
        //        return;
        //    }

        //    foreach (var ally in GetAllAlliesInRadius(_buffRadius))
        //    {
        //        // ���������, ��� ally - ��� ��� Unit
        //        if (ally is Unit unitAlly) // �������� ally � ���� Unit
        //        {
        //            bool buffApplied = false;

        //            // ��������� ������� �������
        //            if (unitAlly.GetActiveBuffs().All(b => b.GetType() != typeof(DoubleShotBuff)))
        //            {
        //                unitAlly.ApplyBuff(new DoubleShotBuff(5f)); // ��������� ���� �������� ��������
        //                vfxView.PlayVFX(unitAlly.Pos, VFXView.VFXType.BuffApplied); // ������������� VFX
        //                Debug.Log($"���� �������� �������� ������� � ����� �� ������� {unitAlly.Pos}.");
        //                buffApplied = true;
        //            }
        //            else
        //            {
        //                Debug.Log($"���� �� ������� {unitAlly.Pos} ��� ����� ���� �������� ��������.");
        //            }

        //            // ��������� ���������� ������� �����
        //            if (unitAlly.GetActiveBuffs().All(b => b.GetType() != typeof(IncreasedShootingRadiusBuff)))
        //            {
        //                unitAlly.ApplyBuff(new IncreasedShootingRadiusBuff(5f, 2.0f)); // ��������� ���� ���������� �������
        //                vfxView.PlayVFX(unitAlly.Pos, VFXView.VFXType.BuffApplied); // ������������� VFX
        //                Debug.Log($"���� ���������� ������� ����� ������� � ����� �� ������� {unitAlly.Pos}.");
        //                buffApplied = true;
        //            }
        //            else
        //            {
        //                Debug.Log($"���� �� ������� {unitAlly.Pos} ��� ����� ���� ���������� ������� �����.");
        //            }

        //            // ���� �� ���� �� ������ �� ��� ��������
        //            if (!buffApplied)
        //            {
        //                Debug.Log($"���� �� ������� {unitAlly.Pos} �� ������� �� ������ �����.");
        //            }
        //        }
        //    }
        //}

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