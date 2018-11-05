using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using JobExecutor;
using RSOI.Services;
using PdfFile = Models.Requests.PdfFile;

namespace RSOI.Jobs
{
    public class RecognizePdfRootJob : BaseJob
    {

        private readonly PdfFile _pdfFile;
        private readonly IDataBaseService _dataBaseService;
        private readonly IRecognizeService _recognizeService;
        private readonly IFileService _fileService;
        private int? _fileId = null;
        private MemoryStream _archive = null;
        private bool _isPdfInfoCreated = false;
        private bool _isPdfFileCreated = false;
        private bool _isPdfFileRecognized = false;
        private bool _isJobCreated = false;

        public string JobId { get; set; } = null; 
        
        private readonly object threadLock3 = new object();
        
        public RecognizePdfRootJob(
            Guid guid,
            PdfFile pdfFile,
            IDataBaseService dataBaseService,
            IRecognizeService recognizeService,
            IFileService fileService)
        {
            _pdfFile = pdfFile;
            Guid = guid;
            _fileService = fileService;
            _recognizeService = recognizeService;
            _dataBaseService = dataBaseService;
        }

        private async Task Stage2()
        {
            Console.WriteLine($"Check st2 {_isJobCreated}");
            if (_isJobCreated)
            {
                var pdfFileInfo = await _pdfFile.GetFileInfo();
                pdfFileInfo.Path = $"{this.JobId}.pdf";

                var bytes = await _pdfFile.ReadFile();
        
                var addFileToDatabaseJob = new AddFileInfoToDatabaseJob(
                    System.Guid.NewGuid(),
                    _dataBaseService,
                    pdfFileInfo,
                    this)
                {
                    OnDone = async (job) =>
                    {
                        var bf = new BinaryFormatter();
                        using (var ms = new MemoryStream(job.Bytes))
                        {
                            _fileId = (int) bf.Deserialize(ms);
                        }

                        _isPdfInfoCreated = true;
                        await Stage3();
                    }
                };


                var savePdfFileJob = new SavePdfFileJob(
                    System.Guid.NewGuid(),
                    _fileService,
                    bytes,
                    pdfFileInfo.Path,
                    this)
                {
                    OnDone = async (job) =>
                    {
                        _isPdfFileCreated = true; 
                        await Stage3();
                    }
                };
        
                var recognizePdfFileJob = new RecognizePdfFileJob(
                    System.Guid.NewGuid(),
                    _recognizeService,
                    bytes,
                    new int[0],
                    this
                )
                {
                    OnDone = async (job) =>
                    {
                        this._archive = new MemoryStream(job.Bytes);
                        this._isPdfFileRecognized = true;
                        await Stage3();
                    }
                };
        
                this.Executor.JobAsyncExecute(addFileToDatabaseJob);
                this.Executor.JobAsyncExecute(savePdfFileJob);
                this.Executor.JobAsyncExecute(recognizePdfFileJob);
            }
        }
        
        private async Task Stage3()
        {
            lock (threadLock3)
            {
                //TODO: delete this
                Console.WriteLine($"Check st3 {_isPdfFileCreated} {_isPdfFileRecognized} {_isPdfInfoCreated}");
                if (_isPdfFileCreated && _isPdfFileRecognized && _isPdfInfoCreated)
                {
                    var zipArch = new ZipArchive(_archive);
                    foreach (var image in zipArch.Entries)
                    {
                        using (var ms = new MemoryStream())
                        {
                            image.Open().CopyTo(ms);
                            var savePdfFileJob = new SavePdfFileJob(
                                System.Guid.NewGuid(),
                                _fileService,
                                ms.ToArray(),
                                image.FullName,
                                this);
                            this.Executor.JobAsyncExecute(savePdfFileJob);
                        }
                    }
                }
            }
        }
        
        
        public override async Task ExecuteAsync()
        {
            //Create job to database
            var createJobToDatabaseJob = new CreateJobToDatabase(_dataBaseService)
            {
                OnDone = async (job) =>
                {
                    var bf = new BinaryFormatter();
                    using (var ms = new MemoryStream(job.Bytes))
                    {
                          JobId = (string) bf.Deserialize(ms);
                    }

                    _isJobCreated = true;
                    await Stage2();
                }
            };

            this.Executor.JobAsyncExecute(createJobToDatabaseJob);
        }

        public override Task Reject()
        {
            return base.Reject();
        }
    }
}