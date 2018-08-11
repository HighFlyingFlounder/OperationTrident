using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.UI
{
    public interface IWeaponInfo
    {
        int CurrentAmmo { get; }
        int TotalAmmo { get; }
        bool isInfinite { get; }
    }
}