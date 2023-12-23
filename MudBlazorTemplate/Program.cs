using MudBlazorTemplate.Components;
using MudBlazor.Services;
using MudBlazorTemplate.Services;
using System.Reflection;

namespace MudBlazorTemplate
{
    public partial class Program
    {
        public static string ConnectionString { get; set; }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddMudServices();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<SessionService>();
            builder.Services.AddSingleton<LogService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            InitConfiguration();

            app.Run();
        }
    }
    partial class Program
    {
        public static string ExecutingDirectory { get; set; }

        public static void InitConfiguration()
        {
            string exeFullName = Assembly.GetExecutingAssembly().Location;
            ExecutingDirectory = Path.GetDirectoryName(exeFullName);
            ConnectionString = $"DataSource={Program.GetFilePath("LocalLog.sdb")};";
        }

        public static string GetFileHref(string folderName, string file)
        {
            return $"/{folderName}/{Path.GetFileName(file)}";
        }

        public static string GetFilePath(string file)
        {
            return $@"{Program.ExecutingDirectory}\{file}";
        }
    }
}
