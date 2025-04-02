using DemoMVC.Service;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

public class QuartzSchedular : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            UserExamService userExamService = new UserExamService(); // Manually create instance
            userExamService.UpdateExamStatusOnEndTime();
            Console.WriteLine("✅ Exam statuses updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating exam statuses: {ex.Message}");
        }
    }
}

public static class QuartzScheduler
{
    public static async Task StartScheduler()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        IJobDetail job = JobBuilder.Create<QuartzSchedular>() // Ensure correct class name
            .WithIdentity("ExamStatusJob", "group1")
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("ExamStatusTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
