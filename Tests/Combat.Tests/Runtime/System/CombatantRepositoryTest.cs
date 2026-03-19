using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class CombatantRepositoryTest
    {
        private CombatantRepository _combatantRepository;

        private StatCard _wolfCard; 
        private CombatantEntity _wolfEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _wolfCard = new StatCard { Health = 3, Attack = 5, Speed = 5 };
            _wolfEntity = new CombatantEntity(new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()), _wolfCard) { IsFriendly = false };
        }

        [SetUp]
        public void SetUp()
        {
            _combatantRepository = new CombatantRepository();
        }

        private void VerifyContains(byte id, bool contains)
        {
            Assert.That(_combatantRepository.Contains(id), Is.EqualTo(contains));
        }

        [Test]
        public void Positive_Add_AddsNewEntity()
        { 
            _combatantRepository.Add(_wolfEntity);

            VerifyContains(0, true);
        }

        [Test]
        public void Positive_Add_AddMultiple_IncrementsID()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            
            VerifyContains(0, true);
            VerifyContains(1, true);
            VerifyContains(2, true);
        }

        [Test]
        public void Positive_Clear_RemovesOne()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Clear();
            
            VerifyContains(0, false);
        }

        [Test]
        public void Positive_Clear_RemovesAll()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);

            _combatantRepository.Clear();
            
            VerifyContains(0, false);
            VerifyContains(1, false);
            VerifyContains(2, false);
        }

        [Test]
        public void Positive_Add_ClearAfterAdd_ResetsID()
        { 
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Clear();
            _combatantRepository.Add(_wolfEntity);
            
            VerifyContains(0, true);
            VerifyContains(1, false);
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _combatantRepository.Add(_wolfEntity);

            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.True);
            VerifyContains(0, true);
        }
        
        [Test]
        public void Positive_Contains_ReturnsFalse()
        {
            bool contains = _combatantRepository.Contains(0);
            
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Positive_GetAll_ReturnsAll()
        {
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            _combatantRepository.Add(_wolfEntity);
            
            CombatantEntity[] entities = _combatantRepository.GetAll().ToArray();
            
            Assert.That(entities, Has.Length.EqualTo(3));
        }

        [Test]
        public void Positive_GetAll_EmptyRepository_ReturnsEmptyArray()
        {
            CombatantEntity[] entities = _combatantRepository.GetAll().ToArray();
            
            Assert.That(entities, Is.Not.Null);
            Assert.That(entities, Has.Length.EqualTo(0));
        }
    }
}