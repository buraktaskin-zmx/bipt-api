using System;
using System.Collections.Generic;
using System.Text;

namespace BookingApi.Domain.Entities;

public class User
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}