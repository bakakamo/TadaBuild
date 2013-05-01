using System;
using System.Collections.Generic;
using System.Text;
using Tada;

namespace Tada.Demo
{
    public class DemoProject : Project
    {
        public DemoProject()
        {
            Add(ref Demo);
        }

        public DemoConfiguration Demo;

        [BuildOption]
        public static bool ProjectOption1;
        [BuildOption]
        public static int ProjectOption2;
        [BuildOption]
        public static string ProjectOption3;
    }

    public class DemoConfiguration : Configuration
    {
        public DemoConfiguration()
        {
            Add(ref DemoTargetGroup1);
        }

        [BuildOption]
        public static bool ConfigurationOption1;
        [BuildOption]
        public static int ConfigurationOption2;
        [BuildOption]
        public static string ConfigurationOption3;

        public DemoTargetGroup1 DemoTargetGroup1;
    }

    public class DemoTargetGroup1 : TargetGroup
    {
        protected override void Initialize()
        {
            Add(ref DemoTarget1);
            Add(ref DemoTarget2);
        }

        public DemoTarget1 DemoTarget1;
        public DemoTarget2 DemoTarget2;
    }

    public class DemoTarget1 : Target
    {
        protected override void DoExecute()
        {
            Log(Tada.LogLevel.Info, "Hello from demo target 1!");
        }
    }

    public class DemoTarget2 : Target
    {
        protected override void DoExecute()
        {
            Log(LogLevel.Info, "Hello from demo target 2!");

            Log(LogLevel.Error, "Error message");
            Log(LogLevel.Warning, "Warning message");
            Log(LogLevel.Info, "Info message");
            Log(LogLevel.Verbose, "Verbose message");
            Log(LogLevel.Debug, "Debug message");
        }
    }
}
