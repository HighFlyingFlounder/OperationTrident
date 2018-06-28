using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using OperationTrident.Weapons;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor {
    private bool m_ShowPluginSupport = false;
    private bool m_ShowGeneral = false;
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
        weapon.Type = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", weapon.Type);

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
        m_ShowGeneral = EditorGUILayout.Foldout(m_ShowGeneral, "General");
        if (m_ShowGeneral) {
            weapon.PlayerWeapon = EditorGUILayout.Toggle("Player's Weapon", weapon.PlayerWeapon);
            if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile)
                weapon.AutoMode = (Auto)EditorGUILayout.EnumPopup("Auto Type", weapon.AutoMode);
            weapon.WeaponModel = (GameObject)EditorGUILayout.ObjectField("Weapon Model", weapon.WeaponModel, typeof(GameObject), true);
            if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Beam)
                weapon.RaycastStartSpot = (Transform)EditorGUILayout.ObjectField("Raycasting Point", weapon.RaycastStartSpot, typeof(Transform), true);

            //根据选择的武器类型显示不同的选项
            if (weapon.Type == WeaponType.Projectile) {
                weapon.Projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", weapon.Projectile, typeof(GameObject), false);
                weapon.ProjectileSpawnSpot = (Transform)EditorGUILayout.ObjectField("Projectile Spawn Point", weapon.ProjectileSpawnSpot, typeof(Transform), true);
            }
            if (weapon.Type == WeaponType.Beam) {
                weapon.Reflect = EditorGUILayout.Toggle("Reflect", weapon.Reflect);
                weapon.ReflectionMaterial = (Material)EditorGUILayout.ObjectField("Reflection Material", weapon.ReflectionMaterial, typeof(Material), false);
                weapon.MaxReflections = EditorGUILayout.IntField("Max Reflections", weapon.MaxReflections);
                weapon.BeamTypeName = EditorGUILayout.TextField("Beam Effect Name", weapon.BeamTypeName);
                weapon.MaxBeamHeat = EditorGUILayout.FloatField("Max Heat", weapon.MaxBeamHeat);
                weapon.InfiniteBeam = EditorGUILayout.Toggle("Infinite", weapon.InfiniteBeam);
                weapon.BeamMaterial = (Material)EditorGUILayout.ObjectField("Material", weapon.BeamMaterial, typeof(Material), false);
                weapon.BeamColor = EditorGUILayout.ColorField("Color", weapon.BeamColor);
                weapon.StartBeamWidth = EditorGUILayout.FloatField("Start Width", weapon.StartBeamWidth);
                weapon.EndBeamWidth = EditorGUILayout.FloatField("End Width", weapon.EndBeamWidth);
            }
            if (weapon.Type == WeaponType.Beam)
                weapon.ShowCurrentAmmo = EditorGUILayout.Toggle("Show Current Heat", weapon.ShowCurrentAmmo);

        }



        //绘制"Power"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Beam) {
            m_ShowPower = EditorGUILayout.Foldout(m_ShowPower, "Power");
            if (m_ShowPower) {
                if (weapon.Type == WeaponType.Raycast)
                    weapon.Power = EditorGUILayout.FloatField("Power", weapon.Power);
                else
                    weapon.BeamPower = EditorGUILayout.FloatField("Power", weapon.BeamPower);

                weapon.ForceMultiplier = EditorGUILayout.FloatField("Force Multiplier", weapon.ForceMultiplier);
                weapon.Range = EditorGUILayout.FloatField("Range", weapon.Range);
            }
        }


        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowROF = EditorGUILayout.Foldout(m_ShowROF, "Rate Of Fire");
            if (m_ShowROF) {
                weapon.RateOfFire = EditorGUILayout.FloatField("Rate Of Fire", weapon.RateOfFire);
                weapon.DelayBeforeFire = EditorGUILayout.FloatField("Delay Before Fire", weapon.DelayBeforeFire);
  
                weapon.BurstRate = EditorGUILayout.IntField("Burst Rate", weapon.BurstRate);
                weapon.BurstPause = EditorGUILayout.FloatField("Burst Pause", weapon.BurstPause);
            }
        }


        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowAmmo = EditorGUILayout.Foldout(m_ShowAmmo, "Ammunition");
            if (m_ShowAmmo) {
                weapon.InfiniteAmmo = EditorGUILayout.Toggle("Infinite Ammo", weapon.InfiniteAmmo);

                if (!weapon.InfiniteAmmo) {
                    weapon.AmmoCapacity = EditorGUILayout.IntField("Ammo Capacity", weapon.AmmoCapacity);
                    weapon.ReloadTime = EditorGUILayout.FloatField("Reload Time", weapon.ReloadTime);
                    weapon.ShowCurrentAmmo = EditorGUILayout.Toggle("Show Current Ammo", weapon.ShowCurrentAmmo);
                    weapon.ReloadAutomatically = EditorGUILayout.Toggle("Reload Automatically", weapon.ReloadAutomatically);
                }
                weapon.ShotPerRound = EditorGUILayout.IntField("Shots Per Round", weapon.ShotPerRound);
            }
        }



        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast) {
            m_ShowAccuracy = EditorGUILayout.Foldout(m_ShowAccuracy, "Accuracy");
            if (m_ShowAccuracy) {
                weapon.Accuracy = EditorGUILayout.FloatField("Accuracy", weapon.Accuracy);
                weapon.AccuracyDropPerShot = EditorGUILayout.FloatField("Accuracy Drop Per Shot", weapon.AccuracyDropPerShot);
                weapon.AccuracyRecoverRate = EditorGUILayout.FloatField("Accuracy Recover Rate", weapon.AccuracyRecoverRate);
            }
        }


        //绘制"Rate Of Fire"折叠框
        if ((weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) && weapon.AutoMode == Auto.Semi) {
            m_ShowWarmup = EditorGUILayout.Foldout(m_ShowWarmup, "Warmup");
            if (m_ShowWarmup) {
                weapon.Warmup = EditorGUILayout.Toggle("Warmup", weapon.Warmup);

                if (weapon.Warmup) {
                    weapon.MaxWarmup = EditorGUILayout.FloatField("Max Warmup", weapon.MaxWarmup);

                    if (weapon.Type == WeaponType.Projectile) {
                        weapon.MultiplyForce = EditorGUILayout.Toggle("Multiply Force", weapon.MultiplyForce);
                        if (weapon.MultiplyForce)
                            weapon.InitialForceMultiplier = EditorGUILayout.FloatField("Initial Force Multiplier", weapon.InitialForceMultiplier);

                        weapon.MultiplyPower = EditorGUILayout.Toggle("Multiply Power", weapon.MultiplyPower);
                        if (weapon.MultiplyPower)
                            weapon.PowerMultiplier = EditorGUILayout.FloatField("Power Multiplier", weapon.PowerMultiplier);
                    } else {
                        weapon.PowerMultiplier = EditorGUILayout.FloatField("Power Multiplier", weapon.PowerMultiplier);
                    }
                    weapon.AllowCancel = EditorGUILayout.Toggle("Allow Cancel", weapon.AllowCancel);
                }
            }
        }


        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast || weapon.Type == WeaponType.Projectile) {
            m_ShowRecoil = EditorGUILayout.Foldout(m_ShowRecoil, "Recoil");
            if (m_ShowRecoil) {
                weapon.UseRecoil = EditorGUILayout.Toggle("Recoil", weapon.UseRecoil);

                if (weapon.UseRecoil) {
                    weapon.RecoilKickBackMin = EditorGUILayout.FloatField("Recoil Move Min", weapon.RecoilKickBackMin);
                    weapon.RecoilKickBackMax = EditorGUILayout.FloatField("Recoil Move Max", weapon.RecoilKickBackMax);
                    weapon.RecoilRotationMin = EditorGUILayout.FloatField("Recoil Rotation Min", weapon.RecoilRotationMin);
                    weapon.RecoilRotationMax = EditorGUILayout.FloatField("Recoil Rotation Max", weapon.RecoilRotationMax);
                    weapon.RecoilRecoveryRate = EditorGUILayout.FloatField("Recoil Recovery Rate", weapon.RecoilRecoveryRate);
                }
            }
        }


        //绘制"Rate Of Fire"折叠框
        m_ShowEffects = EditorGUILayout.Foldout(m_ShowEffects, "Effects");
        if (m_ShowEffects) {
            weapon.UseSpitShells = EditorGUILayout.Toggle("Spit Shells", weapon.UseSpitShells);
            if (weapon.UseSpitShells) {
                weapon.Shell = (GameObject)EditorGUILayout.ObjectField("Shell", weapon.Shell, typeof(GameObject), false);
                weapon.ShellSpitForce = EditorGUILayout.FloatField("Shell Spit Force", weapon.ShellSpitForce);
                weapon.ShellForceRandom = EditorGUILayout.FloatField("Force Variant", weapon.ShellForceRandom);
                weapon.ShellSpitTorqueX = EditorGUILayout.FloatField("X Torque", weapon.ShellSpitTorqueX);
                weapon.ShellSpitTorqueY = EditorGUILayout.FloatField("Y Torque", weapon.ShellSpitTorqueY);
                weapon.ShellTorqueRandom = EditorGUILayout.FloatField("Torque Variant", weapon.ShellTorqueRandom);
                weapon.ShellSpitPosition = (Transform)EditorGUILayout.ObjectField("Shell Spit Point", weapon.ShellSpitPosition, typeof(Transform), true);
            }

            //枪口特效
            EditorGUILayout.Separator();
            weapon.MakeMuzzleEffects = EditorGUILayout.Toggle("Muzzle Effects", weapon.MakeMuzzleEffects);
            if (weapon.MakeMuzzleEffects) {
                weapon.MuzzleEffectsPosition = (Transform)EditorGUILayout.ObjectField("Muzzle FX Spawn Point", weapon.MuzzleEffectsPosition, typeof(Transform), true);

                if (GUILayout.Button("Add")) {
                    List<GameObject> temp = new List<GameObject>(weapon.MuzzleEffects);
                    temp.Add(null);
                    weapon.MuzzleEffects = temp.ToArray();
                }
                EditorGUI.indentLevel++;
                for (int i = 0; i < weapon.MuzzleEffects.Length; i++) {
                    EditorGUILayout.BeginHorizontal();
                    weapon.MuzzleEffects[i] = (GameObject)EditorGUILayout.ObjectField("Muzzle FX Prefabs", weapon.MuzzleEffects[i], typeof(GameObject), false);
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
                weapon.MakeHitEffects = EditorGUILayout.Toggle("Hit Effects", weapon.MakeHitEffects);
                if (weapon.MakeHitEffects) {
                    if (GUILayout.Button("Add")) {
                        List<GameObject> temp = new List<GameObject>(weapon.HitEffects);
                        temp.Add(null);
                        weapon.HitEffects = temp.ToArray();
                    }
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < weapon.HitEffects.Length; i++) {
                        EditorGUILayout.BeginHorizontal();
                        weapon.HitEffects[i] = (GameObject)EditorGUILayout.ObjectField("Hit FX Prefabs", weapon.HitEffects[i], typeof(GameObject), false);
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


        //绘制"Rate Of Fire"折叠框
        if (weapon.Type == WeaponType.Raycast) {
            m_ShowBulletHoles = EditorGUILayout.Foldout(m_ShowBulletHoles, "Bullet Holes");
            if (m_ShowBulletHoles) {

                weapon.MakeBulletHoles = EditorGUILayout.Toggle("Bullet Holes", weapon.MakeBulletHoles);

                if (weapon.MakeBulletHoles) {
                    weapon.BHSystem = (BulletHoleSystem)EditorGUILayout.EnumPopup("Determined By", weapon.BHSystem);

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
                    EditorGUILayout.LabelField("Default Bullet Holes");

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
                    EditorGUILayout.LabelField("Exceptions");

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
                        EditorGUILayout.HelpBox("Assign bullet hole prefabs corresponding with tags, materials, or physic materials.  If you assign multiple bullet holes to the same parameter, one of them will be chosen at random.  The default bullet hole will be used when something is hit that doesn't match any of the other parameters.  The exceptions define parameters for which no bullet holes will be instantiated.", MessageType.None);
                    }
                }

            }
        }


        //绘制"Crosshairs"折叠框
        m_ShowCrosshairs = EditorGUILayout.Foldout(m_ShowCrosshairs, "Crosshairs");
        if (m_ShowCrosshairs) {
            weapon.ShowCrosshair = EditorGUILayout.Toggle("Show Crosshairs", weapon.ShowCrosshair);
            if (weapon.ShowCrosshair) {
                weapon.CrosshairTexture = (Texture2D)EditorGUILayout.ObjectField("Crosshair Texture", weapon.CrosshairTexture, typeof(Texture2D), false);
                weapon.CrosshairLength = EditorGUILayout.IntField("Crosshair Length", weapon.CrosshairLength);
                weapon.CrosshairWidth = EditorGUILayout.IntField("Crosshair Width", weapon.CrosshairWidth);
                weapon.StartingCrosshairSize = EditorGUILayout.FloatField("Start Crosshair Size", weapon.StartingCrosshairSize);
            }
        }


        //绘制"Audio"折叠框
        m_ShowAudio = EditorGUILayout.Foldout(m_ShowAudio, "Audio");
        if (m_ShowAudio) {
            weapon.FireSound = (AudioClip)EditorGUILayout.ObjectField("Fire", weapon.FireSound, typeof(AudioClip), false);
            weapon.ReloadSound = (AudioClip)EditorGUILayout.ObjectField("Reload", weapon.ReloadSound, typeof(AudioClip), false);
            weapon.DryFireSound = (AudioClip)EditorGUILayout.ObjectField("Out of Ammo", weapon.DryFireSound, typeof(AudioClip), false);
        }


        //当值改变时，让Inspector面板重新绘制
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

