using Console.Types;
using IdelPog.SimulationEngine.Skill;

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
            SkillID skillID = _skillIDResolver.Resolve(arguments[0]);

            return new SkillChangeArguments
            {
                SkillID = skillID
            };
        }
    }
}