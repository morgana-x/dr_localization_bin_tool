using System.Text;

namespace dr_localisation_bin_extract
{
    internal class LocalizationSection
    {
        public int Id;
        public string Name;
        public int Unknown;
        public List<string> Values = new();
        static string ReadString(BinaryReader reader, int length=-1)
        {
            if (length == -1)
                length = reader.ReadInt32();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
        
        public LocalizationSection(BinaryReader reader) // Read Binary
        {
            Id = reader.ReadInt32();
            Name = ReadString(reader);
            Unknown = reader.ReadInt32();

            int numberOfStrings = reader.ReadInt32();

            List<int> offsets = new List<int>();
            for (int i = 0; i < numberOfStrings+1; i++)
                offsets.Add(reader.ReadInt32());

            long startOffset = reader.BaseStream.Position;

            for (int i=0;  i < numberOfStrings; i++)
            {
                int offset = offsets[i];
                reader.BaseStream.Position = startOffset + offset;

                int nextOffset = offsets[i + 1];
      
                // The -1 is to account for the NULL character afterwards
                Values.Add(ReadString(reader, nextOffset-offset-1 ).Replace("\n","<br>"));
                reader.BaseStream.Position += 1;
            }
        }

        public LocalizationSection(string filePath) // Read Text
        {
            string fileName =Path.GetFileNameWithoutExtension(filePath);
            string[] fileNameSplit = fileName.Split("-");
            Id = int.Parse(fileNameSplit[0]);
            Name = fileNameSplit[1];
            Unknown = int.Parse(fileNameSplit[2]);
            StreamReader reader = new StreamReader(filePath,Encoding.UTF8);
            while(!reader.EndOfStream)
                Values.Add(reader.ReadLine().Replace("<br>", Encoding.UTF8.GetString(new byte[] {0x0A})));
            reader.Close();
        }
    }
}
