using Models.Responses;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Xunit;

namespace RSOI.Test.Controllers
{
    public class FilesControllerTest : BaseControllerTest
    {
        [Theory]
        [InlineData("api/files")]
        public async Task GetImage200_pageNo0(string url)
        {
            var jobStatusServiceId = Guid.NewGuid();
            var imgServiceId = Guid.NewGuid();
            var testBytes = new byte[] {1,2,3,4};
            var jobStatus = new JobInfo()
            {
                JobId = jobStatusServiceId.ToString(),
                JobStatus = "Done",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { new ImageResponseModel()
                {
                    ImgId = 1,
                    PageNo = 0,
                    Path = "testImgPath"
                } }
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(jobStatusServiceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
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

            FileServiceMock.Setup(s => s.GetFile(new GRPCService.GRPCProto.Path()
            {
                Path_ = jobStatus.Images.First().Path
            })).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = imgServiceId.ToString()
            }).Callback(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                jobExecutor.SetJobStatusByServiceGuid(imgServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, testBytes);
            });

            var response = await Client.GetAsync($"{url}/{jobStatusServiceId.ToString()}/image");
            var image = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/jpeg", response.Content.Headers.ContentType.ToString());
            Assert.Equal(testBytes, image);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/files")]
        public async Task GetImage200_pageNo10(string url)
        {
            var jobStatusServiceId = Guid.NewGuid();
            var imgServiceId = Guid.NewGuid();
            var testBytes = new byte[] { 1, 2, 3, 4 };
            var jobStatus = new JobInfo()
            {
                JobId = jobStatusServiceId.ToString(),
                JobStatus = "Done",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { new ImageResponseModel()
                {
                    ImgId = 1,
                    PageNo = 10,
                    Path = "testImgPath"
                } }
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(jobStatusServiceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
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

            FileServiceMock.Setup(s => s.GetFile(new GRPCService.GRPCProto.Path()
            {
                Path_ = jobStatus.Images.First().Path
            })).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = imgServiceId.ToString()
            }).Callback(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                jobExecutor.SetJobStatusByServiceGuid(imgServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, testBytes);
            });

            var response = await Client.GetAsync($"{url}/{jobStatusServiceId.ToString()}/image?PageNo={jobStatus.Images.First().PageNo}");
            var image = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/jpeg", response.Content.Headers.ContentType.ToString());
            Assert.Equal(testBytes, image);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/files")]
        public async Task GetPdf200(string url)
        {
            var jobStatusServiceId = Guid.NewGuid();
            var pdfServiceId = Guid.NewGuid();
            var testBytes = new byte[] { 1, 2, 3, 4 };
            var jobStatus = new JobInfo()
            {
                JobId = jobStatusServiceId.ToString(),
                JobStatus = "Done",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { new ImageResponseModel()
                {
                    ImgId = 1,
                    PageNo = 10,
                    Path = "testImgPath"
                } }
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(jobStatusServiceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
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

            FileServiceMock.Setup(s => s.GetFile(new GRPCService.GRPCProto.Path()
            {
                Path_ = jobStatus.PdfPath
            })).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = pdfServiceId.ToString()
            }).Callback(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                jobExecutor.SetJobStatusByServiceGuid(pdfServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, testBytes);
            });

            var response = await Client.GetAsync($"{url}/{jobStatusServiceId.ToString()}/pdf");
            var pdf = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.ToString());
            Assert.Equal(testBytes, pdf);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }

        [Theory]
        [InlineData("api/files")]
        public async Task GetImages200(string url)
        {
            var jobStatusServiceId = Guid.NewGuid();
            var imgInfoServiceId = Guid.NewGuid();
            var img1ServiceId = Guid.NewGuid();
            var img2ServiceId = Guid.NewGuid();
            var testBytes = new List<byte[]>()
            {
                new byte[] { 1, 2, 3, 4 },
                new byte[] { 1, 2, 3, 4, 5, 6 }
            };
            var jobStatus = new JobInfo()
            {
                JobId = jobStatusServiceId.ToString(),
                JobStatus = "Done",
                PdfId = 1,
                PdfPath = "testpath",
                Images = new List<ImageResponseModel>() { 
                    new ImageResponseModel()
                    {
                        ImgId = 1,
                        PageNo = 0,
                        Path = "testImgPath_1.jpeg"
                    },
                    new ImageResponseModel()
                    {
                        ImgId = 2,
                        PageNo = 1,
                        Path = "testImgPath_2.jpeg"
                    } 
                }
            };

            DataBaseServiceMock.Setup(s => s.GetJobInfo(jobStatusServiceId.ToString())).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
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

            DataBaseServiceMock.Setup(s => s.ImagesInfo(jobStatusServiceId.ToString(), 0, 10)).ReturnsAsync(
                new GRPCService.GRPCProto.JobInfo()
                {
                    JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                    JobId = imgInfoServiceId.ToString()
                }).Callback(async () => {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                
                byte[] bytes;
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, new ImagesResponseModel() {
                        Images = jobStatus.Images
                    });
                    bytes = ms.ToArray();
                }
                jobExecutor.SetJobStatusByServiceGuid(imgInfoServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, bytes);
            });

            var imgPaths = jobStatus.Images.Select(img => img.Path).ToList();
            FileServiceMock.Setup(s => s.GetFiles(imgPaths)).ReturnsAsync(new GRPCService.GRPCProto.JobInfo()
            {
                JobStatus = GRPCService.GRPCProto.EnumJobStatus.Execute,
                JobId = img1ServiceId.ToString()
            }).Callback(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                var jobExecutor = JobExecutor.JobExecutor.Instance;
                var memStream = new MemoryStream();
                using (var zipArc = new ZipArchive(memStream, ZipArchiveMode.Create, true))
                {
                    var i = 0;
                    foreach (var bytes in testBytes)
                    {
                        using(var entry = zipArc.CreateEntry(jobStatus.Images[i].Path).Open())
                        using(var ms = new MemoryStream(bytes))
                            await ms.CopyToAsync(entry);
                        
                        ++i;
                    }
                }
                jobExecutor.SetJobStatusByServiceGuid(img1ServiceId, GRPCService.GRPCProto.EnumJobStatus.Done, memStream.ToArray());
                memStream.Close();
            });
            

            var response = await Client.GetAsync($"{url}/{jobStatusServiceId.ToString()}/images");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/zip", response.Content.Headers.ContentType.ToString());
            var listBytes = new List<byte[]>();
            using (var zipArc = new ZipArchive(await response.Content.ReadAsStreamAsync()))
            {
                foreach (var entry in zipArc.Entries)
                {
                    using (var ms = new MemoryStream())
                    {
                        await entry.Open().CopyToAsync(ms);
                        listBytes.Add(ms.ToArray());
                    }
                }
            }
            Assert.Equal(testBytes,listBytes);
            DataBaseServiceMock.Reset();
            FileServiceMock.Reset();
        }
    }
}
