using UnityEngine;
using System.Collections;

namespace TinyRoar.Framework
{
    public enum SaveMethod
    {
        None,
        Xml,
        BinaryCrypt,
        TextAsset,

    }

    public class GameConfig : Singleton<GameConfig>
    {

        public SaveMethod UseSaveMethod
        {
            get;
            set;
        }

        public GameConfig()
        {
            // set Cryption
            if(UseSaveMethod == SaveMethod.None)
            {
                UseSaveMethod = SaveMethod.Xml;
            }
        }

        public const string KeySoundAllowmusic = "AllowMusic";
        public const string KeySoundAlloweffects = "AllowEffects";
    }
}