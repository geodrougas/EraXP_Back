using EraXP_Back.Models.Database;

namespace EraXP_Back.Models.Dto;

public class UniversityForm
{
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AddressName { get; set; }
        public string? GoogleLocationId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid LanguageId { get; set; }
        public string? Language { get; set; }
        public string? LanguagePoints { get; set; }
        public IFormFile? Image { get; set; }

        public UniversityLanguage GetLanguage(List<UniversityLanguageLevel> languageLevel)
        {
                List<Tuple<string, decimal>> immutableLanguageLevel = languageLevel
                        .Select(m => Tuple.Create(m.Name ?? "", m.Value))
                        .ToList();
                
                UniversityLanguage language = new UniversityLanguage(
                        LanguageId,
                        Language,
                        immutableLanguageLevel
                );
                
                return language;
        }
}