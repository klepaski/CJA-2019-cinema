using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace version1
{
    public class StaticClass
    {
        public static int activeId;
        public static string activeLogin;       ///гость null
        public static string activeName;
        public static string activeSurname;
        public static string activeEmail;
        public static string activePassword;
        public static string activeRole;

        public static int activeIdFilm;
        public static string activeTitleFilm;

        public static bool pageOrRegistartion = true;   ///user T, admin F
    }
}
