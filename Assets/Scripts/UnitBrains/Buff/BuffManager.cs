using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager<T> where T : IBuffable<T>
{
    private Dictionary<T, List<ActiveBuff<T>>> activeBuffs = new Dictionary<T, List<ActiveBuff<T>>>();

    public void ApplyBuff(T character, BuffDebuff<T> buff)
    {
        if (buff.CanApply(character))
        {
            if (!activeBuffs.ContainsKey(character))
            {
                activeBuffs[character] = new List<ActiveBuff<T>>();
            }
            buff.Apply(character);
            activeBuffs[character].Add(new ActiveBuff<T>(buff));
        }

    }

    public void UpdateActiveBuffs(float deltaTime)
    {
        foreach (var characterBuffs in activeBuffs)
        {
            for (int i = characterBuffs.Value.Count - 1; i >= 0; i--)
            {
                var buff = characterBuffs.Value[i];
                buff.RemainingDuration -= deltaTime;

                if (buff.RemainingDuration <= 0)
                {
                    buff.Buff.Remove(characterBuffs.Key);
                    characterBuffs.Value.RemoveAt(i);
                }
            }
        }
    }

}
public class ActiveBuff<T> where T : IBuffable<T>
{
    public BuffDebuff<T> Buff { get; private set; }
    public float RemainingDuration { get; set; }

    public ActiveBuff(BuffDebuff<T> buff)
    {
        Buff = buff;
        RemainingDuration = buff.Duration;
    }
}