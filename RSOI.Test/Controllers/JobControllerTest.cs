
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                JobStatus = "Execute"
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
                JobStatus = "Execute"
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
                JobStatus = "Execute",
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

        [Theory]
        [InlineData("api/job")]
        public async Task DeleteJob200(string url)
        {
            var serviceId = Guid.NewGuid();
            var updateJobServiceId = Guid.NewGuid();
            var deleteJobInfoServiceId = Guid.NewGuid();

            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Done",
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

            DataBaseServiceMock.Setup(s => s.UpdateOrCreateJob(new GRPCService.GRPCProto.JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute
            })).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = updateJobServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, serviceId.ToString());
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(updateJobServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            DataBaseServiceMock.Setup(s => s.DeleteJobInfo(serviceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = deleteJobInfoServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, true);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(deleteJobInfoServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var deletePdfFIleServiceId = Guid.NewGuid();
            FileServiceMock.Setup(s => s.DeleteFile(jobStatus.PdfPath)).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = deletePdfFIleServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, true);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(deletePdfFIleServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var deleteImgFIleServiceId = Guid.NewGuid();
            FileServiceMock.Setup(s => s.DeleteFile(jobStatus.Images[0].Path)).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = deleteImgFIleServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, true);
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(deleteImgFIleServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var response = await Client.DeleteAsync($"{url}/{serviceId.ToString()}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/job")]
        public async Task DeleteJob409(string url)
        {
            var serviceId = Guid.NewGuid();

            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Execute",
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

            var response = await Client.DeleteAsync($"{url}/{serviceId.ToString()}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            DataBaseServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/job")]
        public async Task RecognizePdfJob200(string url)
        {
            var serviceId = Guid.NewGuid();
            var testPdfBytes = new byte[] { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Execute"
            };

            var createJobInfoServiceId = Guid.NewGuid();
            DataBaseServiceMock.Setup(s => s.UpdateOrCreateJob(new GRPCService.GRPCProto.JobInfo())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = createJobInfoServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, serviceId.ToString());
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(createJobInfoServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });



            var memoryStream = new MemoryStream(testPdfBytes);
            var content = new MultipartFormDataContent();
            content.Add(CreateFileContent(memoryStream, "testPdf.pdf", "multipart/form-data"));
            var response = await Client.PostAsync($"{url}",content);
            var jsonObject = JsonConvert.DeserializeObject<JobInfo>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(jobStatus, jsonObject);
            DataBaseServiceMock.Reset();
        }

        private static StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
        {
            try
            {
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "File",
                    FileName = "\"" + fileName + "\""
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                return fileContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [Theory]
        [InlineData("api/job")]
        public async Task UpdatePdfJob200(string url)
        {
            var serviceId = Guid.NewGuid();
            var testPdfBytes = new byte[] { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            var jobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Done",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { new ImageResponseModel()
                {
                    ImgId = 1,
                    PageNo = 1,
                    Path = "testImgPath"
                } }
            };

            var resultJobStatus = new JobInfo()
            {
                JobId = serviceId.ToString(),
                JobStatus = "Execute"
            };

            var jobStatusServiceId = Guid.NewGuid();
            DataBaseServiceMock.Setup(s => s.GetJobInfo(serviceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = jobStatusServiceId.ToString()
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
                jobExecutor.SetJobStatusByServiceGuid(jobStatusServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var updateJobInfoServiceId = Guid.NewGuid();
            DataBaseServiceMock.Setup(s => s.UpdateOrCreateJob(new GRPCService.GRPCProto.JobInfo() {
                JobId = serviceId.ToString(),
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute
            })).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = updateJobInfoServiceId.ToString()
            }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, serviceId.ToString());
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(updateJobInfoServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });




            var memoryStream = new MemoryStream(testPdfBytes);
            var content = new MultipartFormDataContent();
            content.Add(CreateFileContent(memoryStream, "testPdf.pdf", "multipart/form-data"));
            var response = await Client.PatchAsync($"{url}/{serviceId.ToString()}", content);
            var jsonObject = JsonConvert.DeserializeObject<JobInfo>(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(resultJobStatus, jsonObject);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }
    }

}