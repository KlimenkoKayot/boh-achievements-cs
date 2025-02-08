using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;

namespace BoH_MyLibrary
{
    internal enum OutputType
    {
        Console,
        File,
    }

    public class Application
    {
        private AchievementData _achievementData;
        private List<AchievementFilter> _achievementFilters;
        private OutputType _outputType;
        private StreamWriter _file;
        private TextWriter _consoleOutput;
        private string _jsonString = "";
        private static string _desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public void Run()
        {
            for (; ;) 
            {
                try
                {
                    Menu();
                }
                catch (Exception ex)
                {
                    IO.Fatal(ex.Message);
                    IO.Continue();
                    continue;
                }
            }
        }

        // Дальше идут встроенные функции


        private void Menu()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Write("1. Ввести данные(консоль / файл)", cursor == 1);
                IO.Write("2. Отфильтровать данные.", cursor == 2);
                IO.Write("3. Отсортировать данные.", cursor == 3);
                IO.Write("4. Открыть меню с достижениями.", cursor == 4, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("5. Вывод процента получивших достижение.", cursor == 5, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("6. Выбор режима вывода (консоль / файл)", cursor == 6);
                IO.Write("7. Вывести данные.", cursor == 7);
                IO.Write("8. Экспорт введенных данных.", cursor == 8);
                IO.Write("9. Выход.", cursor == 9);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 9) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuSwitch(cursor);
        }

        private void MenuSwitch(int type)
        {
            switch (type)
            {
                case 1: { MenuInput(); break; }
                case 2: { MenuFilter(); break; }
                case 3: { MenuSort(); break; }
                case 4: { MenuAchievement(); break; }
                case 5: { MenuGetStat(); break; }
                case 6: { MenuOutput(); break; }
                case 7: 
                    {
                        AchievementData data = _achievementData;
                        foreach (AchievementFilter filter in _achievementFilters) filter.Filter(ref data);
                        if (_outputType == OutputType.File) Console.SetOut(_file);
                        IO.PrintAchievementData(data); 
                        if (_outputType == OutputType.File) Console.SetOut(_consoleOutput);
                        IO.Continue();
                        break; 
                    }
                case 8: 
                    {
                        Console.Clear();
                        if (_jsonString == "")
                        {
                            IO.Info("Нет данных :(");
                        }
                        else
                        {
                            if (_outputType == OutputType.File) Console.SetOut(_file);
                            Console.WriteLine(_jsonString);
                            if (_outputType == OutputType.File) Console.SetOut(_consoleOutput);
                        }
                        IO.Continue();
                        break;
                    }
                case 9: { IO.Stop(); break; }
                default: { throw new Exception("unknown command type"); }
            }
        }

        private void MenuGetStat()
        {
            // 1028310 - ид игры
            // не успеваю лучше, сорян
            JsonObject all = SteamWebApi.GetGameAchievementsJson("1028310");
            JsonObject temp = new JsonObject(all.GetField("achievementpercentages"));

            List<JsonObject> allAchievements = JsonParser.DeserializeArray(temp.GetField("achievements"));

            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Вывод в формате id_achievement => XX%...");
                IO.Write("1. Назад", cursor == 1);
                foreach (AchievementCategory category in _achievementData.AchievementCategories())
                {
                    foreach (Achievement achievement in category.Achievements())
                    {
                        string str = allAchievements.Where(item => item.GetField("name") == achievement.Id()).ToList()[0].GetField("percent");
                        if (str.Length > 4)
                        {
                            str = str.Substring(0, 4);
                        }
                        IO.Write($"{achievement.Id()} => {str}%", false, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                    }
                }
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 1) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
        }

        private void MenuFilter()
        {
            int cursor = 1;
            int commandCount = 2 + _achievementFilters.Count;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Для удаления фильтра выберите его и нажмите enter");
                IO.Write("1. Добавить новый фильтр.", cursor == 1);
                IO.Write("2. Назад.", cursor == 2);
                for (int i = 0; i < _achievementFilters.Count; i++)
                {
                    string str = "";
                    var values = _achievementFilters[i].GetValues();
                    for (int j = 0; j < values.Length; j++)
                    {
                        str += values[i];
                        if (j+1 == values.Length) {
                            break;
                        }
                        str += ", ";
                        if ((str.Length + values[i].Length) > 40)
                        {
                            str += "...";
                            break;
                        }

                    }
                    IO.Write($"{_achievementFilters[i].GetField()} == {{{str}}}", cursor == (3+i), ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                }
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < commandCount) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuFilterSwitch(cursor);
        }

        private void MenuFilterSwitch(int type)
        {
            switch (type)
            {
                case 1: 
                    { 
                        MenuFilterField(); 
                        break; 
                    }
                case 2: { break; }
                default: 
                    {
                        int len = _achievementFilters.Count;
                        if ((type-3) < len)
                        {
                            _achievementFilters.RemoveAt(type - 3);
                        } else
                        {
                            throw new Exception("index at null element");
                        }
                        break;
                    }
            }
        }

