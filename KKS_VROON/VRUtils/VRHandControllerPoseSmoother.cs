using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KKS_VROON.VRUtils
{
    public class VRHandControllerPoseSmoother
    {
        private Queue<Vector3> positionBuffer = new Queue<Vector3>();
        private Queue<Quaternion> rotationBuffer = new Queue<Quaternion>();
        private int BufferSize { get; set; }

        public VRHandControllerPoseSmoother(int bufferSize)
        {
            BufferSize = bufferSize;
        }

        public void AddTransform(Transform transform)
        {
            positionBuffer.Enqueue(transform.position);
            while (BufferSize < positionBuffer.Count) positionBuffer.Dequeue();
            rotationBuffer.Enqueue(transform.rotation);
            while (BufferSize < rotationBuffer.Count) rotationBuffer.Dequeue();
        }

        public Vector3 GetPosition()
        {
            return new Vector3(positionBuffer.Average(v => v.x), positionBuffer.Average(v => v.y), positionBuffer.Average(v => v.z));
        }

        public Quaternion GetRotation()
        {
            var sum = Vector4.zero;
            foreach (var rotation in rotationBuffer) sum += new Vector4(rotation.x, rotation.y, rotation.z, rotation.w);
            return NormalizeQuaternion(sum / rotationBuffer.Count);
        }

        private Quaternion NormalizeQuaternion(Vector4 q)
        {
            float length = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            return new Quaternion(q.x / length, q.y / length, q.z / length, q.w / length);
        }
    }
}
