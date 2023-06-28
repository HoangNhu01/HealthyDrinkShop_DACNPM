using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.AppSystem.ExternalUser
{
    public class FaceBookUserInfor
    {
        public string Id { get; set; }
        public string Access_token { get; set; }
        public string Name { get; set; }
        public string First_name { get; set; }
        public string Last_name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Birthday { get; set; }
        public Image Picture { get; set; }

        public class Location
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Image
        {
            public Data Data { get; set; }
        }

        public class Data
        {
            public bool Is_silhouette { get; set; }
            public string Url { get; set; }
        }
    }
}
