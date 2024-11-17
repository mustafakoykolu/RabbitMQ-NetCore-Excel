using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExcelCreator.Models
{
    public enum FileStatus
    {
        Creating,
        Completed
    }
    public class UserFile
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string FileName { get; set; }
        public  string? FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public FileStatus FileStatus { get; set; }

        [NotMapped]
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";

    }

}
