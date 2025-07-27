using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractCommander : MonoBehaviour
{
    List<AttractBoid> m_Boids = new List<AttractBoid>();    //引き付けているオブジェクト群

    [SerializeField] float m_InitialSpeed;          //初速
    [SerializeField] float m_NeighborDistance;      //近いと判定する距離
    [SerializeField] float m_MaxDistance;           //離れらる最大距離
    [SerializeField] float m_SeparationFactor;      //分離の重み
    [SerializeField] float m_SynchronizeFactor;     //整列の重み
    [SerializeField] float m_CohesionFactor;        //結合の重み

    public float INITIAL_SPEED { get { return m_InitialSpeed; } }
    public float NEIGHBOR_DISTANCE { get { return m_NeighborDistance; } }
    public float MAX_DISTANCE { get { return m_MaxDistance; } }
    public float SEPARATION_FACTOR { get { return m_SeparationFactor; } }
    public float SYNCHRONIZE_FACTOR { get { return m_SynchronizeFactor; } }
    public float COHESION_FACTOR { get { return m_CohesionFactor; } }

    void Start()
    {
        m_Boids.AddRange(GameObject.FindObjectsOfType<AttractBoid>());
        int a = 2;
    }

    void Update()
    {

    }

    //ボイドを追加する
    void JoinBoid(AttractBoid boid)
    {
        m_Boids.Add(boid);
    }

    //ボイドを取得する
    public List<AttractBoid> GetBoid()
    {
        return m_Boids;
    }
}
