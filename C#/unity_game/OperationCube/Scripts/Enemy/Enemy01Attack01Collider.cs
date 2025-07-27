using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Attack01Collider : EnemyAttackCollider
{
    EffectPlayer m_EffectPlayer;            //エフェクトプレイヤー
    [SerializeField] float m_Speed;         //飛翔速度
    [SerializeField] float m_Distance;      //飛翔距離
    Vector3 m_InitialPosition;              //初期位置
    Vector3 m_Direction;                    //飛翔方向
    bool m_Fire = false;                    //飛翔トリガー

    protected override void Start()
    {
        base.Start();
        m_InitialPosition = transform.position;
        m_EffectPlayer = GameObject.Find("GameManager").GetComponent<EffectPlayer>();
        StartCoroutine(PlayEffect());
    }

    protected override void Update()
    {
        if (m_Fire)
        {
            m_EffectPlayer.PlayVanishCube(2, transform, 0.1f, AttractObject.Size.Small, 0.05f);
            transform.position += m_Direction * m_Speed * Time.deltaTime;
            if (ReachDistance()) Destroy(gameObject);
        }
    }

    //飛翔方向を設定
    public void SetDirection(Vector3 position)
    {
        m_Direction = Vector3.Normalize(transform.position - position);
        m_Direction = new Vector3(m_Direction.x, 0.0f, m_Direction.z);
        m_Fire = true;
    }

    //飛翔距離に到達したか？
    bool ReachDistance()
    {
        return m_Distance <= Vector3.Distance(m_InitialPosition, transform.position);
    }

    //キューブエフェクトを再生
    IEnumerator PlayEffect()
    {
        while (true)
        {
            m_EffectPlayer.PlayCube(2, transform, 0.1f, AttractObject.Size.Small, 0.05f);
            m_EffectPlayer.PlayCube(1, transform, 0.1f, AttractObject.Size.Regular, 0.05f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
