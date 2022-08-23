﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackendLib;
using LocalApp.CLI;

namespace LocalApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Menu menu = new Menu("Author: Rubens Pirie", "\x1b[38;5;119mDevelopment Mode\x1b[0m");
            Input inputs = new Input(menu);
            Log logger = new Log(menu);

            menu.Setup();
            logger.Event("Program has started and menu has been created successfully.");

            ProgressBar testingBar = new ProgressBar("This is a testing bar", 10, menu);
            testingBar.DisplayProgress();
            Action updateAction = testingBar.GetIncrementAction();

            for (int i = 0; i < 10; i++)
            {
                updateAction();
                Thread.Sleep(500);
            }

        }
    }
}
