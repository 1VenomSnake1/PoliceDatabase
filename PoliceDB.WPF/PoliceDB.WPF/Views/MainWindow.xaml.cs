using PoliceDB.WPF.ViewModels;
using PoliceDB.WPF.Views;

public MainWindow()
{
    InitializeComponent();

    // Тестируем разные роли:
    // 1. Следователь (видит: добавить улику, просмотр улик, описание дела)
    // var mockUser = new PoliceDB.Core.Models.User { Role = PoliceDB.Core.Models.UserRole.Investigator };

    // 2. Старший следователь (видит: изменить улику, просмотр улик, описание дела)
    // var mockUser = new PoliceDB.Core.Models.User { Role = PoliceDB.Core.Models.UserRole.SeniorInvestigator };

    // 3. Присяжный (видит только: описание дела)
    // var mockUser = new PoliceDB.Core.Models.User { Role = PoliceDB.Core.Models.UserRole.Juror };

    // 4. Администратор (видит всё)
    var mockUser = new PoliceDB.Core.Models.User
    {
        Username = "admin",
        Role = PoliceDB.Core.Models.UserRole.Administrator
    };

    _viewModel = new MainViewModel(mockUser, "CASE-001");
    DataContext = _viewModel;
}