using System;
using System.Collections.Generic;

namespace ModelServer.Models;

public partial class CarCompany
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string CountryOrigin { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
