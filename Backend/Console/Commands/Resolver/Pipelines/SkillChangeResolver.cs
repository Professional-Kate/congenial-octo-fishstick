using Console.Commands.Domains.Arguments;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver.Pipelines
{
    public class SkillChangeResolver : IArgumentResolverPipeline<SkillChangeArguments>
    {
        private readonly IArgumentResolver<SkillID> _skillIDResolver;

        public SkillChangeResolver(IArgumentResolver<SkillID> skillIDResolver)
        {
            _skillIDResolver = skillIDResolver;
        }

        public SkillChangeArguments Resolve(ReadOnlySpan<string> arguments)
        {
            // arguments[0] == CHANGE
            SkillID skillID = _skillIDResolver.Resolve(arguments[1]);

            return new SkillChangeArguments
            {
                SkillID = skillID
            };
        }
    }
}