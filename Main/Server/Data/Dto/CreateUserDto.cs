﻿namespace Server.Data.Dto;

public class CreateUserDto
{
    public string Email { get; set; }
    
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string Name { get; set; }
    
    public string LastName { get; set; }
}