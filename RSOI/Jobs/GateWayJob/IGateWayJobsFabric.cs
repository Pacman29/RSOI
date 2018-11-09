using System.Collections.Generic;
using GRPCService.GRPCProto;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public interface IGateWayJobsFabric
    {
        UpdateJobToDatabase GetUpdateJobToDatabase(string JobId, EnumJobStatus status);
        SaveFileJob GetSaveFileJob(byte[] bytes, string path);
        RecognizePdfHighOrderJob GetRecognizePdfHighOrderJob(PdfFile pdfFile);
        RecognizePdfFileJob GetRecognizePdfFileJob(byte[] pdfFile, List<int> pages);
        CreateJobToDatabase GetCreateJobToDatabase();
        AddFileInfoToDatabaseJob GetAddFileInfoToDatabaseJob(FileInfo fileInfo);
        PackageJob GetPackageJob();
    }
}