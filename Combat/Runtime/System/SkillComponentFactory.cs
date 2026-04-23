using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class SkillComponentFactory : ISkillComponentFactory
    {
        public SkillComponent Create(SkillCard skillCard)
        {
            return new SkillComponent
            {
                SkillType = skillCard.SkillType,
                TargetingType = skillCard.Strategy.TargetingType
            };
        }

        public SkillComponent[] CreateMultiple(SkillCard[] skillCards)
        {
            SkillComponent[] components = new SkillComponent[skillCards.Length];
            for (int i = 0; i < skillCards.Length; i++)
            {
                SkillCard skillCard = skillCards[i];
                components[i] = Create(skillCard);
            }
            
            return components;
        }
    }
}