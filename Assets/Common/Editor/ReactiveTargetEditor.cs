using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using OperationTrident.Common;

[CustomEditor(typeof(ReactiveTarget))]
public class ReactiveTargetEditor : Editor {

    public override void OnInspectorGUI() {
        ReactiveTarget reactiveTarget = (ReactiveTarget)target;

        reactiveTarget.IsPlayer = EditorGUILayout.Toggle(new GUIContent("Is Player", "是否为Player对象"), reactiveTarget.IsPlayer);
        reactiveTarget.CanBeHurt = EditorGUILayout.Toggle(new GUIContent("Can Be Hurt", "能否受到伤害"), reactiveTarget.CanBeHurt);
        reactiveTarget.MaxHealth = EditorGUILayout.FloatField(new GUIContent("Max Health", "最大血量值"), reactiveTarget.MaxHealth);
        reactiveTarget.ShowHealth = EditorGUILayout.Toggle(new GUIContent("Show Health", "是否在界面上显示剩余血量值"), reactiveTarget.ShowHealth);


        if (reactiveTarget.IsPlayer) {
            reactiveTarget.ReplaceWhenDie = EditorGUILayout.Toggle(new GUIContent("Replace When Die", "死亡之后是否生成替换模型"), reactiveTarget.ReplaceWhenDie);
            if (reactiveTarget.ReplaceWhenDie) {
                //只接受prefab
                reactiveTarget.DeadReplacement = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Dead Replacememnt", "用于替换的模型"), reactiveTarget.DeadReplacement, typeof(GameObject), false);
            }

            reactiveTarget.UseDeadCamera = EditorGUILayout.Toggle(new GUIContent("Use Dead Camera", "死亡之后是否使用Camera"), reactiveTarget.UseDeadCamera);
            if (reactiveTarget.UseDeadCamera) {
                reactiveTarget.DeathCamera = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Death Camera", "死亡之后激活的摄像机"), reactiveTarget.DeathCamera, typeof(GameObject), true);
            }
        } else {
            reactiveTarget.MakeExplosion = EditorGUILayout.Toggle(new GUIContent("Make Explosion", "死亡之后是否生成爆炸特效"), reactiveTarget.MakeExplosion);
            if (reactiveTarget.MakeExplosion) {
                //只接受Prefab
                reactiveTarget.Explosion = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Explosion", "爆炸特效"), reactiveTarget.Explosion, typeof(GameObject), false);
            }
        }

        reactiveTarget.AC = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Hit Sound", "啊！我被击中了！"), reactiveTarget.AC, typeof(AudioClip), false);

        //当值改变时，让Inspector面板重新绘制
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }
}
