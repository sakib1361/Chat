using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Services
{
    class WorkerService
    {
        public static WorkerService Instance = new WorkerService();
        private WorkerService()
        {

        }

        public int ErrorCode { get; internal set; }
        public string ErrorMessage { get; internal set; }
    }
}
