using Pathfinding;
using UnityEngine;

namespace Manager
{
    public class PathfindingManager: MonoSingleton<PathfindingManager>
    {
        private AstarPath astarPath;
        public void Init()
        {
            astarPath = GetComponent<AstarPath>();

            var lowerLeftAnchor = GameObject.Find("PathfindingLowerLeft").transform.position;
            var upperRightAnchor = GameObject.Find("PathfindingUpperRight").transform.position;
            
            var activeGridGraph = AstarPath.active.data.gridGraph;
            var width = (int) (upperRightAnchor.x - lowerLeftAnchor.x);
            var depth = (int) (upperRightAnchor.y - lowerLeftAnchor.y);
            activeGridGraph.SetDimensions(width, depth, 1f);

            // Set 2D
            activeGridGraph.is2D = true;
            activeGridGraph.collision.use2D = true;
            activeGridGraph.collision.type = ColliderType.Capsule;
            activeGridGraph.collision.diameter = 1.2f;
            
            // 9번 layer (Map) 만 장애물로 인식
            activeGridGraph.collision.mask = 1 << 9;
            
            // 0f 로 설정하면 벽에 딱 붙어서 path finding route가 결정될거임
            activeGridGraph.collision.diameter = 2f;
            
            AstarPath.active.Scan();
        }
    }
}