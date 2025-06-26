using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class SkillMediatorTest
    {
        private ISkillMediator _skillMediator { get; set; }
        private Mock<IExperienceService> _experienceServiceMock { get; set; }
        private Mock<IStateRepository<SkillID, Skill>> _repositoryMock { get; set; }
        private Mock<ILevelService> _levelServiceMock { get; set; }
        private Mock<ICurrentSkillProvider>  _currentSkillProviderMock { get; set; }
        private Skill _miningSkill { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningSkill = SkillFactory.CreateMining();
            
            _experienceServiceMock = new Mock<IExperienceService>();
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill>>();
            _levelServiceMock = new Mock<ILevelService>();
            _currentSkillProviderMock = new Mock<ICurrentSkillProvider>();
            _skillMediator = new SkillMediator(_experienceServiceMock.Object, _levelServiceMock.Object, _repositoryMock.Object,  _currentSkillProviderMock.Object);

            _repositoryMock.Setup(library => library.Get(_miningSkill.SkillID)).Returns(_miningSkill);
            _repositoryMock.Setup(library => library.Contains(_miningSkill.SkillID)).Returns(true);
            _currentSkillProviderMock.Setup(library => library.GetCurrentSkill()).Returns(SkillID.MINING);
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
            ServiceResponse response = _skillMediator.ProcessSkillAction();
            
            Assert.That(response.IsSuccess, Is.True);

            VerifyDependencyCalls(1, 1, 1);
        }

        [Test]
        public void Positive_ProcessSkillAction_SkillLevelsUp()
        {
            _levelServiceMock.Setup(library => library.CanSkillLevel(_miningSkill.Levelable))
                .Returns(true);

            ServiceResponse response = _skillMediator.ProcessSkillAction();
            
            Assert.That(response.IsSuccess, Is.True);
            
            VerifyDependencyCalls(1, 1, 1, 1);
        }
        
        [Test]
        public void Negative_ProcessSkillAction_Catches_Exception()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningSkill.Levelable))
                .Throws<Exception>();
            
            ServiceResponse response = _skillMediator.ProcessSkillAction();
            
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Message, Is.Not.Null);
            
            VerifyDependencyCalls(1, 0, 1);
        }
    }
}