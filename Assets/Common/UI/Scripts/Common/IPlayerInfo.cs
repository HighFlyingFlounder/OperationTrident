using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OperationTrident.Common.UI
{
    public interface IPlayerInfo
    {
        int CurrentHP { get; }
        int TotalHP { get; }
    }
}