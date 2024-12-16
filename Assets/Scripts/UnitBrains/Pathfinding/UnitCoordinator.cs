using System.Collections.Generic;
using System.Linq;
using Model.Runtime; // ���������, ��� ��� ������������ ���� ��������� �������
using Model.Runtime.ReadOnly; // ��� IReadOnlyRuntimeModel
using UnityEngine;
using Utilities;
using Model;
using Model.Config;
using View;

public class UnitCoordinator : MonoBehaviour
{
    private IReadOnlyRuntimeModel _runtimeModel;
    private TimeUtil _timeUtil;

    // �����������
    public UnitCoordinator(IReadOnlyRuntimeModel runtimeModel, TimeUtil timeUtil)
    {
        _runtimeModel = runtimeModel ?? throw new System.ArgumentNullException(nameof(runtimeModel), "Failed to get IReadOnlyRuntimeModel from ServiceLocator.");
        _timeUtil = timeUtil ?? TimeUtil.Create(); // ������� ��� �������� ������������ ��������� TimeUtil
    }

    public List<Unit> GetUnitsInRadius(Vector2Int position, float radius)
    {
        return _runtimeModel.RoUnits
            .OfType<Unit>() // �������� � ���� Unit
            .Where(unit => Vector2Int.Distance(unit.Pos, position) <= radius && !unit.IsDead)
            .ToList();
    }

    public Vector2Int RecommendedTarget { get; private set; }
    public Vector2Int RecommendedPosition { get; private set; }

    public void UpdateRecommendations(int attackRadius, int mapHalf)
    {
        var enemyUnits = GetEnemyUnits();
        Vector2Int playerBasePos = GetPlayerBasePosition();

        if (enemyUnits.Any(unit => unit.Pos.x < mapHalf))
        {
            // ���� ����� �� ����� �������� �����
            RecommendedTarget = GetClosestEnemy(enemyUnits, playerBasePos);
            RecommendedPosition = playerBasePos + new Vector2Int(-1, 0); // ����� ����� �����
        }
        else
        {
            // ����� �� ������ �������
            RecommendedTarget = GetWeakestEnemy(enemyUnits);
            RecommendedPosition = RecommendedTarget + new Vector2Int(0, 1); // ����� �� ���������� �������� �� ���������� �����
        }
    }

    private List<Unit> GetEnemyUnits()
    {
        return _runtimeModel.RoUnits
            .OfType<Unit>() // �������� � ���� Unit
            .ToList(); // ��������� ������ �������, � �� ������ �������
    }

    private Vector2Int GetPlayerBasePosition()
    {
        var playerBase = _runtimeModel.RoBases.ElementAtOrDefault(RuntimeModel.PlayerId) as MainBase;
        if (playerBase == null)
        {
            Debug.LogError($"Player base not found for PlayerId: {RuntimeModel.PlayerId}");
            return Vector2Int.zero; // ������� �������� �� ���������
        }
        return playerBase.Pos; // �������� ������� ���� ������
    }

    private Vector2Int GetClosestEnemy(List<Unit> enemies, Vector2Int playerBase)
    {
        if (!enemies.Any())
        {
            Debug.LogWarning("No enemies found.");
            return Vector2Int.zero; // ������� �������� �� ���������
        }

        return enemies
            .OrderBy(unit => Vector2Int.Distance(unit.Pos, playerBase))
            .FirstOrDefault()?.Pos ?? Vector2Int.zero; // ����� unit.Pos - ��� Vector2, ����������� ��� � Vector2Int
    }

    private Vector2Int GetWeakestEnemy(List<Unit> enemies)
    {
        if (!enemies.Any())
        {
            Debug.LogWarning("No enemies found.");
            return Vector2Int.zero; // ������� �������� �� ���������
        }

        return enemies
            .Where(unit => !unit.IsDead) // ��������, ��� ���� �� �����
            .OrderBy(unit => unit.Health) // ��������� �� ��������
             .FirstOrDefault()?.Pos ?? Vector2Int.zero;
    }
}