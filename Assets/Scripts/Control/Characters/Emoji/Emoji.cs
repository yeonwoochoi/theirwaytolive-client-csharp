using System;
using UnityEngine;

namespace Control.Characters.Emoji
{
    public enum EmojiType
    {
        Adore,
        Angry,
        Awkward,
        Cheerless,
        Confidence,
        Cute,
        Delighted,
        Embarrassed,
        Energetic,
        Fluttered,
        Grinned,
        Infatuated,
        Interested,
        Overjoyed,
        Smiled,
        Sad,
        Soso,
        Wink,
        None
    }
    
    [Serializable]
    public class Emoji
    {
        public EmojiType type;
        public Sprite sprite;
    }
}