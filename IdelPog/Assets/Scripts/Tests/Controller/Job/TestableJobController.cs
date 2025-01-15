using IdelPog.Controller;
using IdelPog.Orchestration;

namespace Tests.Controller
{
    internal class TestableJobController :  JobController
    {
        internal TestableJobController(IJobMediator jobMediatorMock)
        {
            Mediator = jobMediatorMock;
        }
    }
}