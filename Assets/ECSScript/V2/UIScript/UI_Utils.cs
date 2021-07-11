using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KaizerWaldCode.V2.UI
{
    public struct UIMapSettings
    {
        public int ChunkBoundXZ;
        public int ChunkBoundY;

    };
    public static class UI_Utils
    {
        public static UIMapSettings ProcessNewMapSettings()
        {
            UIMapSettings uiMapSet = new UIMapSettings();

            return uiMapSet;
        }

        public static int ChunkBoundY(int chunkBoundXZ)
        {
            return chunkBoundXZ;
        }
    }
}
