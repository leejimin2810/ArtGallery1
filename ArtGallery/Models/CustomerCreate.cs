﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGallery.Models
{
    public class CustomerCreate
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
