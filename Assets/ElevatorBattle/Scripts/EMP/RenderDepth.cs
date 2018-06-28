using UnityEngine;
using System.Collections;

namespace OperationTrident.Elevator
{
    [ExecuteInEditMode]
    public class RenderDepth : MonoBehaviour
    {
        void OnEnable()
        {
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
        }
    }
}