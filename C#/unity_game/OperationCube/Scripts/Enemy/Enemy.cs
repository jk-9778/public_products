using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected enum ActionState
    {
        Wait,       //待機
        Emerge,     //出現
        Idle,       //停止
        Move,       //移動
        Attack01,   //攻撃1
        Attack02,   //攻撃2
        Damage,     //ダメージ
        Dead,       //死亡
    }
    [SerializeField] protected ActionState m_ActionState = ActionState.Wait;           //ステート
    [SerializeField] protected ActionState m_PreviousState = ActionState.Idle;


    [SerializeField] protected Transform[] m_BodyBones;     //体ボーン
    [SerializeField] protected GameObject[] m_BodyObjects;  //体オブジェクト

    float m_GenerateTimer = 0.0f;                           //生成タイマ
    float m_DelayTime = 0.3f;                               //生成遅延間隔
    protected int m_ProcessCount = 0;                       //生成処理カウント
    [SerializeField] protected float m_StateTime;           //ステート内経過時間
    protected bool m_StateTrigger = false;                  //ステート内用トリガー

    protected Transform m_Player;                           //プレイヤー
    protected Transform m_FieldObjects;                     //フィールドにあるオブジェクトを仕舞うオブジェクト
    protected BoxCollider m_FootCollider;                   //足元のコライダー(ステージの揺れ用)
    protected Animator m_Animator;                          //アニメーター
    protected AnimatorStateInfo m_AnimStateInfo;            //アニメーター情報
    protected CameraManager m_Camera;                       //メインカメラ
    protected EffectPlayer m_EffectPlayer;                  //エフェクトプレイヤー
    protected SceneMaker m_SceneMaker;                      //シーンメーカー
    protected SoundManager m_Sound;                         //サウンドマネージャー

    [SerializeField] protected float[] m_AttackRange;       //攻撃距離
    [SerializeField] protected int[] m_AttackPower;         //攻撃力
    [SerializeField] protected float[] m_CollideScale;      //攻撃判定範囲
    [SerializeField] protected float m_AttackAngle;         //攻撃角度(正面)
    [SerializeField] protected float m_MoveSpeed;           //移動速度(m/s)

    [SerializeField] protected int m_HP;                    //体力

    protected virtual void Start()
    {
        //各コンポーネント取得
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
        m_FieldObjects = GameObject.Find("FieldObjects").transform;
        m_FootCollider = GetComponent<BoxCollider>();
        m_FootCollider.enabled = false;
        m_Animator = GetComponent<Animator>();
        m_AnimStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
        m_EffectPlayer = GameObject.Find("GameManager").GetComponent<EffectPlayer>();
        m_SceneMaker = GameObject.Find("GameManager").GetComponent<SceneMaker>();
        m_Sound = GetComponent<SoundManager>();
    }

    protected virtual void Update()
    {
        switch (m_ActionState)
        {
            case ActionState.Wait: WaitUpdate(); break;
            case ActionState.Emerge: EmergeUpdate(); break;
            case ActionState.Idle: IdleUpdate(); break;
            case ActionState.Move: MoveUpdate(); break;
            case ActionState.Attack01: Attack01Update(); break;
            case ActionState.Attack02: Attack02Update(); break;
            case ActionState.Damage: DamageUpdate(); break;
            case ActionState.Dead: DeadUpdate(); break;
        }
        m_StateTime += Time.deltaTime;
    }

    //待機
    protected virtual void WaitUpdate()
    {
        //プレイヤーが近づいたら
        if (IsPlayerInAttackRange(m_AttackRange[0])) ChangeState(ActionState.Emerge);
    }

    //出現
    protected virtual void EmergeUpdate()
    {

    }

    //停止
    protected virtual void IdleUpdate()
    {

    }

    //移動
    protected virtual void MoveUpdate()
    {

    }

    //攻撃1
    protected virtual void Attack01Update()
    {

    }

    //攻撃2
    protected virtual void Attack02Update()
    {

    }

    //ダメージ
    protected virtual void DamageUpdate()
    {

    }

    //死亡
    protected virtual void DeadUpdate()
    {
        
    }

    //状態遷移
    protected void ChangeState(ActionState state)
    {
        m_StateTime = 0.0f;
        //ステートトリガーをリセット
        m_StateTrigger = false;
        //前のステートのトリガーをリセット
        m_Animator.ResetTrigger(m_PreviousState.ToString());
        //アニメーションをセット
        m_Animator.SetTrigger(state.ToString());
        //現在のステートを前のステートに登録
        m_PreviousState = m_ActionState;
        m_ActionState = state;
    }

    //生成
    protected bool Generate(Transform[] bones, GameObject[] objs, float time)
    {
        //タイマを更新
        m_GenerateTimer += Time.deltaTime;
        //一定間隔ごとに処理
        if (m_GenerateTimer >= m_DelayTime)
        {
            //オブジェクトをボーンの位置まで移動
            LeanTween.move(objs[m_ProcessCount], bones[m_ProcessCount], time)
                .setEase(LeanTweenType.easeInOutCubic);
                //.setOnComplete(() =>
                //{
                //});
            m_GenerateTimer = 0.0f;
            //オブジェクトをボーンの子にする
            objs[m_ProcessCount].transform.parent = bones[m_ProcessCount].transform;
            //処理カウントを更新、ボーンの数だけ処理する
            m_ProcessCount++;

            m_Sound.PlaySE(10);
        }
        if (m_ProcessCount == bones.Length)
        {
            m_ProcessCount = 0;
            return true;
        }
        else return false;
    }

    //プレイヤーが攻撃範囲内にいるか？
    protected bool IsPlayerInAttackRange(float range)
    {
        //自分からプレイヤーまでの距離
        float distansToPlayer = Vector3.Distance(m_Player.position, transform.position);
        //プレイヤーが見える距離内にいるかどうかを返却する
        return (distansToPlayer <= range);
    }

    //プレイヤーが攻撃視野角内にいるか？
    protected bool IsPlayerInAttackAngle()
    {
        //自分からプレイヤーへの方向ベクトル（ワールド座標系）
        Vector3 directionToPlayer = m_Player.position - transform.position;
        //自分の正面向きベクトルとプレイヤーへのベクトルの差分角度
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        //見える視野角の範囲内にプレイヤーがいるかどうかを返却する
        return (Mathf.Abs(angleToPlayer) <= m_AttackAngle);
    }

    //プレイヤーの方向を向く
    protected void LookAtToPlayer()
    {
        //自分からプレイヤーへの方向ベクトル（X-Z平面）
        Vector3 directionToPlayer = Vector3.Scale((m_Player.position - transform.position), new Vector3(1.0f, 0.0f, 1.0f)).normalized;
        //現在の向きと移動方向の向きを混ぜる
        Vector3 dir = Vector3.Slerp(transform.forward, directionToPlayer, 0.05f);
        //混ぜた方向を向く
        transform.LookAt(transform.position + dir);
    }

    //攻撃を受ける
    public virtual void ToDamage(int damage)
    {
        m_HP -= damage;
    }

    //指定時間後に選択したシーンへ遷移する
    protected IEnumerator LoadSelectScene(SceneMaker.SceneType type)
    {
        m_SceneMaker.ChangeNextScene(type);
        yield return new WaitForSeconds(4.0f);
        m_SceneMaker.LoadScene();
    }
}
