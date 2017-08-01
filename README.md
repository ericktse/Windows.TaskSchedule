# Windows.TaskSchedule

## 简介

Windows下的微型任务调度框架，整合Cron表达式，封装任务调度逻辑，让任务开发者只需关注具体任务逻辑。

支持通过配置文件快速定义调度任务，支持快速部署为WindowService，支持DLL和EXE两种任务类型。

## 范例

引用**Windows.TaskSchedule.Impl**，实现IJob接口（逻辑上exe类型可不实现，但建议所有任务都继承）。

### 任务配置

DLL任务

```xml
<Job name="dlljob" type="assembly" path="namespance.dlljob,namespance" cornExpress="0/5 * * * * ?"  />
```

EXE任务

```xml
<Job name="exejob" type="exe" path="${basedir}\exejob.exe" arguments="1" cornExpress="0/5 * * * * ?"  />
```



### 部署WindowService

在命令行执行以下指令即可，具体参考Topshelf

安装

```
Windows.TaskSchedule.exe install
Windows.TaskSchedule.exe start
```

卸载

```
Windows.TaskSchedule.exe stop
Windows.TaskSchedule.exe uninstall
```



## 来源

此版本为优化版本，原项目请参考以下link

[Windows-TaskSchedule]: https://github.com/leleroyn/Windows-TaskSchedule