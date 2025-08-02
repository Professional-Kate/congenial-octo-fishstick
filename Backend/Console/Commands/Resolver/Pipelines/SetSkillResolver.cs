using Console.Commands.Domains.Arguments;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver.Pipelines
{
    public class SetSkillResolver : IArgumentResolverPipeline<SetSkillArguments>
    {
        private readonly IArgumentResolver<SkillID> _skillIDResolver;

        public SetSkillResolver(IArgumentResolver<SkillID> skillIDResolver)
        {
            _skillIDResolver = skillIDResolver;
        }

        public SetSkillArguments Resolve(ReadOnlySpan<string> arguments)
        {
            // arguments[0] == CHANGE
            SkillID skillID = _skillIDResolver.Resolve(arguments[1]);

            return new SetSkillArguments
            {
                SkillID = skillID,
            };
        }
    }
}