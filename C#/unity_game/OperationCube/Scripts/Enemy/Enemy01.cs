using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    new enum ActionState
    {
        Wait,               //待機
        Emerge,             //出現
        Idle,               //停止
        Move,               //移動
        Attack01,           //攻撃1
        Attack02,           //攻撃2
        Attack03,           //攻撃3
        Attack04,           //攻撃4
        Regenerate,         //腕再生
        Damage,             //ダメージ
        Dead,               //死亡
    }
    enum EmergeState
    {
        GenerateBody,       //体を生成
        GenerateRightArm,   //右腕を生成
        GenerateLeftArm,    //左腕を生成
    }

    [SerializeField] EmergeState m_EmergeState;
    [SerializeField] ActionState m_State = ActionState.Wait;
    [SerializeField] ActionState m_NextAttackState = ActionState.Attack01;
    [SerializeField] ActionState m_PrevState = ActionState.Idle;


    public GameObject m_ArmObject;              //アームオブジェクト(ボーン1本につき1個、LargeCubeを使用)
    public GameObject m_CollideObject;          //攻撃の当たり判定に使用するオブジェクト
    public GameObject m_CollideObjectAttack01;  //攻撃の当たり判定に使用するオブジェクト
    public Transform[] m_RightArmBones;         //右腕ボーン
    public Transform[] m_LeftArmBones;          //右腕ボーン
    public List<GameObject> m_RightArmObjects = new List<GameObject>(); //右腕オブジェクト
    public List<GameObject> m_LeftArmObjects = new List<GameObject>();  //左腕オブジェクト

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        switch (m_State)
        {
            case ActionState.Wait: WaitUpdate(); break;
            case ActionState.Emerge: EmergeUpdate(); break;
            case ActionState.Idle: IdleUpdate(); break;
            case ActionState.Move: MoveUpdate(); break;
            case ActionState.Attack01: Attack01Update(); break;
            case ActionState.Attack02: Attack02Update(); break;
            case ActionState.Attack03: Attack03Update(); break;
            case ActionState.Attack04: Attack04Update(); break;
            case ActionState.Regenerate: RegenerateUpdate(); break;
            case ActionState.Damage: DamageUpdate(); break;
            case ActionState.Dead: DeadUpdate(); break;
        }
        //ステートタイマ更新
        m_StateTime += Time.deltaTime;
        //アニメーター情報更新
        m_AnimStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
    }

    /****************************** ActionState ******************************/
    //待機
    protected override void WaitUpdate()
    {
        if (IsPlayerInAttackRange(12.0f))
        {
            if (!m_StateTrigger)
            {
                m_Sound.PlaySE(9);
                m_StateTime = 0.0f;
                m_StateTrigger = true;
            }
        }
        if (m_StateTrigger && m_StateTime >= 3.0f)
            ChangeState(ActionState.Emerge);
    }

    protected override void EmergeUpdate()
    {
        switch (m_EmergeState)
        {
            case EmergeState.GenerateBody: GenerateBodyUpdate(); break;
            case EmergeState.GenerateRightArm: GenerateRightArmUpdate(); break;
            case EmergeState.GenerateLeftArm: GenerateLeftArmUpdate(); break;
        }
    }

    //停止
    protected override void IdleUpdate()
    {
        //3秒間停止(とりあえず)
        if (m_StateTime >= 3.0f) ChangeState(ActionState.Move);
    }

    //移動
    protected override void MoveUpdate()
    {
        //プレイヤーの方を向く
        LookAtToPlayer();
        //前方へ移動
        transform.position += transform.forward * Time.deltaTime * m_MoveSpeed;
        //次の攻撃範囲まで近づいたら
        if (IsPlayerInAttackRange(m_AttackRange[(int)m_NextAttackState - (int)ActionState.Attack01]) &&
            IsPlayerInAttackAngle())
        {
            //攻撃へ状態遷移
            ChangeState(m_NextAttackState);
            //次の攻撃をセット
            SetNextAttackState();
        }

        //一定時間移動し続けたら強制的に攻撃に移る
        if (m_StateTime >= 7.0f)
            ChangeState(ActionState.Attack03);
    }

    //攻撃1
    protected override void Attack01Update()
    {
        AttackCommon();
    }

    //攻撃2
    protected override void Attack02Update()
    {
        if (m_StateTime >= 0.05f && m_StateTrigger)
        {
            m_ProcessCount++;
            //右腕
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].tag = "EnemyAttackCollider";
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].GetComponent<BoxCollider>().enabled = false;
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().isKinematic = false;
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].transform.parent = m_FieldObjects;
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().velocity = SetDirection(m_ProcessCount * -(180.0f / m_RightArmObjects.Count) + 90.0f) * 50.0f;
            if (m_ProcessCount >= 2) m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount + 1].GetComponent<BoxCollider>().enabled = true;
            //左腕
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].tag = "EnemyAttackCollider";
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].GetComponent<BoxCollider>().enabled = false;
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().isKinematic = false;
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].transform.parent = m_FieldObjects;
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].GetComponent<Rigidbody>().velocity = SetDirection(m_ProcessCount * -(180.0f / m_LeftArmObjects.Count) - 90.0f) * 50.0f;
            if (m_ProcessCount >= 2) m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount + 1].GetComponent<BoxCollider>().enabled = true;
            //タイマをリセット
            m_StateTime = 0.0f;

            m_Sound.PlaySE(3);
        }
        if (m_ProcessCount == m_RightArmObjects.Count && m_StateTrigger == true)
        {
            //右腕
            m_RightArmObjects[m_RightArmObjects.Count - m_ProcessCount].GetComponent<BoxCollider>().enabled = true;
            m_RightArmObjects.Clear();
            //左腕
            m_LeftArmObjects[m_LeftArmObjects.Count - m_ProcessCount].GetComponent<BoxCollider>().enabled = true;
            m_LeftArmObjects.Clear();
            //トリガーとカウントをリセット
            m_StateTrigger = false;
            m_ProcessCount = 0;
        }

        if (IsAnimCorrect() && m_AnimStateInfo.normalizedTime >= 1.0f)
        {
            m_Animator.SetTrigger("Regenerate");
        }
    }

    //攻撃3
    void Attack03Update()
    {
        AttackCommon();
    }

    //攻撃4
    void Attack04Update()
    {
        AttackCommon();

    }

    //腕再生
    void RegenerateUpdate()
    {
        if (m_AnimStateInfo.IsName("Enemy01RegenerateArm_vmd"))
        {
            //状態遷移
            ChangeState(ActionState.Emerge);
        }
    }

    //ダメージ
    protected override void DamageUpdate()
    {
        if (m_StateTime >= 1.0f)
            ChangeState(ActionState.Move);
    }

    //死亡
    protected override void DeadUpdate()
    {

    }

    /****************************** EmergeState ******************************/
    //体を生成
    void GenerateBodyUpdate()
    {
        if (Generate(m_BodyBones, m_BodyObjects, 1.0f))
        {
            m_EmergeState = EmergeState.GenerateRightArm;
        }
    }

    //右腕を生成
    void GenerateRightArmUpdate()
    {
        //アームオブジェクトのキネマティックをオン、DetectionModeをDynamicに
        for (int i = 0; i < 7; i++)
        {
            m_RightArmObjects[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
            m_RightArmObjects[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        if (Generate(m_RightArmBones, m_RightArmObjects.ToArray(), 1.0f))
            m_EmergeState = EmergeState.GenerateLeftArm;
    }

    //左腕を生成
    void GenerateLeftArmUpdate()
    {
        //アームオブジェクトのキネマティックをオン、DetectionModeをDynamicに
        for (int i = 0; i < 7; i++)
        {
            m_LeftArmObjects[i].GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
            m_LeftArmObjects[i].GetComponent<Rigidbody>().isKinematic = true;
        }

        if (Generate(m_LeftArmBones, m_LeftArmObjects.ToArray(), 1.0f))
        {
            ChangeState(ActionState.Idle);
            m_EmergeState = EmergeState.GenerateRightArm;
        }
    }

    /****************************** Other ******************************/
    //状態遷移
    void ChangeState(ActionState state)
    {
        //ステートタイマをリセット
        m_StateTime = 0.0f;
        //ステートトリガーをリセット
        m_StateTrigger = false;
        //
        m_Animator.ResetTrigger(m_PrevState.ToString());
        //アニメーションをセット
        m_Animator.SetTrigger(state.ToString());
        //
        m_PrevState = m_State;
        //遷移
        m_State = state;
    }

    public override void ToDamage(int damage)
    {
        base.ToDamage(damage);
        Debug.Log("Enemy01は " + damage.ToString() + " の攻撃を受けた！ 残り体力は " + m_HP.ToString() + " です");
        m_Sound.PlaySE(1);

        //体力が0以下で死亡する
        if (m_HP <= 0)
        {
            Dead();
        }
        //状況によってノックバック
        else if (m_State == ActionState.Idle || m_State == ActionState.Move)
        {
            m_Animator.SetTrigger("Damage");
            ChangeState(ActionState.Damage);
        }
    }

    //現在のステートと同じモーションが再生されているか
    bool IsAnimCorrect()
    {
        return m_AnimStateInfo.IsName("Enemy01" + m_State.ToString() + "_vmd");
    }

    //攻撃に共通する処理
    void AttackCommon()
    {
        if (!m_StateTrigger)
            LookAtToPlayer();
        if (m_AnimStateInfo.normalizedTime >= 1.0f)
            ChangeState(ActionState.Move);
    }

    //次の攻撃ステートにランダムな攻撃ステートをセットする
    void SetNextAttackState()
    {
        m_NextAttackState = (ActionState)Random.Range(4, 8);
    }

    //Attack2用、正面から見て引数方向のベクトルを返す
    Vector3 SetDirection(float angle)
    {
        float x = 0.0f, z = 0.0f;
        //※弧度法
        x = transform.position.x + Mathf.Cos((transform.rotation.y + angle - transform.eulerAngles.y) * (Mathf.PI / 180.0f)) * 5.0f;
        z = transform.position.z + Mathf.Sin((transform.rotation.y + angle - transform.eulerAngles.y) * (Mathf.PI / 180.0f)) * 5.0f;
        //Yの値で下向きの角度を微調整する
        return (new Vector3(x, transform.position.y - 0.5f, z) - transform.position).normalized;
    }

    //地面とカメラを揺らす
    void ShakeTheGroundAndCamera(float magnitude, float duration, float scale, Vector3 position)
    {
        Vector3 offset = position - transform.position;
        //コライダーの位置を変更
        m_FootCollider.center = new Vector3(offset.x, m_FootCollider.center.y, offset.z);
        //コライダーのサイズを変更
        m_FootCollider.size = new Vector3(scale, m_FootCollider.size.y, scale);
        //足元のコライダーをオンにする
        StartCoroutine(OnFootCollider());
        //カメラを揺らす
        m_Camera.Shake(magnitude, duration, transform.position);
    }

    //足元のコライダーを一瞬オンにする
    IEnumerator OnFootCollider()
    {
        m_FootCollider.enabled = true;
        yield return null;
        m_FootCollider.enabled = false;
    }

    //腕オブジェクト生成
    void GenerateArmObject()
    {
        float r = 5.0f;
        float force = 7.0f;
        for (int i = 0; i < 7; i++)
        {
            //パーティクル生成
            m_RightArmObjects.Add(Instantiate(m_ArmObject, new Vector3(transform.position.x + Random.Range(-r, r), transform.position.y, transform.position.z + Random.Range(-r, r)), transform.rotation, m_FieldObjects));
            m_LeftArmObjects.Add(Instantiate(m_ArmObject, new Vector3(transform.position.x + Random.Range(-r, r), transform.position.y, transform.position.z + Random.Range(-r, r)), transform.rotation, m_FieldObjects));
            //ランダムな上方向に放出
            m_RightArmObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(-1.0f, 1.0f)) * force;
            m_LeftArmObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0.5f, 1.0f), Random.Range(-1.0f, 1.0f)) * force;
        }
    }

    //死亡時の処理
    void Dead()
    {
        m_Animator.SetTrigger("Dead");
        m_Sound.PlaySE(7);
        ChangeState(ActionState.Dead);

    }

    //足元の当たり判定
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "StageObj")
        {
            other.GetComponent<StageObject>().SetAnimation(transform.position + new Vector3(m_FootCollider.center.x, 0.0f, m_FootCollider.center.z), m_FootCollider.size.x);
        }
    }

    /****************************** モーションイベント ******************************/
    void Attack01StartEvent()
    {
        m_StateTrigger = true;
    }

    void Attack01HitEvent()
    {
        //カメラと地面を揺らす
        ShakeTheGroundAndCamera(0.2f, 0.2f, 5.0f, m_RightArmObjects[6].transform.position);
        //AttackColliderを射出
        GameObject col = Instantiate(m_CollideObjectAttack01,
            new Vector3(m_RightArmObjects[6].transform.position.x, 0.0f, m_RightArmObjects[6].transform.position.z),
            m_RightArmObjects[6].transform.rotation,
            transform);
        col.GetComponent<Enemy01Attack01Collider>().SetDirection(transform.position);
        col.GetComponent<Enemy01Attack01Collider>().Init(new Vector3(m_CollideScale[0], m_CollideScale[0], m_CollideScale[0]), m_AttackPower[0]);

        m_Sound.PlaySE(4);
    }

    void Attack02StartEvent()
    {
        //m_StateTrigger = true;
    }

    void Attack02HitEvent()
    {
        m_StateTrigger = true;
        m_StateTime = 0.0f;
    }

    void Attack03StartEvent()
    {
        m_Sound.PlaySE(6);
        m_StateTrigger = true;
        LeanTween.move(gameObject, new Vector3(m_Player.position.x, 0.0f, m_Player.position.z), 2.0f)
            .setEase(LeanTweenType.linear);
    }

    void Attack03HitEvent()
    {
        //カメラと地面を揺らす
        ShakeTheGroundAndCamera(0.5f, 0.3f, 15.0f, transform.position);
        //エフェクト再生
        m_EffectPlayer.PlaySystem(EffectPlayer.Effects.LandingShock, transform);
        m_EffectPlayer.PlayCube(10, transform, 2.0f, AttractObject.Size.Small, 7.0f);
        m_EffectPlayer.PlayVanishCube(7, transform, 2.0f, AttractObject.Size.Small, 7.0f);
        m_EffectPlayer.PlayVanishCube(5, transform, 2.0f, AttractObject.Size.Small, 7.0f, 2.0f);
        m_EffectPlayer.PlayCube(5, transform, 2.0f, AttractObject.Size.Regular, 7.0f);
        //コライダーを生成
        GameObject col = Instantiate(m_CollideObject, transform.position, transform.rotation);
        col.GetComponent<EnemyAttackCollider>().Init(new Vector3(m_CollideScale[2], 2.0f, m_CollideScale[2]), m_AttackPower[2], 0.3f);

        m_Sound.PlaySE(5);
    }

    void Attack04StartEvent()
    {
        m_StateTrigger = true;
    }

    void Attack04HitEvent()
    {
        //カメラと地面を揺らす
        ShakeTheGroundAndCamera(0.2f, 0.2f, 5.0f, m_RightArmObjects[6].transform.position);
        //エフェクト再生
        m_EffectPlayer.PlayCube(7, transform, 2.0f, AttractObject.Size.Small, 3.0f);
        m_EffectPlayer.PlayVanishCube(4, transform, 2.0f, AttractObject.Size.Small, 3.0f);
        //コライダーを生成
        GameObject col = Instantiate(m_CollideObject, m_RightArmObjects[6].transform.position, m_RightArmObjects[6].transform.rotation);
        col.GetComponent<EnemyAttackCollider>().Init(new Vector3(m_CollideScale[3], m_CollideScale[3], m_CollideScale[3]), m_AttackPower[3], 0.3f);

        m_Sound.PlaySE(2);
    }
    void RegenerateStartEvent()
    {
        m_Sound.PlaySE(6);
    }

    void RegenerateHitEvent()
    {
        //カメラを揺らす
        ShakeTheGroundAndCamera(0.5f, 0.3f, 15.0f, transform.position);
        //エフェクト再生
        m_EffectPlayer.PlaySystem(EffectPlayer.Effects.LandingShock, transform);
        m_EffectPlayer.PlayCube(10, transform, 2.0f, AttractObject.Size.Small, 7.0f);
        m_EffectPlayer.PlayVanishCube(7, transform, 2.0f, AttractObject.Size.Small, 7.0f);
        m_EffectPlayer.PlayCube(5, transform, 2.0f, AttractObject.Size.Regular, 7.0f);
        //コライダーを生成
        GameObject col = Instantiate(m_CollideObject, transform.position, transform.rotation);
        col.GetComponent<EnemyAttackCollider>().Init(new Vector3(m_CollideScale[2], 2.0f, m_CollideScale[2]), m_AttackPower[2], 0.3f);

        m_Sound.PlaySE(5);

        //腕用のキューブエフェクトを生成
        m_RightArmObjects = m_EffectPlayer.PlayCube(7, transform, 5.0f, AttractObject.Size.Large, 7.0f);
        m_LeftArmObjects = m_EffectPlayer.PlayCube(7, transform, 5.0f, AttractObject.Size.Large, 7.0f);

        //子にする
        for (int i = 0; i < 7; i++)
        {
            m_RightArmObjects[i].transform.parent = GameObject.Find("Enemy01Objects").transform;
            m_LeftArmObjects[i].transform.parent = GameObject.Find("Enemy01Objects").transform;
        }
        //ステートタイマをリセット
        m_StateTime = 0.0f;
        //ステートトリガーをリセット
        m_StateTrigger = false;
        m_State = ActionState.Regenerate;
    }

    void DeadFallEvent()
    {
        //ブロックを崩れさせる
        for (int i = 0; i < m_LeftArmObjects.Count; i++)
        {
            m_LeftArmObjects[i].GetComponent<Rigidbody>().isKinematic = false;
            m_LeftArmObjects[i].GetComponent<Rigidbody>().useGravity = true;
            m_RightArmObjects[i].GetComponent<Rigidbody>().isKinematic = false;
            m_RightArmObjects[i].GetComponent<Rigidbody>().useGravity = true;
        }
        foreach (GameObject body in m_BodyObjects)
        {
            body.GetComponent<Rigidbody>().isKinematic = false;
            body.GetComponent<Rigidbody>().useGravity = true;
        }

        m_Sound.PlaySE(8);

        StartCoroutine(LoadSelectScene(SceneMaker.SceneType.CreditScene));
    }


    void EnemyMoveEvent()
    {
        m_Sound.PlaySE(0);
    }
}
