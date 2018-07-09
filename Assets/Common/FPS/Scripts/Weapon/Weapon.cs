using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OperationTrident.Common;

namespace OperationTrident.FPS.Weapons {
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
    public class Weapon : MonoBehaviour,NetSyncInterface {
        //武器射击的方式
        public WeaponType Type = Weapons.WeaponType.Projectile;         
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        //开枪模式，全自动或者半自动
        public Auto AutoMode = Auto.Full;

        #region 通用
        //当前的Object是否为本地Object
        public bool IsLocalObject = true;
        //是否为Player使用的武器
        public bool PlayerWeapon = true;
        //武器的模型
        public GameObject WeaponModel;
        //不进行检测的物理层
        public string LayerMaskName;
        //发射的起点和方向
        public Transform RaycastStartSpot;
        //射击前等待的时间
        public float DelayBeforeFire = 0.0f;
        //被射出的抛射物
        public GameObject Projectile;
        //抛射物射出的起始位置和方向
        public Transform ProjectileSpawnSpot;
        #endregion

        #region 瞄准镜
        //是否使用瞄准镜
        public bool UseMirror = false;
        //开镜时枪的位置和朝向
        public Transform MirrorSpot;
        //开镜时是否使用其它的Camera
        public bool UseMirrorCamera = false;
        //开镜时使用的Camera对象
        public GameObject MirrorCamera;
        //开镜时子弹发射的位置和方向
        public Transform MirrorRaycastingPoint;

        //当前是否正在使用瞄准镜
        private bool m_IsUsingMirror;
        //武器的初始位置
        private Vector3 m_OriginalPosition;
        //武器的初始朝向
        private Quaternion m_OriginalRotation;
        #endregion

        #region 蓄力射击
        //是否开启蓄力射击
        public bool Warmup = false;
        //最长蓄力时间
        public float MaxWarmup = 2.0f;
        //蓄力是否提高击退力
        public bool MultiplyForce = true;
        //蓄力能否提高伤害
        public bool MultiplyPower = false;
        //武器伤害增幅系数
        public float PowerMultiplier = 1.0f;
        //击退力增幅系数
        public float InitialForceMultiplier = 1.0f;
        //蓄力时是否允许取消射击
        public bool AllowCancel = false;
        //用于记录蓄力时间
        private float m_Heat = 0.0f;
        #endregion

        #region 激光
        //能否进行反射
        public bool Reflect = true;
        //能反射激光的材质
        public Material ReflectionMaterial;
        //最大反射次数
        public int MaxReflections = 5;
        //激光名称
        public string BeamTypeName = "laser_beam";
        //激光最长持续时间
        public float MaxBeamHeat = 1.0f;
        //能否无限发射激光
        public bool InfiniteBeam = false;
        //激光使用的材质
        public Material BeamMaterial;
        //激光的颜色
        public Color BeamColor = Color.red;
        //激光初始粗细
        public float StartBeamWidth = 0.5f;
        //激光最终粗细
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
        public bool UseForce = false;
        //武器的伤害大小
        public float Power = 80.0f;
        //武器击退力的增幅系数
        public float ForceMultiplier = 0f;
        //激光的伤害
        public float BeamPower = 1.0f;
        //武器的射击距离
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
        //是否使用有限弹药模式
        public bool LimitedTotalAmmo;
        //弹药总容量 
        public int TotalAmmo = 30;
        //武器的弹夹容量
        public int AmmoCapacity = 30;
        //单次射击（弹药量减一）射出的子弹数
        public int ShotPerRound = 1;
        //更换弹夹的时间
        public float ReloadTime = 2.0f;
        //是否显示当前弹夹剩余的弹药量
        public bool ShowCurrentAmmo = true;
        //在弹药量用光时是否自动更换弹夹
        public bool ReloadAutomatically = true;

        //武器当前的总弹药量
        private int m_CurrentTotalAmmo;
        //武器当前的弹药量
        private int m_CurrentAmmo;
        #endregion

        #region 射击准度
        //最高射击准度
        public float Accuracy = 80.0f;
        //武器当前射击准度
        private float CurrentAccuracy;
        //单次射击射击准度下降的幅度
        public float AccuracyDropPerShot = 1.0f;
        //射击后射击准度的恢复速度
        public float AccuracyRecoverRate = 0.1f;
        #endregion

