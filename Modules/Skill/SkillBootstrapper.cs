using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Skill.Factory;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Mediator;
using IdelPog.Skill.Service;

namespace IdelPog.Skill
{
    public static class SkillBootstrapper
    {
        /// <summary>
        /// Registers the <see cref="SetSkill"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="currentSkillSetter">Used together with <see cref="ICurrentSkillProvider"/></param>
        /// <remarks>
        /// Listens to -> <see cref="SetSkill"/>. On Success -> <see cref="SetSkillResponse"/>. On Error -> <see cref="SetSkillError"/>
        /// </remarks>
        public static void RegisterSetSkill(IBufferManager bufferManager, ICurrentSkillSetter  currentSkillSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetSkillError, SetSkill> setSkillErrorFactory = new SetSkillErrorFactory(baseErrorFactory );
            
            IDispatchOne<SetSkillError> setSkillErrorDispatcher = new ManagedDispatcher<SetSkillError>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetSkillResponseFactory setSkillResponseFactory = new SetSkillResponseFactory();
            
            IDispatchOne<SetSkillResponse> setSkillResponseDispatcher = new ManagedDispatcher<SetSkillResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISingleMediator<SetSkill> setSkillMediator = new SetSkillMediator(currentSkillSetter, setSkillResponseFactory, setSkillResponseDispatcher);
            ISingleController<SetSkill> setSkillController = new ManagedSingleController<SetSkill>(setSkillMediator);
        }
    }
}