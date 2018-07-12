using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    [CreateAssetMenu(menuName="AI/Weapon Type")]
    public class AIWeaponType : ScriptableObject
    {
		[Header("Bullet")]
        public Transform projectile; // Projectile prefab
        public Transform muzzle; // Muzzle flash prefab  
        public Transform impact; // Impact prefab

		[Header("Audio")]
        public AudioClip shoot; // Shot prefab
        public float shootDelay; // Shot delay
		[Range(0, 1)]
		public float volume;

    }
}
