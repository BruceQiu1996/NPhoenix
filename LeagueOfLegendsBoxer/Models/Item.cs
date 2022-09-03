namespace LeagueOfLegendsBoxer.Models
{
    public class Item
    {
        internal static bool firstordeafult;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceTotal { get; set; }
        public string ItemImage => Id switch
        {
            7013 => "https://game.gtimg.cn/images/lol/act/img/item/3802.png",
            0 => "https://wegame.gtimg.com/g.26-r.c2d3c/helper/lol/assis/images/resources/items/0.png",
            _ => $"https://game.gtimg.cn/images/lol/act/img/item/{Id}.png"
        };
    }
}
