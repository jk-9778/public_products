using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandChildPoint : MonoBehaviour
{
    public enum Status
    {
        Empty,
        Full,
    }
    [SerializeField]
    Status m_Status = Status.Empty;
    Player m_Player;

    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {

    }

    public void Attract(AttractObject obj)
    {
        //オブジェクトの状態を変更し、追従対象を設定
        obj.ChangeState(AttractObject.State.Following);
        obj.tag = "AttractObj";
        obj.SetTarget(transform);
        //引き寄せ済みオブジェクトとに格納
        if (m_Player.m_ActionState == Player.ActionState.Attract)
            m_Player.AddAttractingObject(obj);
        //DetectionModeをDynamic、キネマティックをオンに、当たり判定をオフに
        obj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.GetComponent<BoxCollider>().enabled = false;
        //引き寄せたポイントを引き寄せ済みに変更
        ChangeStatus(Status.Full);
    }

    //ステータスを取得
    public Status GetStatus()
    {
        return m_Status;
    }

    //ステータスを変更
    public void ChangeStatus(Status status)
    {
        m_Status = status;
    }
}
