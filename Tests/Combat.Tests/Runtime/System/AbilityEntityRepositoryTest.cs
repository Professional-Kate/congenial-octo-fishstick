using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityEntityRepositoryTest
    {
        private AbilityEntityRepository _abilityEntityRepository;

        private AbilityEntity _abilityEntity;
        private AbilityEntity _anotherAbilityEntity;
        
        [SetUp]
        public void Setup()
        { 
            _abilityEntityRepository = new AbilityEntityRepository();
            _abilityEntity = TestAbilityEntityFactory.Create(1, 1);
            _anotherAbilityEntity = TestAbilityEntityFactory.Create(2, 2);
        }
        
        private void VerifyContains(AbilityEntity abilityEntity, bool expectedContains)
        {
            Assert.That(_abilityEntityRepository.Contains(abilityEntity.InstanceID), Is.EqualTo(expectedContains));
        }

        [Test]
        public void Positive_SeedAbilities_SeedsSingle()
        {
            _abilityEntityRepository.SeedAbilities([_abilityEntity]);

            VerifyContains(_abilityEntity, true);
            VerifyContains(_anotherAbilityEntity, false);
        }
        
        [Test]
        public void Positive_SeedAbilities_SeedsMultiple()
        {
            _abilityEntityRepository.SeedAbilities([_abilityEntity, _anotherAbilityEntity]);

            VerifyContains(_abilityEntity, true);
            VerifyContains(_anotherAbilityEntity, true);
        }

        [Test]
        public void Positive_EnumerateAbilities_ReturnsAllAbilitiesForCombatant()
        {
            _abilityEntityRepository.SeedAbilities([_abilityEntity, _anotherAbilityEntity, _abilityEntity]);
            
            AbilityEntity[] abilityEntities = _abilityEntityRepository.EnumerateAbilities(_abilityEntity.InstanceID).ToArray();
            
            Assert.That(abilityEntities, Is.Not.Null);
            Assert.That(abilityEntities, Has.Length.EqualTo(2));
        }
        
        [Test]
        public void Positive_EnumerateAbilities_NoAbilities_ReturnsNothing()
        {
            AbilityEntity[] abilityEntities = _abilityEntityRepository.EnumerateAbilities(_abilityEntity.InstanceID).ToArray();
            
            Assert.That(abilityEntities, Is.Not.Null);
            Assert.That(abilityEntities, Has.Length.EqualTo(0));
        }

        [Test]
        public void Positive_Clear_ClearsAbilities()
        {
            _abilityEntityRepository.SeedAbilities([_anotherAbilityEntity]);
            VerifyContains(_anotherAbilityEntity, true);
            
            _abilityEntityRepository.Clear();
            VerifyContains(_anotherAbilityEntity, false);
        }
        
        [Test]
        public void Positive_Clear_NoAbilities_ClearsAbilities()
        { 
            Assert.DoesNotThrow(() => _abilityEntityRepository.Clear());
        }
    }
}