using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    int m_AttackPower;       //攻撃力

    public void Init(Vector3 size, int power, float time = 0.0f)
    {
        GetComponent<BoxCollider>().size = size;
        m_AttackPower = power;
        if (time != 0.0f) StartCoroutine(OnCollider(time));
    }

    virtual protected void Start()
    {

    }

    virtual protected void Update()
    {

    }

    //攻撃力を返す
    public int GetAttackPower()
    {
        return m_AttackPower;
    }

    IEnumerator OnCollider(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
