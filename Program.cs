using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Configuration;

namespace FileManager
{
    internal class Program
        {
        //const int WINDOW_WIDTH = Properties.Settings.Default.width; // Ширина окна консоли
        //const int WINDOW_HEIGHT = Properties.Settings.Default.height; // Высота окна консоли
        const string FILE_LOGS = @"logs.txt"; // Путь к файлу с логами
        private static string currentDir = Directory.GetCurrentDirectory();  
        static void Main(string[] args)
        {
            if (Properties.Settings.Default.height < 13 | Properties.Settings.Default.height > 49)
            {
                Console.WriteLine("Указана неккоректная высота окна.");
                Console.ReadKey(true);
            }
            else
            {
                Console.Title = "FileManager";
                Console.SetWindowSize(Properties.Settings.Default.width, Properties.Settings.Default.height);
                Console.SetBufferSize(Properties.Settings.Default.width, Properties.Settings.Default.height);
                Directory.CreateDirectory(@"error");

                DrawWindow(0, 0, Properties.Settings.Default.width - 31, Properties.Settings.Default.height - 10);
                DrawWindow(120, 0, Properties.Settings.Default.width - 120, Properties.Settings.Default.height - 2);
                UpdateInformation("");
                UpdateConsole();
            }
        }
        /// <summary>
        /// Обновление окна с информацией об объекте
        /// </summary>
        /// <param name="size">Размер объекта</param>
        /// <param name="attributes">Атрибуты объекта</param>
        /// <param name="extension">Расширение объекта</param>
        /// <param name="time">Время последнего взаимодействия с объектом</param>
        static void UpdateFileInfo(long size, string attributes, string extension, string time)
        {
            DrawWindow(120, 0, Properties.Settings.Default.width - 120, Properties.Settings.Default.height - 2);
            string temp = "";
            if (extension == ".Directory")
                temp = "Directory";
            else
                temp = "File";
            Console.SetCursorPosition(121, 1);
            Console.Write($"{temp}Size: [Bytes]");
            Console.SetCursorPosition(121, 2);
            Console.Write(size);

            Console.SetCursorPosition(121, 4);
            Console.Write($"{temp}Extension:");
            Console.SetCursorPosition(121, 5);
            Console.Write(extension);

            Console.SetCursorPosition(121, 7);
            Console.Write($"{temp}LastWriteTime:");
            Console.SetCursorPosition(121, 8);
            Console.Write(time);

            Console.SetCursorPosition(121, 10);
            Console.Write($"{temp}Attributes:");
            Console.SetCursorPosition(121, 11);
            Console.Write(attributes);
        }
        /// <summary>
        /// Обновление инфомационного окна
        /// </summary>
        /// <param name="strError">Описание ошибки</param>
        static void UpdateInformation(string strError)
        {
            DrawWindow(0, Properties.Settings.Default.height - 10, Properties.Settings.Default.width - 31, 5);
            Console.SetCursorPosition(1, Properties.Settings.Default.height - 9);
            Console.Write($"Текущая директория: {currentDir}");
            Console.SetCursorPosition(1, Properties.Settings.Default.height - 8);
            Console.Write(strError);
        }
        /// <summary>
        /// Обновление консоли
        /// </summary>
        static void UpdateConsole()
        {
            DrawConsole(0, Properties.Settings.Default.height - 5, Properties.Settings.Default.width - 31, 3);
            CommandEnter(Properties.Settings.Default.width - 31);
        }
        /// <summary>
        /// Отрисовка консоли
        /// </summary>
        /// <param name="x">Координата Х</param>
        /// <param name="y">Координата Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        static void DrawConsole(int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height / 2);
            Console.Write(">");
        }
        /// <summary>
        /// Метод ввода команды
        /// </summary>
        /// <param name="width">Ширина консоли</param>
        static void CommandEnter(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();
            string[] commandLogs = File.ReadAllLines(FILE_LOGS);
            int logID = commandLogs.Length;
            ConsoleKey key;
            int i = 0;
            do
            {
                key = Console.ReadKey().Key;
                (int currentLeft, int currentTop) = GetCursorPosition();
                if (currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }
                switch (key)
                {
                    case ConsoleKey.Backspace:
                        if (command.Length > 0)
                        {
                            command.Remove(command.Length - 1, 1);
                        }
                        if (currentLeft >= left)
                        {
                            Console.SetCursorPosition(currentLeft, top);
                            Console.Write(" ");
                            Console.SetCursorPosition(currentLeft, top);
                        }
                        else
                        {
                            Console.SetCursorPosition(left, top);
                        }
                        break;
                    case ConsoleKey.Enter:
                        break;
                    //case (char)37:left
                    //case (char)39:right
                    case ConsoleKey.UpArrow:
                        try
                        {
                            if (i == 0)
                            {
                                logID--;
                                if (logID >= commandLogs.Length | logID < 0)
                                    logID = commandLogs.Length - 1;
                                Console.Write(commandLogs[logID]);
                                command.Append(commandLogs[logID]);
                                i++;
                            }
                            else
                            {
                                logID--;
                                if (logID >= commandLogs.Length | logID < 0)
                                    logID = commandLogs.Length - 1;
                                command.Clear();
                                DrawConsole(0, Properties.Settings.Default.height - 5, Properties.Settings.Default.width - 31, 3);
                                Console.Write(commandLogs[logID]);
                                command.Append(commandLogs[logID]);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            ErrorParse("outOfLogs");
                            UpdateInformation("Отсутствуют предыдущие команды");
                            Console.SetCursorPosition(left, top);
                        }
                        break;//top
                    case ConsoleKey.DownArrow:
                        try
                        {
                            if (i == 0)
                            {
                                logID--;
                                if (logID >= commandLogs.Length | logID < 0)
                                    logID = commandLogs.Length - 1;
                                Console.WriteLine(commandLogs[logID]);
                                command.Append(commandLogs[logID]);
                                i++;
                            }
                            else
                            {
                                logID++;
                                if (logID >= commandLogs.Length | logID < 0)
                                    logID = commandLogs.Length - 1;
                                command.Clear();
                                DrawConsole(0, Properties.Settings.Default.height - 5, Properties.Settings.Default.width - 31, 3);
                                Console.Write(commandLogs[logID]);
                                command.Append(commandLogs[logID]);
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            ErrorParse("outOfLogs");
                            UpdateInformation("Отсутствуют предыдущие команды");
                            Console.SetCursorPosition(left, top);
                        }//down
                        break;
                    case ConsoleKey.Oem1:
                        command.Append(":");
                        break;
                    case ConsoleKey.Oem5:
                        command.Append("\\");
                        break;
                    case ConsoleKey.OemPeriod:
                        command.Append(".");
                        break;
                    case ConsoleKey.OemComma:
                        command.Append(",");
                        break;
                    case ConsoleKey.OemMinus:
                        command.Append("-");
                        break;
                    default:
                        command.Append((char)key);
                        break;
                }
            }
            while (key != ConsoleKey.Enter);

            CommandParse(command.ToString());
        }
        /// <summary>
        /// Обработка команд
        /// </summary>
        /// <param name="command">Команда</param>
        static void CommandParse(string command)
        {
            DrawWindow(120, 0, Properties.Settings.Default.width - 120, Properties.Settings.Default.height - 2);
            string[] commandParams = command.ToLower().Split(' ');
            if (commandParams.Length > 0)
            {
                switch (commandParams[0])
                {
                    case "cd":
                        try
                        {
                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {
                                currentDir = commandParams[1];
                                File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                            }
                            else throw new DirectoryNotFoundException();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorParse("dirEx");
                            UpdateInformation("Директория не указана, либо не существует");
                            UpdateConsole();
                        }   
                        break;
                    case "ls":
                        try
                        {
                            if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            {
                                if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), n);
                                    currentDir = commandParams[1];
                                    GetInfo(commandParams[1], 1);
                                    File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                }
                                else
                                {
                                    DrawTree(new DirectoryInfo(commandParams[1]), 1);
                                    currentDir = commandParams[1];
                                    GetInfo(commandParams[1], 1);
                                    File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                }
                            }
                            else throw new DirectoryNotFoundException();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            ErrorParse("unAccess");
                            UpdateInformation("Недостаточно прав для просмотра директории");
                            UpdateConsole();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorParse("dirEx");
                            UpdateInformation("Директория не указана, либо не существует");
                            UpdateConsole(); 
                        }
                        break;
                    case "file":
                        try
                        {
                            if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            {
                                if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                                {
                                    string[] readText = File.ReadAllLines(commandParams[1]);
                                    DrawFileText(readText, n);
                                    currentDir = commandParams[1];
                                    GetInfo(commandParams[1], 2);
                                    File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                }
                                else
                                {
                                    string[] readText = File.ReadAllLines(commandParams[1]);
                                    DrawFileText(readText, 1);
                                    currentDir = commandParams[1];
                                    GetInfo(commandParams[1], 2);
                                    File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException();
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            ErrorParse("unAccess");
                            UpdateInformation("Недостаточно прав для просмотра директории");
                            UpdateConsole();
                        }
                        catch (FileNotFoundException)
                        {
                            ErrorParse("fileEx");
                            UpdateInformation("Файл не указан, либо не найден");
                            UpdateConsole();
                        }
                        break;
                    case "cp":
                        try
                        {
                            if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            {
                                File.Copy(commandParams[1], commandParams[2], true);
                                currentDir = commandParams[2];
                                File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                UpdateInformation("Файл успешно скопирован");
                                UpdateConsole();
                            }
                            else
                            {
                                DirectoryCopy(commandParams[1], commandParams[2]);
                                currentDir = commandParams[2];
                                File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                UpdateInformation("Директория успешно скопирована");
                                UpdateConsole();
                            }
                        }
                        catch(IndexOutOfRangeException)
                        {
                            ErrorParse("inOut");
                            UpdateInformation("Не указана одна из директорий операции");
                            UpdateConsole();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorParse("dirEx");
                            UpdateInformation("Одна из директорий не существует");
                            UpdateConsole();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            ErrorParse("unAccess");
                            UpdateInformation("Недостаточно прав для просмотра директории");
                            UpdateConsole();
                        }
                        break;
                    case "rm":
                        try
                        {
                            if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            {
                                File.Delete(commandParams[1]);
                                if (!Directory.Exists(commandParams[1]))
                                {
                                    currentDir = Directory.GetCurrentDirectory();
                                }
                                File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                UpdateInformation("Удаление файла прошло успешно");
                                UpdateConsole();
                            }
                            else
                            {
                                Directory.Delete(commandParams[1], true);
                                if (!Directory.Exists(commandParams[1]))
                                {
                                    currentDir = Directory.GetCurrentDirectory();
                                }
                                File.AppendAllText(FILE_LOGS, command.ToLower() + "\n");
                                UpdateInformation("Удаление каталога прошло успешно");
                                UpdateConsole();
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            ErrorParse("inOut");
                            UpdateInformation("Не указана директория операции");
                            UpdateConsole();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            ErrorParse("dirEx");
                            UpdateInformation("Директория не существует");
                            UpdateConsole();
                        }
                        break;
                    default:
                        ErrorParse("comEx");
                        UpdateInformation("Команда введена некорректно");
                        UpdateConsole();
                        break;
                }
            }
            UpdateInformation("");
            UpdateConsole();
        }
        /// <summary>
        /// Отображение информации об объекте
        /// </summary>
        /// <param name="dir">Путь к объекту</param>
        /// <param name="param">Параметр, передеваемый для опознавания файла или директории</param>
        static void GetInfo(string dir, int param)
        {
            switch (param)
            {
                case 1:
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    UpdateFileInfo(DirectorySize(dirInfo), dirInfo.Attributes.ToString(), ".Directory", dirInfo.LastWriteTime.ToString());
                    break;
                case 2:
                    FileInfo fInfo = new FileInfo(dir);
                    UpdateFileInfo(fInfo.Length, fInfo.Attributes.ToString(), fInfo.Extension, fInfo.LastWriteTime.ToString());
                    break;
            }

        }
        /// <summary>
        /// Рекурсивное измерение размера директории
        /// </summary>
        /// <param name="dir">Путь к директории</param>
        /// <returns></returns>
        static long DirectorySize(DirectoryInfo dir)
        {
            long size = 0;
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo file in files)
            {
                size += file.Length;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach(DirectoryInfo di in dirs)
            {
                size += DirectorySize(di);
            }
            return size;
        }
        /// <summary>
        /// Рекурсивное копирование директории
        /// </summary>
        /// <param name="sourceDirectory">Копируемая директория</param>
        /// <param name="targetDirectory">Путь для новой директории</param>
        /// <exception cref="DirectoryNotFoundException">Ошибка осутствующей директории</exception>
        static void DirectoryCopy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory); 
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(targetDirectory, file.Name);
                file.CopyTo(temppath);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(targetDirectory, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }
        /// <summary>
        /// Отрисовка содержимого файла
        /// </summary>
        /// <param name="readText">Содержимое текстового файла</param>
        /// <param name="page">Страница отображения</param>
        static void DrawFileText(string[] readText, int page)
        {
            try
            {
                DrawWindow(0, 0, Properties.Settings.Default.width - 31, Properties.Settings.Default.height - 10);
                int pageLines = Properties.Settings.Default.height - 12;
                (int currentLeft, int currentTop) = GetCursorPosition();
                int pageTotal = (readText.Length + pageLines - 1) / pageLines;
                if (page > pageTotal)
                {
                    page = pageTotal;
                }

                for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
                {
                    if (readText.Length - 1 > i)
                    {
                        Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                        Console.Write(readText[i]);
                    }
                    else
                    {
                        if (readText.Length == 1)
                        {
                            Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                            Console.Write(readText[0]);
                            break;
                        }
                    }
                }
                string footer = $"╡ {page} of {pageTotal} ╞";
                Console.SetCursorPosition((Properties.Settings.Default.width - 31) / 2 - footer.Length / 2, Properties.Settings.Default.height - 11);
                Console.WriteLine(footer);
            }
            catch
            {

            }
        }
        /// <summary>
        /// Отрисовка каталога
        /// </summary>
        /// <param name="dir">Директория каталога</param>
        /// <param name="page">Страница отрисовки</param>
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            DrawWindow(0, 0, Properties.Settings.Default.width - 31, Properties.Settings.Default.height - 10);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = Properties.Settings.Default.height - 12;
            string[] lines = tree.ToString().Split(new char[] { '\n' });
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
                page = pageTotal;

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }

