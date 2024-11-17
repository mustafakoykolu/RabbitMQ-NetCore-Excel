using ExcelCreator.Models;
using ExcelCreator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ExcelCreator.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        public IActionResult Index()
        {
            return View();
        }
        public ProductController(AppDbContext appDbContext, UserManager<IdentityUser> userManager, RabbitMQPublisher rabbitMQPublisher)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1,10)}";

            UserFile userFile = new() { UserId=user.Id,FileStatus=FileStatus.Creating, FileName = fileName };
            _appDbContext.UserFiles.Add(userFile);
            _appDbContext.SaveChanges();
            var product = new CreateExcelMessage() { FileId = userFile.Id, UserId = user.Id };
            _rabbitMQPublisher.Publish(product);

            TempData["StartCreatingExcel"]=true;

            return RedirectToAction(nameof(Files));
        }
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return View( _appDbContext.UserFiles.Where(x=> x.UserId==user.Id).ToList());
        }
    }
}
