using BackendLib;
using System.IO;

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
            menuInstance.WriteLine("You have selected to read a new image and turn it into a route-able map, during this the following steps will occur:");
            menuInstance.WriteLine();
            menuInstance.WriteLine("1. You will be asked to supply an image to process.");
            menuInstance.WriteLine("2. The image will be checked to make sure it is valid, if it is not you will have to pick another and start again.");
            menuInstance.WriteLine("3. You will be shown the image to check if it is the right one, as well as some file details about it. You can chose to end here if you wish.");
            menuInstance.WriteLine("4. You will have some options as to how to pick out the roads. There are some presets as well as a step by step version.");
            menuInstance.WriteLine("5. After the roads have been picked out you will be able to click on different points and find the most efficient root through them.");
            menuInstance.WriteLine("6. You can chose to save that map or not.");
        }

        public static void FileDetails(Menu menuInstance, Structures.RawImage rawImage)
        {
            menuInstance.WriteLine("Your image file information:");
            menuInstance.WriteLine($"    Name of image: {Log.Green}{Path.GetFileNameWithoutExtension(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine($"    Folder it's contained within: {Log.Green}{Path.GetDirectoryName(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine($"    Type of image: {Log.Green}{Path.GetExtension(rawImage.Path)}{Log.Blank}");
            menuInstance.WriteLine();
        }

    }
}
