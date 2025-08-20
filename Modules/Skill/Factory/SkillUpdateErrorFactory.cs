using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Scheduler;

namespace IdelPog.Skill.Factory
{
    public class SkillUpdateErrorFactory : IErrorFactory<SkillUpdateError, ScheduleTick>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SkillUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SkillUpdateError Create<TException>(TException exception, ScheduleTick context) where TException : Exception
        {
            return new SkillUpdateError
            {
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}