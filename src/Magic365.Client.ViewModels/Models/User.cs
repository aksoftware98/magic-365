using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.Models
{
	public class User
	{

        public User(string token, string fullName, string profilePicture, string email)
        {
			AccessToken = token;
			FullName = fullName;
			ProfilePicture = profilePicture;
            Email = email;
        }

        public string AccessToken { get; set; }

		public string FullName { get; set; }

		public string ProfilePicture { get; set; }

        public string Email
        {
            get; set;
        }

	}
}
