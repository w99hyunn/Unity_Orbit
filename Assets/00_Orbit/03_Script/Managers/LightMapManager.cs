using UnityEditor;
using UnityEngine;

namespace STARTING
{
    public class LightMapManager : MonoBehaviour
    {
        public LightingDataAsset sceneLightingData;

        void Start()
        {
            Lightmapping.lightingDataAsset = sceneLightingData;
        }

    }
}