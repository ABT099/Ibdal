﻿using Ibdal.Models.Abstractions;

namespace Ibdal.Models;

public class Message : BaseModel
{
    public required string ChatId { get; set; }
    public required string SenderId { get; set; }
    public required string Text { get; set; }
}