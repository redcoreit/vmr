using System;

Console.WriteLine("Hello World!");

Person p = new () 
{ 
    FirstName = "", 
    LastName = "" 
};

public record Person 
{
    public string FirstName { get; init; } = default;
    public string LastName { get; init; } = default;
}