using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Integration.Tests.Combat.Tools
{
    internal class AbilityTracker
    {
        internal required int Attacks { get; set; }
        internal required AbilityType AbilityType { get; set; }
        internal required uint TotalDamage { get; set; }
    }
}