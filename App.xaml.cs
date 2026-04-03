using MauiAppAMASBE.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace MauiAppAMASBE
{
    public partial class App : Application
    {
        static SQLiteDatabaseHelper _db;
        public static SQLiteDatabaseHelper Db
        {
            get
            {
                if (_db == null)
                {
                    string path = Path.Combine(
                       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                       "banco_sqlite_compras.db3");
                    _db = new SQLiteDatabaseHelper(path);
                }

                return _db;
            }

        }

        public App()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            MainPage = new NavigationPage(new MainPage());
        }






    }
}