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
        GetJobStatusHighOrderJob GetJobStatusHighOrderJob(string jobId);
        GetPdfFileHighOrderJob GetPdfFileHighOrderJob(string jobId);
        GetFileJob GetFileJob(string path);
        GetImagesHighOrderJob GetImagesHighOrderJob(string jobId, int pageNo, int count);
        GetImagesInfoJob GetImagesInfoJob(string jobId, int pageNo, int count);
        GetFilesJob GetFilesJob(List<string> paths);
    }
}