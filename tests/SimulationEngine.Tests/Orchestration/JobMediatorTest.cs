using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Orchestration;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class JobMediatorTest
    {
        private IJobMediator _jobMediator { get; set; }
        private Mock<IExperienceService> _experienceServiceMock { get; set; }
        private Mock<IStateRepository<SkillID, Skill>> _repositoryMock { get; set; }
        private Mock<ILevelService> _levelServiceMock { get; set; }
        private Skill _miningSkill { get; set; }

        [SetUp]
        public void SetUp()
        {
            _miningSkill = JobFactory.CreateMining();
            
            _experienceServiceMock = new Mock<IExperienceService>();
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill>>();
            _levelServiceMock = new Mock<ILevelService>();
            _jobMediator = new JobMediator(_experienceServiceMock.Object, _levelServiceMock.Object, _repositoryMock.Object);

            _repositoryMock.Setup(library => library.Get(_miningSkill.SkillID)).Returns(_miningSkill);
            _repositoryMock.Setup(library => library.Contains(_miningSkill.SkillID)).Returns(true);
        }

        private void VerifyDependencyCalls(int getCalls = 0, int updateCalls = 0, int serviceCalls = 0, int levelServiceCalls = 0)
        {
            _repositoryMock.Verify(library => library.Get(_miningSkill.SkillID), Times.Exactly(getCalls));
            _repositoryMock.Verify(library => library.Update(_miningSkill.SkillID, _miningSkill), Times.Exactly(updateCalls));
            _experienceServiceMock.Verify(library => library.AddExperience(_miningSkill.Levelable), Times.Exactly(serviceCalls));
            _levelServiceMock.Verify(library => library.LevelUpJob(_miningSkill.Levelable), Times.Exactly(levelServiceCalls));
        }

        [Test]
        public void Positive_ProcessJobAction_ReturnsSuccess()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningSkill.Levelable));
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningSkill.SkillID);
            
            Assert.That(response.IsSuccess, Is.True);

            VerifyDependencyCalls(1, 1, 1);
        }

        [Test]
        public void Positive_ProcessJobAction_JobLevelsUp()
        {
            _levelServiceMock.Setup(library => library.CanJobLevel(_miningSkill.Levelable)).Returns(true);

            ServiceResponse response = _jobMediator.ProcessJobAction(_miningSkill.SkillID);
            
            Assert.That(response.IsSuccess, Is.True);
            
            VerifyDependencyCalls(1, 1, 1, 1);
        }
        
        [Test]
        public void Negative_ProcessJobAction_Catches_Exception()
        {
            _experienceServiceMock.Setup(library => library.AddExperience(_miningSkill.Levelable))
                .Throws<Exception>();
            
            ServiceResponse response = _jobMediator.ProcessJobAction(_miningSkill.SkillID);
            
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Message, Is.Not.Null);
            
            VerifyDependencyCalls(1, 0, 1);
        }
    }
}