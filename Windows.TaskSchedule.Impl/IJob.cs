using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.TaskSchedule.Impl
{
    public interface IJob
    {
        void Init();
        void Execute();
        void OnError(Exception ex);
    }
}
