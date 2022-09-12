using System;
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

            Run(menu, inputs, logger);
        }

        private static void Run(Menu m, Input i, Log l)
        {
            bool running = true;

            while (running)
            {
                int opt = i.GetOption("Please select an option to continue:",
                    new[] { "Process New Image Into Map Data File", "Recall Map From Data File", "Exit Program" });

                switch (opt)
                {
                    // New
                    case 0: 



                        break;
                    // Recall
                    case 1:



                        break;
                    case 2:
                        running = false;
                        break;
                }
            }
        }

    }
}
