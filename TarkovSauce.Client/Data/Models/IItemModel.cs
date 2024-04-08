using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkovSauce.Client.Data.Models
{
    public interface IItemModel
    {
        string ItemName { get; set; }
        string ShortName { get; set; }
        string Description { get; set; }
        int Avg24hPrice { get; set; }
        string GridImageLink { get; set; }
    }
}
