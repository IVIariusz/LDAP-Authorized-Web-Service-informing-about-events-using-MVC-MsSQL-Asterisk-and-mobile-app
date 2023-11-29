namespace Multimedia.Models
{
    public class LdapConfig
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
        public string AdDomain { get; set; }
        public string ServiceAccountUsername { get; set; }
        public string ServiceAccountPassword { get; set; }
    }
}
