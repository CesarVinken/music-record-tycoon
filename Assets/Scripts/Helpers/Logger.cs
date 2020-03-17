using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Logger
{
    public static LogCategory General = new LogCategory("General");
    public static LogCategory Character = new LogCategory("Character");
    public static LogCategory Locomotion = new LogCategory("Locomotion");
    public static LogCategory Building = new LogCategory("Building");
    public static LogCategory Pathfinding = new LogCategory("Pathfinding");
    public static LogCategory Initialisation = new LogCategory("Initialisation");
    public static LogCategory Datawriting = new LogCategory("Datawriting");
    public static LogCategory Datareading = new LogCategory("Datareading");
    public static LogCategory UI = new LogCategory("UI");

    public static Color stringColor = new Color(144, 123, 35);
    public static Color intColor = new Color(54, 0, 153);
    public static Color objectColor = new Color(72, 109, 132);

    public static void Log(params object[] args)
    {
        args[0] = ColorString(args);

        Debug.Log(FormatString(args));
    }

    public static void Log(LogCategory category, params object[] args)
    {
        if (!category.enableLogs) return;

        if(category.Name == "General")
        {
            args[0] = ColorString(args);
        }
        else
        {
            args[0] = category.Name + " :: " + ColorString(args);
        }
        Debug.Log(FormatString(args));
    }

    public static void Warning(params object[] args)
    {
        args[0] = ColorString(args);

        Debug.LogWarning(FormatString(args));
    }

    public static void Warning(LogCategory category, params object[] args)
    {
        if (!category.enableLogs) return;
        
        args[0] = "<color=yellow>*" + category.Name + " warning*</color> :: " + ColorString(args);
        Debug.LogWarning(FormatString(args));
    }

    public static void Error (params object[] args)
    {
        args[0] = ColorString(args);

        Debug.LogError(FormatString(args));
    }

    public static void Error(LogCategory category, params object[] args)
    {        
        args[0] = "<color=red>*" + category.Name + " error*</color> :: " + ColorString(args);
        Debug.LogError(FormatString(args));
    }

    private static string ColorString(params object[] args)
    {
        string stringArg = args[0].ToString();
        string adjustedString = stringArg;

        for (int i = stringArg.Length - 1; i >= 0; i--)
        {
            if (stringArg[i].ToString() == "{")
            {
                if (args[int.Parse(stringArg[i + 1].ToString()) + 1].GetType() == typeof(string))
                {
                    adjustedString = adjustedString.Insert(i + 3, "</color>");
                    adjustedString = adjustedString.Insert(i, string.Format("<color=#{0:X2}{1:X2}{2:X2}>", (byte)(stringColor.r), (byte)(stringColor.g), (byte)(stringColor.b)));
                }
                else if (args[int.Parse(stringArg[i + 1].ToString()) + 1].GetType() == typeof(int))
                {
                    adjustedString = adjustedString.Insert(i + 3, "</color>");
                    adjustedString = adjustedString.Insert(i, string.Format("<color=#{0:X2}{1:X2}{2:X2}>", (byte)(intColor.r), (byte)(intColor.g), (byte)(intColor.b)));
                }
                else
                {
                    adjustedString = adjustedString.Insert(i + 3, "</color>");
                    adjustedString = adjustedString.Insert(i, string.Format("<color=#{0:X2}{1:X2}{2:X2}>", (byte)(objectColor.r), (byte)(objectColor.g), (byte)(objectColor.b)));
                }

            }
        }

        return adjustedString;
    }
    private static string FormatString(params object[] args)
    {
        return string.Format(args[0].ToString(), args.Skip(1).ToArray());
    }
}

public struct LogCategory
{
    public bool enableLogs;
    public string Name;

    public LogCategory(string name)
    {
        enableLogs = true;
        Name = name;
    }
}