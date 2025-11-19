
using System;
using System.ComponentModel.DataAnnotations;

namespace BAMF_API.DTOs.Requests;
public class CreateGroupRequest
{
    [Required, MaxLength(100)]
    public string ObjectId { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    public Guid CategoryId { get; set; }
}
