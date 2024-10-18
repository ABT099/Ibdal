﻿using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Station : AppUserModel
{
    public required string Name { get; set; }
    public required string City { get; set; }
    public required string Address { get; set; }
    public IList<Repair> Repairs { get; set; } = [];
}