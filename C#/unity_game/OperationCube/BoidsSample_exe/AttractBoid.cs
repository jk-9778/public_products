using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractBoid : MonoBehaviour
{
    AttractCommander m_Commander;           //指揮官
    Vector3 m_Velocity = Vector3.one;       //移動量
    Vector3 m_Acceleration = Vector3.zero;  //加速度

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }
    public Vector3 Velocity
    {
        get
        {
            return m_Velocity;
        }
        set
        {
            m_Velocity = value;
            transform.forward = m_Velocity.normalized;
        }
    }
    void Start()
    {
        m_Commander = GameObject.FindObjectOfType<AttractCommander>();
        Velocity *= m_Commander.INITIAL_SPEED;
    }

    void Update()
    {
        // dtと前回の加速度から位置差分・速度を計算  
        float dt = Time.deltaTime;
        Vector3 dPos = Velocity * dt + 0.5f * m_Acceleration * dt * dt;  // d = v0*t + 1/2*a*t^2  
        Velocity = Velocity + m_Acceleration * dt;    // v = v0 + a*t  
        // 速度はBoidMinV以上BoidMaxV以下でなければならない  
        float clamped = Mathf.Clamp(Velocity.magnitude, 0.5f, 2.0f);
        Velocity = Velocity / Velocity.magnitude * clamped;
        Position = Position + dPos;

        // 加速度更新  
        m_Acceleration = (m_Commander.SEPARATION_FACTOR * Separation() +
            m_Commander.SYNCHRONIZE_FACTOR * Synchronize() +
            m_Commander.COHESION_FACTOR * Cohesion()) /
            (m_Commander.SEPARATION_FACTOR + m_Commander.SYNCHRONIZE_FACTOR + m_Commander.COHESION_FACTOR);
    }

    //近隣のボイドを取得する
    List<AttractBoid> GetNeighbors()
    {
        //他のボイドを取得
        List<AttractBoid> boids = m_Commander.GetBoid();
        List<AttractBoid> neighbor = new List<AttractBoid>();
        foreach (AttractBoid boid in boids)
        {
            //近くにいるボイドをリストに追加していく
            if (boid == this)
                continue;
            if (Vector3.Distance(Position, boid.Position) <= m_Commander.NEIGHBOR_DISTANCE)
                neighbor.Add(boid);
        }
        //指揮官から離れすぎそうな場合に、指揮官と逆方向の最大距離の位置に仮想ボイドを設置する
        if (Vector2.Distance(Position, m_Commander.transform.position) <= m_Commander.MAX_DISTANCE - m_Commander.NEIGHBOR_DISTANCE)
        {
            //neighbor.Add(new AttractBoid());
            //neighbor[neighbor.Count - 1].transform.position = m_Commander.transform.position + (transform.position - m_Commander.transform.position).normalized * m_Commander.MAX_DISTANCE;
        }
        return neighbor;
    }

    //他のボイドとぶつからないように避ける
    Vector3 Separation()
    {
        List<AttractBoid> neighbors = GetNeighbors();

        //近隣のボイドが居なければゼロを返す
        if (neighbors.Count == 0)
            return Vector3.zero;

        Vector3 velocity = Vector3.zero;
        //遠いときはゆっくり、近いときは急に
        foreach (AttractBoid neighbor in neighbors)
        {
            Vector3 diff = neighbor.Position - Position;
            velocity += -1 * diff.normalized * 10.0f / (diff.magnitude * diff.magnitude);
        }
        //近隣のボイド全体で平均する
        return velocity / neighbors.Count;
    }

    //移動速度を近隣のボイドと合わせようとする
    Vector3 Synchronize()
    {
        List<AttractBoid> neighbors = GetNeighbors();

        //近隣のボイドが居なければゼロを返す
        if (neighbors.Count == 0)
            return Vector3.zero;

        //近隣のボイドの移動量を平均して自分の移動量との差分を返す
        Vector3 velocity = Vector3.zero;
        foreach (AttractBoid neighbor in neighbors)
        {
            velocity += neighbor.Velocity;
        }
        Vector3 average = velocity / neighbors.Count;
        return average - Velocity;
    }

    //指揮官の近くに居ようとする
    private Vector3 Cohesion()
    {
        return m_Commander.transform.position - Position;
    }
}
