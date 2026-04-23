using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;

namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ISkillComponentFactory
    { 
        public SkillComponent Create(SkillCard skillCard);
        
        public SkillComponent[] CreateMultiple(SkillCard[] skillCards);
    }
}