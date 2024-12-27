using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JarvisWeb.Services.Services
{
    public class GlobalStateService
    {
        public static event Func<Guid, Task>? Notify;

        public async void StateHasChanged(Guid userId)
        {
            if (Notify != null)
            {
                await Notify.Invoke(userId);
            }
        }
    }
}
