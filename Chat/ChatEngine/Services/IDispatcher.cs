using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatEngine.Services
{
    public interface IDispatcher
    {
        void RunAsync(Action action);
    }
}
