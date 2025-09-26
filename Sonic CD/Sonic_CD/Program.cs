using System;
using Microsoft.Xna.Framework;

namespace Sonic_CD
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}