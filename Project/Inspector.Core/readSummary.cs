using System.Collections;
using System.Text.Json;

namespace Inspector.Core;

public class ReadSummarys
{
    private readonly string _path;

    public ReadSummarys()
    {
        _path = Path.Combine(AppContext.BaseDirectory,"..", "..", "..", "..", "src", "summary");
    }

    public string[] ListAllSummaries()
    {
        Console.WriteLine("Az aktuális összefoglaló nem elérhető"); // szinezni kell!!!
        string[] files = Directory.GetFiles(_path);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileName(files[i]);
        }
        return files;
    }

    public List<PacketData> ReadSummary(string file)
    {
        List<PacketData> res = new List<PacketData>();
        StreamReader streamReader = new StreamReader(Path.Combine(_path, file));
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            res.Add(JsonSerializer.Deserialize<PacketData>(line));
        }
        return res;
    }

}