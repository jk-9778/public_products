using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractPoint : MonoBehaviour
{
    public enum Status
    {
        Empty,
        Afford,
        Full,
    }
    [SerializeField]
    Status m_Status = Status.Empty;
    ChildPoint[] m_ChildPoints;     //子の引き寄せポイント
    Player m_Player;                //プレイヤー

    // Use this for initialization
    void Start()
    {
        //子の引き寄せポイントを取得
        m_ChildPoints = GetComponentsInChildren<ChildPoint>();
        //プレイヤーを取得
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //オブジェクトを自分に引き寄せる
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

    //RegularとSmallは子へ
    public void ChildAttract(AttractObject obj)
    {
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            //ポイントがすでにオブジェクトを持っていたら次のポイントへ
            if (m_ChildPoints[i].GetStatus() == ChildPoint.Status.Full) continue;

            switch (obj.GetSize())
            {
                case AttractObject.Size.Regular:
                    if (m_ChildPoints[i].GetStatus() == ChildPoint.Status.Afford) continue;
                    m_ChildPoints[i].Attract(obj);
                    break;
                case AttractObject.Size.Small:
                    m_ChildPoints[i].ChildAttract(obj);
                    break;
            }
            ChangeStatus(CheckAllChildAttract(ChildPoint.Status.Full) ? Status.Full : Status.Afford);
            break;
        }
    }

    //自分と全ての子の状態をフルに
    public void SetAllFull()
    {
        ChangeStatus(Status.Full);
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            m_ChildPoints[i].GetComponent<ChildPoint>().SetAllFull();
        }
    }

    //自分と全ての子の状態を空に
    public void SetAllEmpty()
    {
        ChangeStatus(Status.Empty);
        for (int i = 0; i < m_ChildPoints.Length; i++)
        {
            m_ChildPoints[i].SetAllEmpty();
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
    public bool CheckAllChildAttract(ChildPoint.Status status)
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
