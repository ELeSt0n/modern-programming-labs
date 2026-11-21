using System.Text;
using OnlineStore.App.Demos;

Console.OutputEncoding = Encoding.UTF8;

// Запуск: dotnet run --project src/OnlineStore.App -- <номер лабораторної>
switch (args.FirstOrDefault() ?? "1")
{
    case "1": CourseInfo.Print(); break;
    case "2": Lab02Demo.Run(); break;
    default: Console.WriteLine("Unknown lab number."); break;
}
