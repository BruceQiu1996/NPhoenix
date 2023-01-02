using CommunityToolkit.Mvvm.ComponentModel;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using System;

namespace LeagueOfLegendsBoxer.Models
{
    public class PostBrief : ObservableObject
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }
        public PostCategory PostCategory { get; set; }
        public string Image_1 { get; set; }
        public string Image_2 { get; set; }
        public string Image_3 { get; set; }

        private int _goodCount;
        public int GoodCount
        {
            get { return _goodCount; }
            set { SetProperty(ref _goodCount, value); }
        }

        public int CommmentsCount { get; set; }

        private bool _hadGood;
        public bool HadGood 
        {
            get { return _hadGood; }
            set { SetProperty(ref _hadGood, value); }
        }
        //发布人信息
        public long Publisher { get; set; }
        public string UserName { get; set; }
        public string Desc { get; set; }
        public string CreateTimeText => ConvertDateTimeToText(CreateTime);
        public bool HasImages => IsContainImages();
        public string PostCategoryText { get; set; }
        public string Image_1_loc { get; set; }
        public string Image_2_loc { get; set; }
        public string Image_3_loc { get; set; }

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

        public bool IsContainImages()
        {
            return !string.IsNullOrEmpty(Image_1) || !string.IsNullOrEmpty(Image_2) || !string.IsNullOrEmpty(Image_3);
        }
    }
}
