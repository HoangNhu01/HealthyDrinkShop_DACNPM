using eShopSolution.ViewModels.AppSystem.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Models
{
    public class NavigationViewModel
    {
        public List<LanguageVm> Languages { get; set; }

        public string CurrentLanguageId { get; set; }
    }
}