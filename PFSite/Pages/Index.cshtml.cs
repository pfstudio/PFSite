using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PFSite.Models;
using PFSite.Utils;
using System.Collections.Generic;

namespace PFSite.Pages
{
    [TypeFilter(typeof(LocalUserFilter))]
    public class IndexModel : PageModel
    {
        public List<Slide> Slides { get; set; }

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            Slides = new List<Slide>();
            _configuration.GetSection("Slides").Bind(Slides);
        }
    }
}