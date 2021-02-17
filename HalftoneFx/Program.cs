namespace HalftoneFx
{
    using System;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// Main entry point of program.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
