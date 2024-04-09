namespace WebApplication1.Models;

public class Visit
{
    public DateTime VisitDate { get; set; }
    public int TreatedAnimalId { get; set; }
    public string VisitDescription { get; set; }
    public double VisitPrice { get; set; }
}