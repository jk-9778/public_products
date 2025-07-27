using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Idle,
        Walk,
        Run,
        Fall,
        Damage,
        Down,
        Dead,
    }

    public enum ActionState
    {
        None,
        Attract,
        Shoot,
        Guard,
    }
    public MoveState m_MoveState = MoveState.Idle;          //現在のステート
    public MoveState m_PreviousState = MoveState.Idle;      //一つ前のステート
    public ActionState m_ActionState = ActionState.None;    //アクションステート
    Transform m_Camera;                         //カメラ
    Animator m_Animator;                        //アニメーター
    CharacterController m_CC;                   //キャラクターコントローラー
    protected InputChecker m_Input;                       //入力チェッカー
    SceneMaker m_SceneMaker;                    //シーンメーカー
    SoundManager m_Sound;                       //サウンドマネージャー

    public int m_HP;                            //体力
    public Image m_HPGageImage;                 //体力ゲージに使う画像
    HitPointGage m_HPGage;                      //体力ゲージ
    public float m_WalkSpeed;                   //歩く速度
    public float m_RunSpeed;                    //走る速度
    public float m_TrunSpeed;                   //回転速度
    public float m_JumpPower;                   //ジャンプ力
    public float m_Gravity;                     //重力
    public float m_ShootForce;                  //発射力
    float m_VelocityY;                          //垂直方向の移動量
    Vector3 m_Velocity;                         //移動量

    AttractPoint[] m_AttractPoints;             //引き寄せる位置
    List<AttractObject> m_AttractObjects = new List<AttractObject>();               //照準内にある引き寄せられるオブジェクト
    List<LargeGuardPoint> m_LargeGuardPoints = new List<LargeGuardPoint>();         //防御時の大オブジェクトを引き寄せる位置
    List<RegularGuardPoint> m_RegularGuardPoints = new List<RegularGuardPoint>();   //防御時の中オブジェクトを引き寄せる位置
    List<SmallGuardPoint> m_SmallGuardPoints = new List<SmallGuardPoint>();         //防御時の小オブジェクトを引き寄せる位置

    List<AttractObject> m_AttractingObjects = new List<AttractObject>();            //引き寄せ中のオブジェクト

    public float m_AttractAngle;                //照準の広さ
    public float m_AttractRange;                //引き寄せられる距離

    // Use this for initialization
    void Start()
    {
        //各種コンポーネントとオブジェクトを取得
        m_Animator = GetComponent<Animator>();
        m_CC = GetComponent<CharacterController>();
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        m_AttractPoints = GetComponentsInChildren<AttractPoint>();
        m_Input = GameObject.Find("GameManager").GetComponent<InputChecker>();
        m_SceneMaker = GameObject.Find("GameManager").GetComponent<SceneMaker>();
        m_Sound = GetComponent<SoundManager>();
        m_HPGage = new HitPointGage(m_HPGageImage, m_HP);
        //ガ―ドポイントを取得
        for (int i = 0; i < 18; i++)
        {
            m_SmallGuardPoints.Add(GameObject.Find("SGP" + i.ToString()).GetComponent<SmallGuardPoint>());
            if (i >= 6) continue;
            m_RegularGuardPoints.Add(GameObject.Find("RGP" + i.ToString()).GetComponent<RegularGuardPoint>());
            if (i >= 2) continue;
            m_LargeGuardPoints.Add(GameObject.Find("LGP" + i.ToString()).GetComponent<LargeGuardPoint>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_MoveState)
        {
            case MoveState.Idle: IdleUpdate(); break;
            case MoveState.Walk: WalkUpdate(); break;
            case MoveState.Run: RunUpdate(); break;
            case MoveState.Fall: FallUpdate(); break;
            case MoveState.Damage: DamageUpdate(); break;
            case MoveState.Down: DownUpdate(); break;
            case MoveState.Dead: DeadUpdate(); break;
            default: break;
        }

        switch (m_ActionState)
        {
            case ActionState.None: NoneUpdate(); break;
            case ActionState.Attract: AttractUpdate(); break;
            case ActionState.Shoot: ShootUpdate(); break;
            case ActionState.Guard: GuardUpdate(); break;
            default: break;
        }

        //ズーム
        if (m_Input.GetButtonDown(ButtonCode.LB))
            m_Camera.GetComponent<CameraManager>().ZoomIn();
        else if (m_Input.GetButtonUp(ButtonCode.LB))
            m_Camera.GetComponent<CameraManager>().ZoomOut();

        //状態遷移
        if (m_MoveState == MoveState.Run) m_Animator.SetBool("Run", true);
        else m_Animator.SetBool("Run", false);

        //移動量を足す
        m_CC.Move(m_Velocity * Time.deltaTime);

        //足場がなくなったら落下する
        if (m_MoveState != MoveState.Fall &&
            m_MoveState != MoveState.Damage &&
            m_MoveState != MoveState.Down &&
            !IsGround() && !m_CC.isGrounded)
        {
            if (DistanceToGround() > 0.4f)
                m_Animator.SetTrigger("Fall");
            ChangeState(MoveState.Fall);
        }
    }

    //待機状態
    void IdleUpdate()
    {
        //アニメーターに移動速度のパラメータを渡す
        m_Animator.SetFloat("Speed", 0.0f);

        //移動入力がされたら
        //ガード中は移動できない
        if (m_Input.GetLeftStick() && m_ActionState == ActionState.None) ChangeState(MoveState.Walk);

        //ジャンプ
        if (m_Input.GetButtonDown(ButtonCode.A) && m_ActionState == ActionState.None) Jumping();
    }

    //歩き状態
    void WalkUpdate()
    {
        //移動処理
        Move(m_WalkSpeed);

        //移動入力が途切れたら
        if (!m_Input.GetLeftStick()) ChangeState(MoveState.Idle);
        //走るボタンが押されていて、かつスティックが倒されていたら
        else if (m_Input.GetButtonDown(ButtonCode.LS) && m_Input.GetLeftStick()) ChangeState(MoveState.Run);

        //ジャンプ
        if (m_Input.GetButtonDown(ButtonCode.A) && m_ActionState == ActionState.None) Jumping();
    }

    //走り状態
    void RunUpdate()
    {
        //移動処理
        Move(m_RunSpeed);

        //移動入力が途切れたら
        if (!m_Input.GetLeftStick()) ChangeState(MoveState.Idle);

        //ジャンプ
        if (m_Input.GetButtonDown(ButtonCode.A) && m_ActionState == ActionState.None) Jumping();
    }

    //落下状態
    void FallUpdate()
    {
        m_VelocityY -= m_Gravity * Time.deltaTime;

        //y軸方向の移動量を加味する
        m_Velocity.y = m_VelocityY;

        if (m_VelocityY < 0.0f && m_CC.isGrounded)
        {
            m_Animator.SetTrigger("Land");
            m_VelocityY = 0.0f;
            m_Velocity = Vector3.zero;
            //ジャンプする直前の状態に戻る
            ChangeState(m_PreviousState);
        }

        //ジャンプ中にわずかに移動できる
        Move(m_WalkSpeed * 0.5f);
    }

    //ダメージ状態
    void DamageUpdate()
    {
        //落下
        m_VelocityY -= m_Gravity * 0.5f * Time.deltaTime;
        //y軸方向の移動量を加味する
        m_Velocity.y = m_VelocityY;
        //接地したら落下停止
        if (m_VelocityY <= 0.0f && m_CC.isGrounded) m_VelocityY = 0.0f;
    }

    //転倒状態
    void DownUpdate()
    {
        //落下
        m_VelocityY -= m_Gravity * 0.5f * Time.deltaTime;
        //y軸方向の移動量を加味する
        m_Velocity.y = m_VelocityY;
        //接地したら落下停止
        if (m_VelocityY <= 0.0f && m_CC.isGrounded)
        {
            m_VelocityY = 0.0f;

            if (m_HP <= 0)
            {
                //HPが0以下なら起き上がらず死亡状態へ遷移
                ChangeState(MoveState.Dead);
                StartCoroutine(LoadSelectScene(SceneMaker.SceneType.GameOverScene));
            }
            else
                //そうでなければ起き上がる
                m_Animator.SetTrigger("StandUp");
        }
    }

    //死亡状態
    void DeadUpdate()
    {

    }

    //何もアクションをしていない
    void NoneUpdate()
    {
        //行動不能の時は操作を受け付けない
        if (!IsActionable()) return;

        //LBで引き寄せ
        if (m_Input.GetRightTrigger())
        {
            m_Animator.SetTrigger("Attract");
            m_ActionState = ActionState.Attract;
            m_Animator.ResetTrigger("AttractEnd");
            ChangeState(MoveState.Idle);
        }

        //RBで放射
        if (m_Input.GetButtonDown(ButtonCode.RB))
        {
            m_Animator.SetTrigger("Shoot");
            m_ActionState = ActionState.Shoot;
            ChangeState(MoveState.Idle);
        }

        //Bでガード
        if (m_Input.GetLeftTrigger())
        {
            GatheringObject();
            m_Animator.SetBool("Guard", true);
            m_ActionState = ActionState.Guard;
            ChangeState(MoveState.Idle);
        }
    }

    //引き寄せ状態
    void AttractUpdate()
    {
        //正面方向を向く
        LookAtCameraForword();

        //ボタンが離されたら引き寄せる
        if (!m_Input.GetRightTrigger())
        {
            m_Animator.SetTrigger("AttractEnd");
            m_Camera.GetComponent<CameraManager>().ZoomOut();
        }
    }

    //放射状態
    void ShootUpdate()
    {
        LookAtCameraForword();
    }

    //ガード状態
    virtual protected void GuardUpdate()
    {
        LookAtCameraForword();
        if (!m_Input.GetLeftTrigger())
        {
            RevertingObject();
            m_Animator.SetBool("Guard", false);
            m_ActionState = ActionState.None;
        }
    }

    //状態遷移
    void ChangeState(MoveState state)
    {
        //落下中は移動量をそのままに
        if (state == MoveState.Idle && m_MoveState != MoveState.Fall && m_MoveState != MoveState.Damage) m_Velocity = Vector3.zero;
        //直前の状態を登録
        m_PreviousState = m_MoveState;
        m_MoveState = state;
    }

    //移動
    void Move(float speed)
    {
        //カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;

        //方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * m_Input.GetValueLV() + m_Camera.transform.right * m_Input.GetValueLH();

        //移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        switch (m_MoveState)
        {
            case MoveState.Walk:
                m_Velocity = moveForward * speed;
                //アニメーターに移動速度のパラメータを渡す
                m_Animator.SetFloat("Speed", (moveForward * speed).magnitude);
                break;
            case MoveState.Run:
                m_Velocity = moveForward.normalized * speed;
                //アニメーターに移動速度のパラメータを渡す
                m_Animator.SetFloat("Speed", (moveForward.normalized * speed).magnitude);
                break;
            case MoveState.Fall:
                if (m_Velocity.magnitude <= m_WalkSpeed * 0.3f)
                    m_Velocity += moveForward.normalized * speed;
                break;
            default: m_Velocity = Vector3.zero; break;
        }

        //キャラクターの向きを進行方向に
        if (moveForward != Vector3.zero)
        {
            //現在の向きと移動方向の向きを混ぜる
            Vector3 dir = Vector3.Slerp(transform.forward, new Vector3(moveForward.x, 0.0f, moveForward.z), Time.deltaTime * m_TrunSpeed);
            //混ぜた方向を向く
            transform.LookAt(transform.position + dir);
        }
    }

    //ジャンプする
    void Jumping()
    {
        //接地状態をリセット
        m_Animator.ResetTrigger("Land");

        m_Animator.SetTrigger("Jump");
        m_VelocityY = m_JumpPower;
        ChangeState(MoveState.Fall);
    }

    //照準内にあるオブジェクトを引き寄せる
    void AttractingObject()
    {
        //照準内にあるオブジェクトを取得
        m_AttractObjects.Clear();
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("AttractingObj").Length; i++)
            m_AttractObjects.Add(GameObject.FindGameObjectsWithTag("AttractingObj")[i].GetComponent<AttractObject>());

        //オブジェクトを大きい順にソート
        m_AttractObjects.Sort((a, b) => b.GetComponent<AttractObject>().GetSize() - a.GetComponent<AttractObject>().GetSize());

        //引き寄せ位置一つごとに処理
        for (int i = 0; i < m_AttractObjects.Count; i++)
        {
            for (int j = 0; j < m_AttractPoints.Length; j++)
            {
                //ポイントがすでにオブジェクトを持っていたら次のポイントへ
                if (m_AttractPoints[j].GetStatus() == AttractPoint.Status.Full) continue;

                switch (m_AttractObjects[i].GetSize())
                {
                    case AttractObject.Size.Large:
                        if (m_AttractPoints[j].GetStatus() == AttractPoint.Status.Afford) continue;
                        m_AttractPoints[j].Attract(m_AttractObjects[i]);
                        break;
                    case AttractObject.Size.Regular:
                        if (m_AttractPoints[j].CheckAllChildAttract(ChildPoint.Status.Afford)) continue;
                        m_AttractPoints[j].ChildAttract(m_AttractObjects[i]);
                        break;
                    case AttractObject.Size.Small:
                        m_AttractPoints[j].ChildAttract(m_AttractObjects[i]);
                        break;
                }
                break;
            }
        }
    }

    //持っているオブジェクトを発射する
    void ShootingObject()
    {
        //カメラの正面方向を向く
        LookAtCameraForword();
        //引き寄せ済みオブジェクトを全て発射する
        foreach (AttractObject obj in m_AttractingObjects)
        {
            //当たり判定をオン、キネマティックをオフ、DetectionModeをDynamicに
            obj.GetComponent<BoxCollider>().enabled = true;
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //発射するオブジェクトの状態をノーマルに変更
            obj.GetComponent<AttractObject>().ChangeState(AttractObject.State.Normal);
            //追従対象を解除
            obj.GetComponent<AttractObject>().SetTarget(null);
            //カメラの向いている方向に発射する(そのままだと少し低いためY軸に補正をかける)
            obj.GetComponent<Rigidbody>().velocity = (m_Camera.forward + new Vector3(0.0f, 0.08f, 0.0f)) * m_ShootForce;
            //タグをプレイヤーの攻撃物に変更しエネミーとの当たり判定に使う
            obj.tag = "PlayerAmmo";
        }
        //引き寄せ済みオブジェクトから削除
        m_AttractingObjects.Clear();
        //引き寄せポイントを空にする
        foreach (AttractPoint point in m_AttractPoints)
        {
            point.SetAllEmpty();
        }
    }

    /////////////テスト///////////////////////////
    IEnumerator ShootCoroutine()
    {
        //カメラの正面方向を向く
        LookAtCameraForword();
        //引き寄せ済みオブジェクトを全て発射する
        foreach (AttractObject obj in m_AttractingObjects)
        {
            yield return new WaitForSeconds(0.1f);
            //当たり判定をオン、キネマティックをオフ、DetectionModeをDynamicに
            obj.GetComponent<BoxCollider>().enabled = true;
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //発射するオブジェクトの状態をノーマルに変更
            obj.GetComponent<AttractObject>().ChangeState(AttractObject.State.Normal);
            //追従対象を解除
            obj.GetComponent<AttractObject>().SetTarget(null);
            //カメラの向いている方向に発射する(そのままだと少し低いためY軸に補正をかける)
            obj.GetComponent<Rigidbody>().velocity = (m_Camera.forward + new Vector3(0.0f, 0.08f, 0.0f)) * m_ShootForce;
            //タグをプレイヤーの攻撃物に変更しエネミーとの当たり判定に使う
            obj.tag = "PlayerAmmo";
            //SE再生
            m_Sound.PlaySE(3);
        }
        //引き寄せ済みオブジェクトから削除
        m_AttractingObjects.Clear();
        //引き寄せポイントを空にする
        foreach (AttractPoint point in m_AttractPoints)
        {
            point.SetAllEmpty();
        }
    }

    //オブジェクトを受け取り、引き寄せ済みオブジェクトに格納する
    public void AddAttractingObject(AttractObject obj)
    {
        m_AttractingObjects.Add(obj);
    }

    //オブジェクトを前方に集める
    void GatheringObject()
    {
        //オブジェクトを大きい順にソート
        m_AttractingObjects.Sort((a, b) => b.GetComponent<AttractObject>().GetSize() - a.GetComponent<AttractObject>().GetSize());

        //オブジェクトのサイズごとにガードポイントを指定
        for (int i = 0; i < m_AttractingObjects.Count; i++)
        {
            switch (m_AttractingObjects[i].GetSize())
            {
                case AttractObject.Size.Large:
                    for (int j = 0; j < m_LargeGuardPoints.Count; j++)
                    {
                        if (m_LargeGuardPoints[j].GetStatus() == LargeGuardPoint.Status.Disabled)
                            continue;
                        //当たり判定をオンに
                        m_AttractingObjects[i].GetComponent<BoxCollider>().enabled = true;
                        //追従先にガードポイントを設定
                        m_AttractingObjects[i].SetTarget(m_LargeGuardPoints[j].transform);
                        //ガードポイントを使用不可に
                        m_LargeGuardPoints[j].SetAllDisabled();
                        break;
                    }
                    break;
                case AttractObject.Size.Regular:
                    for (int j = 0; j < m_RegularGuardPoints.Count; j++)
                    {
                        if (m_RegularGuardPoints[j].GetStatus() == RegularGuardPoint.Status.Disabled) continue;
                        //当たり判定をオンに
                        m_AttractingObjects[i].GetComponent<BoxCollider>().enabled = true;
                        //追従先にガードポイントを設定
                        m_AttractingObjects[i].SetTarget(m_RegularGuardPoints[j].transform);
                        //ガードポイントを使用不可に
                        m_RegularGuardPoints[j].SetAllDisabled();
                        break;
                    }
                    break;
                case AttractObject.Size.Small:
                    for (int j = 0; j < m_SmallGuardPoints.Count; j++)
                    {
                        if (m_SmallGuardPoints[j].GetStatus() == SmallGuardPoint.Status.Disabled) continue;
                        //当たり判定をオンに
                        m_AttractingObjects[i].GetComponent<BoxCollider>().enabled = true;
                        //追従先にガードポイントを設定
                        m_AttractingObjects[i].SetTarget(m_SmallGuardPoints[j].transform);
                        //ガードポイントを使用不可に
                        m_SmallGuardPoints[j].SetDisabled();
                        break;
                    }
                    break;
            }
        }
        //引き寄せポイントを全て空に
        foreach (AttractPoint point in m_AttractPoints)
        {
            point.SetAllEmpty();
        }
    }

    //オブジェクトを通常の位置に戻す
    void RevertingObject()
    {
        //オブジェクトを大きい順にソート
        m_AttractingObjects.Sort((a, b) => b.GetComponent<AttractObject>().GetSize() - a.GetComponent<AttractObject>().GetSize());

        //引き寄せ位置一つごとに処理
        for (int i = 0; i < m_AttractingObjects.Count; i++)
        {
            for (int j = 0; j < m_AttractPoints.Length; j++)
            {
                //ポイントがすでにオブジェクトを持っていたら次のポイントへ
                if (m_AttractPoints[j].GetStatus() == AttractPoint.Status.Full) continue;

                switch (m_AttractingObjects[i].GetSize())
                {
                    case AttractObject.Size.Large:
                        if (m_AttractPoints[j].GetStatus() == AttractPoint.Status.Afford) continue;
                        m_AttractPoints[j].Attract(m_AttractingObjects[i]);
                        break;
                    case AttractObject.Size.Regular:
                        if (m_AttractPoints[j].CheckAllChildAttract(ChildPoint.Status.Afford)) continue;
                        m_AttractPoints[j].ChildAttract(m_AttractingObjects[i]);
                        break;
                    case AttractObject.Size.Small:
                        m_AttractPoints[j].ChildAttract(m_AttractingObjects[i]);
                        break;
                }
                break;
            }
        }
        //ガードポイントを使用可能に
        foreach (LargeGuardPoint point in m_LargeGuardPoints)
        {
            point.SetAllEnabled();
        }
    }

    //接地しているか？ 
    bool IsGround()
    {
        float r = 0.1f;
        float dis = 0.22f;
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z + r), -transform.up, dis) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z - r), -transform.up, dis) ||
            Physics.Raycast(new Vector3(transform.position.x + r, transform.position.y + 0.1f, transform.position.z), -transform.up, dis) ||
            Physics.Raycast(new Vector3(transform.position.x - r, transform.position.y + 0.1f, transform.position.z), -transform.up, dis);
    }

    //地面までの距離
    float DistanceToGround()
    {
        RaycastHit hitInfo;
        Physics.Raycast(transform.position, -transform.up, out hitInfo);
        return hitInfo.distance;
    }

    //カメラの正面方向を向く
    void LookAtCameraForword()
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;

        //現在の向きとターゲットへの向きを混ぜる
        Vector3 dir = Vector3.Slerp(transform.forward, cameraForward, 12.0f * Time.deltaTime);
        //混ぜた方向を向く
        transform.LookAt(transform.position + dir);
    }

    //後方にある壁までの距離を取得
    float RearWallDistance(Vector3 dir)
    {
        //Rayの原点
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit))
            return hit.distance - 0.5f;
        else
            return -1f;
    }

    //後方へ吹き呼ぶ
    void BlownAway(Vector3 pos)
    {
        //攻撃判定の中心位置方向を向く
        gameObject.transform.LookAt(new Vector3(pos.x, transform.position.y, pos.z));
        //攻撃判定からPlayerへの向き
        Vector3 colliderToPlayer = transform.position - pos;
        //単位ベクトルにする
        Vector3 dir = new Vector3(colliderToPlayer.x, 0.0f, colliderToPlayer.z).normalized;
        Vector3 to;
        //後方スペースの距離を取得する
        float dis = RearWallDistance(dir);
        //距離に応じて飛ぶ距離を設定
        if (dis <= 0f) to = dir * 0f;
        else if (dis >= 3f) to = dir * 3f;
        else to = dir * dis;

        //LeanTween.move(gameObject, transform.position + to, 2f).setEase(LeanTweenType.easeOutQuart);
        LeanTween.moveLocalX(gameObject, transform.position.x + to.x, 2f).setEase(LeanTweenType.easeOutQuart);
        LeanTween.moveLocalZ(gameObject, transform.position.z + to.z, 2f).setEase(LeanTweenType.easeOutQuart);
    }

    //ダメージを受ける
    void ToDamage(int damage)
    {
        m_HP -= damage;
        m_HPGage.Decrease(damage);
    }

    //行動可能か？
    bool IsActionable()
    {
        return
            m_MoveState != MoveState.Damage &&
            m_MoveState != MoveState.Down &&
            m_MoveState != MoveState.Dead;
    }

    public void SetHit(Vector3 pos, int power)
    {
        //Enemyの攻撃に衝突したら
        if (IsActionable())
        {
            //移動量をリセット
            m_Velocity = Vector3.zero;
            BlownAway(pos);
            m_Animator.SetTrigger("Damage");
            ChangeState(MoveState.Damage);

            //体力を減らす
            ToDamage(power);
        }
    }

    //指定時間後に選択したシーンへ遷移する
    protected IEnumerator LoadSelectScene(SceneMaker.SceneType type)
    {
        m_SceneMaker.ChangeNextScene(type);
        yield return new WaitForSeconds(4.0f);
        m_SceneMaker.LoadScene();
    }


    /****************************** モーションイベント ******************************/
    void AttractStartEvent()
    {
        AttractingObject();
        m_Sound.PlaySE(3);
    }

    void ShootStartEvent()
    {
        //ShootingObject();
        StartCoroutine(ShootCoroutine());
    }

    virtual protected void AttractEndEvent()
    {
        m_ActionState = ActionState.None;
    }

    virtual protected void ShootEndEvent()
    {
        m_ActionState = ActionState.None;
    }

    void StandUpEndEvent()
    {
        m_Animator.ResetTrigger("StandUp");
        ChangeState(MoveState.Walk);
    }

    void DamageEndEvent()
    {
        ChangeState(MoveState.Down);
    }


    void FootEvent()
    {
        m_Sound.PlaySE(0);
    }
    void PlayerDamageEvent()
    {
        m_Sound.PlaySE(1);
    }
    void PlayerStandUpEvent()
    {
        m_Sound.PlaySE(2);
    }
    void PlayerAttractingEvent()
    {
        m_Sound.PlaySE(4);
    }
}
