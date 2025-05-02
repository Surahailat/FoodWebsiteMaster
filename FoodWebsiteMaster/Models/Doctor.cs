using System;
using System.Collections.Generic;

namespace FoodWebsiteMaster.Models;

public partial class Doctor
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Position { get; set; }

    public string? ProfileImage { get; set; }

    public string? InstagramLink { get; set; }
}
