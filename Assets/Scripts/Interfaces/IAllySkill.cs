using UnityEngine;

public interface IAllySkill
{
    void UseOnSelf(GameObject caster);               // 자기에게 사용
    void UseOnAlly(GameObject caster, GameObject ally); // 파트너에게 사용
}
