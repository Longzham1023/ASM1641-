using System;
namespace ASM1641_.Dtos
{
	public class ChangePasswordDto
	{
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
		public string PasswordConfirm { get; set; } = string.Empty;
	}
}

