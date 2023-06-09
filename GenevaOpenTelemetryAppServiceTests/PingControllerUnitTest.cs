using Castle.Core.Logging;
using GenevaOpenTelemetryAppService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using System.Diagnostics;

namespace GenevaOpenTelemetryAppServiceTests
{
    [Ignore]
    [TestClass]
    public class PingControllerUnitTest
    {
        private Mock<ILogger<PingController>>? _mockLogger;

        [TestInitialize]
        public void TestInitiatlize()
        {
            _mockLogger = new Mock<ILogger<PingController>>();
            _mockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()))
                .Callback(new InvocationAction(invocation =>
                {
                    var logLevel = (LogLevel)invocation.Arguments[0]; // The first two will always be whatever is specified in the setup above
                    var eventId = (EventId)invocation.Arguments[1];  // so I'm not sure you would ever want to actually use them
                    var state = invocation.Arguments[2];
                    var exception = (Exception)invocation.Arguments[3];
                    var formatter = invocation.Arguments[4];

                    var invokeMethod = formatter.GetType().GetMethod("Invoke");
                    var logMessage = (string)invokeMethod?.Invoke(formatter, new[] { state, exception });

                    Trace.WriteLine($"{logLevel} - {logMessage}");
                }));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void Get_UnitTest_Ping()
        {
            // create unit test for PingController.cs
            
        }
    }
}