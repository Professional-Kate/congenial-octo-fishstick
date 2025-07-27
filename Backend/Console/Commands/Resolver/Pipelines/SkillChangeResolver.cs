using Console.Commands.Domains.Arguments;
using IdelPog.Common.Enums;

namespace Console.Commands.Resolver.Pipelines
{
    public class SkillChangeResolver : IArgumentResolverPipeline<SkillChangeArguments>
    {
        private readonly IArgumentResolver<SkillID> _skillIDResolver;
        private readonly IArgumentResolver<ResourceID> _resourceIDResolver;

        public SkillChangeResolver(IArgumentResolver<SkillID> skillIDResolver, IArgumentResolver<ResourceID> resourceIDResolver)
        {
            _skillIDResolver = skillIDResolver;
            _resourceIDResolver = resourceIDResolver;
        }

        public SkillChangeArguments Resolve(ReadOnlySpan<string> arguments)
        {
            // arguments[0] == CHANGE
            SkillID skillID = _skillIDResolver.Resolve(arguments[1]);
            ResourceID resourceID = _resourceIDResolver.Resolve(arguments[2]);

            return new SkillChangeArguments
            {
                SkillID = skillID,
                ResourceID = resourceID
            };
        }
    }
}