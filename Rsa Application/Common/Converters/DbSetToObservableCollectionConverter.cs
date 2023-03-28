using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System;
using Rsa_Application.Database.Entities.Base;

namespace Rsa_Application.Common.Converters
{
    internal class DbSetToObservableCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                DbSet<Entity> d = (DbSet<Entity>)value; 
                return new ObservableCollection<Entity>(d);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }    
    }
}
