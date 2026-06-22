using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendario
{
    internal static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            var authController = new Calendario.CONTROLLER.AuthController();
            if (authController.CheckSessioneValida())
            {
                Application.Run(new FrmMain());
            }
            else
            {
                Application.Run(new Calendario.VIEW.FrmLogin());
            }
        }
    }
}
