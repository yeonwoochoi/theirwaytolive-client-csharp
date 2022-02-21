using System;
using Control.Characters.Type;
using Control.Layer;
using UnityEngine;
using Util;

namespace Control.Weapon
{
    public class FieldOfView: MonoBehaviour
    {
        private bool isSet = false;
        
        private Mesh mesh;
        private MeshRenderer meshRenderer;
        private Vector3 origin;

        private float fov;
        private float startinAngle;
        private float viewDistance;
        private int rayCount;

        public void Init(Vector3 origin, float viewDistance, Vector3 initDirection)
        {
            if (isSet) return;
            
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            meshRenderer = GetComponent<MeshRenderer>();
            
            // Set color
            SetColor(new Color(0.25f, 1f, 0));

            // Set origin
            SetOrigin(origin);

            // field of view = fov
            SetFoV(90f);

            // vertice 몇개 할건지 => 숫자가 높을 수록 호에 가까워짐
            rayCount = 50;

            // 호 반지름 (범위)
            SetViewDistance(viewDistance);

            // 초기 방향 설정
            SetAimDirection(initDirection);

            SetLayer();


            isSet = true;
        }

        private void LateUpdate()
        {
            if (!isSet) return;
            
            // 현재 각도
            var angle = startinAngle;
            
            // rayCount 만큼 각도가 나뉠거아님 fov 에서 -> 각도 하나를 계산해야하니 나누지
            var angleIncrease = fov / rayCount;

            Vector3[] vertices = new Vector3[rayCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
            for (int i = 0; i < rayCount; i++)
            {
                Vector3 vertex;
                
                // 부딪히는 물체 확인 (map만)
                /*
                RaycastHit2D raycastHit2D =
                    Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), viewDistance, 1 << 9);
                
                if (raycastHit2D.collider == null) vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
                else vertex = raycastHit2D.point;
                */
                vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
                
                vertices[vertexIndex] = vertex;

                if (i > 0)
                {
                    // 삼각형은 중심, 이전 vertex와 다음 vertex를 꼭지점으로 할거니까
                    triangles[triangleIndex + 0] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;
                    
                    triangleIndex += 3;
                }

                vertexIndex++;
                angle -= angleIncrease;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }

        private void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
        }

        public void SetAimDirection(Vector3 aimDirection)
        {
            var dir = UtilsClass.GetMoveDirectionFromVector(aimDirection);
            startinAngle = 90f + dir switch
            {
                Direction.Left => UtilsClass.GetAngleFromVectorFloat(Vector3.left) - fov / 2f,
                Direction.Right => UtilsClass.GetAngleFromVectorFloat(Vector3.right) - fov / 2f,
                Direction.Up => UtilsClass.GetAngleFromVectorFloat(Vector3.up) - fov / 2f,
                Direction.Down => UtilsClass.GetAngleFromVectorFloat(Vector3.down) - fov / 2f,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public void SetFoV(float fov) {
            this.fov = fov;
        }

        public void SetViewDistance(float viewDistance) {
            this.viewDistance = viewDistance;
        }

        public void SetColor(Color color)
        {
            meshRenderer.materials[0].SetColor("Color_d19e734d6e6140b3bfb5581fad04b9cf", color);
        }

        public void Disable()
        {
            Destroy(gameObject);
        }
        
        private void SetLayer()
        {
            meshRenderer.sortingLayerName = LayerType.Layer1.LayerTypeToString();
            meshRenderer.sortingOrder = 1000;
        }
    }
}