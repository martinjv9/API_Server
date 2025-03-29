using System;
using System.Collections.Generic;

namespace ModelServer.Models;

public partial class Car
{
    public int Id { get; set; }

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public int CarCompany { get; set; }

    public virtual CarCompany CarCompanyNavigation { get; set; } = null!;
}
