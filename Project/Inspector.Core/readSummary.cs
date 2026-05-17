using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Inspector.Core;

public class ReadSummarys
{
    private readonly string _path;
    private readonly BlackList _blackList;

    public ReadSummarys()
    {
        _path = Path.Combine(AppContext.BaseDirectory,"..", "..", "..", "..", "src", "summary");
    }

    public string[] ListAllSummaries()
    {
        try
        {
            string[] files = Directory.GetFiles(_path);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }

            return files;
        }
        catch(IOException ioException)
        {
            Console.WriteLine("Hiba történt a fájlok beolvaása során: " + ioException.Message);
            return  null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Váratlan hiba történt: " + e.Message);
            return null;
        }
        
    }

    public List<PacketData> ReadSummary(string file)
    {
        try
        {
            List<PacketData> res = new List<PacketData>();
            using StreamReader streamReader = new StreamReader(Path.Combine(_path, file));
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                res.Add(JsonSerializer.Deserialize<PacketData>(line));
            }

            return res;
        }
        catch (IOException ioException)
        {
            Console.WriteLine("Hiba történt a " + file + "beolvaása során: " + ioException.Message);
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("Váratlan hiba történt: " + e.Message);
            return null;
        }

    }

    public List<PacketData> GetPotentialDangerIPs(List<PacketData> summaryPackets)
    {
        var potentailDanger = from pack in summaryPackets where pack.PotentialDanger == true select pack;
        return potentailDanger.ToList();
    }

}