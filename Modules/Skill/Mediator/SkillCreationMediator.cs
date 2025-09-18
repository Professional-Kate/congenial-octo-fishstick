using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion.Pipelines;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Mediator
{
    public sealed class SkillCreationMediator : IBatchMediator<SkillCreation>
    {
        private readonly IStateRepository<SkillID, Contracts.Skill> _skillRepository;
        private readonly ISkillCreationResponseFactory _responseFactory;
        private readonly IDispatchOne<SkillCreationResponse> _responseDispatcher;
        private readonly ILevelableAssertionPipeline _levelableAssertionPipeline;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public SkillCreationMediator(IStateRepository<SkillID, Contracts.Skill> skillRepository, ISkillCreationResponseFactory responseFactory, IDispatchOne<SkillCreationResponse> responseDispatcher, ILevelableAssertionPipeline levelableAssertionPipeline, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _skillRepository = skillRepository;
            _responseFactory = responseFactory;
            _responseDispatcher = responseDispatcher;
            _levelableAssertionPipeline = levelableAssertionPipeline;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<SkillCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            foreach (SkillCreation skillCreation in messages)
            {
                SkillID skillID = skillCreation.SkillID;
                _uniqueAssertion.AssertUnique(skillID, _skillRepository.Contains(skillID));

                ReadOnlyLevelable readOnlyLevelable = skillCreation.ReadOnlyLevelable;
                Contracts.Skill skill = new()
                {
                    SkillID = skillID,
                    Information = skillCreation.Information,
                    Levelable = new Levelable(readOnlyLevelable.Level, readOnlyLevelable.Experience, readOnlyLevelable.NextLevelExperience, readOnlyLevelable.ExperiencePerAction)
                };
                
                _levelableAssertionPipeline.AssertLevelable(skill.Levelable);
                _skillRepository.Add(skill.SkillID, skill);
            }
            
            _responseDispatcher.Dispatch(_responseFactory.Create(messages.ToArray()));
        }
    }
}