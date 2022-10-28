using System;
using System.Collections.Generic;
using UnityEngine.Experimental.U2D.Animation;

namespace Client.Battle.View
{
    // TODO : maybe replace with parent-child components
    [Serializable]
    public struct Skin
    {
        public List<SpriteResolver> Resolvers;
    }
}