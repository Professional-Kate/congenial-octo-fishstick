using IdelPog.Common.Errors;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public class HarvestNodeUpdateErrorFactory : IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public HarvestNodeUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeUpdateError Create<TException>(SkillUpdateResponse context, TException exception) where TException : Exception
        {
            return new HarvestNodeUpdateError
            {
                SkillUpdateResponse = context,
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}