using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Domain.Models
{
    public class ApiKey : UserData
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
