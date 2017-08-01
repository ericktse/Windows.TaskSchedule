using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.TaskSchedule.Impl;
using Quartz;

namespace Windows.TaskSchedule.Utility
{
    public class JobFactory
    {
        private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Jobs.config");
        public static readonly string ServerName;
        public static readonly string Description;
        public static readonly string DisplayName;

        static JobFactory()
        {
            XDocument jobxml = XDocument.Load(ConfigPath);
            ServerName = jobxml.Element("Jobs").Attribute("serverName").Value;
            Description = jobxml.Element("Jobs").Attribute("description").Value;
            DisplayName = jobxml.Element("Jobs").Attribute("displayName").Value;
        }

        public static List<JobDetail> GeneralJobs()
        {
            List<JobDetail> jobList = new List<JobDetail>();

            XDocument jobxml = XDocument.Load(ConfigPath);
            var jobs = jobxml.Element("Jobs").Elements("Job");
            foreach (var job in jobs)
            {
                JobDetail jobDetail = new JobDetail();

                if (job.Attributes().Any(o => o.Name.ToString() == "name"))
                {
                    job.Name = job.Attribute("name").Value;
                }

                if (job.Attributes().Any(o => o.Name.ToString() == "type"))
                {
                    jobDetail.JobType = (JobType)Enum.Parse(typeof(JobType), job.Attribute("type").Value, true);
                }

                if (job.Attributes().Any(o => o.Name.ToString() == "path"))
                {
                    string path = job.Attribute("path").Value;
                    switch (jobDetail.JobType)
                    {
                        case JobType.Assembly:
                            {
                                string className = path.Split(',')[0];
                                string assembly = path.Split(',')[1];

                                bool runInSandbox = false;
                                if (job.Attributes().Any(o => o.Name.ToString() == "runInSandbox"))
                                {
                                    runInSandbox = Convert.ToBoolean(job.Attribute("runInSandbox").Value);
                                }

                                if (runInSandbox)
                                {
                                    Random r = new Random();
                                    var name = jobDetail.Name + r.Next(1000);

                                    //创建sandbox
                                    jobDetail.Sandbox = Sandbox.Create(name);
                                    jobDetail.AssemblyName = assembly;
                                    jobDetail.TypeName = className;
                                    jobDetail.RunInSandbox = true;
                                }
                                else
                                {
                                    var targetAssembly = Assembly.Load(assembly);
                                    jobDetail.Instance = targetAssembly.CreateInstance(className) as IJob;
                                    jobDetail.RunInSandbox = false;
                                }
                            }
                            break;
                        case JobType.Exe:
                            {
                                jobDetail.Path = path.Replace("${basedir}", AppDomain.CurrentDomain.BaseDirectory);
                                if (job.Attributes().Any(o => o.Name.ToString() == "arguments"))
                                {
                                    jobDetail.Arguments = job.Attribute("arguments").Value;
                                }

                                if (job.Attributes().Any(o => o.Name.ToString() == "expireSecond"))
                                {
                                    jobDetail.ExpireSecond = int.Parse(job.Attribute("expireSecond").Value);
                                }
                            }
                            break;
                    }
                }

                if (job.Attributes().Any(o => o.Name.ToString() == "cornExpress"))
                {
                    jobDetail.CornExpress = job.Attribute("cornExpress").Value;
                    if (!CronExpression.IsValidExpression(jobDetail.CornExpress))
                    {
                        throw new Exception(string.Format("corn表达式：{0}不正确。", jobDetail.CornExpress));
                    }
                }

                jobList.Add(jobDetail);
            }
            return jobList;
        }
    }
}
