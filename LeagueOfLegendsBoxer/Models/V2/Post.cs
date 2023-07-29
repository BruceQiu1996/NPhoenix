using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using Microsoft.Extensions.Primitives;
using System;
using System.Windows.Media;

namespace LeagueOfLegendsBoxer.Models.V2
{
    public class Post : ObservableObject
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public PostCategory PostCategory { get; set; }
        public bool IsTop { get; set; }
        public bool IsDeleted { get; set; }
        public string PublisherName { get; set; }
        public string Tag { get; set; }
        public string CreateTimeText => ConvertDateTimeToText(CreateTime);
        private int _goodCount;
        public int GoodCount
        {
            get { return _goodCount; }
            set { SetProperty(ref _goodCount, value); }
        }

        private bool _hadGood;
        public bool HadGood
        {
            get { return _hadGood; }
            set { SetProperty(ref _hadGood, value); }
        }

        public string PostCategoryText { get; set; }
        public SolidColorBrush PostCategoryColor { get; set; }
        public string ConvertDateTimeToText(DateTime dateTime)
        {
            if (dateTime.Year == DateTime.Now.Year && dateTime.DayOfYear == DateTime.Now.DayOfYear)
            {
                //今天
                return dateTime.ToString("t");
            }
            else if (dateTime.Year == DateTime.Now.Year)
            {
                //今年
                return dateTime.ToString("MM-dd HH:mm");
            }
            else
            {
                return dateTime.ToString("yyyy/MM/dd HH:mm");
            }
        }
    }
}
