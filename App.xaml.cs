using MauiAppAMASBE.Helpers;
using MauiAppAMASBE.Models;
using MauiAppAMASBE.Pages;
using System.Globalization;

namespace MauiAppAMASBE
{
    public partial class App : Application
    {
        public static CadastroSaudeUsuario UsuarioLogado { get; set; }

        /// <summary>
        /// Verifica se há usuário logado; se não, redireciona para o login.
        /// Chame em OnAppearing de qualquer página que exija autenticação.
        /// </summary>
        public static bool VerificarLogin()
        {
            if (UsuarioLogado != null) return true;
            Current.MainPage = new NavigationPage(new LoginPage());
            return false;
        }

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

            // Cultura pt-BR para todas as threads (principal e background)
            var cultura = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture   = cultura;
            CultureInfo.DefaultThreadCurrentUICulture = cultura;
            Thread.CurrentThread.CurrentCulture   = cultura;
            Thread.CurrentThread.CurrentUICulture = cultura;

            MainPage = new NavigationPage(new LoginPage());

            // Seed de dados iniciais em background — não bloqueia a UI
            Task.Run(async () =>
            {
                await App.Db.CriarAdministradorPadrao();
                await App.Db.CriarConteudosPadrao();
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Width  = 400;
            window.Height = 800;
            return window;
        }
    }
}
