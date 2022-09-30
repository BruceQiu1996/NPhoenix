namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class CreateGameRecordByClientDto
    {
        public long UserId { get; set; }
        public long GameId { get; set; }
        public int GameMode { get; set; }
        public int Kill { get; set; }
        public int Death { get; set; }
        public int Assit { get; set; }
        public bool IsMvp { get; set; }
        public bool IsSvp { get; set; }
        public bool IsLeastScore { get; set; }
        public bool IsHighestDamageConvert { get; set; }
        public bool IsLeastDamageConvert { get; set; }
    }
}
