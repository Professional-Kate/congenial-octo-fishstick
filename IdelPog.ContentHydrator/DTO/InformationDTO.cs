namespace ContentHydrator.DTO
{
    public readonly struct InformationDTO(string name, string description)
    {
        public readonly string Name = name;
        public readonly string Description  = description;
    }
}