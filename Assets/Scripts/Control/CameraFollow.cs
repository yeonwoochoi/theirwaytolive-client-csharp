using UnityEngine;

namespace Control
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform target;
        private float lerpSpeed = 1.0f;
        private Vector3 offset;
        private Vector3 targetPos;

        private bool isSet = false;

        private void Update()
        {
            if (!isSet) return;

            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }

        public void Init(Transform targetObj, float speed = 1.0f)
        {
            transform.position = new Vector3(targetObj.position.x, targetObj.position.y, -10);
            target = targetObj;
            lerpSpeed = speed;
            offset = transform.position - target.position;
            isSet = true;
        }
    }
}