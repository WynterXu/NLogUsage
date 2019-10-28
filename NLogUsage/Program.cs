using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLogUsage.CommonUtils;

namespace NLogUsage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();
            try
            {
                using (IServiceScope scope = host.Services.CreateScope())
                {
                    IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    //��ȡ��appsettings.json�е������ַ���
                    string sqlString = configuration.GetSection("ConectionStrings:MySqlConnection").Value;
                    //ȷ��NLog.config�������ַ�����appsettings.json��ͬ��
                    NLogUtil.EnsureNlogConfig("NLog.config", sqlString);
                }
                //throw new Exception("�����쳣");//for test

                //������Ŀ����ʱ��Ҫ��������
                //code
                NLogUtil.WriteDBLog(NLog.LogLevel.Trace, LogType.Web, "��վ�����ɹ�");
                host.Run();
            }
            catch (Exception ex)
            {
                //ʹ��nlogд��������־�ļ�����һ���ݿ�û����/���ӳɹ���
                string errorMessage = "��վ������ʼ�������쳣";
                NLogUtil.WriteFileLog(NLog.LogLevel.Error, LogType.Web, errorMessage, new Exception(errorMessage, ex));
                NLogUtil.WriteDBLog(NLog.LogLevel.Error, LogType.Web, errorMessage, new Exception(errorMessage, ex));
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                }).UseNLog();  // NLog: ����ע��Nlog
    }
}