        private void MenuFilterField()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Выберете значение фильтра");
                IO.Write("1. Назад.", cursor == 1);
                int len = 1;
                foreach (Field field in Enum.GetValues(typeof(Field)))
                {
                    IO.Write($"{len++} {field}.", cursor == len);
                }
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < len) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuFilterFieldSwitch(cursor);
        }

        private void MenuFilterFieldSwitch(int type)
        {
            var fields = Enum.GetValues(typeof(Field));
            switch (type)
            {
                case 1: { break; }
                default:
                    {
                        if (type <= (fields.Length + 1)) MenuFilterValue((Field)fields.GetValue(type - 2));
                        else throw new Exception("unknown command");
                        break;
                    }
            }
        }

        private void MenuFilterValue(Field field)
        {
            Console.Clear();
            IO.Info($"Введите значения для поля {field} через пробел.");
            string[] values = Console.ReadLine().Split(' ');
            _achievementFilters.Add(new AchievementFilter(field, values));
        }

        private void MenuSort()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Write("1. Сортировка категорий.", cursor == 1);
                IO.Write("2. Сортировка достижений.", cursor == 2);
                IO.Write("3. Назад.", cursor == 3);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 3) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuSortSwitch(cursor);
        }

        private void MenuSortSwitch(int type)
        {
            switch (type)
            {
                case 1: { MenuSortCategory(); break; }
                case 2: { MenuSortAchievement(); break; }
                case 3: { break; }
                default: { throw new Exception("unknown command type"); }
            }
        }

        private void MenuSortCategory()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Выберете поле для сортировки.");
                IO.Write("1. Назад.", cursor == 1);
                IO.Write("Id.", cursor == 2, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("Label.", cursor == 3, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 3) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuSortCategorySwitch(cursor);
        }

        private void MenuSortCategorySwitch(int type)
        {
            switch (type)
            {
                case 1: { break; }
                case 2: case 3:
                    {
                        int choosedDir = MenuSortDirection();
                        if (choosedDir == 1) break;
                        bool dirFromLower = choosedDir == 2;
                        List<AchievementCategory> categories = _achievementData.AchievementCategories();
                        if (type == 2 && dirFromLower) categories = categories.OrderBy(item => item.Id()).ToList();
                        if (type == 3 && dirFromLower) categories = categories.OrderBy(item => item.Label()).ToList();
                        if (type == 2 && !dirFromLower) categories = categories.OrderByDescending(item => item.Id()).ToList();
                        if (type == 3 && !dirFromLower) categories = categories.OrderByDescending(item => item.Label()).ToList();
                        _achievementData = new AchievementData(categories);
                        break;
                    }
                default: { throw new Exception("unknown command type"); }
            }
        }
        private void MenuSortAchievement()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Выберете поле для сортировки.");
                IO.Write("1. Назад.", cursor == 1);
                IO.Write("Id.", cursor == 2, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("Label.", cursor == 3, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("Category.", cursor == 4, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("IconUnlocked", cursor == 5, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("DescriptionUnlocked.", cursor == 6, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 6) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuSortAchievementSwitch(cursor);
        }

        private void MenuSortAchievementSwitch(int type)
        {
            switch (type)
            {
                case 1: { break; }
                case 2: case 3: case 4: case 5: case 6: 
                    {
                        int choosedDir = MenuSortDirection();
                        if (choosedDir == 1) break;
                        bool dirFromLower = choosedDir == 2;
                        List<AchievementCategory> categories = _achievementData.AchievementCategories();
                        List<AchievementCategory> newCategories = [];
                        IO.Info("SORTING");
                        foreach (AchievementCategory category in categories)
                        {
                            List<Achievement> achievements = category.Achievements();
                            if (type == 2 && dirFromLower) achievements = achievements.OrderBy(item => item.Id()).ToList();
                            if (type == 2 && !dirFromLower) achievements = achievements.OrderByDescending(item => item.Id()).ToList();
                            if (type == 3 && dirFromLower) achievements = achievements.OrderBy(item => item.Label()).ToList();
                            if (type == 3 && !dirFromLower) achievements = achievements.OrderByDescending(item => item.Label()).ToList();
                            if (type == 4 && dirFromLower) achievements = achievements.OrderBy(item => item.Category()).ToList();
                            if (type == 4 && !dirFromLower) achievements = achievements.OrderByDescending(item => item.Category()).ToList();
                            if (type == 5 && dirFromLower) achievements = achievements.OrderBy(item => item.IconUnlocked()).ToList();
                            if (type == 5 && !dirFromLower) achievements = achievements.OrderByDescending(item => item.IconUnlocked()).ToList();
                            if (type == 6 && dirFromLower) achievements = achievements.OrderBy(item => item.DescriptionUnlocked()).ToList();
                            if (type == 6 && !dirFromLower) achievements = achievements.OrderByDescending(item => item.DescriptionUnlocked()).ToList();
                            newCategories.Add(new AchievementCategory(category.Id(), category.Label(), category.IconUnlocked(), achievements));
                        }
                        IO.Info("SORTED");
                        _achievementData = new AchievementData(newCategories);
                        break;
                    }
                default: { throw new Exception("unknown command type"); }
            }
        }

        private int MenuSortDirection()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Info("Выберете направление сортировки.");
                IO.Write("1. Назад", cursor == 1);
                IO.Write("По убыванию.", cursor == 2, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                IO.Write("По возростанию.", cursor == 3, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 3) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            return cursor;
        }

        private void MenuOutput()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Write("1. Вывод в консоль.", cursor == 1);
                IO.Write("2. Вывод в файл (нужно будет указать)", cursor == 2);
                IO.Write("3. Назад.", cursor == 3);
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 3) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuOutputSwitch(cursor);
        }

        private void MenuOutputSwitch(int type)
        {
            switch (type)
            {
                case 1:
                    {
                        Console.SetOut(_consoleOutput);
                        _outputType = OutputType.Console; 
                        break; 
                    }
                case 2: 
                    { 
                        IO.Write("Файл будет на вашем рабочем столе..");
                        IO.Info("Введите название файла в формате name.txt.");
                        string fileName = Console.ReadLine();
                        _file = new StreamWriter($"{_desktopPath}\\{fileName}");
                        _outputType = OutputType.File; 
                        break; 
                    }
                case 3: { break; }
                default: { throw new Exception("unknown command type"); }
            }
        }

        private void MenuInput()
        {
            int cursor = 1;
            for (; ; )
            {
                Console.Clear();
                IO.Write("1. Ввод в консоли.", cursor == 1);
                IO.Write("2. Путь до файла (.json)", cursor == 2);
                IO.Write("3. Назад.", cursor == 3); var key = Console.ReadKey();
                if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                if (key.Key == ConsoleKey.DownArrow && cursor < 3) cursor++;
                if (key.Key == ConsoleKey.Enter) break;
            }
            MenuInputSwitch(cursor);
        }

        private void MenuInputSwitch(int type)
        {
            /*Тут мы идем в BoH_MyLibrary.Json*/
            string jsonData = "";
            switch (type) 
            {
                case 1: 
                    {
                        IO.Info("Вводите json данные и пустую строку для выхода.");
                        for (; ;)
                        {
                            string cur = Console.ReadLine();
                            if (cur == "") {
                                break;
                            }
                            jsonData += cur;
                            _jsonString = jsonData;
                        }
                        _achievementData = AchievementParser.FromJsonToAchievementData(new JsonObject(JsonParser.Deserialize(jsonData)));
                        break; 
                    }
                case 2: 
                    {
                        IO.Info("Введите путь до файла .json.");
                        string path = Console.ReadLine();
                        if (File.Exists(path))
                        {
                            jsonData = File.ReadAllText(path);
                            _jsonString = jsonData;
                            _achievementData = AchievementParser.FromJsonToAchievementData(new JsonObject(JsonParser.Deserialize(jsonData)));
                        }
                        else
                        {
                            throw new Exception("file not found");
                        }
                        break; 
                    }
                case 3:
                    {
                        break;
                    }
                default: { throw new Exception("unknown command type"); }
            }
        }

        private void MenuAchievement()
        {
            AchievementData data = _achievementData;
            foreach (AchievementFilter filter in _achievementFilters) filter.Filter(ref data);
            List<AchievementCategory> categories = data.AchievementCategories();
            
            int achievementsCount = categories.Count;
            bool[] pressed = new bool[achievementsCount];

            int cursor = 1;
            for (; ; )
            {
                for (; ; )
                {
                    Console.Clear();
                    IO.Write("1. Назад.", cursor == 1);
                    for (int i = 0; i < achievementsCount; i++)
                    {
                        if (pressed[i])
                        {
                            IO.OpenCategory(categories[i], cursor == (i+2), ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                        } 
                        else
                        {
                            IO.Write(categories[i].Label(), (i+2) == cursor, ConsoleColor.DarkMagenta, ConsoleColor.Magenta, ConsoleColor.White);
                        }
                    }

                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.UpArrow && cursor > 1) cursor--;
                    if (key.Key == ConsoleKey.DownArrow && cursor < achievementsCount + 1) cursor++;
                    if (key.Key == ConsoleKey.Enter) break;
                }
                // смещение относительно кнопки назад и учет индексации с нуля
                if (cursor == 1) break;
                pressed[cursor - 2] = !pressed[cursor - 2];
            }
        }

        public Application()
        {
            _outputType = OutputType.Console;
            _consoleOutput = Console.Out;
            _achievementData = new AchievementData();
            _achievementFilters = [];
        }
    }
}
