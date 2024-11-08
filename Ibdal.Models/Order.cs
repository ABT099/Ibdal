﻿using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Order : Archivable
{
    public int OrderNumber { get; set; }
    public required string PurchaseId { get; set; }
    public required string StationId { get; set; }
    public required string StationName { get; set; }
    public List<ProductOrder> ProductsInfo { get; set; } = [];
    public string Status { get; set; } = "pending";
}

public class ProductOrder
{
    public required Product Product { get; set; }
    public int Quantity { get; set; }
}