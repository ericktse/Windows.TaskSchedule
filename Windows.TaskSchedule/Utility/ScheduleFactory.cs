using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Windows.TaskSchedule.Utility
{
    public class ScheduleFactory
    {
        private List<JobDetail> _jobs = new List<JobDetail>();

        public void Start()
        {
            _jobs = JobFactory.GeneralJobs();
            BatchProcess(_jobs);

            Logger.Debug(string.Format("当前服务运行目录:【{0}】,共找到【{1}】个任务.", AppDomain.CurrentDomain.BaseDirectory, _jobs.Count));
            Logger.Debug("服务启动成功.");
        }

        public void Stop()
        {
            foreach (var job in _jobs)
            {
                if (job.RunInSandbox && job.Sandbox != null)
                {
                    job.Sandbox.Dispose();
                }
            }

            Logger.Debug("服务停止成功.");
        }


        private void BatchProcess(List<JobDetail> jobs)
        {
            foreach (var job in jobs)
            {
                Task jobTask = new Task(() =>
                {
                    while (true)
                    {
                        if (!job.IsRunning)
                        {
                            job.IsRunning = true;

                            RunJob(job);

                            job.IsRunning = false;
                        }
                        Thread.Sleep(600);
                    }
                });

                jobTask.Start();
            }
        }

        private void RunJob(JobDetail job)
        {
            try
            {
                if (CornTrigger(job.CornExpress, DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))))
                {
                    if (!job.IsTriggering)
                    {
                        job.IsTriggering = true;

                        switch (job.JobType)
                        {
                            case JobType.Assembly:
                                RunAssembly(job);
                                break;
                            case JobType.Exe:
                                RunExe(job);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("任务【{0}】执行异常，Message:{1}.", job.Name, ex.Message), ex);
            }
            finally
            {
                job.IsTriggering = false;
            }
        }

        private void RunAssembly(JobDetail job)
        {
            try
            {
                if (job.RunInSandbox)
                {
                    job.Sandbox.Execute(job.AssemblyName, job.TypeName, "Init", null);
                    job.Sandbox.Execute(job.AssemblyName, job.TypeName, "Excute", null);
                }
                else
                {
                    job.Instance.Init();
                    job.Instance.Execute();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("任务【{0}】执行异常，Message:{1}.", job.Name, ex.Message), ex);

                try
                {
                    if (job.RunInSandbox)
                    {
                        job.Sandbox.Execute(job.AssemblyName, job.TypeName, "OnError", ex);
                    }
                    else
                    {
                        job.Instance.OnError(ex);
                    }
                }
                catch (Exception innerEx)
                {
                    Logger.Error(string.Format("任务【{0}】执行异常后执行异常输出处理操作异常，Message:{1}.", job.Name, innerEx.Message), innerEx);
                }
            }
        }

        private void RunExe(JobDetail job)
        {
            try
            {
                using (var process = new Process())
                {
                    if (string.IsNullOrWhiteSpace(job.Arguments))
                    {
                        process.StartInfo = new ProcessStartInfo(job.Path);
                    }
                    else
                    {
                        process.StartInfo = new ProcessStartInfo(job.Path, job.Arguments);
                    }
                    process.Start();
                    //如果设置了最长运行时间，到达时间时，自动中止进程
                    if (job.ExpireSecond.HasValue)
                    {
                        bool processIsExit = process.WaitForExit(job.ExpireSecond.Value * 1000);
                        if (!processIsExit)
                        {
                            Logger.Info(string.Format("任务【{0}】因长时间：{1}秒未返回运行状态，程序已自动将其Kill.", job.Name, job.ExpireSecond));
                            process.Kill();
                        }
                    }
                    else
                    {
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("任务【{0}】执行异常，Message:{1}.", job.Name, ex.Message), ex);
            }
        }

        private bool CornTrigger(string cornExpress, DateTime dateUtc)
        {
            CronExpression corn = new CronExpression(cornExpress);
            return corn.IsSatisfiedBy(dateUtc);
        }
    }
}
