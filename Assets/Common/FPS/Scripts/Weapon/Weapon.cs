using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Weapons {
    //武器类型
    public enum WeaponType {
        //抛射物
        Projectile,
        //射线
        Raycast,
        //激光
        Beam
    }
    //武器模式
    public enum Auto {
        Full,
        Semi
    }
    public enum BulletHoleSystem {
        Tag,
        Material,
        Physic_Material
    }

    //子弹池
    [System.Serializable]
    public class SmartBulletHoleGroup {
        public string Tag;
        public Material Material;
        public PhysicMaterial PhysicMaterial;
        public BulletHolePool BulletHole;

        public SmartBulletHoleGroup() {
            Tag = "Everything";
            Material = null;
            PhysicMaterial = null;
            BulletHole = null;
        }
        public SmartBulletHoleGroup(string t, Material m, PhysicMaterial pm, BulletHolePool bh) {
            Tag = t;
            Material = m;
            PhysicMaterial = pm;
            BulletHole = bh;
        }
    }

    [RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviour {
        //武器种类
        public WeaponType Type = Weapons.WeaponType.Projectile;

        //public bool ShooterAIEnabled = false; 
        //public bool BloodyMessEnabled = false;
        //public int WeaponType = 0;            
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        //开枪模式，全自动或者半自动
        public Auto AutoMode = Auto.Full;

        #region 通用
        //是否是Player使用的武器
        public bool PlayerWeapon = true;
        //武器的模型
        public GameObject WeaponModel;
        //射线武器发射射线的起点
        public Transform RaycastStartSpot;
        //延迟射击的时间
        public float DelayBeforeFire = 0.0f;
        #endregion

        #region 蓄力射击
        //半自动射线和抛射武器是否开启蓄力射击
        public bool Warmup = false;
        //最大蓄力时间
        public float MaxWarmup = 2.0f;
        //蓄力是否会影响武器击退力的大小
        public bool MultiplyForce = true;
        // Whether or not the damage of the projectile should be multiplied based on warmup heat value - only for projectiles
        // Also only has an effect on projectiles using the direct damage system - an example would be an arrow that causes more damage the longer you pull back your bow
        public bool MultiplyPower = false;
        //武器伤害增幅系数
        public float PowerMultiplier = 1.0f;
        //武器击退力增幅系数
        public float InitialForceMultiplier = 1.0f;
        //蓄力时是否允许取消射击
        public bool AllowCancel = false;
        //用于记录蓄力时间
        private float m_Heat = 0.0f;
        #endregion

        //抛射物武器射出的抛射物
        public GameObject Projectile;
        //抛射物射出的起始位置
        public Transform ProjectileSpawnSpot;

        #region 激光
        //激光能否反射
        public bool Reflect = true;
        //能反射激光的材质
        public Material ReflectionMaterial;
        //最大反射次数
        public int MaxReflections = 5;
        //激光名称
        public string BeamTypeName = "laser_beam";
        //激光最长持续时间
        public float MaxBeamHeat = 1.0f;
        //是否设置无限激光
        public bool InfiniteBeam = false;
        //激光材质
        public Material BeamMaterial;
        //激光颜色
        public Color BeamColor = Color.red;
        //激光初始宽度
        public float StartBeamWidth = 0.5f;
        //激光最终宽度
        public float EndBeamWidth = 1.0f;
        //激光冷却计时器
        private float m_BeamHeat = 0.0f;
        //激光是否冷却
        private bool m_CoolingDown = false; 
        //保存激光Object的引用
        private GameObject m_BeamGO;
        //当前是否正在产生激光
        private bool m_IsBeaming = false;
        #endregion

        #region 武器威力
        //武器的伤害
        public float Power = 80.0f;
        //武器击退力的增幅系数
        public float ForceMultiplier = 10.0f;
        //激光的伤害
        public float BeamPower = 1.0f;
        //武器的射击距离（激光武器或者射线武器）
        public float Range = 9999.0f;
        #endregion


        #region 开枪频率
        //每秒钟开枪的次数
        public float RateOfFire = 10;
        //根据rateOfFire计算的开枪频率
        private float ActualROF;
        //开枪计时器
        private float FireTimer;
        #endregion

        #region 弹药
        //是否是无限弹药模式
        public bool InfiniteAmmo = false;
        //武器的弹药容量
        public int AmmoCapacity = 12;
        //单次射击射出的子弹数
        public int ShotPerRound = 1;
        //武器当前的弹药量
        private int CurrentAmmo;
        //武器换弹时间
        public float ReloadTime = 2.0f;
        //是否显示武器当前的弹药量
        public bool ShowCurrentAmmo = true;
        //武器在弹药量用光时是否自动换弹
        public bool ReloadAutomatically = true;
        #endregion

        #region 射击准度
        //武器射击准度（0—100）
        public float Accuracy = 80.0f;
        //武器当前射击准度
        private float CurrentAccuracy;
        //每次射击武器准度下降的幅度
        public float AccuracyDropPerShot = 1.0f;
        //武器射击后，射击准度恢复速度（0—1）
        public float AccuracyRecoverRate = 0.1f;
        #endregion

        #region 连续射击子弹数
        //连续射击子弹数（按住开火键不放）
        public int BurstRate = 3;
        //射击了burstRate颗子弹之后，不能射击的时长
        public float BurstPause = 0.0f;
        //当前已射击子弹数
        private int BurstCounter = 0;
        //计时器
        private float BurstTimer = 0.0f;
        #endregion

        #region 后坐力
        //是否使用后坐力
        public bool UseRecoil = true;
        //射击时武器的抖动范围
        public float RecoilKickBackMin = 0.1f;
        public float RecoilKickBackMax = 0.3f;
        //射击时武器的旋转范围
        public float RecoilRotationMin = 0.1f;
        public float RecoilRotationMax = 0.25f;
        //射击时镜头往上抬的最大角度
        public float RecoilMaxAngle = 1f;
        //武器射击后，恢复至正常位置的速度
        public float RecoilRecoveryRate = 0.01f;
        #endregion

        #region 特效
        //射击时是否吐出弹壳
        public bool UseSpitShells = false;
        //弹壳Prefab
        public GameObject Shell;
        //吐出弹壳时给弹壳施加的力
        public float ShellSpitForce = 1.0f;
        //shellSpitForce的变化范围（左右）
        public float ShellForceRandom = 0.5f;
        //x轴上的扭矩
        public float ShellSpitTorqueX = 0.0f;
        //y轴上的扭矩
        public float ShellSpitTorqueY = 0.0f;
        //扭矩的变化范围（左右）
        public float ShellTorqueRandom = 1.0f;
        //弹壳掉落的位置
        public Transform ShellSpitPosition;
        //是否使用枪口火光特效
        public bool MakeMuzzleEffects = true;
        //枪口火光特效Prefab
        public GameObject[] MuzzleEffects =
            new GameObject[] { null };
        //枪口火光特效产生的位置
        public Transform MuzzleEffectsPosition;
        //是否使用打击特效
        public bool MakeHitEffects = true;
        //打击特效的Prefab
        public GameObject[] HitEffects =
            new GameObject[] { null }; 
        #endregion

        #region 弹孔
        //是否产生弹孔
        public bool MakeBulletHoles = true;
        //根据什么条件产生弹孔
        public BulletHoleSystem BHSystem = BulletHoleSystem.Tag;
        //弹孔池对象名
        public List<string> BulletHolePoolNames = new
            List<string>();
        //默认弹孔池对象名
        public List<string> DefaultBulletHolePoolNames =
            new List<string>();
        //产生弹孔的条件
        public List<SmartBulletHoleGroup> BulletHoleGroups =
            new List<SmartBulletHoleGroup>();
        //保存默认弹孔，当没有弹孔满足条件时，使用默认弹孔
        public List<BulletHolePool> DefaultBulletHoles =
            new List<BulletHolePool>();
        //不产生任何弹孔（包括默认弹孔）的条件
        public List<SmartBulletHoleGroup> BulletHoleExceptions =
            new List<SmartBulletHoleGroup>(); 
        #endregion 

        #region 屏幕射击准星
        //是否绘制准星
        public bool ShowCrosshair = true;
        //用于绘制准星的贴图
        public Texture2D CrosshairTexture;
        //准星的长度
        public int CrosshairLength = 10;
        //准星的宽度
        public int CrosshairWidth = 4;
        //准星的初始大小
        public float StartingCrosshairSize = 10.0f;
        //准星当前大小
        private float CurrentCrosshairSize;
        #endregion

        #region 音效
        //射击音效
        public AudioClip FireSound;
        //补充弹药音效
        public AudioClip ReloadSound;
        //弹药不足时射击音效
        public AudioClip DryFireSound;
        #endregion

        //武器是否能射击
        private bool m_CanFire = true;

        private Hashtable m_RecoilParam;

        // Use this for initialization
        void Start() {
            //计算开火的频率
            if (RateOfFire != 0)
                ActualROF = 1.0f / RateOfFire;
            else
                ActualROF = 0.01f;

            //初始化当前准星大小
            CurrentCrosshairSize = StartingCrosshairSize;

            //重置计时器
            FireTimer = 0.0f;

            //初始化武器的当前弹药量
            CurrentAmmo = AmmoCapacity;

            //确保含有AudioSource组件
            if (GetComponent<AudioSource>() == null) {
                gameObject.AddComponent(typeof(AudioSource));
            }

            //确保设置了射线起点
            if (RaycastStartSpot == null)
                RaycastStartSpot = gameObject.transform;

            //确保设置了枪口火光的产生位置
            if (MuzzleEffectsPosition == null)
                MuzzleEffectsPosition = gameObject.transform;

            //确保设置了抛射物抛射的起始位置
            if (ProjectileSpawnSpot == null)
                ProjectileSpawnSpot = gameObject.transform;

            //确保设置了武器Model
            if (WeaponModel == null)
                WeaponModel = gameObject;

            //确保设置了绘制准星的贴图
            if (CrosshairTexture == null)
                CrosshairTexture = new Texture2D(0, 0);

            //初始化弹孔池list
            for (int i = 0; i < BulletHolePoolNames.Count; i++) {
                GameObject g = GameObject.Find(BulletHolePoolNames[i]);

                if (g != null && g.GetComponent<BulletHolePool>() != null)
                    BulletHoleGroups[i].BulletHole = g.GetComponent<BulletHolePool>();
                else
                    Debug.LogWarning("Bullet Hole Pool does not exist or does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
            }

            //初始化默认弹孔池list
            for (int i = 0; i < DefaultBulletHolePoolNames.Count; i++) {
                GameObject g = GameObject.Find(DefaultBulletHolePoolNames[i]);

                if (g.GetComponent<BulletHolePool>() != null)
                    DefaultBulletHoles[i] = g.GetComponent<BulletHolePool>();
                else
                    Debug.LogWarning("Default Bullet Hole Pool does not have a BulletHolePool component.  Please assign GameObjects in the inspector that have the BulletHolePool component.");
            }

            //初始化后坐力参数
            m_RecoilParam = new Hashtable {
                {"shootInterval", ActualROF },
                {"maxAngle", RecoilMaxAngle }
            };
        }

        void Update() {
            //计算当前武器射击精度
            CurrentAccuracy = Mathf.Lerp(CurrentAccuracy, Accuracy, AccuracyRecoverRate * Time.deltaTime);

            //根据精度计算准星的大小
            CurrentCrosshairSize = StartingCrosshairSize + (Accuracy - CurrentAccuracy) * 0.8f;

            //更新开火计时器
            FireTimer += Time.deltaTime;

            //如果是Player使用的武器，使用键盘输入触发开火
            if (PlayerWeapon) {
                CheckForUserInput();
            }

            //如果设置了自动换弹，弹药用光之后进行换弹
            if (ReloadAutomatically && CurrentAmmo <= 0)
                Reload();

            //从后坐力偏移恢复至起始位置
            if (PlayerWeapon && UseRecoil && Type != Weapons.WeaponType.Beam) {
                WeaponModel.transform.position = Vector3.Lerp(WeaponModel.transform.position, transform.position, RecoilRecoveryRate * Time.deltaTime);
                WeaponModel.transform.rotation = Quaternion.Lerp(WeaponModel.transform.rotation, transform.rotation, RecoilRecoveryRate * Time.deltaTime);
            }

            //确保一开始没有发射激光
            if (Type == Weapons.WeaponType.Beam) {
                if (!m_IsBeaming)
                    StopBeam();
                //当前没有发射激光
                m_IsBeaming = false;
            }
        }

        //使用键盘输入触发武器开火
        void CheckForUserInput() {
            //射线武器
            if (Type == Weapons.WeaponType.Raycast) {
                if (FireTimer >= ActualROF && BurstCounter < BurstRate && m_CanFire) {
                    if (Input.GetButton("Fire1")) {
                        if (!Warmup) {   //没有蓄力射击，正常射击
                            Fire();
                        } else if (m_Heat < MaxWarmup) {   //开始蓄力
                            m_Heat += Time.deltaTime;
                        }
                    }
                    if (Warmup && Input.GetButtonUp("Fire1")) {
                        if (AllowCancel && Input.GetButton("Cancel")) {
                            m_Heat = 0.0f;
                        } else {
                            Fire();
                        }
                    }
                }
            }

            //抛射武器
            if (Type == Weapons.WeaponType.Projectile) {
                if (FireTimer >= ActualROF && BurstCounter < BurstRate && m_CanFire) {
                    if (Input.GetButton("Fire1")) {
                        if (!Warmup) {   //没有蓄力射击，正常射击
                            Launch();
                        } else if (m_Heat < MaxWarmup) {   //开始蓄力
                            m_Heat += Time.deltaTime;
                        }
                    }
                    if (Warmup && Input.GetButtonUp("Fire1")) {
                        if (AllowCancel && Input.GetButton("Cancel")) {
                            m_Heat = 0.0f;
                        } else {
                            Launch();
                        }
                    }
                }

            }

            //重置已发射的子弹数
            if (BurstCounter >= BurstRate) {
                BurstTimer += Time.deltaTime;
                if (BurstTimer >= BurstPause) {
                    BurstCounter = 0;
                    BurstTimer = 0.0f;
                }
            }

            //激光武器
            if (Type == Weapons.WeaponType.Beam) {
                if (Input.GetButton("Fire1") && m_BeamHeat <= MaxBeamHeat && !m_CoolingDown) {
                    Beam();
                } else {
                    //停止发射激光
                    StopBeam();
                }
                if (m_BeamHeat >= MaxBeamHeat) {
                    m_CoolingDown = true;
                } else if (m_BeamHeat <= MaxBeamHeat - (MaxBeamHeat / 2)) {
                    m_CoolingDown = false;
                }
            }

            //按下换弹键，直接换弹
            if (Input.GetButtonDown("Reload"))
                Reload();

            //如果是半自动武器，松开开火键时才能重新开火
            if (Input.GetButtonUp("Fire1"))
                m_CanFire = true;
        }

        //非键盘输入触发武器开火，用于网络或者AI
        public void RemoteFire() {
            AIFiring();
        }

        //确定AI什么时候开火
        public void AIFiring() {
            //射线武器
            if (Type == Weapons.WeaponType.Raycast) {
                if (FireTimer >= ActualROF && BurstCounter < BurstRate && m_CanFire) {
                    StartCoroutine(DelayFire());
                }
            }
            //抛射物武器
            if (Type == Weapons.WeaponType.Projectile) {
                if (FireTimer >= ActualROF && m_CanFire) {
                    StartCoroutine(DelayLaunch());
                }
            }
            //重置当前已发射的子弹数
            if (BurstCounter >= BurstRate) {
                BurstTimer += Time.deltaTime;
                if (BurstTimer >= BurstPause) {
                    BurstCounter = 0;
                    BurstTimer = 0.0f;
                }
            }
            //激光武器
            if (Type == Weapons.WeaponType.Beam) {
                if (m_BeamHeat <= MaxBeamHeat && !m_CoolingDown) {
                    Beam();
                } else {
                    //停止发射激光
                    StopBeam();
                }
                if (m_BeamHeat >= MaxBeamHeat) {
                    m_CoolingDown = true;
                } else if (m_BeamHeat <= MaxBeamHeat - (MaxBeamHeat / 2)) {
                    m_CoolingDown = false;
                }
            }
        }

        //延迟射击协程，用于射线武器
        IEnumerator DelayFire() {
            //重置计时器
            FireTimer = 0.0f;

            //增加当前已射击子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi)
                m_CanFire = false;

            // Send a messsage so that users can do other actions whenever this happens
            SendMessageUpwards("OnEasyWeaponsFire", SendMessageOptions.DontRequireReceiver);

            yield return new WaitForSeconds(DelayBeforeFire);
            Fire();
        }

        //延迟抛射协程，用于抛射物武器
        IEnumerator DelayLaunch() {
            //重置计时器
            FireTimer = 0.0f;

            //增加当前已射击子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi)
                m_CanFire = false;

            // Send a messsage so that users can do other actions whenever this happens
            SendMessageUpwards("OnEasyWeaponsLaunch", SendMessageOptions.DontRequireReceiver);

            yield return new WaitForSeconds(DelayBeforeFire);
            Launch();
        }

        //延迟发射激光协程，用于激光武器
        IEnumerator DelayBeam() {
            yield return new WaitForSeconds(DelayBeforeFire);
            Beam();
        }

        //UI绘制函数
        void OnGUI() {
            // Crosshairs
            if (Type == Weapons.WeaponType.Projectile || Type == Weapons.WeaponType.Beam) {
                CurrentAccuracy = Accuracy;
            }

            //绘制准星
            if (ShowCrosshair) {
                //准星的中心
                Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

                //左边
                Rect leftRect = new Rect(center.x - CrosshairLength - CurrentCrosshairSize, center.y - (CrosshairWidth / 2), CrosshairLength, CrosshairWidth);
                GUI.DrawTexture(leftRect, CrosshairTexture, ScaleMode.StretchToFill);
                //右边
                Rect rightRect = new Rect(center.x + CurrentCrosshairSize, center.y - (CrosshairWidth / 2), CrosshairLength, CrosshairWidth);
                GUI.DrawTexture(rightRect, CrosshairTexture, ScaleMode.StretchToFill);
                //上边
                Rect topRect = new Rect(center.x - (CrosshairWidth / 2), center.y - CrosshairLength - CurrentCrosshairSize, CrosshairWidth, CrosshairLength);
                GUI.DrawTexture(topRect, CrosshairTexture, ScaleMode.StretchToFill);
                //下边
                Rect bottomRect = new Rect(center.x - (CrosshairWidth / 2), center.y + CurrentCrosshairSize, CrosshairWidth, CrosshairLength);
                GUI.DrawTexture(bottomRect, CrosshairTexture, ScaleMode.StretchToFill);
            }

            //显示弹药量
            if (ShowCurrentAmmo) {
                if (Type == Weapons.WeaponType.Raycast || Type == Weapons.WeaponType.Projectile)
                    GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Ammo: " + CurrentAmmo);
                else if (Type == Weapons.WeaponType.Beam)
                    GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Heat: " + (int)(m_BeamHeat * 100) + "/" + (int)(MaxBeamHeat * 100));
            }
        }

        //射线武器，开火
        void Fire() {
            //重置计时器
            FireTimer = 0.0f;

            //增加已射击的子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi)
                m_CanFire = false;

            //判断当前是否仍有弹药
            if (CurrentAmmo <= 0) {
                DryFire();
                return;
            }

            //如果不是无线弹药模式，减少弹药量
            if (!InfiniteAmmo)
                CurrentAmmo--;

            //单次射击，射出shotPerRound颗子弹
            for (int i = 0; i < ShotPerRound; i++) {
                //计算当前的精确度
                float accuracyVary = (100 - CurrentAccuracy) / 1000;
                Vector3 direction = RaycastStartSpot.forward;
                direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                CurrentAccuracy -= AccuracyDropPerShot;
                if (CurrentAccuracy <= 0.0f)
                    CurrentAccuracy = 0.0f;

                //创建射线
                Ray ray = new Ray(RaycastStartSpot.position, direction);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Range)) {
                    //蓄力
                    float damage = Power;
                    if (Warmup) {
                        damage *= m_Heat * PowerMultiplier;
                        m_Heat = 0.0f;
                    }

                    ////造成伤害
                    //hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -damage, SendMessageOptions.DontRequireReceiver);

                    //if (shooterAIEnabled) {
                    //    hit.transform.SendMessageUpwards("Damage", damage / 100, SendMessageOptions.DontRequireReceiver);
                    //}

                    //if (BloodyMessEnabled) {
                    //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limb")) {
                    //        Vector3 directionShot = hit.collider.transform.position - transform.position;

                    //        /*
                    //        if (hit.collider.gameObject.GetComponent<Limb>())
                    //        {
                    //            GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;
                    //            CharacterSetup character = parent.GetComponent<CharacterSetup>();
                    //            character.ApplyDamage(damage, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
                    //        }
                    //        */
                    //    }
                    //}

                    //判断被击中物体是否满足不产生弹孔的条件
                    bool exception = false;
                    if (BHSystem == BulletHoleSystem.Tag) {
                        foreach (SmartBulletHoleGroup bhg in BulletHoleExceptions) {
                            if (hit.collider.gameObject.tag == bhg.Tag) {
                                exception = true;
                                break;
                            }
                        }
                    } else if (BHSystem == BulletHoleSystem.Material) {
                        foreach (SmartBulletHoleGroup bhg in BulletHoleExceptions) {
                            MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);
                            if (mesh != null) {
                                if (mesh.sharedMaterial == bhg.Material) {
                                    exception = true;
                                    break;
                                }
                            }
                        }
                    } else if (BHSystem == BulletHoleSystem.Physic_Material) {
                        foreach (SmartBulletHoleGroup bhg in BulletHoleExceptions) {
                            if (hit.collider.sharedMaterial == bhg.PhysicMaterial) {
                                exception = true;
                                break;
                            }
                        }
                    }

                    //如果需要生成弹孔，且物体满足生成弹孔条件
                    if (MakeBulletHoles && !exception) {
                        //保存所有符合条件的SmartBulletHoleGroup
                        List<SmartBulletHoleGroup> holes = new List<SmartBulletHoleGroup>();

                        if (BHSystem == BulletHoleSystem.Tag) {
                            foreach (SmartBulletHoleGroup bhg in BulletHoleGroups) {
                                if (hit.collider.gameObject.tag == bhg.Tag) {
                                    holes.Add(bhg);
                                }
                            }
                        } else if (BHSystem == BulletHoleSystem.Material) {

                            MeshRenderer mesh = FindMeshRenderer(hit.collider.gameObject);

                            foreach (SmartBulletHoleGroup bhg in BulletHoleGroups) {
                                if (mesh != null) {
                                    if (mesh.sharedMaterial == bhg.Material) {
                                        holes.Add(bhg);
                                    }
                                }
                            }
                        } else if (BHSystem == BulletHoleSystem.Physic_Material) {
                            foreach (SmartBulletHoleGroup bhg in BulletHoleGroups) {
                                if (hit.collider.sharedMaterial == bhg.PhysicMaterial) {
                                    holes.Add(bhg);
                                }
                            }
                        }

                        //保存被选中的SmartBulletHoleGroup
                        SmartBulletHoleGroup sbhg = null;

                        //没有满足条件的弹孔，使用默认弹孔
                        if (holes.Count == 0) {
                            List<SmartBulletHoleGroup> defaultsToUse = new List<SmartBulletHoleGroup>();
                            foreach (BulletHolePool h in DefaultBulletHoles) {
                                defaultsToUse.Add(new SmartBulletHoleGroup("Default", null, null, h));
                            }

                            //随机选择一个SmartBulletHoleGroup
                            sbhg = defaultsToUse[Random.Range(0, defaultsToUse.Count)];
                        }else {
                            //随机选择一个SmartBulletHoleGroup
                            sbhg = holes[Random.Range(0, holes.Count)];
                        }

                        //在场景中生成弹孔
                        if (sbhg.BulletHole != null)
                            sbhg.BulletHole.PlaceBulletHole(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    }

                    //击中的特效
                    if (MakeHitEffects) {
                        foreach (GameObject hitEffect in HitEffects) {
                            if (hitEffect != null)
                                Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        }
                    }

                    //给被射中的物体加上一个击退的力
                    if (hit.rigidbody) {
                        hit.rigidbody.AddForce(ray.direction * Power * ForceMultiplier);
                    }
                }
            }

            //如果使用后坐力，调用后坐力函数
            if (UseRecoil)
                Recoil();

            //如果使用枪口火焰效果，就显示枪口火焰
            if (MakeMuzzleEffects) {
                GameObject muzfx = MuzzleEffects[Random.Range(0, MuzzleEffects.Length)];
                if (muzfx != null)
                    Instantiate(muzfx, MuzzleEffectsPosition.position, MuzzleEffectsPosition.rotation);
            }

            //创建弹壳
            if (UseSpitShells) {
                GameObject shellGO = Instantiate(Shell, ShellSpitPosition.position, ShellSpitPosition.rotation) as GameObject;
                shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(ShellSpitForce + Random.Range(0, ShellForceRandom), 0, 0), ForceMode.Impulse);
                shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(ShellSpitTorqueX + Random.Range(-ShellTorqueRandom, ShellTorqueRandom), ShellSpitTorqueY + Random.Range(-ShellTorqueRandom, ShellTorqueRandom), 0), ForceMode.Impulse);
            }

            //播放射击音效
            GetComponent<AudioSource>().PlayOneShot(FireSound);
        }

        //抛射物武器，抛射
        public void Launch() {
            //重置计时器
            FireTimer = 0.0f;

            //增加已发射的子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi)
                m_CanFire = false;

            //判断当前是否仍有弹药
            if (CurrentAmmo <= 0) {
                DryFire();
                return;
            }

            //如果不是无限弹药模式，减少弹药量
            if (!InfiniteAmmo)
                CurrentAmmo--;

            //单次射击，射出shotPerRound颗子弹
            for (int i = 0; i < ShotPerRound; i++) {
                //创建抛射物
                if (Projectile != null) {
                    GameObject proj = Instantiate(Projectile, ProjectileSpawnSpot.position, ProjectileSpawnSpot.rotation) as GameObject;

                    //蓄力
                    if (Warmup) {
                        if (MultiplyPower)
                            proj.SendMessage("MultiplyDamage", m_Heat * PowerMultiplier, SendMessageOptions.DontRequireReceiver);
                        if (MultiplyForce)
                            proj.SendMessage("MultiplyInitialForce", m_Heat * InitialForceMultiplier, SendMessageOptions.DontRequireReceiver);

                        m_Heat = 0.0f;
                    }
                } else {
                    Debug.Log("Projectile to be instantiated is null.  Make sure to set the Projectile field in the inspector.");
                }
            }

            //如果使用后坐力，调用后坐力函数
            if (UseRecoil)
                Recoil();

            //如果使用枪口火焰效果，就显示枪口火焰
            if (MakeMuzzleEffects) {
                GameObject muzfx = MuzzleEffects[Random.Range(0, MuzzleEffects.Length)];
                if (muzfx != null)
                    Instantiate(muzfx, MuzzleEffectsPosition.position, MuzzleEffectsPosition.rotation);
            }

            //创建弹壳
            if (UseSpitShells) {
                GameObject shellGO = Instantiate(Shell, ShellSpitPosition.position, ShellSpitPosition.rotation) as GameObject;
                shellGO.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(ShellSpitForce + Random.Range(0, ShellForceRandom), 0, 0), ForceMode.Impulse);
                shellGO.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(ShellSpitTorqueX + Random.Range(-ShellTorqueRandom, ShellTorqueRandom), ShellSpitTorqueY + Random.Range(-ShellTorqueRandom, ShellTorqueRandom), 0), ForceMode.Impulse);
            }

            //播放射击音效
            GetComponent<AudioSource>().PlayOneShot(FireSound);
        }

        //激光武器，发射激光
        void Beam() {
            // Send a messsage so that users can do other actions whenever this happens
            SendMessageUpwards("OnEasyWeaponsBeaming", SendMessageOptions.DontRequireReceiver);

            //当前正在发射激光
            m_IsBeaming = true;

            //如果不是无限激光，开始计时
            if (!InfiniteBeam)
                m_BeamHeat += Time.deltaTime;

            //如果没有设置激光Object，创建一个带有line renderer的GameObject
            if (m_BeamGO == null) {
                m_BeamGO = new GameObject(BeamTypeName, typeof(LineRenderer));
                //设置为Weapon的子对象
                m_BeamGO.transform.parent = transform;
                
            }
            //设置线渲染器属性
            LineRenderer beamLR = m_BeamGO.GetComponent<LineRenderer>();
            beamLR.material = BeamMaterial;
            beamLR.material.SetColor("_TintColor", BeamColor);
            //beamLR.SetWidth(startBeamWidth, endBeamWidth);
            beamLR.startWidth = StartBeamWidth;
            beamLR.endWidth = EndBeamWidth;

            //反射次数
            int reflections = 0;

            //保存激光的所有反射点
            List<Vector3> reflectionPoints = new List<Vector3>();
            //添加第一个反射点
            reflectionPoints.Add(RaycastStartSpot.position);

            //保存上一个反射点
            Vector3 lastPoint = RaycastStartSpot.position;

            //用于保存激光的入射方向和反射方向
            Vector3 incomingDirection;
            Vector3 reflectDirection;

            //激光是否需要继续进行反射
            bool keepReflecting = true;

            //投射射线
            Ray ray = new Ray(lastPoint, RaycastStartSpot.forward);
            RaycastHit hit;

            do {
                //如果没有检测到任何物体，那么下一个点为终点
                Vector3 nextPoint = ray.direction * Range;

                if (Physics.Raycast(ray, out hit, Range)) {
                    //保存碰撞点
                    nextPoint = hit.point;

                    //计算反射方向
                    incomingDirection = nextPoint - lastPoint;
                    reflectDirection = Vector3.Reflect(incomingDirection, hit.normal);
                    ray = new Ray(nextPoint, reflectDirection);

                    //更新上一次的反射点
                    lastPoint = hit.point;

                    //创建击中特效
                    if (MakeHitEffects) {
                        foreach (GameObject hitEffect in HitEffects) {
                            if (hitEffect != null)
                                Instantiate(hitEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                        }
                    }

                    //hit.collider.gameObject.SendMessageUpwards("ChangeHealth", -BeamPower, SendMessageOptions.DontRequireReceiver);

                    //if (ShooterAIEnabled) {
                    //    hit.transform.SendMessageUpwards("Damage", BeamPower / 100, SendMessageOptions.DontRequireReceiver);
                    //}

                    //if (BloodyMessEnabled) {
                    //    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Limb")) {
                    //        Vector3 directionShot = hit.collider.transform.position - transform.position;

                    //        /*
                    //        if (hit.collider.gameObject.GetComponent<Limb>())
                    //        {
                    //            GameObject parent = hit.collider.gameObject.GetComponent<Limb>().parent;

                    //            CharacterSetup character = parent.GetComponent<CharacterSetup>();
                    //            character.ApplyDamage(beamPower, hit.collider.gameObject, weaponType, directionShot, Camera.main.transform.position);
                    //        }
                    //        */

                    //    }
                    //}


                    // 增加反射次数
                    reflections++;
                } else {
                    //不再进行反射
                    keepReflecting = false;
                }

                //添加反射点
                reflectionPoints.Add(nextPoint);

            } while (keepReflecting && reflections < MaxReflections && Reflect && (ReflectionMaterial == null || (FindMeshRenderer(hit.collider.gameObject) != null && FindMeshRenderer(hit.collider.gameObject).sharedMaterial == ReflectionMaterial)));

            //设置线渲染器的顶点
            //beamLR.SetVertexCount(reflectionPoints.Count);
            beamLR.positionCount = reflectionPoints.Count;

            for (int i = 0; i < reflectionPoints.Count; i++) {
                beamLR.SetPosition(i, reflectionPoints[i]);

                //枪口反射特效
                // Doesn't make the FX on the first iteration since that is handled later.  This is so that the FX at the muzzle point can be parented to the weapon
                if (MakeMuzzleEffects && i > 0)
                {
                    GameObject muzfx = MuzzleEffects[Random.Range(0, MuzzleEffects.Length)];
                    if (muzfx != null) {
                        Instantiate(muzfx, reflectionPoints[i], MuzzleEffectsPosition.rotation);
                    }
                }
            }

            //枪口闪烁特效
            if (MakeMuzzleEffects) {
                GameObject muzfx = MuzzleEffects[Random.Range(0, MuzzleEffects.Length)];
                if (muzfx != null) {
                    GameObject mfxGO = Instantiate(muzfx, MuzzleEffectsPosition.position, MuzzleEffectsPosition.rotation) as GameObject;
                    mfxGO.transform.parent = RaycastStartSpot;
                }
            }

            //播放激光音效
            if (!GetComponent<AudioSource>().isPlaying) {
                GetComponent<AudioSource>().clip = FireSound;
                GetComponent<AudioSource>().Play();
            }
        }

        public void StopBeam() {
            // Restart the beam timer
            m_BeamHeat -= Time.deltaTime;
            if (m_BeamHeat < 0)
                m_BeamHeat = 0;
            GetComponent<AudioSource>().Stop();

            // Remove the visible beam effect GameObject
            if (m_BeamGO != null) {
                Destroy(m_BeamGO);
            }

            // Send a messsage so that users can do other actions whenever this happens
            SendMessageUpwards("OnEasyWeaponsStopBeaming", SendMessageOptions.DontRequireReceiver);
        }


        //补充弹药
        void Reload() {
            //更新弹药量
            CurrentAmmo = AmmoCapacity;
            //停止计时器
            FireTimer = -ReloadTime;
            //播放换弹音效
            GetComponent<AudioSource>().PlayOneShot(ReloadSound);

            // Send a messsage so that users can do other actions whenever this happens
            SendMessageUpwards("OnEasyWeaponsReload", SendMessageOptions.DontRequireReceiver);
        }

        //射击时没有弹药
        void DryFire() {
            GetComponent<AudioSource>().PlayOneShot(DryFireSound);
        }


        //后坐力函数
        void Recoil() {
            //对于非玩家持有的武器，不使用后坐力
            if (!PlayerWeapon)
                return;

            //确保设置了武器模型
            if (WeaponModel == null) {
                Debug.Log("Weapon Model is null.  Make sure to set the Weapon Model field in the inspector.");
                return;
            }

            //计算枪的抖动值和旋转值
            float kickBack = Random.Range(RecoilKickBackMin, RecoilKickBackMax);
            float kickRot = Random.Range(RecoilRotationMin, RecoilRotationMax);

            //让枪动起来
            WeaponModel.transform.Translate(new Vector3(0, 0, -kickBack), Space.Self);
            WeaponModel.transform.Rotate(new Vector3(-kickRot, 0, 0), Space.Self);

            //只有射线武器才会有镜头的后坐力效果
            if(Type == WeaponType.Raycast) {
                SendMessageUpwards("RecoilEffect", m_RecoilParam, SendMessageOptions.DontRequireReceiver);
            }
        }

        //获得一个GameObject的MeshRenderer，如果没有就返回其子Object或者父Object上的MeshRenderer
        MeshRenderer FindMeshRenderer(GameObject go) {
            MeshRenderer hitMesh;

            //如果自己有MeshRenderer，返回自身的
            if (go.GetComponent<Renderer>() != null) {
                hitMesh = go.GetComponent<MeshRenderer>();
            } 
            //如果没有尝试找子对象上的MeshRenderer
            else {

                hitMesh = go.GetComponentInChildren<MeshRenderer>();

                //如果子对象上没有，尝试找父对象上的MeshRenderer
                if (hitMesh == null) {
                    GameObject curGO = go;
                    while (hitMesh == null && curGO.transform != curGO.transform.root) {
                        curGO = curGO.transform.parent.gameObject;
                        hitMesh = curGO.GetComponent<MeshRenderer>();
                    }
                }
            }

            return hitMesh;
        }
    }
}