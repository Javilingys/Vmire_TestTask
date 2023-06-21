using System.ComponentModel.DataAnnotations;

namespace Parser.DTOs
{
    public record ParseUrlsDto(List<string> Urls, [Required] string SearchWord);
}
