
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Models.Responses;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace RSOI.Test.Controllers
{
    public class JobControllerTest : BaseControllerTest
    {     
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