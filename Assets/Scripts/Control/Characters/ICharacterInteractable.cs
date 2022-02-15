using UnityEngine;

namespace Control.Characters
{
    public interface ICharacterInteractable
    {
        public void Init();
        public GameObject GetGameObject();
        public Vector3 GetPosition();

        public bool IsDead();
    }
}