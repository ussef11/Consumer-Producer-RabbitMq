public class Order
{
    public int Id { get; set; }
    public int OriginNodeId { get; set; }
    public int TargetNodeId { get; set; }
    public int Load { get; set; }
    public int Value { get; set; }
    public DateTime DeliveryDateUtc { get; set; }
    public DateTime ExpirationDateUtc { get; set; }
    public string Token { get; set; }


    public override string ToString()
    {
        return $"Order Details:\n" +
               $"Id: {Id}\n" +
               $"OriginNodeId: {OriginNodeId}\n" +
               $"TargetNodeId: {TargetNodeId}\n" +
               $"Load: {Load}\n" +
               $"Value: {Value}\n" +
               $"DeliveryDateUtc: {DeliveryDateUtc}\n" +
               $"ExpirationDateUtc: {ExpirationDateUtc}\n" +
               $"Token: {Token}";
    }

}
