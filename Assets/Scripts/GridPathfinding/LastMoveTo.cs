/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections.Generic;

//using myNameSpace;

namespace GridPathfinding {

    public class LastMoveTo {

        public List<MapPos> mapPos;
        public global::GridPathfinding.GridPathfinding.UnitMovementCallbackType callbackType;
        public object obj;
        public UnitMovement.PathCallback callback;

        public LastMoveTo(List<MapPos> _mapPos, global::GridPathfinding.GridPathfinding.UnitMovementCallbackType _callbackType, object _obj, UnitMovement.PathCallback _callback) {
            mapPos = _mapPos;
            callbackType = _callbackType;
            obj = _obj;
            callback = _callback;
        }
        public LastMoveTo(MapPos _mapPos, global::GridPathfinding.GridPathfinding.UnitMovementCallbackType _callbackType, object _obj, UnitMovement.PathCallback _callback) {
            mapPos = new List<MapPos>() { _mapPos };
            callbackType = _callbackType;
            obj = _obj;
            callback = _callback;
        }
    }

}