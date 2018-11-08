using System;
using System.Collections.Generic;
using GRPCService.GRPCProto;
using RSOI.Services;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    class IdSetter<T> where T: GateWayJob
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

        public RecognizePdfRootJob GetRecognizePdfJob(PdfFile pdfFile)
        {
            return IdSetter<RecognizePdfRootJob>.SetJobGuid(new RecognizePdfRootJob(pdfFile)
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
    }
}