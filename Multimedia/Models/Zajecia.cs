using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.EntityFrameworkCore;


namespace Multimedia.Models
{
    public class Zajecia
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public DateTime DataRozpoczecia { get; set; }
        public DateTime DataZakonczenia { get; set; }
        public DayOfWeek DzienTygodnia { get; set; }
        public string Lokalizacja { get; set; }
    }

}
