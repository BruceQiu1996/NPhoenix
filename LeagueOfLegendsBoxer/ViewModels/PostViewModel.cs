using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Data;
using LeagueOfLegendsBoxer.Application.Teamup;
using LeagueOfLegendsBoxer.Application.Teamup.Dtos;
using LeagueOfLegendsBoxer.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = HandyControl.Controls.MessageBox;

namespace LeagueOfLegendsBoxer.ViewModels
{
    public class PostViewModel : ObservableObject
    {
        private string _imageUri1;
        private string _imageUri2;
        private string _imageUri3;
        public List<ImageSelector> ImageSelectors { get; set; }

        private Dictionary<string, string> _postCategories;
        public Dictionary<string, string> PostCategories
        {
            get { return _postCategories; }
            set => SetProperty(ref _postCategories, value);
        }

        private KeyValuePair<string, string> _postCategory;
        public KeyValuePair<string, string> PostCategory 
        {
            get { return _postCategory; }
            set=>SetProperty(ref _postCategory, value);
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set => SetProperty(ref _title, value);
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set => SetProperty(ref _content, value);
        }

        private readonly ITeamupService _teamupService;
        private readonly ILogger<PostViewModel> _logger;

        public AsyncRelayCommand PostCommandAsync { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public PostViewModel(ITeamupService teamupService, ILogger<PostViewModel> logger)
        {
            _logger = logger;
            _teamupService = teamupService;
            ImageSelectors = new List<ImageSelector>();
            PostCategories = GetEnumItemValueDesc(typeof(PostCategory));
            PostCategory = PostCategories.FirstOrDefault();
            PostCommandAsync = new AsyncRelayCommand(PostAsync);
            CancelCommand = new RelayCommand(Cancel);
        }

        public async Task PostAsync()
        {
            try
            {
                int index = 0;
                foreach (var selector in ImageSelectors)
                {
                    index++;
                    if (selector.HasValue && File.Exists(selector.Uri.LocalPath))
                    {
                        FileInfo file = new FileInfo(selector.Uri.LocalPath);
                        if (file.Length > 1024 * 1024 * 2)
                        {
                            MessageBox.Show($"图片{index}大于2M");
                            return;
                        }
                    }
                }

                if (string.IsNullOrEmpty(Title) || Title.Length > 50)
                {
                    MessageBox.Show("标题为空或超过50字");
                    return;
                }

                if (string.IsNullOrEmpty(Content) || Content.Length > 500)
                {
                    MessageBox.Show("内容为空或超过500字");
                    return;
                }

                index = 0;
                foreach (var selector in ImageSelectors)
                {
                    index++;
                    if (selector.HasValue && File.Exists(selector.Uri.LocalPath))
                    {
                        FileInfo file = new FileInfo(selector.Uri.LocalPath);
                        var base64 = Convert.ToBase64String(File.ReadAllBytes(selector.Uri.LocalPath));
                        var fileloc = await _teamupService.UploadImageAsync(new UploadPostImageDto()
                        {
                            ImageBase64 = base64,
                            FileName = file.Name
                        });

                        if (string.IsNullOrEmpty(fileloc))
                        {
                            MessageBox.Show($"图片{index}上传失败");
                            return;
                        }

                        if (index == 1)
                            _imageUri1 = fileloc;
                        else if (index == 2)
                            _imageUri2 = fileloc;
                        else
                            _imageUri3 = fileloc;
                    }
                }

                var result = await _teamupService.CreateOrUpdatePostAsync(new PostCreateOrUpdateDto()
                {
                    Id = null,
                    Title = Title,
                    Content = Content,
                    Image_1 = _imageUri1,
                    Image_2 = _imageUri2,
                    Image_3 = _imageUri3,
                    PostCategory = (PostCategory)int.Parse(PostCategory.Key)
                });

                if (result)
                {
                    Growl.SuccessGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "发帖成功!",
                        ShowDateTime = false
                    });
                    Clear();
                }
                else
                {
                    Growl.WarningGlobal(new GrowlInfo()
                    {
                        WaitTime = 2,
                        Message = "发帖失败,发生错误!",
                        ShowDateTime = false
                    });
                }
            }
            catch (Exception ex) 
            {
                Growl.WarningGlobal(new GrowlInfo()
                {
                    WaitTime = 2,
                    Message = "发生错误!",
                    ShowDateTime = false
                });
                _logger.LogError(ex.ToString());
            }
        }

        private void Cancel() 
        {
            Clear();
            App.ServiceProvider.GetRequiredService<Post>().Hide();
        }

        private void Clear() 
        {
            Title = null;
            Content = null;
            PostCategory = PostCategories.FirstOrDefault();
            ImageSelectors.ForEach(x =>
            {

            });
            _imageUri1 = null;
            _imageUri2 = null;
            _imageUri3 = null;
        }

        public Dictionary<string, string> GetEnumItemValueDesc(Type enumType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    dic.Add(strValue, strText);
                }
            }
            return dic;
        }
    }
}
