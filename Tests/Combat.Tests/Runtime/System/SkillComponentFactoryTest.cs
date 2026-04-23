using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class SkillComponentFactoryTest
    {
        private SkillComponentFactory _skillComponentFactory;

        private SkillCard _skillCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillComponentFactory = new SkillComponentFactory();

            _skillCard = new SkillCard { SkillType = SkillType.BASIC_ATTACK, Strategy = new Strategy { TargetingType = TargetingType.HIGH_ATTACK }};
        }

        private static void VerifyComponent(SkillCard source, SkillComponent skillComponent)
        {
            Assert.Multiple(() =>
            {
                Assert.That(skillComponent.SkillType, Is.EqualTo(source.SkillType));
                Assert.That(skillComponent.TargetingType, Is.EqualTo(source.Strategy.TargetingType));
            });
        }

        [Test]
        public void Positive_Create_CreatesSkillComponent()
        {
            SkillComponent skillComponent = _skillComponentFactory.Create(_skillCard);

            VerifyComponent(_skillCard, skillComponent);
        }

        [Test]
        public void Positive_CreateMultiple_CreatesSkillComponents()
        {
            SkillComponent[] skillComponents = _skillComponentFactory.CreateMultiple([_skillCard, _skillCard]);

            Assert.That(skillComponents, Has.Length.EqualTo(2));
            foreach (SkillComponent skillComponent in skillComponents)
            {
                VerifyComponent(_skillCard, skillComponent);
            }
        }

        [Test]
        public void Positive_CreateMultiple_EmptyInputCollection_ReturnsEmptyCollection()
        {
            SkillComponent[] skillComponents = _skillComponentFactory.CreateMultiple([]);
            
            Assert.That(skillComponents, Has.Length.EqualTo(0));
        }
    }
}