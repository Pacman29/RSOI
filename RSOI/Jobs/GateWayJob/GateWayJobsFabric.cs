using System;
using System.Collections.Generic;
using GRPCService.GRPCProto;
using JobExecutor;
using RSOI.Services;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    class IdSetter<T> where T: BaseJob
    {
        public static T SetJobGuid(T job)
        {
            job.Guid = Guid.NewGuid();
            return job;
        }
    }
    
    public class GateWayJobsFabric : IGateWayJobsFabric
    {
        private IDataBaseService DataBaseService { get; set; }
        private IFileService FileService { get; set; }
        private IRecognizeService RecognizeService { get; set; }

        public GateWayJobsFabric(
            IDataBaseService dataBaseService,
            IFileService fileService,
            IRecognizeService recognizeService)
        {
            DataBaseService = dataBaseService;
            FileService = fileService;
            RecognizeService = recognizeService;
        }

        
        public UpdateJobToDatabase GetUpdateJobToDatabase(string JobId, EnumJobStatus status)
        {
            return IdSetter<UpdateJobToDatabase>.SetJobGuid(new UpdateJobToDatabase(JobId, status)
            {
                DataBaseService = this.DataBaseService
            });
        }

        public SaveFileJob GetSaveFileJob(byte[] bytes, string path)
        {
            return IdSetter<SaveFileJob>.SetJobGuid(new SaveFileJob(bytes, path)
            {
                FileService = this.FileService
            });
        }

        public RecognizePdfHighOrderJob GetRecognizePdfHighOrderJob(PdfFile pdfFile)
        {
            return IdSetter<RecognizePdfHighOrderJob>.SetJobGuid(new RecognizePdfHighOrderJob(pdfFile)
            {
                GateWayJobsFabric = this
            });
        }

        public RecognizePdfFileJob GetRecognizePdfFileJob(byte[] pdfFile, List<int> pages)
        {
            return IdSetter<RecognizePdfFileJob>.SetJobGuid(new RecognizePdfFileJob(pdfFile, pages)
            {
                RecognizeService = this.RecognizeService
            });
        }

        public CreateJobToDatabase GetCreateJobToDatabase()
        {
            return IdSetter<CreateJobToDatabase>.SetJobGuid(new CreateJobToDatabase()
            {
                DataBaseService = this.DataBaseService
            });
        }

        public AddFileInfoToDatabaseJob GetAddFileInfoToDatabaseJob(FileInfo fileInfo)
        {
            return IdSetter<AddFileInfoToDatabaseJob>.SetJobGuid(new AddFileInfoToDatabaseJob(fileInfo)
            {
                DataBaseService = this.DataBaseService
            });
        }

        public PackageJob GetPackageJob()
        {
            return IdSetter<PackageJob>.SetJobGuid(new PackageJob());
        }

        public GetJobStatusHighOrderJob GetJobStatusHighOrderJob(string jobId)
        {
            return IdSetter<GetJobStatusHighOrderJob>.SetJobGuid(new GetJobStatusHighOrderJob(jobId)
            {
                DataBaseService = this.DataBaseService
            });
        }

        public GetPdfFileHighOrderJob GetPdfFileHighOrderJob(string jobId)
        {
            return IdSetter<GetPdfFileHighOrderJob>.SetJobGuid(new GetPdfFileHighOrderJob(jobId)
            {
                GateWayJobsFabric = this
            });
        }
        
        public GetFileJob GetFileJob(string path)
        {
            return IdSetter<GetFileJob>.SetJobGuid(new GetFileJob(path)
            {
                FileService = this.FileService
            });
        }
    }
}