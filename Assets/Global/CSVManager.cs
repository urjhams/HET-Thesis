using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVManager
{
    private static string directoryName = "Saved Data";
    private static string[] fileHeaders = new string[3]
    {
        "Trial number", 
        "Result",
        "Time",
    };

    static void verifyDirectory()
    {
        string directory = getDirPath();
        if (!Directory.Exists(directory)) 
        {
            Directory.CreateDirectory(directory);
        }
    }

    static void verifyFile(string fileName)
    {
        string file = getFilePath(fileName);
        if (!File.Exists(file))
        {
            createFile(fileName);
        }
    }

    static string getDirPath()
    {
        return Application.dataPath + "/" + directoryName;
    }
    
    static string getFilePath(string fileName)
    {
        return getDirPath() + "/" + fileName;
    }

    public static void createFile(string fileName)
    {
        verifyDirectory();
        using (StreamWriter streamWriter = File.CreateText(getFilePath(fileName)))
        {
            string fileEntry = "";
            for(int index = 0; index < fileHeaders.Length; index++)
            {
                if(fileEntry != "")
                {
                    fileEntry += ", ";
                }
                fileEntry += fileHeaders[index];
            }
            streamWriter.WriteLine(fileEntry);
        }
    }

    public static void appendtoFile(string fileName, string[] entries)
    {
        fileName = fileName + ".csv";
        verifyDirectory();
        verifyFile(fileName);
        using (StreamWriter streamWriter = File.AppendText(getFilePath(fileName)))
        {
            string fileEntry = "";
            for (int index = 0; index < fileHeaders.Length; index++)
            {
                if (fileEntry != "")
                {
                    fileEntry += ", ";
                }
                fileEntry += entries[index];
            }
            streamWriter.WriteLine(fileEntry);
        }
    }
}
