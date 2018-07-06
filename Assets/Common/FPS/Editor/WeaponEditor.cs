using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using OperationTrident.FPS.Weapons;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor {
    private bool m_ShowPluginSupport = false;
    private bool m_ShowGeneral = false;
    private bool m_ShowMirror = false;
    private bool m_ShowAmmo = false;
    private bool m_ShowROF = false;
    private bool m_ShowPower = false;
    private bool m_ShowAccuracy = false;
    private bool m_ShowWarmup = false;
    private bool m_ShowRecoil = false;
    private bool m_ShowEffects = false;
    private bool m_ShowBulletHoles = false;
    private bool m_ShowCrosshairs = false;
    private bool m_ShowAudio = false;

    public override void OnInspectorGUI() {
        //获得脚本引用
        Weapon weapon = (Weapon)target;

        //武器种类
        weapon.Type = (WeaponType)EditorGUILayout.EnumPopup(new GUIContent("Weapon Type", "武器射击的方式"), weapon.Type);

        ////绘制"3rd Party Plugin Support"折叠框
        //m_ShowPluginSupport = EditorGUILayout.Foldout(m_ShowPluginSupport, "3rd Party Plugin Support");
        //if (m_ShowPluginSupport) {
        //    //显示单选框
        //    weapon.ShooterAIEnabled = EditorGUILayout.Toggle(new GUIContent("Shooter AI", "Support for Shooter AI by Gateway Games"), weapon.ShooterAIEnabled);

        //    //显示单选框
        //    weapon.BloodyMessEnabled = EditorGUILayout.Toggle(new GUIContent("Bloody Mess"), weapon.BloodyMessEnabled);
        //    if (weapon.BloodyMessEnabled) {
        //        weapon.WeaponType = EditorGUILayout.IntField("Weapon Type", weapon.WeaponType);
        //    }
        //}

        //绘制"General"折叠框
        m_ShowGeneral = EditorGUILayout.Foldout(m_ShowGeneral, new GUIContent("General", "武器的通用参数"));
        if (m_ShowGeneral) {
            weapon.IsLocalObject = EditorGUILayout.Toggle(new GUIContent("Is Local Object", "当前的Object是否为本地Object，如果不是，则只接受网络同步信息"), weapon.IsLocalObject);
            weapon.PlayerWeapon = EditorGUILayout.Toggle(new GUIContent("Player's Weapon", "是否为Player使用的武器"), weapon.PlayerWeapon);
            if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile)
                weapon.AutoMode = (Auto)EditorGUILayout.EnumPopup(new GUIContent("Auto Type", "开枪模式，全自动或者半自动"), weapon.AutoMode);
            weapon.WeaponModel = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Model", "武器模型对象"), weapon.WeaponModel, typeof(GameObject), true);
            if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Beam)
                weapon.RaycastStartSpot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Raycasting Point", "发射的起点和方向"), weapon.RaycastStartSpot, typeof(Transform), true);

            //根据选择的武器类型显示不同的选项
            if (weapon.Type == WeaponType.Projectile) {
                weapon.Projectile = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Projectile", "被射出的抛射物"), weapon.Projectile, typeof(GameObject), false);
                weapon.ProjectileSpawnSpot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Projectile Spawn Point", "抛射物射出的起始位置和方向"), weapon.ProjectileSpawnSpot, typeof(Transform), true);
            }
            if (weapon.Type == WeaponType.Beam) {
                weapon.Reflect = EditorGUILayout.Toggle(new GUIContent("Reflect", "能否进行反射"), weapon.Reflect);
                weapon.ReflectionMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Reflection Material", "能反射激光的材质"), weapon.ReflectionMaterial, typeof(Material), false);
                weapon.MaxReflections = EditorGUILayout.IntField(new GUIContent("Max Reflections", "最大反射次数"), weapon.MaxReflections);
                weapon.BeamTypeName = EditorGUILayout.TextField(new GUIContent("Beam Effect Name", "激光名称，非必需"), weapon.BeamTypeName);
                weapon.MaxBeamHeat = EditorGUILayout.FloatField(new GUIContent("Max Heat", "激光最长持续时间"), weapon.MaxBeamHeat);
                weapon.InfiniteBeam = EditorGUILayout.Toggle(new GUIContent("Infinite", "能否无限发射激光"), weapon.InfiniteBeam);
                weapon.BeamMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Material", "激光使用的材质"), weapon.BeamMaterial, typeof(Material), false);
                weapon.BeamColor = EditorGUILayout.ColorField(new GUIContent("Color", "激光的颜色"), weapon.BeamColor);
                weapon.StartBeamWidth = EditorGUILayout.FloatField(new GUIContent("Start Width", "激光初始粗细"), weapon.StartBeamWidth);
                weapon.EndBeamWidth = EditorGUILayout.FloatField(new GUIContent("End Width", "激光最终粗细"), weapon.EndBeamWidth);
            }
            if (weapon.Type == WeaponType.Beam)
                weapon.ShowCurrentAmmo = EditorGUILayout.Toggle(new GUIContent("Show Current Heat", ""), weapon.ShowCurrentAmmo);

        }

        //只有射线武器可以设置瞄准镜参数
        if(weapon.Type == WeaponType.Raycast) {
            //绘制"Mirror"折叠框
            m_ShowMirror = EditorGUILayout.Foldout(m_ShowMirror, new GUIContent("Mirror", "武器的瞄准镜参数"));
            if (m_ShowMirror) {
                weapon.UseMirror = EditorGUILayout.Toggle(new GUIContent("Use Mirror", "是否使用瞄准镜"), weapon.UseMirror);

                //根据选择的武器类型显示不同的选项
                if (weapon.UseMirror) {
                    weapon.MirrorSpot = (Transform)EditorGUILayout.ObjectField(new GUIContent("Mirror Spot", "开镜时枪的位置和朝向"), weapon.MirrorSpot, typeof(Transform), true);
                    weapon.UseMirrorCamera = EditorGUILayout.Toggle(new GUIContent("Use Mirror Camera", "开镜时是否使用其它的Camera"), weapon.UseMirrorCamera);
                    weapon.MirrorRaycastingPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Mirror Raycasting Point", "开镜时子弹发射的位置和方向"), weapon.MirrorRaycastingPoint, typeof(Transform), true);

                    if (weapon.UseMirrorCamera) {
                        weapon.MirrorCamera = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Mirror Camera", "开镜时使用的Camera对象"), weapon.MirrorCamera, typeof(GameObject), true);
                    }
                }
            }
        }

        //绘制"Power"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Beam) {
            m_ShowPower = EditorGUILayout.Foldout(m_ShowPower, new GUIContent("Power", "武器的威力参数"));
            if (m_ShowPower) {
                if (weapon.Type == WeaponType.Raycast) {
                    weapon.UseForce = EditorGUILayout.Toggle(new GUIContent("Use Force", "是否对集中的物体施加击退力"), weapon.UseForce);
                    weapon.Power = EditorGUILayout.FloatField(new GUIContent("Power", "武器的伤害大小"), weapon.Power);

                    //如果使用击退力，就设置击退力增幅系数
                    if (weapon.UseForce) {
                        weapon.ForceMultiplier = EditorGUILayout.FloatField(new GUIContent("Force Multiplier", "武器击退力的增幅系数，击退力等于Power * ForceMultiplier"), weapon.ForceMultiplier);
                    }
                } else {
                    weapon.BeamPower = EditorGUILayout.FloatField(new GUIContent("Power", "激光单位时间造成的伤害"), weapon.BeamPower);
                }
                
                weapon.Range = EditorGUILayout.FloatField(new GUIContent("Range", "武器的射击距离"), weapon.Range);
            }
        }


        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowROF = EditorGUILayout.Foldout(m_ShowROF, new GUIContent("Rate Of Fire", "武器的射速参数"));
            if (m_ShowROF) {
                weapon.RateOfFire = EditorGUILayout.FloatField(new GUIContent("Rate Of Fire", "每秒钟开枪的次数"), weapon.RateOfFire);
                weapon.DelayBeforeFire = EditorGUILayout.FloatField(new GUIContent("Delay Before Fire", "射击前等待的时间"), weapon.DelayBeforeFire);
  
                weapon.BurstRate = EditorGUILayout.IntField(new GUIContent("Burst Rate", "触发卡壳的子弹数"), weapon.BurstRate);
                weapon.BurstPause = EditorGUILayout.FloatField(new GUIContent("Burst Pause", "卡壳时不能射击的时长"), weapon.BurstPause);
            }
        }


        //绘制"Ammunition"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowAmmo = EditorGUILayout.Foldout(m_ShowAmmo, new GUIContent("Ammunition", "武器的弹药参数"));
            if (m_ShowAmmo) {
                weapon.InfiniteAmmo = EditorGUILayout.Toggle(new GUIContent("Infinite Ammo", "是否能无限射击"), weapon.InfiniteAmmo);

                if (!weapon.InfiniteAmmo) {
                    weapon.AmmoCapacity = EditorGUILayout.IntField(new GUIContent("Ammo Capacity", "武器的弹夹容量"), weapon.AmmoCapacity);
                    weapon.ReloadTime = EditorGUILayout.FloatField(new GUIContent("Reload Time", "更换弹夹的时间"), weapon.ReloadTime);
                    weapon.ShowCurrentAmmo = EditorGUILayout.Toggle(new GUIContent("Show Current Ammo", "是否显示当前弹夹剩余的弹药量"), weapon.ShowCurrentAmmo);
                    weapon.ReloadAutomatically = EditorGUILayout.Toggle(new GUIContent("Reload Automatically", "在弹药量用光时是否自动更换弹夹"), weapon.ReloadAutomatically);
                }
                weapon.ShotPerRound = EditorGUILayout.IntField(new GUIContent("Shots Per Round", "单次射击（弹药量减一）射出的子弹数"), weapon.ShotPerRound);
            }
        }



        //绘制"Accuracy"折叠框
        if (weapon.Type == WeaponType.Raycast) {
            m_ShowAccuracy = EditorGUILayout.Foldout(m_ShowAccuracy, new GUIContent("Accuracy", "武器的散射参数"));
            if (m_ShowAccuracy) {
                //weapon.Accuracy = EditorGUILayout.FloatField(new GUIContent("Accuracy", "最高射击准度"), weapon.Accuracy);
                weapon.Accuracy = EditorGUILayout.Slider(new GUIContent("Accuracy", "最高射击准度"), weapon.Accuracy, 0f, 100f);
                weapon.AccuracyDropPerShot = EditorGUILayout.FloatField(new GUIContent("Accuracy Drop Per Shot", "单次射击射击准度下降的幅度"), weapon.AccuracyDropPerShot);
                weapon.AccuracyRecoverRate = EditorGUILayout.FloatField(new GUIContent("Accuracy Recover Rate", "射击后射击准度的恢复速度"), weapon.AccuracyRecoverRate);
            }
        }


        //绘制"Warmup"折叠框
        if ((weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) && weapon.AutoMode == Auto.Semi) {
            m_ShowWarmup = EditorGUILayout.Foldout(m_ShowWarmup, new GUIContent("Warmup", "武器的蓄力射击参数"));
            if (m_ShowWarmup) {
                weapon.Warmup = EditorGUILayout.Toggle(new GUIContent("Warmup", "是否开启蓄力射击"), weapon.Warmup);

                if (weapon.Warmup) {
                    weapon.MaxWarmup = EditorGUILayout.FloatField(new GUIContent("Max Warmup", "最长蓄力时间"), weapon.MaxWarmup);

                    if (weapon.Type == WeaponType.Projectile) {
                        weapon.MultiplyForce = EditorGUILayout.Toggle(new GUIContent("Multiply Force", "蓄力是否提高击退力"), weapon.MultiplyForce);
                        if (weapon.MultiplyForce)
                            weapon.InitialForceMultiplier = EditorGUILayout.FloatField(new GUIContent("Initial Force Multiplier", "初始击退力增幅系数"), weapon.InitialForceMultiplier);

                        weapon.MultiplyPower = EditorGUILayout.Toggle(new GUIContent("Multiply Power", "蓄力能否提高伤害"), weapon.MultiplyPower);
                        if (weapon.MultiplyPower)
                            weapon.PowerMultiplier = EditorGUILayout.FloatField(new GUIContent("Power Multiplier", "武器伤害增幅系数"), weapon.PowerMultiplier);
                    } else {
                        weapon.PowerMultiplier = EditorGUILayout.FloatField(new GUIContent("Power Multiplier", "武器伤害增幅系数"), weapon.PowerMultiplier);
                    }
                    weapon.AllowCancel = EditorGUILayout.Toggle(new GUIContent("Allow Cancel", "蓄力时是否允许取消射击"), weapon.AllowCancel);
                }
            }
        }


        //绘制"Recoil"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowRecoil = EditorGUILayout.Foldout(m_ShowRecoil, new GUIContent("Recoil", "武器的后坐力参数"));
            if (m_ShowRecoil) {
                weapon.UseRecoil = EditorGUILayout.Toggle(new GUIContent("Use Recoil", "是否使用后坐力"), weapon.UseRecoil);

                if (weapon.UseRecoil) {
                    weapon.RecoilKickBackMin = EditorGUILayout.FloatField(new GUIContent("Recoil Move Min", "射击时武器抖动最近距离"), weapon.RecoilKickBackMin);
                    weapon.RecoilKickBackMax = EditorGUILayout.FloatField(new GUIContent("Recoil Move Max", "射击时武器抖动最远距离"), weapon.RecoilKickBackMax);
                    weapon.RecoilRotationMin = EditorGUILayout.FloatField(new GUIContent("Recoil Rotation Min", "射击时武器选择的最小角度"), weapon.RecoilRotationMin);
                    weapon.RecoilRotationMax = EditorGUILayout.FloatField(new GUIContent("Recoil Rotation Max", "射击时武器选择的最大角度"), weapon.RecoilRotationMax);
                    weapon.RecoilMaxAngle = EditorGUILayout.FloatField(new GUIContent("Recoil Max Angle", "单次射击镜头上抬的角度"), weapon.RecoilMaxAngle);
                    weapon.RecoilRecoveryRate = EditorGUILayout.FloatField(new GUIContent("Recoil Recovery Rate", "射击后恢复至初始位置的速度"), weapon.RecoilRecoveryRate);
                }
            }
        }


        //绘制"Effects"折叠框
        m_ShowEffects = EditorGUILayout.Foldout(m_ShowEffects, new GUIContent("Effects", "武器的特效参数"));
        if (m_ShowEffects) {
            weapon.UseSpitShells = EditorGUILayout.Toggle(new GUIContent("Spit Shells", "射击时是否抛出弹壳"), weapon.UseSpitShells);
            if (weapon.UseSpitShells) {
                weapon.Shell = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Shell", "射击时是否吐出弹壳"), weapon.Shell, typeof(GameObject), false);
                weapon.ShellSpitForce = EditorGUILayout.FloatField(new GUIContent("Shell Spit Force", "抛出弹壳的力"), weapon.ShellSpitForce);
                weapon.ShellForceRandom = EditorGUILayout.FloatField(new GUIContent("Force Variant", "抛出弹壳的力的变化范围"), weapon.ShellForceRandom);
                weapon.ShellSpitTorqueX = EditorGUILayout.FloatField(new GUIContent("X Torque", "弹壳抛出时在x轴上的初始旋转角度"), weapon.ShellSpitTorqueX);
                weapon.ShellSpitTorqueY = EditorGUILayout.FloatField(new GUIContent("Y Torque", "弹壳抛出时在y轴上的初始旋转角度"), weapon.ShellSpitTorqueY);
                weapon.ShellTorqueRandom = EditorGUILayout.FloatField(new GUIContent("Torque Variant", "弹壳抛出时旋转角度的变化范围"), weapon.ShellTorqueRandom);
                weapon.ShellSpitPosition = (Transform)EditorGUILayout.ObjectField(new GUIContent("Shell Spit Point", "弹壳抛出时旋转角度的变化范围"), weapon.ShellSpitPosition, typeof(Transform), true);
            }

            //枪口特效
            EditorGUILayout.Separator();
            weapon.MakeMuzzleEffects = EditorGUILayout.Toggle(new GUIContent("Muzzle Effects", "是否使用枪口火光特效"), weapon.MakeMuzzleEffects);
            if (weapon.MakeMuzzleEffects) {
                weapon.MuzzleEffectsPosition = (Transform)EditorGUILayout.ObjectField(new GUIContent("Muzzle FX Spawn Point", "枪口火光特效产生的位置"), weapon.MuzzleEffectsPosition, typeof(Transform), true);

                if (GUILayout.Button("Add")) {
                    List<GameObject> temp = new List<GameObject>(weapon.MuzzleEffects);
                    temp.Add(null);
                    weapon.MuzzleEffects = temp.ToArray();
                }
                EditorGUI.indentLevel++;
                for (int i = 0; i < weapon.MuzzleEffects.Length; i++) {
                    EditorGUILayout.BeginHorizontal();
                    weapon.MuzzleEffects[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Muzzle FX Prefabs", "枪口火光特效对象"), weapon.MuzzleEffects[i], typeof(GameObject), false);
                    if (GUILayout.Button("Remove")) {
                        List<GameObject> temp = new List<GameObject>(weapon.MuzzleEffects);
                        temp.Remove(temp[i]);
                        weapon.MuzzleEffects = temp.ToArray();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            //击中特效
            if (weapon.Type != WeaponType.Projectile) {
                EditorGUILayout.Separator();
                weapon.MakeHitEffects = EditorGUILayout.Toggle(new GUIContent("Hit Effects", "子弹打中物体时是否产生特效"), weapon.MakeHitEffects);
                if (weapon.MakeHitEffects) {
                    if (GUILayout.Button("Add")) {
                        List<GameObject> temp = new List<GameObject>(weapon.HitEffects);
                        temp.Add(null);
                        weapon.HitEffects = temp.ToArray();
                    }
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < weapon.HitEffects.Length; i++) {
                        EditorGUILayout.BeginHorizontal();
                        weapon.HitEffects[i] = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Hit FX Prefabs", "子弹打中时产生的特效"), weapon.HitEffects[i], typeof(GameObject), false);
                        if (GUILayout.Button("Remove")) {
                            List<GameObject> temp = new List<GameObject>(weapon.HitEffects);
                            temp.Remove(temp[i]);
                            weapon.HitEffects = temp.ToArray();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }


        //绘制"Bullet Holes"折叠框
        if (weapon.Type == WeaponType.Raycast) {
            m_ShowBulletHoles = EditorGUILayout.Foldout(m_ShowBulletHoles, new GUIContent("Bullet Holes", "武器的弹孔参数"));
            if (m_ShowBulletHoles) {

                weapon.MakeBulletHoles = EditorGUILayout.Toggle(new GUIContent("Bullet Holes", "是否产生弹孔"), weapon.MakeBulletHoles);

                if (weapon.MakeBulletHoles) {
                    weapon.BHSystem = (BulletHoleSystem)EditorGUILayout.EnumPopup(new GUIContent("Determined By", "产生弹孔的条件"), weapon.BHSystem);

                    if (GUILayout.Button("Add")) {
                        weapon.BulletHoleGroups.Add(new SmartBulletHoleGroup());
                        weapon.BulletHolePoolNames.Add("Default");
                    }

                    //设置布局
                    EditorGUILayout.BeginVertical();

                    for (int i = 0; i < weapon.BulletHolePoolNames.Count; i++) {
                        //设置布局
                        EditorGUILayout.BeginHorizontal();

                        if (weapon.BHSystem == BulletHoleSystem.Tag) {
                            weapon.BulletHoleGroups[i].Tag = EditorGUILayout.TextField(weapon.BulletHoleGroups[i].Tag);
                        } else if (weapon.BHSystem == BulletHoleSystem.Material) {
                            weapon.BulletHoleGroups[i].Material = (Material)EditorGUILayout.ObjectField(weapon.BulletHoleGroups[i].Material, typeof(Material), false);
                        } else if (weapon.BHSystem == BulletHoleSystem.Physic_Material) {
                            weapon.BulletHoleGroups[i].PhysicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(weapon.BulletHoleGroups[i].PhysicMaterial, typeof(PhysicMaterial), false);
                        }

                        weapon.BulletHolePoolNames[i] = EditorGUILayout.TextField(weapon.BulletHolePoolNames[i]);

                        //绘制删除按钮
                        if (GUILayout.Button("Remove")) {
                            weapon.BulletHoleGroups.Remove(weapon.BulletHoleGroups[i]);
                            weapon.BulletHolePoolNames.Remove(weapon.BulletHolePoolNames[i]);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    //绘制标题"Default Bullet Holes"
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(new GUIContent("Default Bullet Holes", "默认弹孔"));

                    //绘制添加按钮
                    if (GUILayout.Button("Add")) {
                        weapon.DefaultBulletHoles.Add(null);
                        weapon.DefaultBulletHolePoolNames.Add("Default");
                    }

                    for (int i = 0; i < weapon.DefaultBulletHolePoolNames.Count; i++) {
                        EditorGUILayout.BeginHorizontal();

                        weapon.DefaultBulletHolePoolNames[i] = EditorGUILayout.TextField(weapon.DefaultBulletHolePoolNames[i]);

                        if (GUILayout.Button("Remove")) {
                            weapon.DefaultBulletHoles.Remove(weapon.DefaultBulletHoles[i]);
                            weapon.DefaultBulletHolePoolNames.Remove(weapon.DefaultBulletHolePoolNames[i]);

                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    //绘制标题"Exceptions"
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(new GUIContent("Exceptions", "不产生弹孔的条件"));

                    if (GUILayout.Button("Add")) {
                        weapon.BulletHoleExceptions.Add(new SmartBulletHoleGroup());
                    }

                    for (int i = 0; i < weapon.BulletHoleExceptions.Count; i++) {
                        EditorGUILayout.BeginHorizontal();

                        if (weapon.BHSystem == BulletHoleSystem.Tag) {
                            weapon.BulletHoleExceptions[i].Tag = EditorGUILayout.TextField(weapon.BulletHoleExceptions[i].Tag);
                        } else if (weapon.BHSystem == BulletHoleSystem.Material) {
                            weapon.BulletHoleExceptions[i].Material = (Material)EditorGUILayout.ObjectField(weapon.BulletHoleExceptions[i].Material, typeof(Material), false);
                        } else if (weapon.BHSystem == BulletHoleSystem.Physic_Material) {
                            weapon.BulletHoleExceptions[i].PhysicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(weapon.BulletHoleExceptions[i].PhysicMaterial, typeof(PhysicMaterial), false);
                        }


                        if (GUILayout.Button("Remove")) {
                            weapon.BulletHoleExceptions.Remove(weapon.BulletHoleExceptions[i]);
                        }

                        EditorGUILayout.EndHorizontal();
                    }


                    EditorGUILayout.EndVertical();

                    if (weapon.BulletHoleGroups.Count > 0) {
                        EditorGUILayout.HelpBox("设置弹孔参数时，首选选择弹孔产生的条件，然后点击Add按钮，创建一个弹孔产生条件，其中第一个参数为触发弹孔产生的条件（如果选择Tag（Material、PhysicalMaterial），那么则是能产生弹孔的GameObject对应的TagTag（Material、PhysicalMaterial）），第二个参数为相应的弹孔池对象名（一个弹孔池有很多相同类型的弹孔子对象）。当打中的物体不满足任何一个已创建的条件时，并且也不满足不产生弹孔的条件时，将会产生默认弹孔，点击Add按钮，然后输入默认弹孔池对象名即可设置默认弹孔。Exceptions（不产生弹孔的条件）设置和产生弹孔的条件一样。", MessageType.None);
                    }
                }

            }
        }


        //绘制"Crosshairs"折叠框
        m_ShowCrosshairs = EditorGUILayout.Foldout(m_ShowCrosshairs, new GUIContent("Crosshairs", "武器的准星参数"));
        if (m_ShowCrosshairs) {
            weapon.ShowCrosshair = EditorGUILayout.Toggle(new GUIContent("Show Crosshairs", "是否绘制准星"), weapon.ShowCrosshair);
            if (weapon.ShowCrosshair) {
                weapon.CrosshairTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Crosshair Texture", "用于绘制准星的贴图"), weapon.CrosshairTexture, typeof(Texture2D), false);
                weapon.CrosshairLength = EditorGUILayout.IntField(new GUIContent("Crosshair Length", "准星的长度"), weapon.CrosshairLength);
                weapon.CrosshairWidth = EditorGUILayout.IntField(new GUIContent("Crosshair Width", "准星的宽度"), weapon.CrosshairWidth);
                weapon.StartingCrosshairSize = EditorGUILayout.FloatField(new GUIContent("Start Crosshair Size", "准星的初始大小"), weapon.StartingCrosshairSize);
            }
        }


        //绘制"Audio"折叠框
        m_ShowAudio = EditorGUILayout.Foldout(m_ShowAudio, new GUIContent("Audio", "武器的音效设置"));
        if (m_ShowAudio) {
            weapon.FireSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Fire", "射击音效"), weapon.FireSound, typeof(AudioClip), false);
            weapon.ReloadSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Reload", "补充弹药音效"), weapon.ReloadSound, typeof(AudioClip), false);
            weapon.DryFireSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Out of Ammo", "弹夹没有弹药时射击的音效"), weapon.DryFireSound, typeof(AudioClip), false);
        }


        //当值改变时，让Inspector面板重新绘制
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }
}

