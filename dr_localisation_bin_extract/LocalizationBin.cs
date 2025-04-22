using System.Text;

namespace dr_localisation_bin_extract
{
    internal class LocalizationBin
    {
        Stream stream;
        BinaryReader reader;
        List<LocalizationSection> sections = new();
        
        public LocalizationBin(Stream s)
        {
            stream = s;
            ReadHeader();
        }
        public LocalizationBin(string filePath)
        {
            stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            ReadHeader();
        }
        private void ReadHeader()
        {
            stream.Seek(0, SeekOrigin.Begin);
            reader = new BinaryReader(stream);
        
            sections.Clear();

            int numSections = reader.ReadInt32();
            for (int i = 0; i < numSections; i++)
                sections.Add(new LocalizationSection(reader));
        }
        public void ExtractToFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            foreach (var section in sections)
            {
                string fileName = $"{section.Id}-{section.Name}-{section.Unknown}.txt";
                string filePath = Path.Combine(folder, fileName);
      
                StreamWriter writer = new(filePath,false);
                for(int i=0; i<section.Values.Count; i++)
                {
                    string str = section.Values[i];
                    /*if (section.Id != 0)
                        str = sections.First(x => x.Id == 0).Values[i] + " | " + str;*/
                    writer.WriteLine(str);
                }

                writer.Close();
            }
        }
        public static void RepackFolder(string folder)
        {
            if (!Directory.Exists(folder))
                return;
            string[] files = Directory.GetFiles(folder);

            List<LocalizationSection> sections = new List<LocalizationSection>();
            foreach (var file in files)
            {
                sections.Add(new(file));
            }

            FileStream outbin = new FileStream(folder+".bin", FileMode.Create, FileAccess.Write);

            outbin.Write(BitConverter.GetBytes(sections.Count));

            foreach(var section in sections)
            {
                Console.WriteLine(section.Name);
                outbin.Write(BitConverter.GetBytes(section.Id));

                byte[] namebytes = Encoding.UTF8.GetBytes(section.Name);
                outbin.Write(BitConverter.GetBytes(namebytes.Length));
                outbin.Write(namebytes);

                outbin.Write(BitConverter.GetBytes(section.Unknown));
                outbin.Write(BitConverter.GetBytes(section.Values.Count));

                Console.WriteLine(outbin.Position);
                int offset = 0;

                foreach (var value in section.Values)
                {
                    outbin.Write(BitConverter.GetBytes(offset));
                    offset += Encoding.UTF8.GetBytes(value).Length+1;
                }
                outbin.Write(BitConverter.GetBytes(offset));

                foreach (var value in section.Values)
                {
                    outbin.Write(Encoding.UTF8.GetBytes(value));
                    outbin.WriteByte(0);
                }
            }

            outbin.Close();

        }
    }
}
