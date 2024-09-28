using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class DissolveChildsPingPong : MonoBehaviour
    {
        List<Material> materials = new List<Material>();

        void Start()
        {
            var renders = GetComponents<Renderer>();
            for (int i = 0; i < renders.Length; i++)
            {
                materials.AddRange(renders[i].materials);
            }
        }

        private void Reset()
        {
            Start();
            SetValue(0);
        }

        void Update()
        {
            var value = Mathf.PingPong(Time.time * 0.5f, 1f);
            SetValue(value);
        }

        public void SetValue(float value)
        {
            for (int i = 0; i < materials.Count; i++)
            {
                materials[i].SetFloat("_Dissolve", value);
            }
        }
    }
}