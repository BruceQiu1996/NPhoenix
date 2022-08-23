using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPhoenixAutoUpdateTool.Models
{
  public class NPhoenix
  {
    public int Id { get; set; }

    public string Version { get; set; }

    public string? LinkName { get; set; }

    public string DownUrl { get; set; }

    public string StartName { get; set; }

    public string? Describe { get; set; }
  }
}
