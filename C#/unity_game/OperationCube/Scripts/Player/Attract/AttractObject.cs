using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractObject : MonoBehaviour
{
    public enum State
    {
        Normal,
        Following,
    }
    public enum Size
    {
        Small,
        Regular,
        Large,
        EffectMini,
    }
    [SerializeField]
    State m_State = State.Normal;                       //状態
    [SerializeField]
    Size m_Size;                                        //サイズ
    Player m_Player;                                    //プレイヤー
    Transform m_PlayerEyePoint;                         //プレイヤーの目の位置
    Transform m_Camera;                                 //カメラ
    bool m_Seen = false;                                //カメラに映っているか？
    float m_ElapseTimeForLastAction = 0.0f;             //最後に触れられてからの時間
    static readonly float m_DESTROY_TIME = 50.0f;       //消えるまでの時間

    [HideInInspector]
    public bool m_Attracting = false;                   //引き寄せ可能か？

    [HideInInspector]
    Transform m_Target = null;                          //追従対象
    float m_Speed = 5.0f;                               //追従速度

    public Material[] m_Materials = new Material[2];    //マテリアル

    EffectPlayer m_EffectPlayer;
    SoundManager m_Sound;

    // Use this for initialization
    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_PlayerEyePoint = m_Player.transform.Find("EyePoint");
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        m_EffectPlayer = GameObject.Find("GameManager").GetComponent<EffectPlayer>();
        m_Sound = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case State.Normal: Normal(); break;
            case State.Following: Following(); break;
            default: break;
        }
    }

    //地面に落ちている状態
    void Normal()
    {
        if (tag == "AttractObj" && transform.root.tag != "Enemy")
            m_ElapseTimeForLastAction += Time.deltaTime;
        else
            m_ElapseTimeForLastAction = 0.0f;

        if (m_ElapseTimeForLastAction >= m_DESTROY_TIME)
        {
            StartCoroutine(DestroyCoroutine());
            return;
        }


        //自身がエフェクト用キューブの場合は何もしない
        if (tag == "EffectCube") return;

        //親がエネミーか？
        if (transform.root.tag == "Enemy")
        {
            m_Attracting = false;
        }
        //カメラに映っているか？
        else if (m_Seen)
        {
            //プレイヤーの照準内にいるか？
            if (OneselfInViewingDistans() && OneselfInViewingAngle())
            {
                m_Attracting = true;
            }
            else m_Attracting = false;
        }
        else m_Attracting = false;

        //プレイヤーの照準範囲内にいるときだけタグを変更
        if (m_Attracting && tag != "PlayerAmmo")
        {
            gameObject.tag = "AttractingObj";
            GetComponent<Renderer>().material = m_Materials[1];
        }
        else if (tag != "PlayerAmmo")
        {
            gameObject.tag = "AttractObj";
            GetComponent<Renderer>().material = m_Materials[0];
        }
    }

    //プレイヤーに追従する
    void Following()
    {
        if (m_Target != null)
        {
            if (m_Target.tag == "GuardPoint")
                m_Speed = 10.0f;
            else
                m_Speed = 5.0f;
            //現在の向きとターゲットへの座標を混ぜる
            Vector3 dir = Vector3.Slerp(transform.position, m_Target.position, m_Speed * Time.deltaTime);
            //補間位置へ追従
            transform.position = dir;
        }
    }

    private void OnWillRenderObject()
    {
        //カメラに映った時だけ有効に
        if (Camera.current.tag == "MainCamera")
            m_Seen = true;
        else
            m_Seen = false;
    }

    //プレイヤーから見える距離内にいるか？
    bool OneselfInViewingDistans()
    {
        //自分からプレイヤーまでの距離
        float distansToPlayer = Vector3.Distance(m_PlayerEyePoint.position, transform.position);

        //プレイヤーが見える距離内にいるかどうかを返却する
        return (distansToPlayer <= m_Player.m_AttractRange);
    }

    //プレイヤーの視野角内にいるか？
    bool OneselfInViewingAngle()
    {
        //プレイヤーから自分への方向ベクトル（ワールド座標系）
        Vector3 directionToPlayer = transform.position - m_PlayerEyePoint.position;

        //プレイヤーの正面向きベクトルと自分へのベクトルの差分角度
        float angleToPlayer = Vector3.Angle(m_Camera.forward, directionToPlayer);

        //プレイヤーの視野角の範囲内に自分がいるかどうかを返却する
        return (Mathf.Abs(angleToPlayer) <= m_Player.m_AttractAngle);
    }

    //追従対象を設定
    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    //状態変更
    public void ChangeState(State state)
    {
        m_State = state;
    }

    //サイズを取得
    public Size GetSize()
    {
        return m_Size;
    }

    IEnumerator DestroyCoroutine()
    {
        tag = "EffectCube";
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    void BreakAndPlayEffect()
    {
        switch (m_Size)
        {
            case Size.Small:
                m_EffectPlayer.PlayVanishCube(4, transform, 0.25f, Size.EffectMini, 5.0f, 3.0f);
                break;
            case Size.Regular:
                m_EffectPlayer.PlayCube(3, transform, 0.5f, Size.Small, 5.0f);
                m_EffectPlayer.PlayVanishCube(4, transform, 0.25f, Size.EffectMini, 3.0f, 3.0f);
                break;
            case Size.Large:
                m_EffectPlayer.PlayCube(3, transform, 0.5f, Size.Regular, 5.0f);
                m_EffectPlayer.PlayVanishCube(4, transform, 0.25f, Size.Small, 3.0f, 3.0f);
                break;
        }
    }

    //当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        if (tag == "PlayerAmmo" && collision.gameObject.tag != "PlayerAmmo" && collision.gameObject.tag != "EnemyBody")
        {
            //tag = "AttractObj";
            //m_Sound.PlaySE(0);
            BreakAndPlayEffect();
            Destroy(gameObject);
        }
        if (tag == "EnemyAttackCollider" && collision.gameObject.tag != "EnemyAttackCollider" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "AttractObj" && collision.gameObject.tag != "AttractingObj")
        {
            //tag = "AttractObj";
            //m_Sound.PlaySE(0);
            BreakAndPlayEffect();
            Destroy(gameObject);
        }
    }
}
