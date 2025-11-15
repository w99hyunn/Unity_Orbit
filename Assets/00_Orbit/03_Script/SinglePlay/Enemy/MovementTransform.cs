using UnityEngine;

namespace NOLDA
{
    public class MovementTransform : MonoBehaviour
    {
        public float moveSpeed = 0.0f;
        public Vector3 moveDirection = Vector3.zero;

        // Update is called once per frame
        void Update()
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        public void MoveTo(Vector3 direction)
        {
            moveDirection = direction;
        }
    }
}