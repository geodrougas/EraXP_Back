namespace EraXP_Back.Models.Database;

public record UniversityLanguage(
    Guid Id,
    string Language,
    List<Tuple<string, decimal>> LanguageSkills
);