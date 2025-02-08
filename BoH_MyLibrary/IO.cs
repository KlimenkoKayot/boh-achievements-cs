using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace BoH_MyLibrary
{
    public static class IO
    {

        public static void Continue()
        {
            //Console.Clear();
            Write("Нажмите на любую кнопку, чтобы продолжить...", false, ConsoleColor.Green, ConsoleColor.DarkGreen, ConsoleColor.Gray);
            Console.ReadKey();
            Console.Clear();
        }

        public static void Stop()
        {
            Console.Clear();
            Write("До встречи! by kayot123", false, ConsoleColor.Green, ConsoleColor.DarkGreen, ConsoleColor.Gray);
            System.Environment.Exit(0);
        }

        public static void Write(string str, bool choosed = false, 
            ConsoleColor color1 = ConsoleColor.Blue, 
            ConsoleColor color2 = ConsoleColor.DarkCyan, 
            ConsoleColor color3 = ConsoleColor.Cyan)
        {
            string up = "   ┌────────────────────────────────────────────────────────────────┐";

            string middle = MakeMiddleLine(str);
            string down = "   └────────────────────────────────────────────────────────────────┘";
            if (choosed)
            {
                up = up.Substring(3, up.Length - 3);
                down = down.Substring(3, down.Length - 3);
                middle = middle.Substring(3, middle.Length - 3);
                middle += "▒▒";
                down += "▒▒";
                down += "\n";
                down += "   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒";
            }
            Console.ForegroundColor = color1;
            Console.WriteLine(up);
            Console.ForegroundColor = color2;
            Console.WriteLine(middle);
            Console.ForegroundColor = color3;
            Console.WriteLine(down);
        }
        public static void OpenCategory(AchievementCategory category, bool choosed = false,
            ConsoleColor color1 = ConsoleColor.Blue,
            ConsoleColor color2 = ConsoleColor.DarkCyan,
            ConsoleColor color3 = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color1;
            string up = "   ┌────────────────────────────────────────────────────────────────┐";
            string middle = MakeMiddleLine(category.Label());
            string down = "   ├────────────────────────────────────────────────────────────────┤";
            if (choosed)
            {
                up = up.Substring(3, up.Length - 3);
                down = down.Substring(3, down.Length - 3);
                middle = middle.Substring(3, middle.Length - 3);
            }
            Console.WriteLine(up);
            Console.WriteLine(middle);
            Console.WriteLine(down);
            Console.ForegroundColor = color2;
            foreach (Achievement cur in category.Achievements())
            {
                string line = MakeMiddleLine("- " + cur.Label());
                if (choosed)
                {
                    line = line.Substring(3, line.Length - 3);
                    line += "▒▒";
                }
                Console.WriteLine(line);
            }
            Console.ForegroundColor = color3;
            string megaDown = "   └────────────────────────────────────────────────────────────────┘";
            if (choosed)
            {
                megaDown = megaDown.Substring(3, megaDown.Length - 3);
                megaDown += "▒▒";
            }
            Console.WriteLine(megaDown);
            if (choosed) Console.WriteLine("   ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
        }

        private static string CreateBlockWithData(string key, string value, string prefix = "", int typeDown = 0)
        {
            int len = 66;
            string center = "";
            for (int i = 0; i < len - 4 - key.Length * 2; i++)
            {
                center += ' ';
            }
            string up = prefix + "├────────────────────────────────────────────────────────────────┤";
            string middle = prefix + "│ " + key + center + key + " │";
            for (int i = 0; i < key.Length * 2; i++) center += ' ';
            middle += '\n' + prefix + "│ " + center + " │";
            string down = "";
            if (typeDown == 1) down = prefix + "└──┬─────────────────────────────────────────────────────────────┘";
            else if (typeDown == 2) down = prefix + "├────────────────────────────────────────────────────────────────┤";
            else if (typeDown == 3) down = prefix + "└────────────────────────────────────────────────────────────────┘";
            bool addPrefix = false;
            int added = 4;
            string data = $"{prefix}│   ";
            foreach (char ch in value)
            {
                if (addPrefix)
                {
                    data += '\n';
                    data += prefix + "│   - ";
                    added = 6;
                    addPrefix = false;
                }
                data += ch;
                added += 1;
                if (added+6 >= len)
                {
                    addPrefix = true;
                    data += " -   │";
                }
            }
            bool flag = false;
            for (int i = added; i < len - 4; i++)
            {
                flag = true;
                data += ' ';
            }
            if (flag) data += "   │";
            if (down == "") return up + '\n' + middle + '\n' + data;
            return up + '\n' + middle + '\n' + data + '\n' + down;
        }

        public static void PrintAchievementData(AchievementData achievementData,
            ConsoleColor color1 = ConsoleColor.Magenta,
            ConsoleColor color2 = ConsoleColor.Blue,
            ConsoleColor color3 = ConsoleColor.Cyan)
        {
            Console.Clear();
            if (achievementData.AchievementCategories().Count == 0 )
            {
                Info("Нет данных :(");
                return;
            }
            // iter нужен для красивого вывода, каждая строка должна быть своего цвета
            int iter = 0;
            ConsoleColor[] colors = [color1, color2, color3, color2];

            void localWrite(string str)
            {
                Console.ForegroundColor = colors[(++iter) % 4];
                Console.WriteLine(str);
            }

            localWrite("┌──────────────────────────────────────────────────────────────────────────┐");
            localWrite("│                             AchievementData                              │");
            localWrite("└──┬───────────────────────────────────────────────────────────────────────┘");
            string prefix = "   │   ";
            string prefixConnect = "   ├───";
            var categories = achievementData.AchievementCategories();
            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                localWrite($"{prefix}┌────────────────────────────────────────────────────────────────┐");
                if (i == categories.Count - 1)
                {
                    prefix = "       ";
                    prefixConnect = "   └───";
                }
                localWrite($"{prefixConnect}┤                          Category                              │");
                localWrite(CreateBlockWithData("id", categories[i].Id(), prefix));
                localWrite(CreateBlockWithData("label", categories[i].Label(), prefix));
                localWrite(CreateBlockWithData("iconUnlocked", BoolToString(categories[i].IconUnlocked()), prefix, 1));
                var achievements = category.Achievements();
                for (int j = 0; j <  achievements.Count; j++) 
                {
                    Achievement achievement = achievements[j];
                    string secPrefix = prefix + "   │   ";
                    string secPrefixConnect = prefix + "   ├───";
                    localWrite($"{secPrefix}┌────────────────────────────────────────────────────────────────┐");
                    if (j == achievements.Count - 1)
                    {
                        secPrefix = prefix + "       ";
                        secPrefixConnect = prefix + "   └───";
                    }
                    localWrite($"{secPrefixConnect}┤                          Achievement                           │");
                    localWrite(CreateBlockWithData("id", achievements[i].Id(), secPrefix));
                    localWrite(CreateBlockWithData("label", achievements[i].Label(), secPrefix));
                    localWrite(CreateBlockWithData("category", achievements[i].Category(), secPrefix));
                    localWrite(CreateBlockWithData("iconUnlocked", achievements[i].IconUnlocked(), secPrefix));
                    localWrite(CreateBlockWithData("singleDescription", BoolToString(achievements[i].SingleDescription()), secPrefix));
                    localWrite(CreateBlockWithData("validateOnStorefront", BoolToString(achievements[i].ValidateOnStoreFront()), secPrefix));
                    localWrite(CreateBlockWithData("descriptionUnlocked", achievements[i].DescriptionUnlocked(), secPrefix, 3));
                }
            }
        }

        private static string BoolToString(bool flag)
        {
            if (flag) return "true";
            return "false";
        }

        private static string MakeMiddleLine(string str)
        {
            str = " " + str;
            for (; str.Length < 64;)
            {
                str += " ";
            }
            string middle = $"   │{str}│";
            return middle;
        }
        public static void Info(string str)
        {
            Console.Clear();
            Write(str, false, ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.Yellow);
        }
        public static void Fatal(string str)
        {
            Console.Clear();
            Write(str, false, ConsoleColor.Red, ConsoleColor.DarkRed, ConsoleColor.DarkGray);
        }
    }
}
