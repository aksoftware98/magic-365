using Magic365.Client.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.Interfaces
{
	public interface IAuthenticationProvider
	{

		Task<User> SignInAsync();

		Task SignOutAsync();

	}

	
}
