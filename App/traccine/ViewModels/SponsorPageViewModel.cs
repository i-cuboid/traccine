using System;
using System.Collections.Generic;
using System.Text;

namespace traccine.ViewModels
{
    class SponsorPageViewModel
    {
        public string PowerdBy { get; set; }

        public SponsorPageViewModel()
        {
            PowerdBy = GlobalSettings.PowerdBy;
        }
    }
}