        #region 连续射击子弹数
        //触发卡壳的子弹数
        public int BurstRate = 3;
        //卡壳时不能射击的时长
        public float BurstPause = 0.0f;
        //当前已射击子弹数
        private int BurstCounter = 0;
        //计时器
        private float BurstTimer = 0.0f;
        #endregion

        #region 后坐力
        //是否使用后坐力
        public bool UseRecoil = true;
        //射击时武器抖动最近距离
        public float RecoilKickBackMin = 0.1f;
        //射击时武器抖动最远距离
        public float RecoilKickBackMax = 0.3f;
        //射击时武器选择的最小角度
        public float RecoilRotationMin = 0.1f;
        //射击时武器选择的最大角度
        public float RecoilRotationMax = 0.25f;
        //单次射击镜头上抬的角度
        public float RecoilMaxAngle = 1f;
        //射击后恢复至初始位置的速度
        public float RecoilRecoveryRate = 0.01f;
        #endregion

        #region 特效
        //射击时是否抛出弹壳
        public bool UseSpitShells = false;
        //弹壳对象
        public GameObject Shell;
        //抛出弹壳的力
        public float ShellSpitForce = 1.0f;
        //抛出弹壳的力的变化范围
        public float ShellForceRandom = 0.5f;
        //弹壳抛出时在x轴上的初始旋转角度
        public float ShellSpitTorqueX = 0.0f;
        //弹壳抛出时在y轴上的初始旋转角度
        public float ShellSpitTorqueY = 0.0f;
        //弹壳抛出时旋转角度的变化范围
        public float ShellTorqueRandom = 1.0f;
        ////弹壳抛出的位置和方向
        public Transform ShellSpitPosition;
        //是否使用枪口火光特效
        public bool MakeMuzzleEffects = true;
        //枪口火光特效对象
        public GameObject[] MuzzleEffects =
            new GameObject[] { null };
        //枪口火光特效产生的位置
        public Transform MuzzleEffectsPosition;
        //子弹打中物体时是否产生特效
        public bool MakeHitEffects = true;
        //子弹打中时产生的特效
        public GameObject[] HitEffects =
            new GameObject[] { null }; 
        #endregion

        #region 弹孔
        //是否产生弹孔
        public bool MakeBulletHoles = true;
        //产生弹孔的条件
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
        //弹夹没有弹药时射击的音效
        public AudioClip DryFireSound;
        #endregion

        //武器是否能射击
        private bool m_CanFire = true;
        //保存后坐力参数
        private Hashtable m_RecoilParam;
        //发射射线的起点
        private Transform m_CurrentShootPoint;
        //换弹协程
        private Coroutine m_ReloadCoroutine;

        private bool m_IsReloading;

        private NetSyncController m_NetSyncController;

        #region 生命周期函数
        private void OnEnable() {
            if(m_IsReloading) {
                m_IsReloading = false;

                Reload();
            }

            WeaponModel.SetActive(true);
        }

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

            //设置换弹状态
            m_IsReloading = false;

            //初始化当前总弹药量
            m_CurrentTotalAmmo = TotalAmmo;
            ////开始时先执行一次换弹
            //Reload();
            //初始化武器的当前弹药量
            m_CurrentAmmo = AmmoCapacity;

            //确保设置了射线起点
            if (RaycastStartSpot == null) {
                if(Type == WeaponType.Raycast) {
                    Debug.LogWarning("Please set the RaycastStartSpot...");
                }

                RaycastStartSpot = gameObject.transform;
            }

            //设置武器的射击起点
            m_CurrentShootPoint = RaycastStartSpot;

            //如果使用瞄准镜，设置瞄准镜的状态
            if (UseMirror) {
                //如果没有设置开静后的射击位置，默认为不开镜的射击位置
                if (MirrorRaycastingPoint == null) {
                    Debug.LogWarning("Please set the MirrorRaycastingPoint...");
                    MirrorRaycastingPoint = RaycastStartSpot;
                }

                //如果使用了瞄准镜，就设置瞄准镜的状态
                if (UseMirrorCamera) {
                    if(MirrorCamera == null) {
                        Debug.LogWarning("Please set the MirrorCamera...");
                    } else {
                        //设置瞄准镜状态
                        MirrorCamera.SetActive(false);
                    }
                }


                //初始化开镜状态
                m_IsUsingMirror = false;
            }

