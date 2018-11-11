
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Models.Responses;
using Moq;
using Newtonsoft.Json;
using RSOI.Services;
using Xunit;

namespace RSOI.Test.Controllers
{ 
    public class JobControllerTest
    {
        public Mock<IDataBaseService> DataBaseServiceMock { get; }
        public Mock<IFileService> FileServiceMock { get; }
        public Mock<IRecognizeService> RecognizeServiceMock { get; }

        private TestServer _server;
        public HttpClient Client { get; set; }

        public JobControllerTest()
        {            
            DataBaseServiceMock = new Mock<IDataBaseService>();
            FileServiceMock = new Mock<IFileService>();
            RecognizeServiceMock = new Mock<IRecognizeService>();

            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>().ConfigureTestServices( services =>
            {
                services.AddSingleton<IDataBaseService>(DataBaseServiceMock.Object);
                services.AddSingleton<IFileService>(FileServiceMock.Object);
                services.AddSingleton<IRecognizeService>(RecognizeServiceMock.Object);
            }));

            Client = _server.CreateClient();
        }

        

        [Theory]
        [InlineData("api/job")]
        public async Task Index200(string url)
        {
            var serviceId = Guid.NewGuid();

            var jobList = new List<JobInfoBase>();
            jobList.Add(new JobInfoBase() {
                JobId = Guid.NewGuid().ToString(),
                JobStatus = "Executed"
            });
            jobList.Add(new JobInfoBase()
            {
                JobId = Guid.NewGuid().ToString(),
                JobStatus = "Done"
            });

            DataBaseServiceMock.Setup(s => s.GetAllJobInfos()).ReturnsAsync(new GRPCService.GRPCProto.JobInfo() {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = serviceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, jobList);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(serviceId, GRPCService.GRPCProto.EnumJobStatus.Done,bytes);
            });

            var response = await Client.GetAsync(url);
            var jsonObject = JsonConvert.DeserializeObject<List<JobInfoBase>>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(jobList, jsonObject);
            DataBaseServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/job")]
        public async Task GetJobStatusExecuted200(string url)
        {
            var serviceId = Guid.NewGuid();

            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Executed"
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(serviceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = serviceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, jobStatus);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(serviceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var response = await Client.GetAsync($"{url}/{serviceId.ToString()}");
            var jsonObject = JsonConvert.DeserializeObject<JobInfo>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(jobStatus, jsonObject);
            DataBaseServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/job")]
        public async Task GetJobStatusDone200(string url)
        {
            var serviceId = Guid.NewGuid();

            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Executed",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { new ImageResponseModel()
                {
                    ImgId = 1,
                    PageNo = 1,
                    Path = "testImgPath"
                } }
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(serviceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = serviceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, jobStatus);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(serviceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var response = await Client.GetAsync($"{url}/{serviceId.ToString()}");
            var jsonObject = JsonConvert.DeserializeObject<JobInfo>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(jobStatus, jsonObject);
            DataBaseServiceMock.Reset();
        }
    }
}