using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public class AIWeapon : MonoBehaviour
    {
        // 武器种类
        public AIWeaponType weaponType;
        public Transform audioSource;

        [Header("Socket setup")]
        // 枪口
        public Transform[] sockets;

        // Current firing socket
        int currentSocket = 0;
        // Timer reference                
        int timerID = -1;
        float _time = 0;

        public void Shoot()
        {
            if(timerID == -1)
            {
                timerID = AITimer.Instance.AddTimer(weaponType.shootDelay, ShootImplement);
                ShootImplement();
            }
        }

        public void ShootImplement()
        {
            // Get random rotation that offset spawned projectile
            // Quaternion offset = Quaternion.Euler(UnityEngine.Random.onUnitSphere);
            // Spawn muzzle flash and projectile with the rotation offset at current socket position
            Forge3D.F3DPoolManager.Pools["GeneratedPool"].Spawn(weaponType.muzzle, sockets[currentSocket].position,
                sockets[currentSocket].rotation, sockets[currentSocket]);
            GameObject newGO = Forge3D.F3DPoolManager.Pools["GeneratedPool"].Spawn(weaponType.projectile,
                    sockets[currentSocket].position + sockets[currentSocket].forward,
                    sockets[currentSocket].rotation, null).gameObject;

            // Play shot sound effect
            PlayShootAudio(sockets[currentSocket].position);
            AdvanceSocket();
        }
        public void PlayShootAudio(Vector3 position)
        {
            // Audio source can only be played once for each vulcanDelay
            if (_time >= weaponType.shootDelay)
            {
                // Spawn audio source prefab from pool
                AudioSource aSrc =
                    Forge3D.F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, weaponType.shoot, position, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    // Modify audio source settings specific to it's type
                    aSrc.pitch = Random.Range(0.95f, 1f);
                    aSrc.volume = weaponType.volume;
                    aSrc.minDistance = 5f;
                    aSrc.loop = false;
                    aSrc.Play();

                    // Reset delay timer
                    _time = 0f;
                }
            }
        }
        public void StopShoot()
        {
            if (timerID != -1)
            {
                AITimer.Instance.RemoveTimer(timerID);
                timerID = -1;
            }
        }

        void AdvanceSocket()
        {
            currentSocket++;
            if (currentSocket >= sockets.Length)
                currentSocket = 0;
        }

        private void Update()
        {
            _time += Time.deltaTime;
        }
    }
}