            //初始化武器的初始位置
            m_OriginalPosition = this.transform.localPosition;
            m_OriginalRotation = this.transform.localRotation;

            //确保设置了枪口火光的产生位置
            if (MuzzleEffectsPosition == null) {
                Debug.LogWarning("Please set the MuzzleEffectsPosition...");
                MuzzleEffectsPosition = gameObject.transform;
            }

            //确保设置了抛射物抛射的起始位置
            if (ProjectileSpawnSpot == null) {
                Debug.LogWarning("Please set the ProjectileSpawnSpot...");
                ProjectileSpawnSpot = gameObject.transform;
            }

            //确保设置了武器Model
            if (WeaponModel == null) {
                Debug.LogWarning("Please set the WeaponModel...");
                WeaponModel = gameObject;
            }
                

            //确保设置了绘制准星的贴图
            if (CrosshairTexture == null) {
                Debug.LogWarning("Please set the CrosshairTexture...");
                CrosshairTexture = new Texture2D(0, 0);
            }

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

                BulletHolePool bulletHolePool = g.GetComponent<BulletHolePool>();
                if (bulletHolePool != null)
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
            //如果不是本地Object，不执行任何操作
            if (!IsLocalObject) {
                return;
            }

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
            if (ReloadAutomatically && m_CurrentAmmo <= 0 && !m_IsReloading) {
                Reload();
            }

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

        //UI绘制函数
        private void OnGUI() {
            //如果不是本地Object，不执行任何操作
            if (!IsLocalObject) {
                return;
            }

            // Crosshairs
            if (Type == Weapons.WeaponType.Projectile || Type == Weapons.WeaponType.Beam) {
                CurrentAccuracy = Accuracy;
            }

            //只在不开镜时绘制准星
            if (ShowCrosshair && !m_IsUsingMirror) {
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
                if (Type == Weapons.WeaponType.Raycast || Type == Weapons.WeaponType.Projectile) {
                    GUI.Label(new Rect(10, Screen.height - 60, 100, 20), "Ammo: " + m_CurrentAmmo);
                    if (LimitedTotalAmmo) {
                        GUI.Label(new Rect(10, Screen.height - 40, 400, 20), "Remain Total Ammo: " + m_CurrentTotalAmmo);
                    } else {
                        GUI.Label(new Rect(10, Screen.height - 40, 400, 20), "Remain Total Ammo: Infinite");
                    }
                }
                else if (Type == Weapons.WeaponType.Beam) {
                    GUI.Label(new Rect(10, Screen.height - 30, 100, 20), "Heat: " + (int)(m_BeamHeat * 100) + "/" + (int)(MaxBeamHeat * 100));
                }   
            }
        }

        private void OnDisable() {
            //还没换完弹就换枪
            if(m_IsReloading) {
                //重新开始换弹
                FireTimer = -ReloadTime;
                //停止换弹协程
                StopCoroutine(m_ReloadCoroutine);
            }

            //如果使用瞄准镜，那么关掉瞄准镜
            if (m_IsUsingMirror) {
                SwitchMirrorState();
            }

            WeaponModel.SetActive(false);    
        }
        #endregion

