using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoceTransportApp.Data.Models.Enumerations
{
    public enum DriverExperience
    {
        [Display(Name = "Less than a year of driving experience")]
        None = 0,
        [Display(Name = "1 to 3 years of driving experinece")]
        Beginner = 1,
        [Display(Name = "3 to 5 years of driving experience")]
        Skilled = 2,
        [Display(Name = "5+ years of driving experience")]
        Experienced = 3,
    }
}
