using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Auth.Models;

public partial class Person
{
    public Guid UserGuid { get; set; }

    [Key]
    public int Id { get; set; }

    public string? Fname { get; set; }

    public string? Lname { get; set; }

    public string? Email { get; set; }
}
