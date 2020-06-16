using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileInputAPI.Services
{
    public interface IUpdateOpenWorkOrderNumbers
    {
        void UpdateWONumbersWithEPPlus(IFormFile file);
    }
}
