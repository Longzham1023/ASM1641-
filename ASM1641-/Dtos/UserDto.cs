using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ASM1641_.Dtos
{
	public class UserDto
	{
		public required string UserName { get; set; }

		public required string Password { get; set; }
	}
}

