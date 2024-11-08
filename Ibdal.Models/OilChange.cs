﻿using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class OilChange : BaseModel
{
    public required string CarId { get; set; }
    public required string StationId { get; set; }
    public required string ProductId { get; set; }
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }
    public int Quantity { get; set; }
    public int CurrentCarMeter { get; set; }
    public int NextCarMeter { get; set; }
}