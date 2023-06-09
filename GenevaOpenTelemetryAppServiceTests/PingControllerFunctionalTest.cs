using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GenevaOpenTelemetryAppServiceTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class PingControllerFunctionalTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("FunctionalTest")]
        public async Task Get_FunctionalTest_Ping()
        {
            var uri = (string)TestContext.Properties["URLforTesting"];
            var client = new HttpClient();
            client.BaseAddress = new Uri(uri);
            client.Timeout = new TimeSpan(5, 0, 0);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("/api/ping");
            Assert.AreEqual(StatusCodes.Status200OK, (int)response.StatusCode);
        }
    }
}
