using System;
using System.Threading.Tasks;
using DataBaseServer.DBO;
using Grpc.Core;
using GRPCService.DataBaseProto;
using Microsoft.EntityFrameworkCore;

namespace DataBaseServer
{
    public class DataBaseServerGrpc : DataBase.DataBaseBase, IDisposable
    {
        private RsoiDBContext dbcontext = null;
        
        public DataBaseServerGrpc() : base()
        {
            dbcontext = new RsoiDBContext();
        }

        public override async Task<DatabaseCreateFileInfoResponse> SavePdfFileInfo(PdfFileInfo request, ServerCallContext context)
        {
            try
            {
                var result = await dbcontext.FileInfos.AddAsync(FileInfo.fromPdfFileInfo(request));
                if(result.State != EntityState.Added)
                    throw new Exception("File not added");
                var result2 = await dbcontext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
            return new DatabaseCreateFileInfoResponse()
            {
                Error = new CommonResponse()
                {
                    ErrorMessage = "",
                    IsError = false
                },
                FileId = "testFileId"
            };
        }

        public void Dispose()
        {
            dbcontext?.Dispose();
        }
    }
}