            //footer
            string footer = $"╡ {page} of {pageTotal} ╞";
            Console.SetCursorPosition((Properties.Settings.Default.width - 31) / 2 - footer.Length / 2, Properties.Settings.Default.height - 11);
            Console.WriteLine(footer);
        }
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }
            tree.Append($"{dir.Name}\n");

            FileInfo[] subFiles = dir.GetFiles();
            for(int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length - 1)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }
            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
            {
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
            }
        }
        /// <summary>
        /// Запись ошибок в файл
        /// </summary>
        /// <param name="error">Код ошибки</param>
        static void ErrorParse(string error)
        {
            string path = @"error/random_name_exception.txt";
            switch (error)
            {
                case "dirEx":
                    File.AppendAllText(path, $"DirectoryNotFoundException - {DateTime.Now}\n");
                    break;
                case "unAccess":
                    File.AppendAllText(path, $"UnauthorizedAccessException - {DateTime.Now}\n");
                    break;
                case "inOut":
                    File.AppendAllText(path, $"IndexOutOfRangeException - {DateTime.Now}\n");
                    break;
                case "fileEx":
                    File.AppendAllText(path, $"FileNotFoundException - {DateTime.Now}\n");
                    break;
                case "comEx":
                    File.AppendAllText(path, $"CommandNotFound - {DateTime.Now}\n");
                    break;
                case "outOfLogs":
                    File.AppendAllText(path, $"LogsNotFound - {DateTime.Now}\n");
                    break;

            }

        }
        /// <summary>
        /// Отрисовка окон
        /// </summary>
        /// <param name="x">Позиция X начала окна</param>
        /// <param name="y">Позиция Y начала окна</param>
        /// <param name="width">Ширина окна</param>
        /// <param name="height">Высота окна</param>
        static void DrawWindow(int x, int y, int width, int height)
        {
            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            for(int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }
            Console.WriteLine("╗");
            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("║");
                for(int j = 0; j < width - 2; j++)
                {
                    Console.Write(' ');
                }
                Console.WriteLine("║");
                Console.SetCursorPosition(x, y + 2 + i);
            }
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }
            Console.WriteLine("╝");
            Console.SetCursorPosition(x, y);
        }
        
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }
    }
}
