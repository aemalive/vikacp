using cp.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace cp.helpers
{
    public class ReviewDeleteVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var currentUser = values[0] as User;
            var reviewAuthorId = values[1] as int?;
            var isAdmin = values[2] as bool?;

            if (currentUser == null || reviewAuthorId == null)
                return Visibility.Collapsed;

            if (isAdmin == true || currentUser.Id == reviewAuthorId)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
