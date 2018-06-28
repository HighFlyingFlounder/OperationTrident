using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Weapon {
    public interface IShoot {
        //获得开枪时用于检测的射线
        Ray GetShootRay();

        //发出射线，进行检测
        void Shoot();

        //处理检测结果
        void ProcessHit(RaycastHit hitInfo);
    }
}
