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
        
        public GetImagesHighOrderJob GetImagesHighOrderJob(string jobId, int pageNo, int count)
        {
            return IdSetter<GetImagesHighOrderJob>.SetJobGuid(new GetImagesHighOrderJob(jobId, pageNo, count)
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

        public GetImagesInfoJob GetImagesInfoJob(string jobId, int pageNo, int count)
        {
            return IdSetter<GetImagesInfoJob>.SetJobGuid(new GetImagesInfoJob(jobId, pageNo, count)
            {
                DataBaseService = this.DataBaseService
            });
        }

        public GetImageHighOrderJob GetImageHighOrderJob(string jobId, long pageNo)
        {
            return IdSetter<GetImageHighOrderJob>.SetJobGuid(new GetImageHighOrderJob(jobId, pageNo)
            {
                GateWayJobsFabric = this
            });
        }
        
        public GetFilesJob GetFilesJob(List<string> paths)
        {
            return IdSetter<GetFilesJob>.SetJobGuid(new GetFilesJob(paths)
            {
                FileService = this.FileService
            });
        }

        public DeleteJobHighOrderJob DeleteJobHighOrderJob(string jobId)
        {
            return IdSetter<DeleteJobHighOrderJob>.SetJobGuid(new DeleteJobHighOrderJob(jobId)
            {
                GateWayJobsFabric = this
            });
        }

        public DeleteJobFromDatabase DeleteJobFromDatabase(string jobId)
        {
            return IdSetter<DeleteJobFromDatabase>.SetJobGuid(new DeleteJobFromDatabase(jobId)
            {
                DataBaseService = this.DataBaseService
            });
        }
        
        public DeleteFileJob DeleteFileJob(string path)
        {
            return IdSetter<DeleteFileJob>.SetJobGuid(new DeleteFileJob(path)
            {
                FileService = this.FileService
            });
        }

        public DeleteFileInfoFromDatabaseJob DeleteFileInfoFromDatabaseJob(string jobId, int id, string path, EnumFileType fileType)
        {
            return IdSetter<DeleteFileInfoFromDatabaseJob>.SetJobGuid(new DeleteFileInfoFromDatabaseJob(jobId, id, path,fileType)
            {
                DataBaseService = this.DataBaseService
            });
        }

        public UpdateJobHighOrderJob UpdateJobHighOrderJob(string jobId, PdfFile pdfFile)
        {
            return IdSetter<UpdateJobHighOrderJob>.SetJobGuid(new UpdateJobHighOrderJob(pdfFile,jobId)
            {
                GateWayJobsFabric = this
            });
        }

        public GetAllJobs GetAllJobs()
        {
            return IdSetter<GetAllJobs>.SetJobGuid(new GetAllJobs()
            {
                DataBaseService = this.DataBaseService
            });
        }

        public GetAllJobInfosHighOrderJob GetAllJobInfosHighOrderJob()
        {
            return IdSetter<GetAllJobInfosHighOrderJob>.SetJobGuid(new GetAllJobInfosHighOrderJob()
            {
                GateWayJobsFabric = this
            });
        }
    }
}