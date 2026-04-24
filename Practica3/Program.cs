namespace Practica3
{
    internal static class Program
    {
        // Точка входа в приложение
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}
