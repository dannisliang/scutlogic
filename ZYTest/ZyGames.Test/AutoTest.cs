
/****************************************************************************
    File:		AutoTest.cs
    Desc:		Automated testing
    Date:		2015-7-27
    Author:		guccang
    URL:		http://guccang.github.io
    Email:		guccang@126.com
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ZyGames.Framework.Common.Log;
using System.IO;

namespace ZyGames.Test
{
    /*
     * Class:   AutoTest
     * Desc:   	自动化测试类
     *          读取测试用例(*.txt) 运行。
     * Author：	guccang
     * Date：	2015-7-21 11:11:11
     */
    /// <summary>
    /// AutoTest Document
    /// </summary>


    public class autoTest
    {
        string FilePath = ".\\AutoTask\\";
        public autoTest() { }
        public void Menu()
        {
            string uiStr =
@"///////////////////////////////////////////////////////////////////////////
        cmd01: menu
        cmd02: run
        cmd03: runAll
        cmd04: q/exit
        cmd05: clear
///////////////////////////////////////////////////////////////////////////";
            Console.WriteLine(uiStr);
        }

        class autoTaskData
        {
            public TaskSetting setting { get; set; }
            public string taskName { get; set; }
            public string taskDes { get; set; }
            public int dely { get; set; }
            public autoTaskData()
            {
                setting = new TaskSetting();
            }
        }

        List<autoTaskData> getTasks(string str)
        {
                string begin = "[begin]";
                string end = "[end]";
                System.IO.StreamReader stream = new System.IO.StreamReader(str);
                string line = "";
                autoTaskData atd = null;
                List<autoTaskData> atdLST = new List<autoTaskData>();
                while ((line = stream.ReadLine()) != null)
                {
                    // TODO
                    if (begin == line)
                    {
                        atd = new autoTaskData();
                        continue;
                    }
                    if (end == line)
                    {
                        atdLST.Add(atd);
                        continue;
                    }
                    if (line == "")
                    {
                        continue;
                    }
                    string[] worlds = line.Split('=');
                    string key = worlds[0];
                    string val = worlds[1];
                    switch (key)
                    {
                        case "taskName":    atd.taskName = val; atd.setting.TaskName = val; break;
                        case "taskDes":     atd.taskDes = val;atd.setting.TaskDes=val; break;
                        case "dely":        atd.dely = int.Parse(val); break;
                        case "ThreadNum":   atd.setting.ThreadNum = int.Parse(val); break;
                        case "RunTimes":    atd.setting.Runtimes = int.Parse(val); break;
                        case "child":       atd.setting.childInfo = val; break;
                        case "case":        atd.setting.CaseStepList = new List<string>(val.Split(',')); break;
                        default:            atd.setting.Parms.Add(line); break;
                    }

                }
                stream.Close();
                return atdLST;
        }

        void runAll()
        {
            DirectoryInfo folder = new DirectoryInfo(FilePath);

            List<string> names = new List<string>();
            foreach (FileInfo file in folder.GetFiles())
            {
                string name = file.FullName;
                names.Add(name);
            }

            foreach(var f in names)
            {
                Console.WriteLine(f + " begin");
                doRun(f);
                Console.WriteLine(f + " end");
            }
        }

        void doRun(string fileName)
        {
            try
            {
                List<autoTaskData> tasks = getTasks(fileName);
                string result = "";
                foreach (var v in tasks)
                {
                    result += ThreadManager.RunTest(v.setting);
                    Console.WriteLine("sleep" + v.dely);
                    Thread.Sleep(v.dely);
                }
                Console.WriteLine(result);
                //TraceLog.ReleaseWrite(result);
                write(fileName, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message+":"+fileName);
            }
          
        }

        void write(string name, string info)
        {
            name = name.Substring(name.LastIndexOf("\\") + 1);
            name = name.Remove(name.LastIndexOf('.'));
            string fileName = ".//AutoTask//log//" + name + "-" +DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + ".txt";
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(info);
            sw.Close();
        }
        void run()
        {
            Console.Write("input task filename :");
            string fileName = Console.ReadLine();
            fileName = FilePath + fileName + ".txt";
            doRun(fileName);
        }
        void clear()
        {
            Console.Clear();
        }

        public void RunTasks()
        {
            //try
            {
                while (true)
                {
                    Menu();
                    Console.Write("input cmd:");
                    string cmd = Console.ReadLine();
                    switch (cmd)
                    {
                        case "meun": Menu(); break;
                        case "run": run(); break;
                        case "run all": runAll(); break;
                        case "clear": clear(); break;
                        case "exit": break;
                        case "q": break;
                    }
                    if (cmd == "q" || cmd == "exit")
                    {
                        break;
                    }
                }
            }
            //catch (Exception e)
            {
               // Console.WriteLine("Exception:" + e.Message);
            }
            Console.WriteLine("Press any Key to Exit");
            Console.ReadKey();
        }
    }
}
