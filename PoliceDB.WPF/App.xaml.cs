using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PoliceDB.BLL.Interfaces;
using PoliceDB.BLL.Services;
using PoliceDB.Core.Models;
using PoliceDB.DAL;
using PoliceDB.DAL.Interfaces;
using PoliceDB.DAL.Repositories;
using PoliceDB.WPF.ViewModels;
using PoliceDB.WPF.Views;
using System;
using System.IO;
using System.Windows;

namespace PoliceDB.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceProvider = ConfigureServices();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 1. Загружаем конфигурацию
            var configuration = LoadConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            // 2. Добавляем логирование
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 3. Регистрируем настройки MongoDB
            services.Configure<MongoDBSettings>(configuration.GetSection("MongoDBSettings"));

            // 4. Регистрируем контекст MongoDB
            services.AddSingleton<MongoDbContext>();

            // 5. Регистрируем репозитории с правильными зависимостями
            // Для ILogger<T> - фабрика создаст нужный тип
            services.AddSingleton<IUserRepository, MongoUserRepository>();
            services.AddSingleton<ICaseRepository, MongoCaseRepository>();
            services.AddSingleton<IEvidenceRepository, MongoEvidenceRepository>();
            services.AddSingleton<IPendingChangeRepository, MongoPendingChangeRepository>();

            // 6. Регистрируем сервисы бизнес-логики
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICaseService, CaseService>();
            services.AddSingleton<IEvidenceService, EvidenceService>();
            services.AddSingleton<IAdminService, AdminService>();

            // 7. Регистрируем ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<AddEvidenceViewModel>();
            services.AddTransient<ModifyEvidenceViewModel>();
            services.AddTransient<EvidenceListViewModel>();
            services.AddTransient<EvidenceDetailsViewModel>();
            services.AddTransient<CaseDetailsViewModel>();
            services.AddTransient<VerdictViewModel>();

            // 8. Регистрируем окна
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddEvidenceWindow>();
            services.AddTransient<ModifyEvidenceWindow>();
            services.AddTransient<EvidenceListWindow>();
            services.AddTransient<EvidenceDetailsWindow>();
            services.AddTransient<CaseDetailsWindow>();
            services.AddTransient<VerdictWindow>();

            return services.BuildServiceProvider();
        }

        private IConfiguration LoadConfiguration()
        {
            var basePath = FindAppSettingsFile();
            var builder = new ConfigurationBuilder();

            if (!string.IsNullOrEmpty(basePath))
            {
                builder.SetBasePath(basePath)
                       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            }
            else
            {
                // Запасной вариант
                builder.AddInMemoryCollection(new System.Collections.Generic.Dictionary<string, string>
                {
                    ["MongoDBSettings:ConnectionString"] = "mongodb://admin:admin123@localhost:27017",
                    ["MongoDBSettings:DatabaseName"] = "PoliceDatabase",
                    ["MongoDBSettings:UsersCollection"] = "Users",
                    ["MongoDBSettings:CasesCollection"] = "Cases",
                    ["MongoDBSettings:EvidencesCollection"] = "Evidences",
                    ["MongoDBSettings:PendingChangesCollection"] = "PendingChanges"
                });
            }

            return builder.Build();
        }

        private string FindAppSettingsFile()
        {
            var possiblePaths = new[]
            {
                Directory.GetCurrentDirectory(),
                Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\"),
                AppDomain.CurrentDomain.BaseDirectory,
                @"E:\PoliceDatabase"
            };

            foreach (var path in possiblePaths)
            {
                var fullPath = Path.Combine(path, "appsettings.json");
                if (File.Exists(fullPath))
                {
                    return path;
                }
            }

            return null;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем логгер для диагностики
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<App>();
            logger.LogInformation("=== ЗАПУСК ПРИЛОЖЕНИЯ ===");

            // Проверяем подключение к MongoDB
            try
            {
                var context = ServiceProvider.GetRequiredService<MongoDbContext>();
                var database = context.Users.Database;
                logger.LogInformation("✅ Подключение к MongoDB установлено");

                // Проверяем, что коллекции существуют
                var collections = await database.ListCollectionNames().ToListAsync();
                logger.LogInformation($"Коллекции в базе: {string.Join(", ", collections)}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Ошибка подключения к MongoDB");
                MessageBox.Show($"Не удалось подключиться к базе данных:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Создаем окно авторизации
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void CheckMongoDBConnection()
        {
            try
            {
                var context = ServiceProvider.GetRequiredService<MongoDbContext>();
                // Простая проверка подключения
                var database = context.Users.Database;
                var ping = database.RunCommandAsync(
                    (MongoDB.Driver.Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Result;

                MessageBox.Show("Подключение к MongoDB успешно установлено!",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к MongoDB:\n{ex.Message}\n\n" +
                    "Убедитесь, что Docker контейнер с MongoDB запущен.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}