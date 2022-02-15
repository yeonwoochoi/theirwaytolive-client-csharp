/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

namespace GridPathfinding {

    public class PathQueue {

        public int startX;
        public int startY;
        public int endX;
        public int endY;
        public global::GridPathfinding.GridPathfinding.OnPathCallback callback;

        public PathQueue(int _startX, int _startY, int _endX, int _endY, global::GridPathfinding.GridPathfinding.OnPathCallback _callback) {
            startX = _startX;
            startY = _startY;
            endX = _endX;
            endY = _endY;
            callback = _callback;
        }
    }

}