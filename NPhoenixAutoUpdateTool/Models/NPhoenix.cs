namespace NPhoenixAutoUpdateTool.Models
{
    public class NPhoenix
    {
        public int Id { get; set; }

        public string Version { get; set; }

        public int DownLoadNumber { get; set; }

        public string? LinkName { get; set; }

        public string DownUrl { get; set; }

        public string StartName { get; set; }

        public string? Describe { get; set; }
    }
}
