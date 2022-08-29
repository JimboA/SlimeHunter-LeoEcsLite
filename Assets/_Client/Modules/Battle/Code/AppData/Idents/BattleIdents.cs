using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.AppData
{
    public static class BattleIdents
    {
        public static class Ui
        {
            public const string GoButtonName = "Go_Button";
            public const string SaveButtonName = "Save_Button";
            public const string RetryButtonName = "Retry_Button";
        }
        
        public static class Blueprints
        {
            public const string Hero = "Hero";
            public const string Slime = "Slime";
            public const string Hunter = "Hunter";
        }
        
        public static class Elements
        {
            public const string Fire = nameof(Client.Battle.Simulation.Elements.Fire);
            public const string Water = nameof(Client.Battle.Simulation.Elements.Water);
            public const string Ice = nameof(Client.Battle.Simulation.Elements.Ice);
            public const string Electric = nameof(Client.Battle.Simulation.Elements.Electric);
            public const string Earth = nameof(Client.Battle.Simulation.Elements.Earth);
        }
    }
}
