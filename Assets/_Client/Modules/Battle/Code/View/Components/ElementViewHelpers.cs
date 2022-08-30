using System;
using Client.Battle.Simulation;
using UnityEngine;

namespace Client.Battle.View
{
    public static class ElementViewHelpers
    {
        // temp. until there are no actual visual effects
        public static Color GetElementColor(in this Element element)
        {
            switch (element.Type)
            {
                case Elements.None:
                    break;
                case Elements.Fire:
                    return Color.red;
                case Elements.Water:
                    return Color.blue;
                case Elements.Ice:
                    return Color.cyan;
                case Elements.Electric:
                    return Color.yellow;
                case Elements.Earth:
                    return Color.green;
                case Elements.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return  Color.white;
        }
    }
}