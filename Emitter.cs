namespace Teeny_Tiny
{
    public class Emitter
    {
        public string FullPath { get; set; }
        public string Header { get; set; } = "";
        public string Code { get; set; } = "";

        public Emitter(string fullPath)
        {
            FullPath = fullPath;
        }

        public void Emit(string code)
        {
            Code += code;
        }

        public void EmitLine(string code)
        {
            Code += code + '\n';
        }

        public void HeaderLine(string code)
        {
            Header += code + '\n';
        }

        public void WriteFile()
        {
            using StreamWriter writer = new StreamWriter(FullPath);
            writer.Write(Header + Code);
        }
    }
}
