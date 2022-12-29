using LeagueOfLegendsBoxer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace LeagueOfLegendsBoxer.Windows
{
    /// <summary>
    /// Interaction logic for Post.xaml
    /// </summary>
    public partial class Post : Window
    {
        public Post(PostViewModel postViewModel)
        {
            InitializeComponent();
            DataContext = postViewModel;
            postViewModel.ImageSelectors.Add(img1);
            postViewModel.ImageSelectors.Add(img2);
            postViewModel.ImageSelectors.Add(img3);
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Hide();
        }
    }
}
