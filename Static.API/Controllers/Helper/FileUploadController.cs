using Static.Application.Common.FileUpload.ImageUploadCommand;
using Static.Results;
using Microsoft.AspNetCore.Mvc;
using Static.API.Configurations;

namespace Static.API.Controllers.Helper
{
    public class FileUploadController : ApiControllerBase
    {
        //TODO: Need to add Authorization and Permission::CreateBySuperAdmin
        /// <summary>
        /// Upload image and get an image url. Less then 1 MB image.
        /// </summary>
        // [Authorize(Roles =
        //     $"{RoleConstant.Child}, {RoleConstant.Parent},{RoleConstant.SuperAdmin},{RoleConstant.ContentExpert}")]
        // [StaticPermission(Permissions = new[]
        // {
        //     Permissions.CreateByChild, Permissions.CreateByParent, Permissions.CreateBySuperAdmin,
        //     Permissions.CreateByContentExpert
        // })]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("UploadImage")]
        public async Task<Result<string>> UploadImage(IFormFile file)
        {
            var command = new ImageUploadCommand(file);
            return await Mediator.Send(command);
        }


        /// <summary>
        /// This is the route to get an file stream image file for image view we will remove this
        /// route after we decide a proper image storing service
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult Image(string id)
        {
            var path = Path.GetDirectoryName(typeof(ConfigureServices).Assembly.Location);
            var filePath = path+ "/FileUpload/" +id;

            if (System.IO.File.Exists(filePath))
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new FileStreamResult(fileStream, "image/jpeg");
            }

            return NotFound();
        }
        
    }
}
