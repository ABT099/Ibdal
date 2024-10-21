﻿namespace Ibdal.Api.Forms;

public class CreateProductForm
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required IFormFile Image { get; set; }
}