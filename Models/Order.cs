namespace OrderManager.Models;

public record Order
{
    public string Company { get; set; }
    public string Employee { get; set; }
    public double Freight { get; set; }
    public List<Line> Lines { get; set; }
    public DateTime OrderedAt { get; set; }
    public DateTime RequireAt { get; set; }
    public ShipTo ShipTo { get; set; }
    public string ShipVia { get; set; }
    public object ShippedAt { get; set; }

    [JsonProperty("@metadata")]
    public Metadata Metadata { get; set; }
}

public class Line
{
    public double Discount { get; set; }
    public double PricePerUnit { get; set; }
    public string Product { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class ShipTo
{
    public string City { get; set; }
    public string Country { get; set; }
    public string Line1 { get; set; }
    public object Line2 { get; set; }
    public Location Location { get; set; }
    public string PostalCode { get; set; }
    public string Region { get; set; }
}

public class Metadata
{
    [JsonProperty("@collection")]
    public string Collection { get; set; }

    [JsonProperty("@flags")]
    public string Flags { get; set; }
}