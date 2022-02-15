using System;
using System.Collections;
using UnityEngine;

namespace Control.Characters.Animation
{
    public class AnimationEventTrigger : MonoBehaviour
    {
        private Action beginCallback = null;
        private Action midCallback = null;
        private Action endCallback = null;

        public void Play(Action mainAnimation,
            Action beginCallback = null,
            Action midCallback = null,
            Action endCallback = null
        )
        {
            Reset();
            mainAnimation?.Invoke();
            this.beginCallback = beginCallback;
            this.midCallback = midCallback;
            this.endCallback = endCallback;
        }

        public void OnBeginEvent()
        {
            beginCallback?.Invoke();
        }

        public void OnMidEvent()
        {
            midCallback?.Invoke();
        }

        public void OnEndEvent()
        {
            endCallback?.Invoke();
        }

        private void Reset()
        {
            beginCallback = null;
            midCallback = null;
            endCallback = null;
        }
    }
}