using Leopotam.EcsLite;
using JimmboA.Plugins;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public struct AiTargets: IEcsAutoReset<AiTargets>
    {
        public FastList<int2> Positions;
        
        public void AutoReset(ref AiTargets c)
        {
            if (c.Positions == null)
                c.Positions = new FastList<int2>();
            else 
                c.Positions.Clear();
        }
    }
}