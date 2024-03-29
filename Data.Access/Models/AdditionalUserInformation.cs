﻿using System.ComponentModel.DataAnnotations.Schema;
using AzisFood.DataEngine.Postgres.Models;

namespace Flamma.Auth.Data.Access.Models;

/// <summary>
///     Additional user information
/// </summary>
public class AdditionalUserInformation : PgRepoEntity<AuthDbContext>
{
    /// <summary>
    ///     Users first name
    /// </summary>
    public string FirstName { get; set; }
    
    /// <summary>
    ///     Users last name
    /// </summary>
    public string LastName { get; set; }
    
    /// <summary>
    ///     Identifier of primary location
    /// </summary>
    public Guid PrimaryLocationId { get; set; }
    
    /// <summary>
    ///     Date when user was born
    /// </summary>
    public DateTime BirthDate { get; set; }
    
    /// <summary>
    ///     Link to user data
    /// </summary>
    [ForeignKey("UserData")]
    public Guid UserDataId { get; set; }
    public virtual UserData UserData { get; set; }
}