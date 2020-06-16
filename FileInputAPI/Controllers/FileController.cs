using FileInputAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileInputAPI.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IUpdateOpenWorkOrderNumbers _updateOpenWorkOrderNumbers;
        public FileController(IUpdateOpenWorkOrderNumbers updateOpenWorkOrderNumbers)
        {
            this._updateOpenWorkOrderNumbers = updateOpenWorkOrderNumbers;
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile uploadedFile)
        {
            try
            {
                _updateOpenWorkOrderNumbers.UpdateWONumbersWithEPPlus(uploadedFile);
            }
            catch(Exception ex)
            {
                return Ok(new { error = ex.Message });
            }

            return Ok(new { message = "open work order numbers has been successfully added" });
        }

        [HttpGet]
        public IActionResult GetSomething()
        {
            return Ok("this is just testing that application is working fine or not");
        }
    }
}