        //使用键盘输入触发武器开火
        void CheckForUserInput() {
            //射线武器
            if (Type == Weapons.WeaponType.Raycast) {
                if (FireTimer >= ActualROF && BurstCounter < BurstRate && m_CanFire) {
                    if (Input.GetMouseButton(0)) {
                        if (!Warmup) {   //没有蓄力射击，正常射击
                            Fire();
                        } else if (m_Heat < MaxWarmup) {   //开始蓄力
                            m_Heat += Time.deltaTime;
                        }
                    }
                    if (Warmup && Input.GetMouseButtonUp(0)) {
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
                    if (Input.GetMouseButton(0)) {
                        if (!Warmup) {   //没有蓄力射击，正常射击
                            Launch();
                        } else if (m_Heat < MaxWarmup) {   //开始蓄力
                            m_Heat += Time.deltaTime;
                        }
                    }
                    if (Warmup && Input.GetMouseButtonUp(0)) {
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
                if (Input.GetMouseButton(0) && m_BeamHeat <= MaxBeamHeat && !m_CoolingDown) {
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
            if (Input.GetButtonDown("Reload")) {
                Reload();
            }

            //如果是半自动武器，松开开火键时才能重新开火
            if (Input.GetMouseButtonUp(0)) {
                m_CanFire = true;
            }

            if (Input.GetMouseButtonDown(1)) {
                SwitchMirrorState();
            }
        }

        #region 非玩家调用函数
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
        #endregion

        #region RPC函数
        //射线武器，开火
        public void Fire() {
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Fire");
            }


            //重置计时器
            FireTimer = 0.0f;

            //增加已射击的子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi) {
                m_CanFire = false;
            }

            //判断当前是否仍有弹药
            if (m_CurrentAmmo <= 0) {
                DryFire();
                return;
            }

            //能够开枪，发送开枪消息
            SendMessage("StartShooting", SendMessageOptions.DontRequireReceiver);

            //如果不是无线弹药模式，减少弹药量
            if (!InfiniteAmmo)
                m_CurrentAmmo--;

            //单次射击，射出shotPerRound颗子弹
            for (int i = 0; i < ShotPerRound; i++) {
                Vector3 direction = m_CurrentShootPoint.forward;

                //计算当前的精确度
                float accuracyVary = (100 - CurrentAccuracy) / 1000;
                
                //根据精确度确定散射范围
                direction.x += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                direction.y += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                direction.z += UnityEngine.Random.Range(-accuracyVary, accuracyVary);
                CurrentAccuracy -= AccuracyDropPerShot;
                if (CurrentAccuracy <= 0.0f)
                    CurrentAccuracy = 0.0f;

                //创建射线
                Ray ray = new Ray(m_CurrentShootPoint.position, direction);

                bool hitResult;
                RaycastHit hit;

                int layer = LayerMask.NameToLayer(LayerMaskName);
                if (layer != -1) {
                    //过滤LayerMaskName这一层
                    LayerMask mask = ~(1 << layer);
                    hitResult = Physics.Raycast(ray, out hit, Range, mask);
                } else {
                    hitResult = Physics.Raycast(ray, out hit, Range);
                }

                if (hitResult) {
                        //蓄力
                        float damage = Power;
                    if (Warmup) {
                        damage *= m_Heat * PowerMultiplier;
                        m_Heat = 0.0f;
                    }

                    //造成伤害
                    ReactiveTarget target = hit.collider.gameObject.GetComponent<ReactiveTarget>();
                    if (target) {
                        target.OnHit(this.transform.root.name, false, Power);
                    } else {
                        target = hit.collider.transform.root.gameObject.GetComponent<ReactiveTarget>();
                        if (target) {
                            target.OnHit(this.transform.root.name, false, Power);
                        }
                    }


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
                    if (UseForce && hit.rigidbody) {
                        hit.rigidbody.AddForce(ray.direction * Power * ForceMultiplier);
                    }
                }
            }

            //如果使用后坐力，调用后坐力函数
            if (UseRecoil) {
                Recoil();
            }

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
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Launch");
            }


            //重置计时器
            FireTimer = 0.0f;

            //增加已发射的子弹数
            BurstCounter++;

            //半自动武器不能连续射击
            if (AutoMode == Auto.Semi) {
                m_CanFire = false;
            }

            //判断当前是否仍有弹药
            if (m_CurrentAmmo <= 0) {
                DryFire();
                return;
            }

            //能够开枪，发送开枪消息
            SendMessage("StartShooting", SendMessageOptions.DontRequireReceiver);

            //如果不是无限弹药模式，减少弹药量
            if (!InfiniteAmmo) {
                m_CurrentAmmo--;
            }
                

            //单次射击，射出shotPerRound颗子弹
            for (int i = 0; i < ShotPerRound; i++) {
                //创建抛射物
                if (Projectile != null) {
                    GameObject proj = Instantiate(Projectile, ProjectileSpawnSpot.position, ProjectileSpawnSpot.rotation) as GameObject;
                    //设置名称
                    proj.name = this.transform.root.name;

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
            if (UseRecoil) {
                Recoil();
            }
                

            //如果使用枪口火焰效果，就显示枪口火焰
            if (MakeMuzzleEffects) {
                GameObject muzfx = MuzzleEffects[Random.Range(0, MuzzleEffects.Length)];
                if (muzfx != null) {
                    Instantiate(muzfx, MuzzleEffectsPosition.position, MuzzleEffectsPosition.rotation);
                }
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
        public void Beam() {
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Beam");
            }

            //能够开枪，发送开枪消息
            SendMessage("StartShooting", SendMessageOptions.DontRequireReceiver);

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

                    //造成伤害
                    ReactiveTarget target = hit.collider.gameObject.GetComponent<ReactiveTarget>();
                    if (target) {
                        target.OnHit(this.transform.root.name, false, Power);
                    }


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
            //调用RPC函数
            if (IsLocalObject && Type == WeaponType.Beam) {
                m_NetSyncController.RPC(this, "StopBeam");
            }

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

        //换弹
        public void Reload() {
            //如果子弹数满、或者正在换弹、或者没有子弹可以补充，不执行任何操作
            if (m_CurrentAmmo == AmmoCapacity || m_IsReloading || m_CurrentTotalAmmo == 0) {
                return;
            }

            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "Reload");
            }

            if (m_IsUsingMirror) {
                SwitchMirrorState();
            }

            m_IsReloading = true;
            SendMessage("UpdateReloadState", m_IsReloading, SendMessageOptions.DontRequireReceiver);

            //停止计时器
            FireTimer = -ReloadTime;
            //播放换弹音效
            GetComponent<AudioSource>().PlayOneShot(ReloadSound);
            //开启换弹协程
            m_ReloadCoroutine = StartCoroutine(StartReload());
        }

        //换弹协程
        private IEnumerator StartReload() {
            while(FireTimer < 0f) {
                yield return null;
            }

            //更新弹药信息
            if (LimitedTotalAmmo) {
                if (m_CurrentTotalAmmo >= AmmoCapacity) {
                    //更新弹药量
                    m_CurrentAmmo = AmmoCapacity;
                    //更新总弹药量
                    m_CurrentTotalAmmo -= AmmoCapacity;
                } else {
                    //更新弹药量
                    m_CurrentAmmo = m_CurrentTotalAmmo;
                    //更新总弹药量
                    m_CurrentTotalAmmo = 0;
                }
            } else {
                //更新弹药量
                m_CurrentAmmo = AmmoCapacity;
            }

            m_IsReloading = false;
            SendMessage("UpdateReloadState", m_IsReloading, SendMessageOptions.DontRequireReceiver);
        }

        //切换瞄准镜的状态
        public void SwitchMirrorState() {
            //调用RPC函数
            if (IsLocalObject) {
                m_NetSyncController.RPC(this, "SwitchMirrorState");
            }

            if (!UseMirror) {
                return;
            }

            if (m_IsUsingMirror) {
                this.transform.localPosition = m_OriginalPosition;
                this.transform.localRotation = m_OriginalRotation;

                m_IsUsingMirror = false;
                m_CurrentShootPoint = RaycastStartSpot;

                if (UseMirrorCamera) {
                    MirrorCamera.SetActive(false);
                }
            } else {
                this.transform.position = MirrorSpot.position;
                this.transform.rotation = MirrorSpot.rotation;

                m_IsUsingMirror = true;
                m_CurrentShootPoint = MirrorRaycastingPoint;

                if (UseMirrorCamera) {
                    MirrorCamera.SetActive(true);
                }
            }

            SendMessage("UpdateMirrorState", m_IsUsingMirror, SendMessageOptions.DontRequireReceiver);
        }

        //射击时没有弹药
        void DryFire() {
            GetComponent<AudioSource>().PlayOneShot(DryFireSound);
        }
        #endregion

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

        public void RecvData(SyncData data) {
            throw new System.NotImplementedException();
        }

        public SyncData SendData() {
            return new SyncData();
        }

        public void Init(NetSyncController controller) {
            m_NetSyncController = controller;
        }
    }
}