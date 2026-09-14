namespace TestabilityDemo;

public class Order
{
    private DateTime _now;
    public float Discount => (_now.DayOfWeek == DayOfWeek.Friday) ? 25f : 0f;
   
    public float Quantity { get; set; }
    public string Item { get; set; }
    public float Price { get; set; }

    public float GetTotal() => Price * Quantity * (1 - (Discount / 100));
    

    public Order(DateTime now)
    {
        _now = now;
    }

}