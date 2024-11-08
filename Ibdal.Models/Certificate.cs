﻿using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Certificate : BaseModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IList<string> ImagesUrls { get; set; } = [];
}