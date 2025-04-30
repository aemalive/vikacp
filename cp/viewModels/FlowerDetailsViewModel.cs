using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cp.models;

namespace cp.viewModels
{
    public class FlowerDetailsViewModel : BaseViewModel
    {
        private readonly Flower _flower;

        public string Name => _flower.Name;
        public decimal Price => _flower.Price;
        public string Description => _flower.Description;
        public string ImageURL => _flower.ImageURL;

        public FlowerDetailsViewModel(Flower flower)
        {
            _flower = flower;
        }
    }
}
