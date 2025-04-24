using System.Text.RegularExpressions;

namespace BNF.Timelapse.Models;

public class Timelapse : MongoModel
{
    public string? Name { get; set; }
    public string PrefixName => Regex.Replace(Name ?? "", "[a-zA-Z0-9_-]", "");
    public bool Running { get; set; } = true;
    public int Index { get; set; } = 0;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Started { get; set; } = DateTime.Now;
    public DateTime? Stopped { get; set; }
}