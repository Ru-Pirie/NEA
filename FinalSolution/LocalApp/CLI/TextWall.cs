using BackendLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalApp.CLI
{
    // TODO better name for this
    public static class TextWall
    {
        public static void SaveWelcome(Menu menuInstance)
        {
            menuInstance.WriteLine("You have chosen to re-call a map file which has been previously used. At the next prompt you will be asked to enter the file / the path to it. After that you will have several options open to you:");
            menuInstance.WriteLine();
            menuInstance.WriteLine("1. You can choose to modify the file parameters, i.e. Name, Description or Type");
            menuInstance.WriteLine("2. Delete the file");
            menuInstance.WriteLine("3. Clone the file");
            menuInstance.WriteLine("4. Rename the file");
            menuInstance.WriteLine("5. Run pathfinding on the image");
        }

        public static void ImageWelcome(Menu menuInstance)
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

        public static void FileDetails(Menu menuInstance, Structures.RawImage rawImage)
        {
            menuInstance.WriteLine("Parsed file information:");
            menuInstance.WriteLine($"    Name: {Log.Green}{Path.GetFileNameWithoutExtension(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine($"    Folder: {Log.Green}{Path.GetDirectoryName(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine($"    File extension: {Log.Green}{Path.GetExtension(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine();
        }

    }
}
