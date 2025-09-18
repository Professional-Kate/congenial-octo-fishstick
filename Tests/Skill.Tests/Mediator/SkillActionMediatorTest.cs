using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Scheduler;
using IdelPog.Loot.Service.Interface;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Mediator;
using IdelPog.Skill.Service.Interface;
using IdelPog.Skills.Tests.Service;
using Moq;

namespace IdelPog.Skills.Tests.Mediator
{
    [TestFixture]
    public class SkillActionMediatorTest
    {
        private ISingleMediator<ScheduleTick> _skillActionMediator { get; set; }
        private Mock<IExperienceService> _experienceServiceMock { get; set; }
        private Mock<IStateRepository<SkillID, Skill.Contracts.Skill>> _repositoryMock { get; set; }
        private Mock<ILevelService> _levelServiceMock { get; set; }
        private Mock<ICurrentSkillProvider> _currentSkillProviderMock { get; set; }
        private Mock<IDispatchOne<SkillUpdateResponse>> _skillUpdateDispatcherMock { get; set; }
        private Mock<ISkillUpdateResponseFactory> _skillUpdateFactoryMock { get; set; }
        private Mock<ILootService<SkillID>> _lootServiceMock { get; set; }

        private Skill.Contracts.Skill _miningSkill { get; set; }
        private SkillUpdateResponse _miningSkillUpdateResponse { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningSkill = SkillFactory.CreateMining();

            _experienceServiceMock = new Mock<IExperienceService>();
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill.Contracts.Skill>>();
            _levelServiceMock = new Mock<ILevelService>();
            _currentSkillProviderMock = new Mock<ICurrentSkillProvider>();
            _skillUpdateDispatcherMock = new Mock<IDispatchOne<SkillUpdateResponse>>();
            _skillUpdateFactoryMock = new Mock<ISkillUpdateResponseFactory>();
            _lootServiceMock = new Mock<ILootService<SkillID>>();
            
            _skillActionMediator = new SkillActionMediator(_experienceServiceMock.Object, _levelServiceMock.Object, _repositoryMock.Object, _currentSkillProviderMock.Object, _skillUpdateDispatcherMock.Object, _skillUpdateFactoryMock.Object, _lootServiceMock.Object);

            _repositoryMock.Setup(library => library.Get(_miningSkill.SkillID)).Returns(_miningSkill);
            _repositoryMock.Setup(library => library.Contains(_miningSkill.SkillID)).Returns(true);
            _currentSkillProviderMock.Setup(library => library.GetCurrentSkill()).Returns(SkillID.MINING);
        }

        private void SetupUpdateDTO(Skill.Contracts.Skill skill, bool hasLeveled)
        {
            _miningSkillUpdateResponse = new SkillUpdateResponse
            {
                HasLeveled = hasLeveled,
                SkillID = skill.SkillID,
                ReadOnlyLevelable = new ReadOnlyLevelable
                {
                    Experience = skill.Levelable.Experience,
                    ExperiencePerAction = skill.Levelable.ExperiencePerAction,
                    Level = skill.Levelable.Level,
                    NextLevelExperience = skill.Levelable.NextLevelExperience
                }
            };
        }

        private void VerifyDependencyCalls(int getCalls = 0, int updateCalls = 0, int serviceCalls = 0, int levelServiceCalls = 0, int providerCalls = 1)
        {
            _repositoryMock.Verify(library => library.Get(_miningSkill.SkillID), Times.Exactly(getCalls));
            _repositoryMock.Verify(library => library.Update(_miningSkill.SkillID, _miningSkill), Times.Exactly(updateCalls));
            _experienceServiceMock.Verify(library => library.AddExperience(_miningSkill.Levelable), Times.Exactly(serviceCalls));
            _levelServiceMock.Verify(library => library.LevelUp(_miningSkill.Levelable), Times.Exactly(levelServiceCalls));
            _currentSkillProviderMock.Verify(library => library.GetCurrentSkill(), Times.Exactly(providerCalls));
        }

        [Test]
        public void Positive_ProcessSkillAction_ReturnsSuccess()
        {
            SetupUpdateDTO(_miningSkill, false);

            _skillUpdateFactoryMock.Setup(library => library.Create(_miningSkill, false))
                .Returns(_miningSkillUpdateResponse);

            Assert.DoesNotThrow(() => _skillActionMediator.HandleMessage(new ScheduleTick()));

            VerifyDependencyCalls(1, 1, 1);
            _skillUpdateDispatcherMock.Verify(library => library.Dispatch(_miningSkillUpdateResponse));
            _skillUpdateFactoryMock.Verify(library => library.Create(_miningSkill, false));
            _lootServiceMock.Verify(library => library.DispatchInventoryUpdates(_miningSkill.SkillID), Times.Once);
        }

        [Test]
        public void Positive_ProcessSkillAction_SkillLevelsUp()
        {
            SetupUpdateDTO(_miningSkill, true);

            _skillUpdateFactoryMock.Setup(library => library.Create(_miningSkill, true))
                .Returns(_miningSkillUpdateResponse);

            _levelServiceMock.Setup(library => library.CanLevel(_miningSkill.Levelable))
                .Returns(true);

            Assert.DoesNotThrow(() => _skillActionMediator.HandleMessage(new ScheduleTick()));

            VerifyDependencyCalls(1, 1, 1, 1);
            _skillUpdateDispatcherMock.Verify(library => library.Dispatch(_miningSkillUpdateResponse));
            _skillUpdateFactoryMock.Verify(library => library.Create(_miningSkill, true));
            
            _lootServiceMock.Verify(library => library.DispatchInventoryUpdates(_miningSkill.SkillID), Times.Once);
        }

        [Test]
        public void Negative_ProcessSkillAction_Catches_Exception()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningSkill.Levelable))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _skillActionMediator.HandleMessage(new ScheduleTick()));

            VerifyDependencyCalls(1, 0, 1);
            _lootServiceMock.Verify(library => library.DispatchInventoryUpdates(_miningSkill.SkillID), Times.Never);
        }
    }
}