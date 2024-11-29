using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager
{
    private Dictionary<IBuffable, List<BuffDebuff>> _activeBuffs = new Dictionary<IBuffable, List<BuffDebuff>>();

    public void AddBuff(BuffDebuff buff, IBuffable character)
    {
        if (!_activeBuffs.ContainsKey(character))
        {
            _activeBuffs[character] = new List<BuffDebuff>();
        }

        if (!_activeBuffs[character].Any(b => b.Name == buff.Name))
        {
            _activeBuffs[character].Add(buff);
            buff.Apply(character); // Используем метод абстрактного класса
        }
    }

    public void RemoveBuff(BuffDebuff buff, IBuffable character)
    {
        if (_activeBuffs.TryGetValue(character, out List<BuffDebuff> buffs))
        {
            if (buffs.Remove(buff))
            {
                buff.Remove(character); // Используем метод абстрактного класса
            }

            if (buffs.Count == 0)
            {
                _activeBuffs.Remove(character);
            }
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var unit in new List<IBuffable>(_activeBuffs.Keys))
        {
            for (int i = _activeBuffs[unit].Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[unit][i];
                buff.Update(deltaTime);

                if (buff.IsExpired())
                {
                    RemoveBuff(buff, unit);
                }
            }
        }
    }
}