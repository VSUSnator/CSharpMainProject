using Model;
using UnitBrains.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using Model.Config;
using Model.Runtime.Projectiles;
using Model.Runtime.ReadOnly;
using UnitBrains;
using UnityEngine;
using Utilities;

public class BuffSystem : MonoBehaviour
{
    private BuffManager buffManager;
    private List<BuffDebuff> availableBuffs = new List<BuffDebuff>();

    private void Awake()
    {
        buffManager = new BuffManager();
        ServiceLocator.Register(buffManager); // ����������� BuffManager

        // ����������� ��������� ������
        RegisterBuffs();
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<BuffManager>(); // �������� BuffManager ��� �����������
    }

    private void RegisterBuffs()
    {
        availableBuffs.Add(new SpeedBuff(5f, 1.5f)); // ��������� ���� ��������
        availableBuffs.Add(new AttackSpeedBuff(5f, 1.5f)); // ��������� ���� �������� �����
        availableBuffs.Add(new SpeedDebuff(5f, 0.5f)); // ��������� ������ ��������
        availableBuffs.Add(new AttackSpeedDebuff(5f, 0.5f)); // ��������� ������ �������� �����
    }

    public List<BuffDebuff> GetAvailableBuffs()
    {
        return availableBuffs;
    }

    public BuffManager GetBuffManager()
    {
        return buffManager; // ����� ��� ��������� BuffManager
    }

    private void Update()
    {
        // ��������� ��������� ������
        buffManager.Update(Time.deltaTime);
    }
}