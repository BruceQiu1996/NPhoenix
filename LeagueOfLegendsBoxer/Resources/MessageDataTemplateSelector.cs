using LeagueOfLegendsBoxer.Models;
using System.Windows;
using System.Windows.Controls;

namespace LeagueOfLegendsBoxer.Resources
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;
            var obj = item as ChatMessage;
            DataTemplate dt = null;
            if (obj != null && fe != null)
            {
                if (obj.IsSender)
                    dt = fe.FindResource("chatSender") as DataTemplate;
                else
                    dt = fe.FindResource("chatReceiver") as DataTemplate;
            }
            return dt;
        }
    }
}
