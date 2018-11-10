using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using JobExecutor;

namespace FileServer.Jobs
{
    public class GetFilesJob : BaseJob
    {
        private readonly List<string> _paths;

        public GetFilesJob(List<string> paths)
        {
            _paths = paths;
        }

        public override async Task ExecuteAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var path in _paths)
                        using (var fileStream = new FileStream(path,FileMode.Open))
                        using (var entryStream = archive.CreateEntry(Path.GetFileName(path)).Open())
                            await fileStream.CopyToAsync(entryStream);
                 
                }
                this.Bytes = memoryStream.ToArray();
            }
            
        }
    }
}