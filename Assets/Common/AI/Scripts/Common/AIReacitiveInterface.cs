using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OperationTrident.Common.AI
{
    public interface AIReacitiveInterface
    {
        bool IsParalyzed { get; }
		bool IsAlive { get; }
        float HPPercentage { get; }
    }
}