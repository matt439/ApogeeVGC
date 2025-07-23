using ApogeeVGC_CS.sim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApogeeVGC_CS.sim
{
    public interface IEventMethods
    {

    }

    public interface IPokemonEventMethods : IEventMethods
    {

    }

    public interface ISideEventMethods : IEventMethods
    {
    }

    public interface IFieldEventMethods : IEventMethods
    {
        
    }

    public class PokemonConditionData : IPokemonEventMethods
    {
        // Add properties from Condition as needed
        public bool? Inherit { get; set; }
    }

    public class SideConditionData : ISideEventMethods
    {
        // Add properties from Condition as needed, excluding onStart/onRestart/onEnd
        public bool? Inherit { get; set; }
    }

    public class FieldConditionData : IFieldEventMethods
    {
        // Add properties from Condition as needed, excluding onStart/onRestart/onEnd
        public bool? Inherit { get; set; }
    }

    public interface IConditionData { }

    public class ConditionDataTable : Dictionary<string, IConditionData> { }
    public class ModdedConditionDataTable : Dictionary<string, IConditionData> { }

    public class Condition : BasicEffect, IPokemonEventMethods, ISideEventMethods, IFieldEventMethods
    {
        public string EffectType { get; set; } = "Condition";
        public int? CounterMax { get; set; }
        public int? EffectOrder { get; set; }

        // Event handler delegates (add as needed)
        public Func<Battle, Pokemon, Pokemon, Effect?, int>? DurationCallback { get; set; }
        public Action<Battle, Pokemon>? OnCopy { get; set; }
        public Action<Battle, Pokemon>? OnEnd { get; set; }
        public Func<Battle, Pokemon, Pokemon, Effect, bool?>? OnRestart { get; set; }
        public Func<Battle, Pokemon, Pokemon, Effect, bool?>? OnStart { get; set; }

        public Condition(IAnyObject data) : base(data)
        {
            EffectType = data.ContainsKey("effectType") ? data["effectType"].ToString() ?? "Condition" : "Condition";
        }
    }
}
