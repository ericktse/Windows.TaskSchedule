using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.TaskSchedule.Impl;

namespace Windows.TaskSchedule.Utility
{
    public class JobDetail
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public JobType JobType { get; set; }
        /// <summary>
        /// 执行路径（exe为执行路径，dll为命名空间）
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 可执行程序运行时可用的参数
        /// </summary>
        public string Arguments { get; set; }
        /// <summary>
        /// corn表达式
        /// </summary>
        public string CornExpress { get; set; }
        /// <summary>
        /// 最长运行时间
        /// </summary>
        public int? ExpireSecond { get; set; }
        /// <summary>
        /// 是否运行沙盒
        /// </summary>
        public bool RunInSandbox { get; set; }
        /// <summary>
        /// 任务实体
        /// </summary>
        public IJob Instance { get; set; }
        /// <summary>
        /// 任务正在执行时间段中
        /// </summary>
        public bool IsTriggering { get; set; }
        /// <summary>
        /// 是否运行
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// 实体类名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 沙盒
        /// </summary>
        public Sandbox Sandbox { get; set; }
    }

    /// <summary>
    /// 任务类型
    /// </summary>
    public enum JobType
    {
        Assembly,
        Exe
    }
}
