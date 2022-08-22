namespace BackendLib
{
    public class Logger
    {
        private readonly bool _localApplication;

        public Logger(bool local)
        {
            _localApplication = local;

            CreateDirStructure();

        }

        private void CreateDirStructure()
        {
            Directory.CreateDirectory("./run");
            Directory.CreateDirectory("./logs");

            using (StreamWriter sr = File.AppendText("./logs/master.txt"))
            {
                string mode = _localApplication ? "Local Application" : "Web Application";

                sr.WriteLine("<====================== New Instance ======================>");
                sr.WriteLine($"Datetime: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow:HH:mm:ss}");
                sr.WriteLine($"Mode: ${mode}");
            }
        }

        public Guid CreateRun()
        {
            Guid guidForRun = Uuid();

            Directory.CreateDirectory($"./run/{guidForRun.ToString("N").ToUpper()}");

            using (StreamWriter sr = File.CreateText($"./logs/{guidForRun}.txt"))
            {
                sr.WriteLine("<====================== Begin New Run ======================>");
                sr.WriteLine($"Datetime: {DateTime.UtcNow:dd-MM-yyyy} {DateTime.UtcNow:HH:mm:ss}");
                sr.WriteLine($"Run Object Guid: {guidForRun.ToString().ToUpper()}");
            }

            return guidForRun;
        }

        public void WriteLineToRunFile(Guid currentGuid, string message)
        {
            using StreamWriter sr = File.AppendText($"./logs/{currentGuid}.txt");
            sr.WriteLine(message);
        }

        public void WriteLineToMaster(string message)
        {
            using StreamWriter sr = File.AppendText($"./logs/master.txt");
            sr.WriteLine(message);
        }

        public static Guid Uuid() => Guid.NewGuid();
    }
}
