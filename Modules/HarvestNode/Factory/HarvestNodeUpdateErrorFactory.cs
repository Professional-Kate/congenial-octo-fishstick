using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class HarvestNodeUpdateErrorFactory : IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public HarvestNodeUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeUpdateError Create<TException>(TException exception, SkillUpdateResponse context) where TException : Exception
        {
            return new HarvestNodeUpdateError
            {
                SkillUpdateResponse = context,
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}