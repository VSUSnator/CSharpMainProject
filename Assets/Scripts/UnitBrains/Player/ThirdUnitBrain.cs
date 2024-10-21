using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class ThirdUnitBrain : DefaultPlayerUnitBrain
    {
        public static int IdBilling = 0;
        public int Id;

        public override string TargetUnitName => "Ironclad Behemoth";

        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private const int MaxTarget = 3;
        private const float TransitionTime = 1f; // ����� �������� ����� ����������

        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private bool _isTransitioning = false; // ���� ��������.
        private bool _isMoving = false; // ������� ��������� (�������� ��� �����)


        private Vector2Int _notRangeEnemyPosition;
        private List<Vector2Int> allTargetEnemies = new List<Vector2Int>();

        public ThirdUnitBrain()
        {
            Id = IdBilling++;
        }

        protected override List<Vector2Int> SelectTargets()
        {
            allTargetEnemies.Clear();
            allTargetEnemies = GetAllTargets().ToList();

            Vector2Int targetBaseEnemy = runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];
            allTargetEnemies.Remove(targetBaseEnemy);

            // ���� ����� �������� ������ ���� ����
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
            // ���������� ��������� ��� � ����
            return unit.Pos.CalcNextStepTowards(_notRangeEnemyPosition);
        }

        public override void Update(float deltaTime, float time)
        {
            _cooldownTime += deltaTime;

            if (_isTransitioning)
            {
                if (_cooldownTime >= TransitionTime)
                {
                    _isTransitioning = false;
                    _cooldownTime = 0f; // ����� �������
                    _isMoving = !_isMoving; // ������������ ���������
                }
                return; // �� ����� �������� ������ �� ������
            }

            if (_overheated)
            {
                _cooldownTime += deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);

                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }

            if (_isMoving)
            {
                if (CanAttack())
                {
                    StartTransition(); // ������ ������� � �����
                }
                else
                {
                    Move();
                }
            }
            else
            {
                Attack(); // ��������� �����
            }
        }

        private void StartTransition()
        {
            _isTransitioning = true;
            _cooldownTime = 0f; // ����� �������
        }

        void Move()
        
        {
            Vector2Int nextStep = GetNextStep();
            // ������ ��� �������� � ����
        }

        private void Attack()
        {
            var projectiles = GetProjectiles();
            if (projectiles != null && projectiles.Count > 0)
            {
                unit.ClearPendingProjectiles(); 

                
                List<BaseProjectile> newProjectiles = new List<BaseProjectile>(projectiles);

                
                foreach (var projectile in newProjectiles)
                {
                    // ��������� �� ������
                    ((List<BaseProjectile>)unit.PendingProjectiles).Add(projectile);
                }

                IncreaseTemperature(); // ����������� ����������� ����� ��������
                StartTransition(); // ������ ������� � ��������
            }
        }


        private bool CanAttack()
        {
                return !(_overheated || !HasTargetsInRange());
        }

        

        private int GetTemperature()
        {
            return _overheated ? (int)OverheatTemperature : (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature)
            {
                _overheated = true;
            }
        }
    }
}
