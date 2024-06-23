namespace EraXP_Back.Models.Database;

public record UniversityLanguage(
    string Language,
    List<Tuple<string, decimal>> LanguageSkills
);