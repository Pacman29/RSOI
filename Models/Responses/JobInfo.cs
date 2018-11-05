namespace Models.Responses
{
    public class JobInfo
    {
        public string JobId { get; set; }
        public GRPCService.GRPCProto.EnumJobStatus JobStatus { get; set; }
        public string PdfPath { get; set; } = "";
        public string[] ImagePath { get; set; } = new string[0];
    }
}