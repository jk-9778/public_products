using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildPoint : MonoBehaviour
{
    public enum Status
    {
        Empty,      //空
        Afford,     //余裕あり
        Full,       //満杯
    }
    [SerializeField]
    Status m_Status = Status.Empty;
    GrandChildPoint[] m_ChildPoints;      //孫の引き寄せポイント
    Player m_Player;

    void Start()
    {
        //子の引き寄せポイントを取得
        m_ChildPoints = GetComponentsInChildren<GrandChildPoint>();
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
        SetAllFull();
    }

    //Smallは子に引き寄せる
    public void ChildAttract(AttractObject obj)
    {
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            //ポイントがすでにオブジェクトを持っていたら次のポイントへ
            if (m_ChildPoints[i].GetStatus() == GrandChildPoint.Status.Full) continue;

            m_ChildPoints[i].Attract(obj);
            ChangeStatus(CheckAllChildAttract(GrandChildPoint.Status.Full) ? Status.Full : Status.Afford);
            break;
        }
    }

    //自分と全ての子の状態をフルに
    public void SetAllFull()
    {
        ChangeStatus(Status.Full);
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            m_ChildPoints[i].ChangeStatus(GrandChildPoint.Status.Full);
        }
    }

    //自分と全ての子の状態を空に
    public void SetAllEmpty()
    {
        ChangeStatus(Status.Empty);
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            m_ChildPoints[i].ChangeStatus(GrandChildPoint.Status.Empty);
        }
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

    //全ての子がオブジェクトを持っているか
    bool CheckAllChildAttract(GrandChildPoint.Status status)
    {
        int count = 0;
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            if (m_ChildPoints[i].GetStatus() == status)
                count++;
        }
        if (count == m_ChildPoints.Length) return true;
        else return false;
    }
}