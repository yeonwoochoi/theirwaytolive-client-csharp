using System.Collections;
using System.Collections.Generic;
using Control.Layer;
using UnityEngine;
using Util;

namespace MeshParticleSystem.Scripts
{
    public class BloodParticleSystemHandler : MonoBehaviour {

    public static BloodParticleSystemHandler Instance { get; private set; }

    [SerializeField] private LayerMask hitLayerMask;

    private MeshParticleSystem meshParticleSystem;
    private MeshRenderer meshRenderer;
    private List<Single> singleList;

    private void Awake() {
        Instance = this;
        meshParticleSystem = GetComponent<MeshParticleSystem>();
        meshRenderer = GetComponent<MeshRenderer>();
        singleList = new List<Single>();
        SetLayer();
    }

    private void Update() {
        for (int i=0; i<singleList.Count; i++) {
            Single single = singleList[i];
            single.Update();
            if (single.IsParticleComplete())
            {
                StartCoroutine(single.Disable());
                singleList.RemoveAt(i);
                i--;
            }
        }
    }

    private void SetLayer()
    {
        meshRenderer.sortingLayerName = LayerType.Layer1.LayerTypeToString();
        meshRenderer.sortingOrder = 2;
    }

    public void SpawnBlood(Vector3 position, Vector3 direction) {
        int bloodParticleCount = 3;
        SpawnBlood(bloodParticleCount, position, direction);
    }

    public void SpawnBlood(int bloodParticleCount, Vector3 position, Vector3 direction) {
        for (int i = 0; i < bloodParticleCount; i++) {
            singleList.Add(new Single(position, UtilsClass.ApplyRotationToVector(direction, Random.Range(-3f, 3f)), meshParticleSystem, hitLayerMask));
        }
    }


    /*
     * Represents a single Blood Particle
     * */
    private class Single {

        private MeshParticleSystem meshParticleSystem;
        private LayerMask hitLayerMask;
        private Vector3 position;
        private Vector3 direction;
        private int quadIndex;
        private Vector3 quadSize;
        private float moveSpeed;
        private float rotation;
        private int uvIndex;

        public Single(Vector3 position, Vector3 direction, MeshParticleSystem meshParticleSystem, LayerMask hitLayerMask) {
            this.position = position;
            this.direction = direction;
            this.meshParticleSystem = meshParticleSystem;
            this.hitLayerMask = hitLayerMask;

            quadSize = new Vector3(0.25f, 0.25f);
            rotation = Random.Range(0, 360f);
            moveSpeed = Random.Range(4f, 6f);
            uvIndex = Random.Range(0, 8);

            quadIndex = meshParticleSystem.AddQuad(position, rotation, quadSize, false, uvIndex);
        }

        public void Update() {
            float colliderSize = 1f;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(position + direction * colliderSize, direction, moveSpeed * Time.deltaTime, hitLayerMask);
            if (raycastHit2D.collider != null) {
                // Hit something, stop!
                moveSpeed = 0f;
                return;
            }

            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, quadSize, false, uvIndex);

            float slowDownFactor = 3.5f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsParticleComplete() {
            return moveSpeed < .1f;
        }

        public IEnumerator Disable()
        {
            yield return new WaitForSeconds(2f);
            meshParticleSystem.DestroyQuad(quadIndex);
        }
    }

}
}