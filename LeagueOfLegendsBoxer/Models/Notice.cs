using CommunityToolkit.Mvvm.ComponentModel;

namespace LeagueOfLegendsBoxer.Models
{
    public class Notice : ObservableObject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public int Priority { get; set; }
        public bool IsMust { get; set; }

        private bool isReaded;
        public bool IsReaded
        {
            get { return isReaded; }
            set { SetProperty(ref isReaded, value); }
        }
    }
}
