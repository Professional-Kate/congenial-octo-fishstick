using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Skill;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class SkillActionMediatorTest
    {
        private IScheduledTask _skillActionMediator { get; set; }
        private Mock<IExperienceService> _experienceServiceMock { get; set; }
        private Mock<IStateRepository<SkillID, Skill>> _repositoryMock { get; set; }
        private Mock<ILevelService> _levelServiceMock { get; set; }
        private Mock<ICurrentSkillProvider>  _currentSkillProviderMock { get; set; }
        private Mock<IDispatchOne<SkillUpdateDTO>> _skillUpdateDispatcherMock { get; set; }
        private Mock<ISkillUpdateFactory>  _skillUpdateFactoryMock { get; set; }
        
        private Skill _miningSkill { get; set; }
        private SkillUpdateDTO _miningSkillUpdateDTO { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningSkill = SkillFactory.CreateMining();
            
            _experienceServiceMock = new Mock<IExperienceService>();
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill>>();
            _levelServiceMock = new Mock<ILevelService>();
            _currentSkillProviderMock = new Mock<ICurrentSkillProvider>();
            _skillUpdateDispatcherMock = new Mock<IDispatchOne<SkillUpdateDTO>>();
            _skillUpdateFactoryMock = new Mock<ISkillUpdateFactory>();
            _skillActionMediator = new SkillActionMediator(_experienceServiceMock.Object, _levelServiceMock.Object, _repositoryMock.Object,  _currentSkillProviderMock.Object,  _skillUpdateDispatcherMock.Object, _skillUpdateFactoryMock.Object);

            _repositoryMock.Setup(library => library.Get(_miningSkill.SkillID)).Returns(_miningSkill);
            _repositoryMock.Setup(library => library.Contains(_miningSkill.SkillID)).Returns(true);
            _currentSkillProviderMock.Setup(library => library.GetCurrentSkill()).Returns(SkillID.MINING);
        }

        private void SetupUpdateDTO(Skill skill, bool hasLeveled)
        {
            _miningSkillUpdateDTO = new SkillUpdateDTO()
            {
                HasLeveled = hasLeveled,
                SkillID = skill.SkillID,
                LevelableUpdateDTO = new LevelableUpdateDTO()
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
            _levelServiceMock.Verify(library => library.LevelUpSkill(_miningSkill.Levelable), Times.Exactly(levelServiceCalls));
            _currentSkillProviderMock.Verify(library => library.GetCurrentSkill(), Times.Exactly(providerCalls));
        }

        [Test]
        public void Positive_ProcessSkillAction_ReturnsSuccess()
        {
            SetupUpdateDTO(_miningSkill, false);
            
            _skillUpdateFactoryMock.Setup(library => library.CreateSkillUpdate(_miningSkill, false))
                .Returns(_miningSkillUpdateDTO);

            Assert.DoesNotThrow(() => _skillActionMediator.Run());

            VerifyDependencyCalls(1, 1, 1);
            _skillUpdateDispatcherMock.Verify(library => library.Dispatch(_miningSkillUpdateDTO));
            _skillUpdateFactoryMock.Verify(library => library.CreateSkillUpdate(_miningSkill, false));
        }

        [Test]
        public void Positive_ProcessSkillAction_SkillLevelsUp()
        {
            SetupUpdateDTO(_miningSkill, true);
            
            _skillUpdateFactoryMock.Setup(library => library.CreateSkillUpdate(_miningSkill, true))
                .Returns(_miningSkillUpdateDTO);
            
            _levelServiceMock.Setup(library => library.CanSkillLevel(_miningSkill.Levelable))
                .Returns(true);

            Assert.DoesNotThrow(() => _skillActionMediator.Run());
            
            VerifyDependencyCalls(1, 1, 1, 1);
            _skillUpdateDispatcherMock.Verify(library => library.Dispatch(_miningSkillUpdateDTO));
            _skillUpdateFactoryMock.Verify(library => library.CreateSkillUpdate(_miningSkill, true));
        }
        
        [Test]
        public void Negative_ProcessSkillAction_Catches_Exception()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningSkill.Levelable))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _skillActionMediator.Run());
            
            VerifyDependencyCalls(1, 0, 1);
        }
    }
}