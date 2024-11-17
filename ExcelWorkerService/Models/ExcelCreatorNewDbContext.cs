using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExcelWorkerService.Models;

public partial class ExcelCreatorNewDbContext : DbContext
{
    public ExcelCreatorNewDbContext()
    {
    }

    public ExcelCreatorNewDbContext(DbContextOptions<ExcelCreatorNewDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserFile> UserFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
