using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BNF.Timelapse.Models;

public class Timelapse : MongoModel
{
    public string? Name { get; set; }
    public string BaseName => Regex.Replace(Name ?? "", "[^a-zA-Z0-9_-]", "_");

    public TimelapseState State { get; set; }

    public int Index { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Started { get; set; } = DateTime.Now;
    public DateTime? Stopped { get; set; }
    public DateTime? Completed { get; set; }
}