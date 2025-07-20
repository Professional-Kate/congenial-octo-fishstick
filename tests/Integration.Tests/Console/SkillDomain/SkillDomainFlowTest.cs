using Console;
using Console.Commands.Resolver.Exceptions;
using Console.Exceptions;
using Console.Runtime.Input;
using Console.Runtime.Input.Exceptions;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Skill;
using Integration.Tests.Console.Permission;

namespace Integration.Tests.Console
{
    [TestFixture]
    public class SkillDomainFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;
        private SkillChangeListener _skillChangeListener;

        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
            TestPermissionService.SendAddPermissionCall(_inputHandler, Domain.SKILL);
            
            _skillChangeListener = new SkillChangeListener();
            ManagedSubscribe(_skillChangeListener);
        }

        private static IEnumerable<TestCaseData> ValidSkillChanges()
        {
            yield return new TestCaseData(
                new[] {"SKILL", "change", "wood_cutting"},
                SkillID.WOOD_CUTTING
            ).SetName("WOOD_CUTTING");
            yield return new TestCaseData(
                new[] {"skill", "CHANGE", "mining"},
                SkillID.MINING
            ).SetName("MINING");
            yield return new TestCaseData(
                new[] {"skill", "change", "FARMING"},
                SkillID.FARMING
            ).SetName("FARMING");
            
        }

        [TestCaseSource(nameof(ValidSkillChanges))]
        public void Positive_ChangeSkill_DispatchesUpdate(string[] arguments, SkillID skillID)
        {
            Assert.DoesNotThrow(() => _inputHandler.Input(arguments));
            
            Assert.That(_skillChangeListener.WasCalled, Is.True);
            Assert.Multiple(() =>
            {
                SkillChange skillChange = _skillChangeListener.SkillChange;
                Assert.That(skillChange.SkillID, Is.EqualTo(skillID));
            });
        }

        [TestCase(new[] { "UNKNOWN", "change", "wood_cutting" }, typeof(FailedEnumParseException), TestName = "UnknownDomain_ThrowsFailedEnumParse")]
        [TestCase(new[] { "skill", "change", "WOOD CUTTING" }, typeof(FailedEnumParseException), TestName = "UnknownSkill_ThrowsFailedEnumParse")]
        [TestCase(new[] { "skill", "change", "wood-cutting" }, typeof(FailedEnumParseException), TestName = "UnknownSkill_ThrowsFailedEnumParse")]
        [TestCase(new[] { "skill", "change", "woodcutting" }, typeof(FailedEnumParseException), TestName = "UnknownSkill_ThrowsFailedEnumParse")]
        [TestCase(new[] { "skill", "change" }, typeof(InvalidArgumentCountException), TestName = "MissingSkill_ThrowsInvalidArgumentCountException")]
        [TestCase(new[] { "skill" }, typeof(EmptySpanException), TestName = "MissingChangeAndSkill_ThrowsEmptySpanException")]
        [TestCase(new string[] {}, typeof(EmptySpanException), TestName = "NoArguments_ThrowsEmptySpanException")]
        public void Negative_ChangeSkill_BadArguments_Throws(string[] arguments, Type exception)
        {
            Assert.Throws(exception, () => _inputHandler.Input(arguments));
            Assert.That(_skillChangeListener.WasCalled, Is.False);
        }
        
        [Test]
        public void Negative_PermissionDenied_NoUpdate_Throws()
        {
            TestPermissionService.SendRemovePermissionCall(_inputHandler, Domain.SKILL);
            Assert.Throws<DomainPermissionDeniedException>(() => _inputHandler.Input(new ReadOnlySpan<string>(["skill", "change", "fishing"])));
        }
    }
}