using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    Player m_Player;

    void Start()
    {
        m_Player = transform.root.GetComponent<Player>();
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //Enemyの攻撃に衝突したら
        if (collision.gameObject.tag == "EnemyAttackCollider")
        {
            m_Player.SetHit(collision.transform.position, collision.gameObject.GetComponent<EnemyAttackCollider>().GetAttackPower());
            Debug.Log("Collision!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Enemyの攻撃に衝突したら
        if (other.gameObject.tag == "EnemyAttackCollider")
        {
            m_Player.SetHit(other.transform.position, other.gameObject.GetComponent<EnemyAttackCollider>().GetAttackPower());
            Debug.Log("Trigger!! プレイヤーは " + other.gameObject.GetComponent<EnemyAttackCollider>().GetAttackPower().ToString() + " のダメージをうけた");
        }
    }
}
