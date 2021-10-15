using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LobbyActionMover
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            LAMover lamover = new LAMover();

            Application.Run(lamover);
        }
    }
}
