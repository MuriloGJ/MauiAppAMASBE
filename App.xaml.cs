using MauiAppAMASBE.Helpers;
using MauiAppAMASBE.Models;
using MauiAppAMASBE.Pages;
using System.Globalization;

namespace MauiAppAMASBE
{
    public partial class App : Application

    {
        public static CadastroSaudeUsuario UsuarioLogado { get; set; }
        static SQLiteDatabaseHelper _db;
        public static SQLiteDatabaseHelper Db
        {
            get
            {
                if (_db == null)
                {
                    string path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "banco_sqlite_AMASBE.db3");

                    _db = new SQLiteDatabaseHelper(path);
                }

                return _db;
            }
        }

        public App()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            MainPage = new NavigationPage(new LoginPage());
            Task.Run(async () =>
            {
                await App.Db.CriarAdministradorPadrao();
                await App.Db.CriarConteudosPadrao();
            });
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.Width = 400;
            window.Height = 800;

            return window;// retorno da mesma instancia configurada
        }
    }
}