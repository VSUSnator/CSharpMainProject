using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    void ApplyBuff(BuffDebuff buff);
    void RemoveBuff(BuffDebuff buff);
    void ApplyDebuff(BuffDebuff debuff); 
    void RemoveDebuff(BuffDebuff debuff);
}