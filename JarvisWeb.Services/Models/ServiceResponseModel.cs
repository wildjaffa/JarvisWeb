using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models
{
    public class ServiceResponseModel<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

    }
}
