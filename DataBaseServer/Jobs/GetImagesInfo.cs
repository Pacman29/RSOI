using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DataBaseServer.Contexts;
using GRPCService.GRPCProto;
using JobExecutor;
using Models.Responses;

namespace DataBaseServer.Jobs
{
    public class GetImagesInfo : BaseJob
    {
        private readonly JobsDbManager _jobDbManager;
        private readonly string _jobId;
        private readonly long _firstPageNo;
        private readonly long _count;

        public GetImagesInfo(JobsDbManager jobsDbManager,string jobId, long firstPageNo, long count)
        {
            this._jobDbManager = jobsDbManager;
            this._jobId = jobId;
            _firstPageNo = firstPageNo;
            _count = count;
        }

        public override async Task ExecuteAsync()
        {
            var files = await _jobDbManager.FindInJobAndFileInfoJoin(this._jobId, e =>
                {
                    return e.Where(ent => ent.FileType == EnumFileType.Image).OrderBy(f => f.PageNo)
                        .Skip((int)this._firstPageNo).Take((int)this._count);
                });

            if (files.Count > 0)
            {
                var images = new ImagesResponseModel();
                foreach (var img in files)
                {
                    images.Images.Add(new ImageResponseModel()
                    {
                        PageNo = img.PageNo,
                        Path = img.Path
                    });
                }
                
                var formatter = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    formatter.Serialize(ms, images);
                    this.Bytes = ms.ToArray();
                }
            }
        }
    }
}