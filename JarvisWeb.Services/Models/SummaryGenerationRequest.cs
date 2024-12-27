using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Models
{
    public class SummaryGenerationRequest
    {
        public Guid UserId { get; set; }
        public bool GenerateVideo { get; set; } = false;
    }
}
