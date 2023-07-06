
namespace ArtItems.Models;

public class Config
{
  public string Secret { get; set; } = String.Empty;
  public int ExpirationInMinutes { get; set; }
}