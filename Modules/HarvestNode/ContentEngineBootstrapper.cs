using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Progression.Assertion.Pipelines;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Factory;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;

namespace IdelPog.HarvestNode
{
    public static class ContentEngineBootstrapper
    {
        /// <summary>
        /// Creates and adds the <see cref="SetHarvestNode"/> and <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentResourceProvider">Used together with <see cref="ICurrentResourceProvider"/></param>
        /// <param name="batchRegister">Used to register the NodeCreation flow</param>
        /// <param name="singleRegister">Used to register other flows</param>
        /// <seealso cref="RegisterSetHarvestNode"/>
        public static void RegisterFlows(IBufferManager bufferManager, CurrentResourceProvider currentResourceProvider, IBatchRegister batchRegister, ISingleRegister singleRegister)
        {
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();
            ISkillNodeAccessValidator skillNodeAccessValidator = new SkillNodeAccessValidator(skillNodeRepository, foundAssertion);
            
            RegisterSkillUpdateResponse(bufferManager, currentResourceProvider, skillNodeAccessValidator, singleRegister);
            RegisterSetHarvestNode(bufferManager, currentResourceProvider, skillNodeAccessValidator, singleRegister);
            RegisterNodeCreation(bufferManager, skillNodeRepository, batchRegister);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentResourceProvider">Used together with <see cref="ICurrentResourceSetter"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <remarks>
        /// Listens to -> <see cref="SkillUpdateResponse"/>. On Success -> <see cref="HarvestNodeUpdateResponse"/>. On Error -> <see cref="HarvestNodeUpdateError"/>
        /// </remarks>
        private static void RegisterSkillUpdateResponse(IBufferManager bufferManager, ICurrentResourceProvider currentResourceProvider, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ILevelableAssertionPipeline levelableAssertion = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            
            IStateRepository<ResourceID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ResourceID, Contracts.HarvestNode>();
            ILevelService levelService = new LevelService(levelableAssertion);
            IExperienceService experienceService = new ExperienceService(levelableAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            INodeUpdateResponseFactory responseFactory = new NodeUpdateResponseFactory(levelProgressFactory);

            Contracts.HarvestNode ironHarvestNode = new()
            {
                Information = new Information { Description = "", Name = "" }, 
                ResourceID = ResourceID.IRON, 
                Levelable = new Levelable(0, 0, 20, 0)
            };

            harvestNodeRepository.Add(ironHarvestNode.ResourceID, ironHarvestNode);
                
            
            IDispatchOne<HarvestNodeUpdateResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            INodeUpdateService nodeUpdateService = new NodeUpdateService(harvestNodeRepository, levelService, experienceService, responseFactory, foundAssertion);
            ISingleMediator<SkillUpdateResponse> updateMediator = new NodeUpdateMediator(currentResourceProvider, skillNodeAccessValidator, nodeUpdateService, responseDispatcher);
            ISingleController<SkillUpdateResponse> nodeUpdateController = new ManagedSingleController<SkillUpdateResponse>(updateMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse> harvestNodeErrorFactory = new HarvestNodeUpdateErrorFactory(baseErrorFactory);

            singleRegister.Register(nodeUpdateController, harvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="SetHarvestNode"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="currentResourceSetter">Used together with <see cref="ICurrentResourceProvider"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <remarks>
        /// Listens to -> <see cref="SetHarvestNode"/>. On Success -> <see cref="SetHarvestNodeResponse"/>. On Error -> <see cref="SetHarvestNodeError"/>
        /// </remarks>
        private static void RegisterSetHarvestNode(IBufferManager bufferManager, ICurrentResourceSetter currentResourceSetter, ISkillNodeAccessValidator skillNodeAccessValidator, ISingleRegister singleRegister)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> setHarvestNodeErrorFactory = new SetNodeErrorFactory(baseErrorFactory);

            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetNodeResponseFactory nodeChangeResponseFactor = new SetNodeResponseFactory();
            ISingleMediator<SetHarvestNode> setHarvestNodeMediator = new SetHarvestNodeMediator(skillNodeAccessValidator, currentResourceSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor);
            ISingleController<SetHarvestNode> setHarvestNodeController = new ManagedSingleController<SetHarvestNode>(setHarvestNodeMediator);
            
            singleRegister.Register(setHarvestNodeController, setHarvestNodeErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="NodeCreation"/> flow into the messaging system>
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeRepository">Used to store all <see cref="HarvestNode"/> models</param>
        /// <remarks>
        /// Listens to -> <see cref="NodeCreation"/>. On Success -> <see cref="NodeCreationResponse"/>. On Error -> <see cref="NodeCreationError"/>
        /// </remarks>
        private static void RegisterNodeCreation(IBufferManager bufferManager, IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository, IBatchRegister batchRegister)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            IStateRepository<ResourceID, Contracts.HarvestNode> harvestNodeRepository = new StateRepository<ResourceID, Contracts.HarvestNode>();
            ISkillNodeEntityFactory skillNodeEntityFactory = new SkillNodeEntityFactory();
            IHarvestNodeFactory harvestNodeFactory = new HarvestNodeFactory();
            INodeCreationResponseFactory nodeCreationResponseFactory = new NodeCreationResponseFactory();
            IDispatchOne<NodeCreationResponse> nodeCreationResponseDispatcher = new ManagedDispatcher<NodeCreationResponse>(bufferManager,  objectNullAssertion, collectionAssertion);
            
            IBatchMediator<NodeCreation> creationMediator = new NodeCreationMediator(harvestNodeRepository, skillNodeRepository, skillNodeEntityFactory, harvestNodeFactory, nodeCreationResponseFactory, nodeCreationResponseDispatcher, uniqueAssertion, collectionAssertion);
            IBatchController<NodeCreation> creationController = new ManagedBatchController<NodeCreation>(creationMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<NodeCreationError, IReadOnlyList<NodeCreation>> errorFactory = new NodeCreationErrorFactory(baseErrorFactory);

            batchRegister.Register(creationController, errorFactory);
        }
    }
}