using System;
using System.Collections.Generic;

namespace ExcelWorkerService.Models;

public partial class UserFile
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string? FilePath { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int FileStatus { get; set; }
}
