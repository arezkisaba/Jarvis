namespace MiLight.Lib.Models
{
    public class MiLightHotspot
    {
        public string Bssid { get; set; }
        public byte Channel { get; set; }
        public string Dpid { get; set; }
        public string ExtCh { get; set; }
        public string Nt { get; set; }
        public string Security { get; set; }
        public byte Signal { get; set; }
        public string Ssid { get; set; }
        public string Wps { get; set; }
    }
}