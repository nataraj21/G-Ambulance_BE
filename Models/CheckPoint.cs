namespace AmbulanceAPI.Models
{
    public class CheckPointModel
    {
        public int Id { get; set; }
        public string CheckPointName { get; set; }  
        public double Latitude {  get; set; } 
        public double Longitude { get; set; }

    }
}
