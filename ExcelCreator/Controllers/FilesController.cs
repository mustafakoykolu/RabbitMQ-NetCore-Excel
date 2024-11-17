using ExcelCreator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExcelCreator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public FilesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile? excelFile, [FromQuery]int fileId)
        {
            if (excelFile is not { Length: > 0 }) return BadRequest();

            var userFile = _appDbContext.UserFiles.First(x => x.Id == fileId);
            var filePath = userFile?.FileName + Path.GetExtension(excelFile.FileName);

            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/files", filePath);

            using FileStream stream = new(path, FileMode.Create);
            excelFile.CopyTo(stream);

            userFile.CreatedDate=DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
