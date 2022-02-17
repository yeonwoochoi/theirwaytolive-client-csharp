using Manager;
using UnityEngine;

namespace Control.Stuff
{
    public class GameAssets: MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets i {
            get {
                if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _i;
            }
        }

        public Equipment.Equipment[] defaultEquipments;

        public Transform pfMap1;
        
        public Transform pfBachi;
        public Transform pfDuju;
        public Transform pfPanno;
        public Transform pfSoldierA;
        public Transform pfSosuno;

        public Transform pfEnemy1;

        public Transform pfDamagePopup;
    }
}