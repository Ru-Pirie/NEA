using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.CLI
{
    // TODO better name for this
    internal static class TextWall
    {
        public static void Welcome(Menu menuInstance)
        {
            menuInstance.WriteLine("Before we begin, at the start of this you will be asked to supply an image to be converted into a map. After you have done this the following will occur:");
            menuInstance.WriteLine();
            menuInstance.WriteLine("1. The image you supplied will be checked to see if it is suitable, if it is you will be shown said image and asked to confirm if you wish to proceed.");
            menuInstance.WriteLine("2. The image will have edge detection performed on it to get its roads, you will have the opportunity to enter the variables which control the edge detection.");
            menuInstance.WriteLine("3. The result of your map will either be inverted (black pixels to white or vice versa) depending on the result.");
            menuInstance.WriteLine("4. This image will then have a combination of holistic algorithms run on it to pick out roads.");
            menuInstance.WriteLine("5. You can pathfind through your processed map image.");
            menuInstance.WriteLine("6. You can chose to save that map or not.");
            menuInstance.WriteLine("7. That's it!");
        }


    }
}
