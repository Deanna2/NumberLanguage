using System;
using System.IO;
using System.Linq;

public class Utilities
{
    public static int[] ReadFromFile(string filePath)
    {
        var text = File.ReadAllText(filePath);
        return text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Select(str => int.Parse(str)).ToArray();
    }

    public static void WriteToFile(string filePath, int[] array)
    {
        Console.WriteLine("I am called with value {0}", filePath);
        var textToWrite = String.Join(" ", array);
        File.WriteAllText(filePath, textToWrite);
    }

    public static void PrintIntArray(int[] array)
    {
        Console.WriteLine(String.Join(", ", array));
    }
}