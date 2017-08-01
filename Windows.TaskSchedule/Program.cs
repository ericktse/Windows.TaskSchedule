using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.TaskSchedule.Utility;
using Topshelf;

namespace Windows.TaskSchedule
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ScheduleFactory>(sc =>
                {
                    sc.ConstructUsing(() => new ScheduleFactory());
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                });
                x.SetServiceName(JobFactory.ServerName);
                x.SetDisplayName(JobFactory.DisplayName);
                x.SetDescription(JobFactory.Description);
                x.RunAsLocalSystem();
                x.StartAutomatically();
            });
        }
    }
}
