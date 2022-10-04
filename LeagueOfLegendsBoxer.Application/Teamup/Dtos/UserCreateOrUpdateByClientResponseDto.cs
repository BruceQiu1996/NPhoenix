namespace LeagueOfLegendsBoxer.Application.Teamup.Dtos
{
    public class UserCreateOrUpdateByClientResponseDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
        public string Token { get; set; }
        public string ServerArea { get; set; }
        public string RoleName { get; set; }
    }
}
