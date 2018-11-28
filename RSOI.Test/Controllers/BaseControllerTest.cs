using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RSOI.Services;
using System.Net.Http;

namespace RSOI.Test.Controllers
{
    public class BaseControllerTest
    {

        public Mock<IDataBaseService> DataBaseServiceMock { get; }
        public Mock<IFileService> FileServiceMock { get; }
        public Mock<IRecognizeService> RecognizeServiceMock { get; }

        private TestServer _server;
        public HttpClient Client { get; set; }

        public BaseControllerTest()
        {
            DataBaseServiceMock = new Mock<IDataBaseService>();
            FileServiceMock = new Mock<IFileService>();
            RecognizeServiceMock = new Mock<IRecognizeService>();

            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>().ConfigureTestServices(services =>
            {
                services.AddSingleton<IDataBaseService>(DataBaseServiceMock.Object);
                services.AddSingleton<IFileService>(FileServiceMock.Object);
                services.AddSingleton<IRecognizeService>(RecognizeServiceMock.Object);
            }));

            Client = _server.CreateClient();
        }
    }
}
