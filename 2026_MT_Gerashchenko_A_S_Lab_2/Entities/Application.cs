// <copyright file="Application.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

/// <summary>
/// Represents an application entity with its associated build executions.
/// </summary>
[Table("Applications")]
public class Application : BaseEntity<int>
{
    /// <summary>
    /// Gets or sets the unique identifier for the application.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ApplicationId { get; set; }

    /// <inheritdoc/>
    public override int Id => this.ApplicationId;

    /// <summary>
    /// Gets or sets the name of the application.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository path of the application.
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string RepositoryPath { get; set; } = string.Empty;

    public virtual ICollection<BuildExecution> BuildExecutions { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.ApplicationName} @ {this.RepositoryPath} {val}".TrimEnd());
}