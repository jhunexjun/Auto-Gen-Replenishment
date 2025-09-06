using System.ComponentModel.DataAnnotations.Schema;


namespace AutoGenReplenishment.Models
{
    internal class Location
    {
        public string Code { get; set; }
        public string Name { get; set; }
        //[Column("DESCR")]
        //public string Description { get; set; }
        public string Descr { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip_Cod { get; set; }
        //[Column("ZIP_COD")]
        [NotMapped]
        public string ZipCode
        {
            get => Zip_Cod;
            set => Zip_Cod = value;
        }
    }
}
